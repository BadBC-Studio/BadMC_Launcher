using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using SkiaSharp.Views.Windows;
using Windows.UI;

namespace BadMC_Launcher.Classes.DataClasses;
public struct AccentColors {
    public AccentColors(Color color) {
        var hslColor = color.ToHsl();

        // Light Colors
        LightColorPrimary = ( hslColor with {
            L = hslColor.L + 0.2
        } ).ToColor();
        LightColorSecondary = ( hslColor with {
            S = hslColor.S - 0.3,
            L = hslColor.L + 0.15
        } ).ToColor();
        DarkColorTertiary = ( hslColor with {
            S = hslColor.S - 0.45,
            L = hslColor.L + 0.1
        } ).ToColor();

        // Dark Colors
        DarkColorPrimary = ( hslColor with {
            L = hslColor.L - 0.05
        } ).ToColor();
        DarkColorSecondary = ( hslColor with {
            S = hslColor.S - 0.2,
            L = hslColor.L
        } ).ToColor();
        DarkColorTertiary = ( hslColor with {
            S = hslColor.S - 0.2,
            L = hslColor.L + 0.05
        } ).ToColor();

        // Main Color
        MainColor = color;
    }
    public Color MainColor;

    public Color LightColorPrimary;

    public Color LightColorSecondary;

    public Color LightColorTertiary;

    public Color DarkColorPrimary;

    public Color DarkColorSecondary;

    public Color DarkColorTertiary;
}
