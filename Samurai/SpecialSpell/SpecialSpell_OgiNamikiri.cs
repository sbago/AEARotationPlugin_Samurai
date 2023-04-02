using CombatRoutine;
using Common;
using Common.Define;
using Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.SpecialSpell
{
    internal class SpecialSpell_OgiNamikiri :SpellBase
    {
        public SpecialSpell_OgiNamikiri() :base(SpellsDefine.OgiNamikiri,true)
        { }
        public override int Check()
        {
            if (SpellsDefine.KaeshiSetsugekka.GetSpell().Cooldown.TotalMilliseconds <= 5000)
                return -1;
            if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.Higanbana, 3000))
                return -2;
            return base.Check();
        }
    }
}
