using CombatRoutine;
using Common.Define;
using Common;
using Samurai.RotationEventHandler;
using Common.Helper;

namespace Samurai.SpecialSpell
{
    internal class BaseGCD : SpellBase
    {        
        uint Combo => Core.Get<IMemApiSpell>().GetLastComboSpellId();
        bool HasSetsu => Core.Get<IMemApiSamurai>().HasSetsu();
        bool HasGetsu => Core.Get<IMemApiSamurai>().HasGetsu();
        bool HasKa => Core.Get<IMemApiSamurai>().HasKa();
        public BaseGCD() { SlotMode = SlotMode.Gcd; }
        public override int Check()
        {
            Spell = GetBaseSpell().GetSpell();
            return base.Check();
        }
        private uint GetBaseSpell()
        {
            if (Core.Me.HasAura(AurasDefine.MeikyoShisui) || SamuraiRotationEventHandler.hasMeikyoShisui)
            {
                if (!HasKa)
                    return SpellsDefine.Kasha;
                if (!HasGetsu)
                    return SpellsDefine.Gekko;
                if (!HasSetsu)
                    return SpellsDefine.Yukikaze;
                return SpellsDefine.Kasha;//
            }
            else
            {
                switch (Combo)
                {
                    case SpellsDefine.Hakaze:
                        if (!HasSetsu)
                            return SpellsDefine.Yukikaze;
                        if (!HasGetsu)
                            return SpellsDefine.Jinpu;
                        if (!HasKa)
                            return SpellsDefine.Shifu;
                        return Core.Get<IMemApiBuff>().GetTimeSpanLeft(Core.Me, AurasDefine.Jinpu).TotalMilliseconds >= Core.Get<IMemApiBuff>().GetTimeSpanLeft(Core.Me, AurasDefine.Shifu).TotalMilliseconds ? SpellsDefine.Shifu : SpellsDefine.Jinpu;
                    case SpellsDefine.Jinpu:
                        return SpellsDefine.Gekko;
                    case SpellsDefine.Shifu:
                        return SpellsDefine.Kasha;
                }
                return SpellsDefine.Hakaze;
            }
        }
    }
}
