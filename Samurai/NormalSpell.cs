using CombatRoutine;
using Common.Define;
using Common.Helper;
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
        private bool BurstControl;
        public NormalSpell(uint spellId,bool isGCD,bool burstControl = false) 
        {
            SlotMode = isGCD ? SlotMode.Gcd : SlotMode.OffGcd;
            Spell = spellId.GetSpell();
            BurstControl = burstControl;
        }

        public SlotMode SlotMode { get; }

        public void Build(Slot slot)
        {
            LogHelper.Info(Spell.Name + Helper.GetGCDCooldown().ToString());
            slot.Add(Spell);
        }

        public int Check()
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
