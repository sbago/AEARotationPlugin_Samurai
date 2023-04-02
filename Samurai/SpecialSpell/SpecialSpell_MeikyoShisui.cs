using CombatRoutine;
using Common;
using Common.Define;
using Common.Helper;
using Samurai.RotationEventHandler;
using System.Threading.Tasks;

namespace Samurai.SpecialSpell
{
    internal class SpecialSpell_MeikyoShisui:SpellBase
    {
        uint Combo => Core.Get<IMemApiSpell>().GetLastComboSpellId();
        public static uint canid;
        public SpecialSpell_MeikyoShisui() : base(SpellsDefine.MeikyoShisui,false) 
        { 
        }
        public override int Check()
        {
            if (base.Check() < 0)
                return -100;
            if (Helper.GetGCDCooldown() < 1100 && Helper.GetGCDCooldown()>2000)
                return -4;
            if (Core.Me.HasAura(AurasDefine.MeikyoShisui) || SamuraiRotationEventHandler.hasMeikyoShisui)
                return -2;
            if (Combo == SpellsDefine.Hakaze || Combo == SpellsDefine.Shifu || Combo == SpellsDefine.Jinpu || lastadd == SpellsDefine.Hakaze || lastadd == SpellsDefine.Shifu || lastadd == SpellsDefine.Jinpu)
                return -3;
            if (Core.Get<IMemApiSamurai>().Sen() == 3)
                return -4;
            if (!Core.Get<IMemApiSamurai>().HasSetsu())
                return -5;

            var state = new SamuraiState();
            var ret = Monidao(state, out var check, out var over,out var now);
            LogHelper.Info($"check:{check} over:{over} now:{now}");
            if (!ret && now)
                return 1;
            if (over && !check)
            {
                LogHelper.Info("溢出");
                return 2;
            }
            return -1;
        }
        private uint GetBaseSpell(ref SamuraiState state, out bool combo,out bool stack)
        {
            combo = false;
            stack = false;
            if (state.Cooldown.Ikishoten <= 0)
            {
                state.Buff.OgiReadyTimeLeft = 30000;
                state.Cooldown.Ikishoten = 120000;
            }
            if (state.CanUseKaeshiOgiNamikiri())
                return SpellsDefine.KaeshiNamikiri;

            if (state.CanUseOgiNamikiri() && state.Cooldown.KaeshiSetsugekka > 5000)
                return SpellsDefine.OgiNamikiri;

            if (state.CanUseKaeshiSetsugekka())
                return SpellsDefine.KaeshiSetsugekka;

            if (state.CanUseSetsugekka())
                return SpellsDefine.MidareSetsugekka;

            stack = true;
            if (state.State.MeikyoShisuiStackCount > 0)
            {
                if (!state.Sen.HasKa)
                    return SpellsDefine.Kasha;
                else if (!state.Sen.HasGetsu)
                    return SpellsDefine.Gekko;
                else
                    return SpellsDefine.Yukikaze;
            }
            else
            {
                combo = true;
                switch (state.ComboSpellId)
                {
                    case SpellsDefine.Hakaze:
                        if (!state.Sen.HasSetsu)
                            return SpellsDefine.Yukikaze;
                        if (!state.Sen.HasGetsu)
                            return SpellsDefine.Jinpu;
                        else
                            return SpellsDefine.Shifu;
                    case SpellsDefine.Jinpu:
                        return SpellsDefine.Gekko;
                    case SpellsDefine.Shifu:
                        return SpellsDefine.Kasha;
                }
                return SpellsDefine.Hakaze;
            }
        }
        /// <summary>
        /// 模拟
        /// </summary>
        /// <param name="state">模拟状态</param>
        /// <param name="check">标记 true 表示使用明镜后刚好能补dot</param>
        /// <param name="over">标记 true代表 如果不使用明镜 下次补彼岸花之前将要溢出</param>
        /// <param name="now">标记 true 表示现在使用明镜刚好能补dot</param>
        /// <param name="s"></param>
        /// <param name="first">某一瞬时状态 需要转成gcd判定的时候的状态</param>
        /// <returns>返回true 表示不使用明镜也可以刚好补dot</returns>
        public bool Monidao(SamuraiState state, out bool check, out bool over, out bool now, string? s = null, bool first = true)
        {
            check = false;
            over = false;
            now = false;
            for (var i = 0; state.Buff.HiganbanaTimeLeft > 3000;)
            {
                //LogHelper.Info(state.Sen.ToString());
                if (state.CanUseMeiNotXue() && state.Cooldown.MeikyoShisui >= 0)
                { 
                    i++;
                    var s1 = s;
                    s1 += "<明镜止水>->";
                    var info = state;
                    info.UseMei();
                    if (first)
                    {
                        info.Sub(info.GCDCooldown - 187);
                    }
                    for (; info.Buff.HiganbanaTimeLeft > 3000;)
                    {
                        var Id = GetBaseSpell(ref info, out var c, out var st);
                        s1 += $"{Id.GetSpell().LocalizedName}({info.Buff.HiganbanaTimeLeft / 1000:f2})->";
                        Effect(ref info, Id, c, st);
                    }
                    //if (info.CanCastHiganbana())
                    LogHelper.Info($"{i} "+ s1);
                    if ((info.CanCastHiganbana() && state.Buff.HiganbanaTimeLeft > 3000) || (info.Sen.Counts() ==0 && info.ComboSpellId == SpellsDefine.Hakaze))
                    {
                        if (i == 1)
                            now = true;//标记 true 表示现在使用明镜刚好能补dot
                        check = true;//标记 true 表示使用明镜后刚好能补dot
                    }
                }
                if (first)
                {
                    state.Sub(state.GCDCooldown - 185);//
                    first = false;
                }
                var spellId = GetBaseSpell(ref state, out var combo, out var stack);
                s += $"{spellId.GetSpell().LocalizedName}({state.Buff.HiganbanaTimeLeft / 1000:f2})->";
                Effect(ref state, spellId, combo, stack);
            }

            if (state.Cooldown.MeikyoShisui <= 0)
                over = true;//标记 true代表 如果不使用明镜 下次补彼岸花之前将要溢出
            return state.CanCastHiganbana();// 返回true 表示不使用明镜也可以刚好补dot
        }
        private void Effect(ref SamuraiState state, uint spellId,bool combo,bool stack)
        {
            if (combo)
                state.ComboSpellId = spellId;
            if (stack)
                state.State.MeikyoShisuiStackCount--;
            state.State.KaeshiSetsugekka = false;
            state.State.KaeshiOgiNamikiri= false;
            switch (spellId)
            {
                case SpellsDefine.Kasha: state.Sen.HasKa = true; break;
                case SpellsDefine.Gekko: state.Sen.HasGetsu = true; break;
                case SpellsDefine.Yukikaze: state.Sen.HasSetsu = true; break;
                case SpellsDefine.MidareSetsugekka:
                    state.Sen.HasSetsu = false;
                    state.Sen.HasKa = false;
                    state.Sen.HasGetsu = false;
                    state.State.KaeshiSetsugekka = true; break;
                case SpellsDefine.OgiNamikiri: //奥义
                    state.State.KaeshiOgiNamikiri = true;//回返奥义状态
                    state.Buff.OgiReadyTimeLeft = 0;//去掉buff
                    break;
            }
            state.SubGCD();
        }
    }
}
