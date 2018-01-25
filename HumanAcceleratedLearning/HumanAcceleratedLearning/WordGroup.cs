using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    public enum WordGroup
    {
        [Description("Paired VNS")]
        PairedVNS,

        [Description("Interleaved VNS")]
        InterleavedVNS,

        [Description("No VNS")]
        NoVNS,

        [Description("Gap")]
        Gap,

        [Description("Message")]
        Message
    }
}
