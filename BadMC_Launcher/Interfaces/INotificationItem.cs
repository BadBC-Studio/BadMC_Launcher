using BadMC_Launcher.Models.Enums;

namespace BadMC_Launcher.Interfaces;

public interface INotificationItem {
    public event Action? HideExecuteAction;

    // Notification title
    public string Title { get; set; }

    // Notification message
    public string Message { get; set; }

    // Notification icon
    public IconSource NotificationIcon { get; set; }

    // Notification color
    public Brush NotificationColor { get; set; }

    public void InvokeHideExecuteAction();

    public void Close();
}
