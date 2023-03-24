using Common;
using Common.Define;

namespace Samurai.SpecialSpell
{
    internal class SpecialSpell_MeikyoShisui:SpecialSpellBase
    {
        uint Combo => Core.Get<IMemApiSpell>().GetLastComboSpellId();
        public SpecialSpell_MeikyoShisui() : base(SpellsDefine.MeikyoShisui,false) 
        { 
        }
        public override int Check()
        {
            if (Core.Me.HasAura(AurasDefine.MeikyoShisui))
                return -2;
            if (Combo == SpellsDefine.Hakaze || Combo == SpellsDefine.Shifu || Combo == SpellsDefine.Jinpu)
                return -3;
            return base.Check();
        }
        private bool CheckEffect()
        {
            return false;
        }
    }
}
