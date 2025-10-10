using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Classes.DataClasses;

public class VersionIsolationFilter {
    public required string Id { get; init; }

    public required string ViewName { get; init; }

    public required IconSource ViewIcon { get; init; }

    public required Func<MinecraftEntry, bool> IsVersionIsolation;

    public static bool operator ==(VersionIsolationFilter? left, VersionIsolationFilter? right) {
        if (left is VersionIsolationFilter && right is VersionIsolationFilter) {
            return left.Equals(right);
        }
        return ReferenceEquals(left, right);
    }

    public static bool operator !=(VersionIsolationFilter? left, VersionIsolationFilter? right) {
        if (left is VersionIsolationFilter && right is VersionIsolationFilter) {
            return !left.Equals(right);
        }
        return !ReferenceEquals(left, right);
    }

    public override bool Equals(object? obj) {
        if (obj is VersionIsolationFilter filter) {
            return Id == filter.Id;
        }
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Id);
    }
}
