using System.Numerics;
using System.Threading;
using BadMC_Launcher.Controls.NotificationItem;
using Serilog;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;

public sealed partial class TipNotification : UserControl {
    private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();

    // Register properties
    public static readonly DependencyProperty NotificationItemProperty = DependencyProperty.Register(
         nameof(NotificationItem),
         typeof(TipNotificationItem),
         typeof(TipNotification),
         new PropertyMetadata(null)
    );

    public TipNotification() {
		this.InitializeComponent();

        DelayedHide();
    }

    public event EventHandler? NotificationHided;

    // Properties
    public TipNotificationItem? NotificationItem {
        get => (TipNotificationItem?)GetValue(NotificationItemProperty);
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
        // Wait for 6 seconds before hiding
        try {
            await Task.Delay(6000, cancellationToken.Token);
        }
        catch (TaskCanceledException ex) {
            Log.Information($"Tip-Notification DelayHide method is Exited: {ex.Source}");
        }
#if WINAPPSDK_PACKAGED
        await CloseNotificationAnimation.StartAsync();
#endif

            NotificationHided?.Invoke(this, new EventArgs());
    }

    private Visibility IsStringWhiteSpaceOrEmptyToVisibility(string? parameter) {
        return string.IsNullOrWhiteSpace(parameter) ? Visibility.Collapsed : Visibility.Visible;
    }

    private Orientation IsStringWhiteSpaceOrEmptyToOrientation(string? parameter) {
        return string.IsNullOrWhiteSpace(parameter) ? Orientation.Horizontal : Orientation.Vertical;
    }
}

