using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    public class PhaseNode
    {
        #region Constructor

        public PhaseNode ()
        {
            //empty
        }

        #endregion

        #region Properties

        public string Command { get; set; } = string.Empty;
        public List<Tuple<string, string>> Parameters { get; set; } = new List<Tuple<string, string>> ();
        public List<PhaseNode> Children { get; set; } = new List<PhaseNode>();

        #endregion
    }
}
