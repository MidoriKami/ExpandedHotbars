using System.Drawing;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;

namespace ExpandedHotbars.Extensions;

/// <summary>
/// Extension methods for ImGui to make it a bit less tedious and hopefully these functions are helpful and portable.
/// </summary>
public static class ImGuiExtensions {
    extension(ImGui) {
        /// <summary>
        /// Gets remaining area height.
        /// </summary>
        public static float AreaHeight
            => ImGui.GetContentRegionAvail().Y;

        /// <summary>
        /// Gets remaining area width.
        /// </summary>
        public static float AreaWidth
            => ImGui.GetContentRegionAvail().X;

        /// <summary>
        /// Gets remaining area size.
        /// </summary>
        public static Vector2 Area
            => ImGui.GetContentRegionAvail();

        /// <summary>
        /// Gets the remaining area scaled by <see cref="width"/> and <see cref="height"/>.
        /// </summary>
        /// <param name="width">Scale to apply to width, expected 0.0f -> 1.0f</param>
        /// <param name="height">Scale to apply to height, expects 0.0f -> 1.0f</param>
        public static Vector2 RatioArea(float width, float height)
            => new(ImGui.AreaWidth * width, ImGui.AreaHeight * height);

        /// <summary>
        /// Draws the text colored and cenetered, optionally vertically as well.
        /// </summary>
        public static void CenteredText(Vector4 color, string text, bool vertically = false) {
            if (!vertically) {
                using var colorStyle = ImRaii.PushColor(ImGuiCol.Text, color);

                ImGuiHelpers.CenteredText(text);
            }
            else {
                var yPos = ImGui.AreaHeight / 2.0f - ImGui.CalcTextSize(text).Y;

                foreach (var line in text.Split("\n")) {
                    var lineSize = ImGui.CalcTextSize(line);
                    var stringLength = lineSize.X;

                    ImGui.SetCursorPos(new Vector2(ImGui.AreaWidth / 2.0f - stringLength / 2.0f, yPos));
                    yPos += lineSize.Y;

                    ImGui.TextColored(color, line);
                }
            }
        }

        /// <summary>
        /// Draws the text colored and cenetered, optionally vertically as well.
        /// </summary>
        public static void CenteredText(string text, bool vertically = false)
            => ImGui.CenteredText(KnownColor.White.Vector(), text, vertically);

        /// <summary>
        /// Attaches dalamuds ScaledDummy directly to ImGui namespace.
        /// </summary>
        public static void ScaledDummy(float size)
            => ImGuiHelpers.ScaledDummy(size);

        /// <summary>
        /// Draws a label for an element, centering the text to frame padding, and same-lining.
        /// </summary>
        public static void Label(string text) {
            ImGui.AlignTextToFramePadding();
            ImGui.Text(text);
            ImGui.SameLine(ImGui.AreaWidth / 3.0f);
        }

        /// <summary>
        /// Attaches dalamuds ScaledDummy directly to ImGui namespace.
        /// </summary>
        public static Vector2 ScaledVector(float x, float y)
            => ImGuiHelpers.ScaledVector2(x, y);
    }
}
