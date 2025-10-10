using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.ViewModels.Pages.Settings;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BadMC_Launcher.Views.Pages.Settings;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsDashboardPage : Page {
    public SettingsDashboardPage() {
        this.InitializeComponent();
        DataContext = new SettingsDashboardPageViewModel();

        WeakReferenceMessenger.Default.Register<ValueChangedMessage<Type>, string>(this, MessengerTokenEnum.SettingsDashboardPage_PageNavigateToken.ToString(), NavigateToPage);
    }

    private void NavigateToPage(object recipient, ValueChangedMessage<Type> message) {
        SideBarFrame.Navigate(message.Value);
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e) {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
