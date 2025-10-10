using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.ViewModels.ContentDialogs.Settings;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace BadMC_Launcher.Views.ContentDialogs.Settings;

public sealed partial class MinecraftFolderContentDialog : ContentDialog {
	public MinecraftFolderContentDialog() {
        this.InitializeComponent();
        DataContext = new MinecraftFolderContentDialogViewModel();

        //Register Messenger
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>, string>(this, MessengerTokenEnum.MinecraftFolderContentDialog_ShowRenameFlyoutToken.ToString(), (r, m) => FlyoutBase.ShowAttachedFlyout(AddMinecraftFolderButton));
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>, string>(this, MessengerTokenEnum.MinecraftFolderContentDialog_HideRenameFlyoutToken.ToString(), (r, m) => RenameFlyout.Hide());
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e) {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
