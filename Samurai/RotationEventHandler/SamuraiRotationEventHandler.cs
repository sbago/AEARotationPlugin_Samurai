using CombatRoutine;
using Common.Define;

namespace Samurai.RotationEventHandler
{
    internal class SamuraiRotationEventHandler : IRotationEventHandler
    {
        public void AfterSpell(Spell spell)
        {
            ;
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
