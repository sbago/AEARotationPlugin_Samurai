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
            ImGui.Text("武士存在模拟技能，建议刷新率越高越好");
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
