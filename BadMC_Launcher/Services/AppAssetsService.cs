using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Enums;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BadMC_Launcher.Services;

public class AppAssetsService {
    public AppAssetsService() {
        foreach (var enumItem in Enum.GetValues(typeof(MinecraftEntryImageEnum)).Cast<MinecraftEntryImageEnum>()) {
            MinecraftImageInstance.Add(enumItem, new Lazy<WeakReference<BitmapImage>>(() => new WeakReference<BitmapImage>(
                new BitmapImage(
                    new Uri($@"ms-appx:///Assets/Icons/MinecraftIcons/{enumItem.ToString().ToLower()}.png")))
                )
            );
        }
    }

    public Dictionary<MinecraftEntryImageEnum, Lazy<WeakReference<BitmapImage>>> MinecraftImageInstance { get; } = new();
}
