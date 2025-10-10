using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Services.Settings;
using CommunityToolkit.Mvvm.Input;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;

public partial class JvmArgumentsContentDialogViewModel : ObservableObject {
    private readonly LaunchSettingsService launchSettingsService = App.GetService<LaunchSettingsService>();
    private readonly MinecraftConfigsService minecraftService = App.GetService<MinecraftConfigsService>();

    public JvmArgumentsContentDialogViewModel() {
        minecraftService.PropertyChanged += MinecraftConfig_PropertyChanged;

        JvmArgmentText = string.Empty;
        JvmArgments = new ObservableDataList<JvmArgumentItem>();
        DefaultJvmArgments = launchSettingsService.DefaultJvmArgments;

        JvmArgments.CollectionChanged += OnListChanged;
        IsJvmArgmentsEmpty = !JvmArgments.Any();

        GetJvmArguments();
    }

    [ObservableProperty]
    public partial string JvmArgmentText { get; set; }

    [ObservableProperty]
    public partial ObservableDataList<JvmArgumentItem> JvmArgments { get; set; }

    [ObservableProperty]
    public partial ObservableDataList<JvmArgumentItem> DefaultJvmArgments { get; set; }

    [ObservableProperty]
    public partial bool IsJvmArgmentsEmpty { get; set; }

    [RelayCommand]
    private void AddJvmArgument() {
        if (!string.IsNullOrWhiteSpace(JvmArgmentText)) {
            var item = DefaultJvmArgments.FirstOrDefault(item => item.Argument == JvmArgmentText);
            if (item != null) {
                JvmArgments.Add(item);
                JvmArgmentText = string.Empty;
            }
            else {
                var newItem = new JvmArgumentItem() { Argument = JvmArgmentText };
                JvmArgments.Add(newItem);
                JvmArgmentText = string.Empty;
            }

            // Update setting
            DistinctiveItemBindingList<string> items = new DistinctiveItemBindingList<string>(JvmArgments.Select(item => item.Argument).ToList());
            minecraftService.JvmArguments = items;
        }
    }

    [RelayCommand]
    private void AddJvmArgumentFromItem(AutoSuggestBoxSuggestionChosenEventArgs e) {
        if (e.SelectedItem is JvmArgumentItem selectedJvmArgument) {
            var item = DefaultJvmArgments.FirstOrDefault(item => item.Argument == selectedJvmArgument.Argument);
            if (item != null) {
                JvmArgments.Add(item);
                JvmArgmentText = string.Empty;
            }
            else {
                var newItem = new JvmArgumentItem() { Argument = JvmArgmentText };
                JvmArgments.Add(newItem);
                JvmArgmentText = string.Empty;
            }

            // Update setting
            DistinctiveItemBindingList<string> items = new DistinctiveItemBindingList<string>(JvmArgments.Select(item => item.Argument).ToList());
            minecraftService.JvmArguments = items;
        }
    }

    [RelayCommand]
    private void DeleteJvmArgment(JvmArgumentItem parameter) {
        // Find item
        var deleteItem = JvmArgments.FirstOrDefault(item => item == parameter);
        if (deleteItem != null) {
            JvmArgments.Remove(deleteItem);
            DistinctiveItemBindingList<string> items = new DistinctiveItemBindingList<string>(JvmArgments.Select(item => item.Argument).ToList()) ;
            minecraftService.JvmArguments = items;
        }
    }

    // Get Jvm arguments
    private void GetJvmArguments() {
        // Init list
        JvmArgments.Clear();

        // Add items
        JvmArgments.AddRange(minecraftService.JvmArguments.Select(item => {
            // Find default item
            var defaultAugment = DefaultJvmArgments.FirstOrDefault(defaultItem => defaultItem.Argument == item);
            // If the defaultAugment is not empty, return defaultAugment, otherwise create a new value.
            if (defaultAugment != null) {
                return defaultAugment;
            }

            return new JvmArgumentItem() { Argument = item };
        }));
    }

    private void OnListChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        IsJvmArgmentsEmpty = !JvmArgments.Any();
    }

    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        // Set propertys
        switch (e.PropertyName) {
            case nameof(MinecraftConfigsService.JvmArguments):
                if (!minecraftService.JvmArguments.SequenceEqual(JvmArgments.Select(item => item.Argument))) {
                    GetJvmArguments();
                }
                break;
        }
    }
}
