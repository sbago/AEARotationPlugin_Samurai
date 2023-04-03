using CombatRoutine;
using CombatRoutine.Setting;
using Common;
using Common.Define;
using Common.Helper;
using Samurai.RotationEventHandler;
using System;
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
            var ret = Monidao(state, out var result);
            //LogHelper.Info(result.ToString());
            if (ret)
                return 1;
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
        /// <param name="first">某一瞬时状态 需要转成gcd判定的时候的状态</param>
        /// <returns>返回true 表示不使用明镜也可以刚好补dot</returns>
        public bool Monidao(SamuraiState state, out Result result, bool first = true)
        {
            result = new Result();
            var s = string.Empty;
            var list = new List<Dictionary<int, (uint, double)>>();
            var actionlist = new Dictionary<int, (uint, double)>();
            for (var i = 0; state.Buff.HiganbanaTimeLeft > 2300;)
            {
                //LogHelper.Info(state.Sen.ToString());
                if (state.CanUseMeiNotXue() && state.Cooldown.MeikyoShisui > 0)
                { 
                    i++;
                    //var s1 = s;
                    var list1 = actionlist;
                    //s1 += "<明镜止水>->";
                    var info = state;
                    list1.Add(list1.Count + 1, (SpellsDefine.MeikyoShisui, info.Buff.HiganbanaTimeLeft));
                    info.UseMei();
                    if (first)
                    {
                        info.Sub(info.GCDCooldown - SettingMgr.GetSetting<GeneralSettings>().ActionQueueInMs+15);
                    }
                    var v = 0;
                    for (; info.Buff.HiganbanaTimeLeft > 2300;)
                    {
                        if (info.CanUseMeiNotXue() && info.Cooldown.MeikyoShisui > 0)
                        {
                            //var s2 = s1;
                            var list2 = list1;
                            //s2 += "<明镜止水2>->";
                            var info2 = info;
                            list2.Add(list2.Count + 1, (SpellsDefine.MeikyoShisui, info2.Buff.HiganbanaTimeLeft));
                            info2.UseMei();
                            for (; info2.Buff.HiganbanaTimeLeft > 2300;)
                            {
                                var Id2 = GetBaseSpell(ref info2, out var c2, out var st2);
                                //s2 += $"{Id2.GetSpell().LocalizedName}({info2.Buff.HiganbanaTimeLeft / 1000:f2})->";
                                list2.Add(list2.Count + 1, (Id2, info2.Buff.HiganbanaTimeLeft));
                                Effect(ref info2, Id2, c2, st2);
                            }
                            list.Add(list2);
                            //LogHelper.Info($"{i}2 " + s2);
                            if(info2.CanCastHiganbana())
                            {
                                result.IsNow = true;
                            }
                        }
                        v++;
                        var Id = GetBaseSpell(ref info, out var c, out var st);
                        list1.Add(list1.Count + 1, (Id, info.Buff.HiganbanaTimeLeft));
                        //s1 += $"{Id.GetSpell().LocalizedName}({info.Buff.HiganbanaTimeLeft / 1000:f2})->";
                        Effect(ref info, Id, c, st);
                    }
                    //if (info.CanCastHiganbana())
                    //LogHelper.Info($"{i} "+ s1);
                    list.Add(list1);
                    if (info.CanCastHiganbana() || (info.Sen.Counts() ==0 && info.ComboSpellId == SpellsDefine.Hakaze && info.Buff.HiganbanaTimeLeft >1500) && (v > 1 &&state.CanCastHiganbana()))
                    {
                        if (i == 1)
                        {
                            //LogHelper.Info($"Check now{info.State.MeikyoShisuiStackCount}----------------------------------------------------------------------------------");
                            if(info.State.MeikyoShisuiStackCount != 3)
                                result.IsNow = true;//标记 true 表示现在使用明镜刚好能补dot
                        }
                        result.Check = true;//标记 true 表示使用明镜后刚好能补dot
                    }
                }
                if (first)
                {
                    state.Sub(state.GCDCooldown - 185);//
                    first = false;
                }
                var spellId = GetBaseSpell(ref state, out var combo, out var stack);
                actionlist.Add(actionlist.Count + 1, (spellId, state.Buff.HiganbanaTimeLeft));
                //s += $"{spellId.GetSpell().LocalizedName}({state.Buff.HiganbanaTimeLeft / 1000:f2})->";
                Effect(ref state, spellId, combo, stack);
            }
            list.Add(actionlist);
            if (state.Cooldown.MeikyoShisui <= 0)
                result.Over = true;//标记 true代表 如果不使用明镜 下次补彼岸花之前将要溢出
            result.NoMei = state.CanCastHiganbana();// 返回true 表示不使用明镜也可以刚好补dot
            //foreach(var i in list)
            //{
            //    var st = string.Empty;  
            //    foreach(var v in i)
            //    {
            //        if(v.Value.Item1 == SpellsDefine.MeikyoShisui)
            //            st+= $"<{v.Value.Item1.GetSpell().LocalizedName}({v.Value.Item2 / 1000:f2}>) ->";
            //        else st += $"{v.Value.Item1.GetSpell().LocalizedName}({v.Value.Item2 / 1000:f2})->";
            //    }
            //    LogHelper.Info(st);
            //}
            return result.CastNow();
        }
        private void Effect(ref SamuraiState state, uint spellId,bool combo,bool stack)
        {
            if (combo)
                state.ComboSpellId = spellId;
            if (stack && state.State.MeikyoShisuiStackCount >0)
            {
                state.State.MeikyoShisuiStackCount--;
                if (state.State.MeikyoShisuiStackCount <= 0)
                    state.Buff.MeikyoShisui = 0;
            }
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
