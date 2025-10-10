using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Controls;

public class SettingsSideBarItem {
    public required string ItemName { get; set; }

    public required IconElement ItemIcon { get; set; }

    public required Type NavigatePage { get; set; }

    public Object? PageHead { get; set; }

    public InfoBadge? ItemInfoBadge { get; set; }


}
