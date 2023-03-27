using Common;
using Common.Define;
using Common.Helper;
using Common.MemoryApi;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Samurai
{
    [StructLayout(LayoutKind.Explicit)]
    struct ActionData
    {
        [FieldOffset(0x2C)] public byte CooldownGroup;
        [FieldOffset(0x2D)] public byte AdditionalCooldownGroup;
        public bool IsGCD()
        {
            return CooldownGroup == 58 || AdditionalCooldownGroup == 58;
        }
    }
    internal static class Helper
    {
        public unsafe static bool CanUse(this uint actionId)
        {
            return Core.Get<IMemApiSpell>().GetActionState(actionId) switch
            {
                572 or 580 or 568 => false,
                582 => ((ActionData*)Core.Get<IMemApiFunctionPointer>().GetActionData(actionId))->IsGCD() && Core.Get<IMemApiFunctionPointer>().IsCanQueue(actionId),
                _ => true,
            } ;
        }
        public unsafe static bool Check(this uint actionId)
        {
            //AE 设置的GCD 300ms开始判断 我们目标200ms再开始判断。目的消除buff延迟的影响
            if (((ActionData*)Core.Get<IMemApiFunctionPointer>().GetActionData(actionId))->IsGCD() && GetGCDCooldown() > 200)
                return false;
            if (CanUse(actionId))
            {
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
            return Core.Get<IMemApiSpell>().GetGCDDuration() - Core.Get<IMemApiSpell>().GetElapsedGCD();
        }
    }
}
