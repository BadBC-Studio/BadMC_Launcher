using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace BadMC_Launcher.Services;

public class NotificationService {
    public BindingList<INotificationItem> Notifications { get; } = new BindingList<INotificationItem>();

    public Dictionary<Type, UIElement> NotificationControls { get; } = new Dictionary<Type, UIElement>();

    public void ShowNotification(INotificationItem notification) {
        if (notification is not TipNotificationItem) {
            Notifications.Add(notification);

        }
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<INotificationItem>(notification), MessengerTokenEnum.MainPage_ShowNotificationToken.ToString());
    }

    public void CloseNotification(INotificationItem notification) {
        Notifications.Remove(notification);
        notification.InvokeHideExecuteAction();
    }

    public UIElement? GetNotificationControl(INotificationItem notification, Func<INotificationItem, UIElement, UIElement> setControlFunc) {
        var control = NotificationControls.FirstOrDefault(item => item.Key == notification.GetType()).Value;
        return setControlFunc(notification, control);
    }
}
