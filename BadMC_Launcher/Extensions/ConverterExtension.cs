using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Extensions;
public static class ConverterExtension {
    public static double BytesToGb(this ulong bytes) {
        return Math.Round(bytes / ( 1024.0 * 1024.0 * 1024.0 ), 2);
    }

    public static uint BytesToMb(this ulong bytes) {
        return checked((uint)(bytes / ( 1024 * 1024 )));
    }

    public static uint GetAutoGameMemoryMb(this uint maxMemory, uint usedMemory) {
        // TODO: 1.14.514的时候再做（
        return (maxMemory - usedMemory) / 2;
    }
}
