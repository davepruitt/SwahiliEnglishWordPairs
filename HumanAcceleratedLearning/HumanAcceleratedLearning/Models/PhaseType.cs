using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    public enum PhaseType
    {
        [Description("Instructions screen")]
        InstructionsScreen,

        [Description("Block number screen")]
        BlockScreen,

        [Description("Study session")]
        StudySession,

        [Description("Distraction screen")]
        DistractionScreen,

        [Description("Test session")]
        TestSession,

        [Description("Object location study session")]
        ObjectLocation_StudySession,

        [Description("Object location test session")]
        ObjectLocation_TestSession,

        [Description("Unknown")]
        Unknown
    }
}
