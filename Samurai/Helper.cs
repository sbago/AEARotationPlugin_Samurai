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
                572 or 580 or 568 => false,
                582 => ((ActionData*)Core.Get<IMemApiFunctionPointer>().GetActionData(actionId))->IsGCD() && Core.Get<IMemApiFunctionPointer>().IsCanQueue(actionId),
                _ => true,
            } ;
        }
        public unsafe static bool Check(this uint actionId)
        {
            if (((ActionData*)Core.Get<IMemApiFunctionPointer>().GetActionData(actionId))->IsCastSpell() && Core.Get<IMemApiMove>().IsMoving())
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
        public static void StateWhenNextHiganbana()
        {
            var info = new Info();
            //info.s

        }
    }
}
