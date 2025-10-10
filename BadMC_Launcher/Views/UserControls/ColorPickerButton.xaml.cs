using Windows.UI;
using CommunityToolkit.WinUI.Helpers;
using System.ComponentModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;

public sealed partial class ColorPickerButton : UserControl {
    // Register properties
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
         nameof(SelectedColor),
         typeof(Color),
         typeof(ColorPickerButton),
         new PropertyMetadata("#0077FF".ToColor())
    );

    public ColorPickerButton() {
		this.InitializeComponent();
    }

    public event PropertyChangedEventHandler? ColorChanged;

    // Properties
    public Color SelectedColor {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
    
    private void SetColorColorPicker_Loaded(object sender, RoutedEventArgs e) {
        ShowFlyoutAnimation.Start();
        Picker.Color = SelectedColor;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        HideFlyoutAnimation.Start();
        ColorPickerFlyout.Hide();
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e) {
        SelectedColor = Picker.Color;
        ColorChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedColor)));

        HideFlyoutAnimation.Start();
        ColorPickerFlyout.Hide();
    }

    private string ColorToHex(Color color) => color.ToNoAlphaHex();
}
