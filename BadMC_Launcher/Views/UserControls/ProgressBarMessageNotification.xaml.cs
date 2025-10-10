using System.ComponentModel;
using System.Numerics;
using System.Threading;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Models.Enums;
using Serilog;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;

public sealed partial class ProgressBarMessageNotification : UserControl {
    private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();

    // Register properties
    public static readonly DependencyProperty NotificationItemProperty = DependencyProperty.Register(
         nameof(NotificationItem),
         typeof(ProgressBarNotificationItem),
         typeof(ProgressBarMessageNotification),
         new PropertyMetadata(null)
    );

    public ProgressBarMessageNotification() {
        this.InitializeComponent();
    }

    public event EventHandler? NotificationHided;

    // Properties
    public ProgressBarNotificationItem? NotificationItem {
        get => (ProgressBarNotificationItem?)GetValue(NotificationItemProperty);
        set => SetValue(NotificationItemProperty, value);
    }

#if WINAPPSDK_PACKAGED
    // WinAppSDK use async
    private async void OnControlLoaded(object sender, RoutedEventArgs e) {
        if (NotificationItem != null) {
            NotificationItem.ProgressValueChanged += DelayedHide;
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
            NotificationItem.ProgressValueChanged += DelayedHide;
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

    private bool GetProgressBarState(ProgressBarStateEnum progressBarState, string propertyName) {
        return propertyName switch {
            "IsIndeterminate" => progressBarState == ProgressBarStateEnum.Running ? true : false,
            "ShowPaused" => progressBarState == ProgressBarStateEnum.Pause ? true : false,
            "ShowError" => progressBarState == ProgressBarStateEnum.Error ? true : false,
            _ => false
        };
    }

    public async void DelayedHide(object? sender, PropertyChangedEventArgs args) {
        if (NotificationItem?.ProgressValue >= 100) {
            await Task.Delay(7000);

#if WINAPPSDK_PACKAGED
        await CloseNotificationAnimation.StartAsync();
#endif

            NotificationHided?.Invoke(this, new EventArgs());
        }
    }

    private Visibility IsStringWhiteSpaceOrEmpty(string? parameter) => string.IsNullOrWhiteSpace(parameter) ? Visibility.Collapsed : Visibility.Visible;
}
