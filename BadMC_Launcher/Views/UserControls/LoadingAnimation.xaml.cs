using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;
public sealed partial class LoadingAnimation : UserControl {
    // Register properties
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
         nameof(IsLoading),
         typeof(bool),
         typeof(LoadingAnimation),
         new PropertyMetadata(false, OnIsLoadingChanged)
    );

    public static readonly DependencyProperty LoadDescriptionProperty = DependencyProperty.Register(
        nameof(LoadDescription),
        typeof(string),
        typeof(LoadingAnimation),
        new PropertyMetadata(string.Empty, OnLoadDescriptionChanged)
    );

    public LoadingAnimation() {
        InitializeComponent();
    }

    public bool IsLoading {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public string LoadDescription {
        get => (string)GetValue(LoadDescriptionProperty);
        set => SetValue(LoadDescriptionProperty, value);
    }

    // On property changed
    private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        var control = (LoadingAnimation)d;

        if ((bool)e.NewValue == (bool)e.OldValue) {
            return;
        }
        else if ((bool)e.NewValue) {
#if WINAPPSDK_PACKAGED
            control.LoadBeginAnimation.Start();
#else
            control.Opacity = 1;
#endif

            return;
        }

#if WINAPPSDK_PACKAGED
            control.LoadEndAnimation.Start();
#else
            control.Opacity = 0;
#endif
    }

    private static void OnLoadDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        var control = (LoadingAnimation)d;
        control.LoadLoadDescriptionTextBlock.Text = (string)e.NewValue;
    }
}
