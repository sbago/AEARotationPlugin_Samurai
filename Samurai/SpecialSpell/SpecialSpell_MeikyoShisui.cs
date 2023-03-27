using CombatRoutine;
using Common;
using Common.Define;
using Common.Helper;
using Samurai.RotationEventHandler;

namespace Samurai.SpecialSpell
{
    public struct Info
    {
        public uint ComboSpellId;
        public Sen Sen;
        public double HiganbanaTimeLeft;
        public double MeikyoShisuiCooldown;
        public Info()
        {
            ComboSpellId = Core.Get<IMemApiSpell>().GetLastComboSpellId();
            Sen = new Sen();
            HiganbanaTimeLeft = Core.Me.GetCurrTarget().GetBuffTimespanLeft(AurasDefine.Higanbana).TotalMilliseconds;
            MeikyoShisuiCooldown = SpellsDefine.MeikyoShisui.GetSpell().Cooldown.TotalMilliseconds;
        }
    }
    public struct Sen
    {
        public bool HasSetsu;
        public bool HasGetsu;
        public bool HasKa;
        public Sen()
        {
            var a = Core.Get<IMemApiSamurai>();
            HasSetsu = a.HasSetsu();
            HasGetsu = a.HasGetsu();
            HasKa = a.HasKa();
        }
    }
    internal class SpecialSpell_MeikyoShisui:SpecialSpellBase
    {
        uint Combo => Core.Get<IMemApiSpell>().GetLastComboSpellId();
        public SpecialSpell_MeikyoShisui() : base(SpellsDefine.MeikyoShisui,false) 
        { 
        }
        public override int Check()
        {
            CheckEffect();
            if (Helper.GetGCDCooldown() < 1100 && Helper.GetGCDCooldown()>2000)
                return -4;
            if (Core.Me.HasAura(AurasDefine.MeikyoShisui) || SamuraiRotationEventHandler.hasMeikyoShisui)
                return -2;
            if (Combo == SpellsDefine.Hakaze || Combo == SpellsDefine.Shifu || Combo == SpellsDefine.Jinpu)
                return -3;
            return base.Check();
        }
        private bool CheckEffect()
        {
            var info = new Info();
            var times = (int)(info.HiganbanaTimeLeft / Core.Get<IMemApiSpell>().GetGCDDuration());
            //LogHelper.Info(times.ToString());

            return false;
        }
    }
}
