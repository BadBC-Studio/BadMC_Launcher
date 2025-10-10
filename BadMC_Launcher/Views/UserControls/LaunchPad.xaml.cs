
using BadMC_Launcher.ViewModels.UserControls;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Windows.System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;
public sealed partial class LaunchPad : UserControl {
    // Register properties
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
         nameof(IsOpen),
         typeof(bool),
         typeof(LaunchPad),
         new PropertyMetadata(true, OnIsOpenChanged)
    );

    public LaunchPad() {
        DataContext = new LaunchPadViewModel();
        this.InitializeComponent();
    }

    public bool IsOpen {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        var control = (LaunchPad)d;

        if ((bool)e.NewValue == true) {

#if WINAPPSDK_PACKAGED
            control.OpenLaunchPadAnimation.Start();
#else
            control.Translation = new(0);
#endif
            control.LaunchPadBorder.IsHitTestVisible = true;
        }
        else {
#if WINAPPSDK_PACKAGED
            control.CloseLaunchPadAnimation.Start();
#else
            control.Translation = new(320, 0, 0);
#endif
            control.LaunchPadBorder.IsHitTestVisible = false;
        }
    }

    private void OnLaunchPadPointEntered(object sender, PointerRoutedEventArgs e) {
        if (!IsOpen) {
#if WINAPPSDK_PACKAGED
            LaunchPadPointerInAnimation.Start();
#else
            LaunchPadBorder.Translation = new(280, 0, 0);
#endif
        }
    }

    private void OnLaunchPadPointExited(object sender, PointerRoutedEventArgs e) {
        if (!IsOpen) {
#if WINAPPSDK_PACKAGED
            LaunchPadPointerOutAnimation.Start();
#else
            LaunchPadBorder.Translation = new(320, 0, 0);
#endif
        }
    }

    private void OnLaunchPadPointerPressed(object sender, PointerRoutedEventArgs e) {
        if (!IsOpen) {
            IsOpen = true;
        }
    }
}
