using Common;
using Common.Define;
using Common.MemoryApi;
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
                582 => Core.Get<IMemApiFunctionPointer>().IsCanQueue(actionId),
                _ => true,
            };
        }
        public static bool Check(this uint actionId)
        {
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
    }
}
