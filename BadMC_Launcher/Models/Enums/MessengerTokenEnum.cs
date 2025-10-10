using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Models.Enums;
public enum MessengerTokenEnum {
    #region MainPag
    //Func Token
    MainPage_PageNavigateToken,
    MainPage_PageCloseToken,
    MainPage_PageGoBackToken,
    MainPage_ShowNotificationToken,
    
    //GetValue Token
    MainPage_XamlRootToken,
    #endregion

    #region MinecraftFolderContentDialog
    //Func Token
    MinecraftFolderContentDialog_HideRenameFlyoutToken,
    MinecraftFolderContentDialog_ShowRenameFlyoutToken,
    #endregion

    #region SettingsDashboardPage
    //Func Token
    SettingsDashboardPage_PageNavigateToken
    #endregion
}


