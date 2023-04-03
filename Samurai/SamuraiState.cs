using CombatRoutine;
using Common;
using Common.Define;
using Common.Helper;
using Common.MemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai
{
    internal struct SamuraiState
    {
        public Cooldown Cooldown;
        public Sen Sen;
        public Buff Buff;
        public State State;
        public int GCDDuration;
        public uint ComboSpellId;
        public int GCDCooldown;
        public SamuraiState()
        {
            Cooldown = new Cooldown();
            Sen= new Sen();
            Buff = new Buff();
            State = new State();
            GCDDuration = AI.Instance.GetGCDDuration();
            ComboSpellId = Core.Get<IMemApiSpell>().GetLastComboSpellId();
            GCDCooldown = AI.Instance.GetGCDCooldown();
        }
        public bool CanCastHiganbana()
        {
            return Sen.Counts() == 1;
        }
        public bool CanUseMei()
        {
            if (State.MeikyoShisuiStackCount > 0)
                return false;
            if (ComboSpellId == SpellsDefine.Yukikaze || ComboSpellId == SpellsDefine.Kasha || ComboSpellId == SpellsDefine.Gekko)
                if (Cooldown.MeikyoShisui <= 60000)
                    return true;
            return false;
        }
        public void UseMei()
        {
            State.MeikyoShisuiStackCount = 3;
            Cooldown.MeikyoShisui += 55000;
            Buff.MeikyoShisui = 15000;
        }
        public bool CanUseMeiNotXue()
        {
            if (Sen.HasSetsu && Sen.Counts() != 3)
                return CanUseMei();
            return false;
        }
        public bool CanUseKaeshiOgiNamikiri()
        {
            return State.KaeshiOgiNamikiri;
        }
        public bool CanUseOgiNamikiri()
        {
            return Buff.OgiReadyTimeLeft > 0;
        }
        public bool CanUseKaeshiSetsugekka()
        {
            return State.KaeshiSetsugekka && Cooldown.KaeshiSetsugekka < 60000;
        }
        public bool CanUseSetsugekka()
        {
            return Sen.Counts() == 3;
        }
        public void Sub(int time)
        {
            Cooldown.Sub(time);
            if (Buff.MeikyoShisui > 0 && Buff.MeikyoShisui - time < 0)
                State.MeikyoShisuiStackCount = 0;
            Buff.Sub(time);
        }
        public void SubGCD()
        {
            Sub(GCDDuration);
        }
    }
    public struct Cooldown
    {
        public double KaeshiSetsugekka;
        public double Ikishoten;
        public double MeikyoShisui;
        public Cooldown()
        {
            KaeshiSetsugekka = SpellsDefine.KaeshiSetsugekka.GetSpell().Cooldown.TotalMilliseconds;
            Ikishoten = SpellsDefine.Ikishoten.GetSpell().Cooldown.TotalMilliseconds;
            MeikyoShisui = SpellsDefine.MeikyoShisui.GetSpell().Cooldown.TotalMilliseconds;
        }
        public void Sub(int timeInMs)
        {
            //foreach (var i in GetType().GetFields())
            //{
            //    i.SetValue(this, (double)i.GetValue(this)! - timeInMs);
            //    if ((double)i.GetValue(this) < 0)
            //        i.SetValue(this, 0);
            //}
            KaeshiSetsugekka-= timeInMs;
            Ikishoten-= timeInMs;
            MeikyoShisui-= timeInMs;
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
        public override string ToString()
        {
            return $"雪:{HasSetsu} 月:{HasGetsu} 花:{HasKa}";
        }
        public void SetZero()
        {
            HasGetsu = false;
            HasSetsu = false;
            HasKa = false;
        }
        public void Set(int count)
        {
            SetZero();
            switch (count)
            {
                case 1: HasSetsu = true; break;
                case 2: HasSetsu = true; HasGetsu = true; break;
                case 3: HasSetsu = true; HasGetsu = true; HasKa = true; break;
            }
        }
        public int Counts()
        {
            var i = 0;
            if (HasSetsu)
                i++;
            if (HasGetsu) i++;
            if (HasKa) i++;
            return i;
        }
    }
    public struct Buff
    {
        public double HiganbanaTimeLeft;
        public double OgiReadyTimeLeft;
        public double MeikyoShisui;
        public Buff()
        {
            HiganbanaTimeLeft = Core.Me.GetCurrTarget().GetBuffTimespanLeft(AurasDefine.Higanbana).TotalMilliseconds;
            OgiReadyTimeLeft = Core.Me.GetBuffTimespanLeft(AurasDefine.OgiReady).TotalMilliseconds;
            MeikyoShisui = Core.Me.GetBuffTimespanLeft(AurasDefine.MeikyoShisui).TotalMilliseconds;
        }
        public void Sub(int timeInMs)
        {
            //foreach (var i in GetType().GetFields())
            //{
            //    i.SetValue(this, (double)i.GetValue(this) - timeInMs);
            //    if ((double)i.GetValue(this)! < 0)
            //        i.SetValue(this, 0);
            //}
            HiganbanaTimeLeft-= timeInMs;
            OgiReadyTimeLeft-= timeInMs;               
            MeikyoShisui -= timeInMs;
        }
    }
    public struct State
    {
        public bool KaeshiSetsugekka;
        public bool KaeshiOgiNamikiri;
        public int MeikyoShisuiStackCount;
        public State()
        {
            KaeshiSetsugekka = Core.Get<IMemApiFunctionPointer>().CheckActionCanUse(SpellsDefine.KaeshiSetsugekka) != 572;
            KaeshiOgiNamikiri = Core.Get<IMemApiFunctionPointer>().CheckActionCanUse(SpellsDefine.KaeshiNamikiri) != 572;
            MeikyoShisuiStackCount = Core.Get<IMemApiBuff>().GetStack(Core.Me,AurasDefine.MeikyoShisui);
        }
    }
    public struct Result
    {
        public bool NoMei;//不用明镜可以刚好补dot
        public bool IsNow;//现在使用可以刚好补dot
        public bool Check;//使用明镜可以刚好补dot
        public bool Over;//不使用明镜 明镜会溢出
        public bool CastNow()
        {
            if (IsNow && !NoMei && !Over)//
                return true;
            if (Over && !Check)
                return true;
            if (IsNow && Over)
                return true;
            return false;
            //return (IsNow && !NoMei) 
            //    || (Check && Over);
        }
        public override string ToString()
        {
            var s = string.Empty;
            foreach (var a in GetType().GetFields())
            {
                s += a.Name + ":" + a.GetValue(this) + " ";
            }
            return s;
        }
    }
}
