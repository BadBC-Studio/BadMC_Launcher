using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions.Toolkit;

namespace BadMC_Launcher.Classes;
public abstract class ConfigClass {
    public abstract bool SyncSettingGet();

    public abstract bool SyncSettingSet();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
