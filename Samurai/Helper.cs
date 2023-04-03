using CombatRoutine;
using Common;
using Common.Define;
using Common.Helper;
using Common.MemoryApi;
using Samurai.SpecialSpell;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Samurai
{
    [StructLayout(LayoutKind.Explicit)]
    struct ActionData
    {
        [FieldOffset(0x14)] public ushort Cast100ms;
        [FieldOffset(0x2C)] public byte CooldownGroup;
        [FieldOffset(0x2D)] public byte AdditionalCooldownGroup;
        public bool IsGCD()
        {
            return CooldownGroup == 58 || AdditionalCooldownGroup == 58;
        }
        public bool IsCastSpell()
        {
            return Cast100ms > 0;
        }
    }
    internal static class Helper
    {
        public unsafe static bool CanUse(this uint actionId)
        {
            return Core.Get<IMemApiSpell>().GetActionState(actionId) switch
            {
                //感谢gamous 提供 582-冷却中 581-演出限制（无buff） 579-状态限制（有buff） 573-未学会（包括等级不足） 572-技能附加条件未满足（忍者忍术条件有另外的判定） 568-没蓝 580-咏唱中 0 可用 
                0 => true,
                572 or 580 or 568 or 573 or 581 or 579 => false,
                //todo 应该进步判断有cd的gcd技能问题
                582 => ((ActionData*)Core.Get<IMemApiFunctionPointer>().GetActionData(actionId))->IsGCD() && Core.Get<IMemApiFunctionPointer>().IsCanQueue(actionId),
                _ => false,
            } ;
        }
        public unsafe static bool Check(this uint actionId)
        {
            //移动中判定咏唱技能
            if (((ActionData*)Core.Get<IMemApiFunctionPointer>().GetActionData(actionId))->IsCastSpell() && Core.Get<IMemApiMove>().IsMoving())
                return false;
            if (CanUse(actionId))
            {
                //距离判定。 todo fcs 似乎给了另一个简化距离判定的call
                return Core.Get<IMemApiFunctionPointer>().CheckActionCanUse(actionId) != 572 && Core.Get<IMemApiSpell>().GetActionInRangeOrLoS(actionId) != 566;
            }
            return false;
        }
        public static bool Check(this Spell spell)
        {
            return spell.Id.Check();
        }
        public static int GetGCDCooldown()
        {           
            return AI.Instance.GetGCDCooldown();
        }
        public static void StateWhenNextHiganbana()
        {
            //info.s

        }
    }
}
