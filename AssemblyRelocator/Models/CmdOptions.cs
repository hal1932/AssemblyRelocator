using HlLib.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyRelocator.Models
{
    class CmdOptions
    {
        [CommandLineArg(name = "nowindow")]
        public bool NoWindow { get; set; } = false;
    }
}
