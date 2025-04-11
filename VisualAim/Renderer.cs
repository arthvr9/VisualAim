using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using ClickableTransparentOverlay;
using ImGuiNET;
using System.Windows.Forms;

namespace VisualAim
{
    public class Renderer : Overlay
    {
        public bool aimbot = true;
        public bool aimOnTeam = false;
        public bool spottedAim = false;
        public float smooth = 5.0f;
        public float fov = 5.0f;

        public static List<Entity> EntitiesToRender = new();

        private float[] viewMatrix = new float[16];

        protected override void Render()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(new Vector2(50, 50), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(250, 250), ImGuiCond.Always);

            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.1f, 0.1f, 0.1f, 0.9f));
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1f, 1f, 0f, 1f));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 6f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 10f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));

            ImGui.Begin("VisualAim", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            ImGui.TextColored(new Vector4(1f, 1f, 0f, 1f), "Aimbot");
            ImGui.Separator();
            ImGui.Checkbox("Aimbot", ref aimbot);
            ImGui.Checkbox("Aim ur bros '_'", ref aimOnTeam);
            ImGui.SliderFloat("FOV", ref fov, 1.0f, 20.0f);
            ImGui.SliderFloat("Smooth", ref smooth, 1.0f, 20.0f);

            ImGui.Spacing();
            ImGui.Spacing();

            ImGui.TextColored(new Vector4(0.5f, 0.8f, 1f, 1f), "Visual");
            ImGui.Separator();

            ImGui.End();

            ImGui.PopStyleColor(2);
            ImGui.PopStyleVar(3);
        }

    }
}
