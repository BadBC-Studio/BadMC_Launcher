using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BadMC_Launcher.Controls.NotificationItem;

public partial class TipNotificationItem : ObservableObject, INotificationItem {
    public TipNotificationItem(MessageSeverityEnum notificationSeverity,
        string title,
        string message = "",
        IconSource? notificationIcon = null,
        Brush? notificationColor = null) {
        // Default constructor
        Title = title;
        Message = message;

        // Default severity
        NotificationSeverity = notificationSeverity;

        // Default values
        if (notificationIcon != null) {
            NotificationIcon = notificationIcon;
        }
        else {
            NotificationIcon = NotificationSeverity switch {
                MessageSeverityEnum.Info =>
                    // Info icon
                    new FontIconSource { Glyph = "\uF167" },
                MessageSeverityEnum.Important =>
                    // Important icon
                    new FontIconSource { Glyph = "\uE917" },
                MessageSeverityEnum.Success =>
                    // Success icon
                    new FontIconSource { Glyph = "\uEC61" },
                MessageSeverityEnum.Warning =>
                    // Warning icon
                    new FontIconSource { Glyph = "\uF167" },
                MessageSeverityEnum.Error =>
                    // Error icon
                    new FontIconSource { Glyph = "\uEB90" },
                _ =>
                    // Default icon
                    new FontIconSource() { Glyph = "\uF167" }
            };
        }

        if (notificationColor != null) {
            NotificationColor = notificationColor;
        }
        else {
            NotificationColor = NotificationSeverity switch {
                MessageSeverityEnum.Info => (Brush)Application.Current.Resources["SystemFillColorAttentionBrush"],
                MessageSeverityEnum.Important => (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"],
                MessageSeverityEnum.Success => (Brush)Application.Current.Resources["SystemFillColorSuccessBrush"],
                MessageSeverityEnum.Warning => (Brush)Application.Current.Resources["SystemFillColorCautionBrush"],
                MessageSeverityEnum.Error => (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"],
                _ => (Brush)Application.Current.Resources["SystemFillColorAttentionBrush"],
            };
        }
    }

    public event Action? HideExecuteAction;

    // Notification title
    [ObservableProperty]
    public partial string Title { get; set; }

    // Notification message
    [ObservableProperty]
    public partial string Message { get; set; }

    // Notification severity
    [ObservableProperty]
    public partial MessageSeverityEnum NotificationSeverity { get; set; }

    // Notification icon
    [ObservableProperty]
    public partial IconSource NotificationIcon { get; set; }

    // Notification color
    [ObservableProperty]
    public partial Brush NotificationColor { get; set; }

    public void InvokeHideExecuteAction() {
        HideExecuteAction?.Invoke();
    }

    public void Close() {
        App.GetService<NotificationService>().CloseNotification(this);
    }
}
