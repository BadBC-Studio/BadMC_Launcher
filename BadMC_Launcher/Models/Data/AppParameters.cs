using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Hardware.Info;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using Uno.UI.RemoteControl.Host;
using Windows.Graphics;

namespace BadMC_Launcher.Models.Data;

public static class AppParameters {
    public static SizeInt32 WindowSize { get; } = new() {
        Width = 960,
        Height = 600
    };

    public static HashSet<string> Languages { get; } = new() {
        "zh-Hans", // Chinese (Simplified, China)
        "en-US", // English (United States)
    };
}
