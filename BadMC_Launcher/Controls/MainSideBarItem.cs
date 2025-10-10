using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Controls;

public partial class MainSideBarItem : ObservableObject {
    public required string ItemName { get; set; }

    public required IconElement ItemIcon { get; set; }

    public required Type NavigatePage { get; set; }

    [ObservableProperty]
    public partial InfoBadge? ItemInfoBadge { get; set; }
}
