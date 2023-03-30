using CombatRoutine;
using Common.Define;
using Common.Helper;

namespace Samurai.SpecialSpell
{
    internal class SpellBase :ISlotResolver
    {
        private Spell Spell;
        private bool BurstControl;
        public SpellBase(uint spellId,bool isGCD, bool burstControl = false)
        {
            Spell = spellId.GetSpell();
            SlotMode = isGCD ? SlotMode.Gcd : SlotMode.OffGcd;
            BurstControl = burstControl;

        }

        public SlotMode SlotMode { get; }

        public  void Build(Slot slot)
        {
            LogHelper.Info(Spell.Name+ Helper.GetGCDCooldown().ToString());
            slot.Add(Spell);
        }

        //通用检查 子类同名方法应该都调用此方法
        public virtual int Check()
        {
            if (BurstControl && BattleData.Burst)
                return -4;
            if (Spell == null)
                return -3;
            if (SlotMode == SlotMode.OffGcd && Helper.GetGCDCooldown() < 600)
                return -2;
            if (Spell.Check())
                return 0;
            return -1;
        }
    }
}
