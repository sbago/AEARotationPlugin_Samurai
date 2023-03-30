using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.SpecialSpell
{
    internal class NormalSpell : SpellBase
    {
        public NormalSpell(uint spellId, bool isGCD, bool burstControl = false) : base(spellId, isGCD, burstControl)
        {
        }
    }
}
