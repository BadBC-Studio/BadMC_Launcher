using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Enums;

namespace BadMC_Launcher.Models.Data.Mappings;
public static class UpdateMapping {
    public readonly static Dictionary<string, string> minecraftConfigPropertyNameMapping = new() {
        { "OwO", "MinecraftFolderId" }
    };

    public readonly static Dictionary<string, Func<object, object>> minecraftConfigPropertyTypeMapping = new() {

    };
}
