using CombatRoutine;
using Common.Define;
using Common.Helper;

namespace Samurai.RotationEventHandler
{
    internal class SamuraiRotationEventHandler : IRotationEventHandler
    {
        public static bool hasMeikyoShisui = false;
        public void AfterSpell(Spell spell)
        {
            //LogHelper.Info("After"+spell.LocalizedName);
            if (spell.Id == SpellsDefine.MeikyoShisui)
                hasMeikyoShisui = true;
            hasMeikyoShisui = false;
        }

        public void OnBattleUpdate(int currTime)
        {
            ;
        }

        public Task OnNoTarget()
        {
            return Task.FromResult(true);
        }

        public Task OnPreCombat()
        {
            return Task.FromResult(true);
        }

        public void OnResetBattle()
        {
            ;
        }
    }
}
