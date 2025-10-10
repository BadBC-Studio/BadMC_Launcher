using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BadMC_Launcher.Controls.MainSearch;

public class MainMenuSearchResultItem : IEquatable<MainMenuSearchResultItem> {
    public required string ItemTitle { get; init; }

    public required string ItemSubTitle { get; init; }

    public required FrameworkElement ItemIcon { get; init; }

    public required Action Navigate { get; init; }

    public override string ToString() {
        return ItemTitle;
    }

    public bool Equals(MainMenuSearchResultItem? resultItem) {
        if (resultItem == null) return false;
        return this.ItemTitle == resultItem.ItemTitle 
            && this.ItemSubTitle == resultItem.ItemSubTitle 
            && this.ItemIcon == resultItem.ItemIcon;
    } 

    public override int GetHashCode() {
        int hashItemTitle = ItemTitle.GetHashCode();
        int hashItemSubTitle = ItemSubTitle.GetHashCode();
        int hashItemItemIcon = ItemIcon.GetHashCode();

        return hashItemTitle ^ hashItemSubTitle ^ hashItemItemIcon;
    }
}
