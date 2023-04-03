using CombatRoutine;
using Common.Define;
using Samurai.SpecialSpell;

namespace Samurai
{
    internal class SamuraiSlotResolvers
    {
        public List<ISlotResolver> _slotResolvers = new List<ISlotResolver>()
        {
            new NormalSpell(SpellsDefine.KaeshiNamikiri,true),
            new SpecialSpell_KaeshiSetsugekka(),
            new SpecialSpell_OgiNamikiri(),
            //new NormalSpell(SpellsDefine.OgiNamikiri,true),
            new SpecialSpell_Higanbana(),
            new SpecialSpell_MidareSetsugekka(),
            new BaseGCD(),

            new NormalSpell(SpellsDefine.Shoha,false),
            new NormalSpell(SpellsDefine.Ikishoten,false),
            new NormalSpell(SpellsDefine.HissatsuSenei,false,true),
            new SpecialSpell_MeikyoShisui(),
            new SpecialSpell_HissatsuShinten(),
        };
    }
}
