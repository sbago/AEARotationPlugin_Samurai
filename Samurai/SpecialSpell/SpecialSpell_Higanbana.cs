using Common;
using Common.Define;
using Common.Helper;

namespace Samurai.SpecialSpell
{
    //彼岸花
    internal class SpecialSpell_Higanbana : SpecialSpellBase
    {
        public SpecialSpell_Higanbana() : base(SpellsDefine.Higanbana,true) 
        { 
        }
        public override int Check()
        {
            if(!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.Higanbana, 3000))
                return base.Check();
            return -1;
        }
    }
}
