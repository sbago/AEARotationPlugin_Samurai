using Common;
using Common.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.SpecialSpell
{
    internal class SpecialSpell_HissatsuShinten:SpellBase
    {
        public SpecialSpell_HissatsuShinten() :base(SpellsDefine.HissatsuShinten, false, false)
        {
        }
        public override int Check()
        {
            if (Core.Get<IMemApiSamurai>().Kenki() < 50)
                return -11;
            return base.Check();
        }
    }
}
