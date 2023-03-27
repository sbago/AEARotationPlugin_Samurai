using CombatRoutine.View;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Overlay
{
    internal class SamuraiOverlay
    {
        public void Draw()
        {
            ImGui.Text("My Overlay");
            OverlayHelper.DrawCommon();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            OverlayHelper.DrawTriggerlineInfo();
            if (ImGui.Begin("MCH Special", ImGuiWindowFlags.NoBackground))
            {

            }
            ImGui.End();
        }
    }
}
