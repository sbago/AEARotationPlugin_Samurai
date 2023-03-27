using CombatRoutine;
using Common.Define;
using Common.Helper;

namespace Samurai.SpecialSpell
{
    internal class SpecialSpellBase :ISlotResolver
    {
        private Spell Spell;
        public SpecialSpellBase(uint spellId,bool isGCD)
        {
            Spell = spellId.GetSpell();
            SlotMode = isGCD ? SlotMode.Gcd : SlotMode.OffGcd;
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
            if(Spell == null)
                return -1;
            if(Spell.Check())
                return 0;
            return -1;
        }
    }
}
