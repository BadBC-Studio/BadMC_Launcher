using BadMC_Launcher.Models.Enums;
using CommunityToolkit.WinUI.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;

public sealed partial class Badge : UserControl {
    // Register properties
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
         nameof(Text),
         typeof(string),
         typeof(Badge),
         new PropertyMetadata("")
    );

    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
         nameof(Severity),
         typeof(BadgeSeverity),
         typeof(Badge),
         new PropertyMetadata(BadgeSeverity.Info)
    );


    public Badge() {
		this.InitializeComponent();

    }

    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public BadgeSeverity Severity {
        get => (BadgeSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    private SolidColorBrush SeverityToBrush(BadgeSeverity severity) {
        // Default brush if no resource is found
        var defaultBrush = new SolidColorBrush("#0077FF".ToColor());

        // Return brush based on severity
        switch (severity) {
            case BadgeSeverity.Info:
                return App.Current.Resources["AccentFillColorDefaultBrush"] as SolidColorBrush ?? defaultBrush;

            case BadgeSeverity.Warning:
                return App.Current.Resources["SystemFillColorCautionBrush"] as SolidColorBrush ?? defaultBrush;

            case BadgeSeverity.Error:
                return App.Current.Resources["SystemFillColorCriticalBrush"] as SolidColorBrush ?? defaultBrush;

            case BadgeSeverity.Success:
                return App.Current.Resources["SystemFillColorSuccessBrush"] as SolidColorBrush ?? defaultBrush;

            default:
                return App.Current.Resources["AccentFillColorDefaultBrush"] as SolidColorBrush ?? defaultBrush;
        }
    }
}
