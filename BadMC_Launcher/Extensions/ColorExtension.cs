using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using Windows.UI;

namespace BadMC_Launcher.Extensions;

public static class ColorExtension {
    public static string ToNoAlphaHex(this Color color) => System.Drawing.ColorTranslator.ToHtml(
        System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
    );

    public static Color ToColor(this HslColor hslColor) {
        return CommunityToolkit.WinUI.Helpers.ColorHelper.FromHsl(hslColor.H, hslColor.S, hslColor.L);
    }
}
