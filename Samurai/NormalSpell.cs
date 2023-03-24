using CombatRoutine;
using Common.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai
{
    //能用就用的技能归在这个类的实例
    internal class NormalSpell:ISlotResolver
    {
        private Spell Spell;
        public NormalSpell(uint spellId,bool isGCD) 
        {
            SlotMode = isGCD ? SlotMode.Gcd : SlotMode.OffGcd;
            Spell = spellId.GetSpell();
        }

        public SlotMode SlotMode { get; }

        public void Build(Slot slot)
        {
            slot.Add(Spell);
        }

        public int Check()
        {
            if (Spell.Id.Check())
                return 0;
            return -1;
        }
    }
}
