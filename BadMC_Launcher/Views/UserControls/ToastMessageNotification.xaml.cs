using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Controls.NotificationItem;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Serilog;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;

public sealed partial class ToastMessageNotification : UserControl {
    private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();

    // Register properties
    public static readonly DependencyProperty NotificationItemProperty = DependencyProperty.Register(
         nameof(NotificationItem),
         typeof(TipMessageNotificationItem),
         typeof(ToastMessageNotification),
         new PropertyMetadata(null)
    );

    public ToastMessageNotification() {
        this.InitializeComponent();
        DelayedHide();
    }

    public event EventHandler? NotificationHided;

    // Properties
    public TipMessageNotificationItem? NotificationItem {
        get => (TipMessageNotificationItem?)GetValue(NotificationItemProperty);
        set => SetValue(NotificationItemProperty, value);
    }

#if WINAPPSDK_PACKAGED
    // WinAppSDK use async
    private async void OnControlLoaded(object sender, RoutedEventArgs e) {
        if (NotificationItem != null) {
            NotificationItem.HideExecuteAction += async () => {
                await CloseNotificationAnimation.StartAsync();
                if (cancellationToken.Token.CanBeCanceled) {
                    cancellationToken.Cancel();
                }
                NotificationHided?.Invoke(this, new EventArgs());
            };

            await OpenNotificationAnimation.StartAsync();

            IsHitTestVisible = true;
        }
    }
#else
    // Non-WinAppSDK use sync
    private void OnControlLoaded(object sender, RoutedEventArgs e) {
        if (NotificationItem != null) {
            NotificationItem.HideExecuteAction += () => {
                if (cancellationToken.Token.CanBeCanceled) {
                    cancellationToken.Cancel();
                }
                NotificationHided?.Invoke(this, new EventArgs());
            };

            Translation = new Vector3(-430, 0, 0);

            IsHitTestVisible = true;
        }
    }
#endif

#if WINAPPSDK_PACKAGED
    // WinAppSDK use async
    private async void OnCloseButtonClick(object sender, RoutedEventArgs e) {
#else
    // Non-WinAppSDK use sync
    private void OnCloseButtonClick(object sender, RoutedEventArgs e) {
#endif
        IsHitTestVisible = false;
#if WINAPPSDK_PACKAGED
        await CloseNotificationAnimation.StartAsync();
#endif
        if (cancellationToken.Token.CanBeCanceled) {
            cancellationToken.Cancel();
        }
        NotificationHided?.Invoke(this, new EventArgs());
    }

    public async void DelayedHide() {
        if (NotificationItem?.PrimaryActionButton != null || NotificationItem?.SecondaryActionButton != null) {
            // If there are buttons, Wait for 10 seconds before hiding
            await Task.Delay(10000, cancellationToken.Token);
        }

        // Wait for 7 seconds before hiding
        try {
            await Task.Delay(7000, cancellationToken.Token);
        }
        catch (TaskCanceledException ex) {
            Log.Information($"ToastMessageNotification DelayHide method is Exited: {ex.Source}");
            return;
        }

#if WINAPPSDK_PACKAGED
        await CloseNotificationAnimation.StartAsync();
#endif

        NotificationHided?.Invoke(this, new EventArgs());
    }

    private Visibility IsStringWhiteSpaceOrEmpty(string? parameter) => string.IsNullOrWhiteSpace(parameter) ? Visibility.Collapsed : Visibility.Visible;
}
