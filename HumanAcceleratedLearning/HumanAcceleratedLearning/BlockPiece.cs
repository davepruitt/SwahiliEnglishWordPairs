using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// Types of block pieces
    /// </summary>
    public enum BlockPiece
    {
        [Description("S")]
        S,

        [Description("Sn")]
        Sn,

        [Description("D")]
        D,

        [Description("T")]
        T,

        [Description("Tn")]
        Tn
    }
}
