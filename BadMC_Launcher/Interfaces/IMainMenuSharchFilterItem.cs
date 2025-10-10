using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls.MainSearch;

namespace BadMC_Launcher.Interfaces;

public interface IMainMenuSharchFilterItem {
    public string ItemName { get; init; }

    public string IconGlyph { get; init; }

    public IEnumerable<MainMenuSearchResultItem> Search(string searchText);
}
