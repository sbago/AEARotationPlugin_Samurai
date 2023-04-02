using CombatRoutine;
using Common;
using Common.Define;
using Common.Helper;

namespace Samurai.SpecialSpell
{
    //彼岸花
    internal class SpecialSpell_Higanbana : SpellBase
    {
        public SpecialSpell_Higanbana() : base(SpellsDefine.Higanbana,true) 
        { 
        }
        public override int Check()
        {
            if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.Higanbana, 3000))
            {
                if (SpellsDefine.KaeshiSetsugekka.GetSpell().Cooldown.TotalMilliseconds <= 5000)
                    return -2;
                if (base.Check() >= 0)
                    return 0;
            }
            return -1;
        }
    }
}
