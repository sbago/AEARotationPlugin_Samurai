using CombatRoutine;
using CombatRoutine.Opener;
using Common;
using Common.Define;

namespace Samurai.Opener
{
    internal class Opener_90 : IOpener
    {
        public uint Level => 90;

        public List<Action<Slot>> Sequence { get; } = new List<Action<Slot>>()
        {
            Step0,
        };

        private static void Step0(Slot obj)
        {
            obj.Add(SpellsDefine.Gekko.GetSpell());
            obj.Add(SpellsDefine.Ikishoten.GetSpell());
            obj.Add(SpellsDefine.Yukikaze.GetSpell());
        }

        public Action CompeltedAction { get; set; } = () => { };

        public void InitCountDown(CountDownHandler countDownHandler)
        {
            countDownHandler.AddAction(10000, SpellsDefine.MeikyoShisui.GetSpell());
            countDownHandler.AddAction(500, SpellsDefine.Kasha.GetSpell());
        }

        public int StartCheck()
        {
            if (!SpellsDefine.Ikishoten.Check())
                return -1;
            if (!SpellsDefine.MeikyoShisui.Check())
                return -2;
            if (Core.Me.InCombat)
                return -3;
            return 1;
        }

        public int StopCheck(int index)
        {
            return -1;
        }

    }
}
