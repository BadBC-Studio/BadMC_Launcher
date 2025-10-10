using System;
using System.ComponentModel;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BadMC_Launcher.Controls.NotificationItem;

public partial class ProgressBarNotificationItem : ObservableObject, INotificationItem {
    public ProgressBarNotificationItem(ProgressBarStateEnum notificationState,
        string title,
        int progressValue,
        string message = "",
        string progressTitle = "",
        string progressString = "",
        string progressStatus = "",
        IconSource? notificationIcon = null,
        Brush? notificationColor = null) {
        // Default constructor
        Title = title;
        Message = message;
        ProgressTitle = progressTitle;
        ProgressString = progressString;
        ProgressValue = progressValue;
        ProgressStatus = progressStatus;

        // Default severity
        NotificationState = notificationState;

        // Default values
        if (notificationIcon != null) {
            NotificationIcon = notificationIcon;
        }
        else {
            NotificationIcon = new FontIconSource { Glyph = "\uF167" };
            NotificationIcon = NotificationState switch {
                ProgressBarStateEnum.Error =>
                    // Info icon
                    new FontIconSource { Glyph = "\uEB90" },
                ProgressBarStateEnum.Pause =>
                    // Info icon
                    new FontIconSource { Glyph = "\uF8AE" },
                _ =>
                    // Default icon
                    new FontIconSource() { Glyph = "\uF167" }
            };
        }

        // Default color
        if (notificationColor != null) {
            NotificationColor = notificationColor;
        }
        else {
            NotificationColor = NotificationState switch {
                ProgressBarStateEnum.Running => (Brush)Application.Current.Resources["SystemFillColorAttentionBrush"],
                ProgressBarStateEnum.Pause => (Brush)Application.Current.Resources["SystemFillColorCautionBrush"],
                ProgressBarStateEnum.Error => (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"],
                _ => (Brush)Application.Current.Resources["SystemFillColorAttentionBrush"],
            };
        }
    }

    public event Action? HideExecuteAction;

    public event PropertyChangedEventHandler? ProgressValueChanged;

    // Notification title
    [ObservableProperty]
    public partial string Title { get; set; }

    // Notification message
    [ObservableProperty]
    public partial string Message { get; set; }

    // Notification progress
    [ObservableProperty]
    public partial string ProgressTitle { get; set; }

    [ObservableProperty]
    public partial string ProgressString { get; set; }

    [ObservableProperty]
    public partial int ProgressValue { get; set; }

    [ObservableProperty]
    public partial string ProgressStatus { get; set; }

    // Notification severity
    [ObservableProperty]
    public partial ProgressBarStateEnum NotificationState { get; set; }

    // Notification icon
    [ObservableProperty]
    public partial IconSource NotificationIcon { get; set; }

    // Notification color
    [ObservableProperty]
    public partial Brush NotificationColor { get; set; }

    // Primary button properties
    [ObservableProperty]
    public partial ButtonBase? PrimaryActionButton { get; set; }

    // Secondary button properties
    [ObservableProperty]
    public partial ButtonBase? SecondaryActionButton { get; set; }

    public void InvokeHideExecuteAction() {
        HideExecuteAction?.Invoke();
    }

    public void Close() {
        App.GetService<NotificationService>().CloseNotification(this);
    }

    partial void OnProgressValueChanged(int value) {
        ProgressValueChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressValue)));
    }
}
