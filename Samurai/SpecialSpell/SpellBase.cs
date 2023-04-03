using CombatRoutine;
using Common;
using Common.Define;
using Common.Helper;

namespace Samurai.SpecialSpell
{
    internal class SpellBase :ISlotResolver
    {
        protected Spell Spell { get; set; }
        protected bool BurstControl;
        public static uint lastadd;
        public SpellBase(uint spellId,bool isGCD, bool burstControl = false)
        {
            Spell = spellId.GetSpell();
            SlotMode = isGCD ? SlotMode.Gcd : SlotMode.OffGcd;
            BurstControl = burstControl;
        }
        public SpellBase()
        { 
            Spell = new Spell();
        }

        public SlotMode SlotMode { get; set; }

        public void Build(Slot slot)
        {
            //LogHelper.Info($"AddAction:{Spell.LocalizedName} GCDtime:{Helper.GetGCDCooldown()} dot:{Core.Me.GetCurrTarget().GetBuffTimespanLeft(AurasDefine.Higanbana).TotalMilliseconds/1000:f2}");
            lastadd = Spell.Id;
            slot.Add(Spell);
        }

        //通用检查 子类同名方法应该都调用此方法
        public virtual int Check()
        {
            if (BurstControl && (AI.Instance.BattleData.CurrBattleTimeInMs / 1000) < 9)
                return -6;
            if (BurstControl && !BattleData.Instance.Burst)
                return -4;
            if (Spell == null)
                return -3;
            if (SlotMode == SlotMode.OffGcd)
            {
                if(Helper.GetGCDCooldown() < 600)
                    return -2;
                if (Helper.GetGCDCooldown() > 1900)
                    return -5;
            }
            if (Spell.Check())
                return 0;
            return -1;
        }
    }
}
