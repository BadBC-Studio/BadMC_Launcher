using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using BadMC_Launcher.ViewModels.ContentDialogs.Settings;

namespace BadMC_Launcher.Views.ContentDialogs.Settings;

public sealed partial class JvmArgumentsContentDialog : ContentDialog
{
	public JvmArgumentsContentDialog()
	{
		this.InitializeComponent();
       DataContext = new JvmArgumentsContentDialogViewModel();

   }
}
