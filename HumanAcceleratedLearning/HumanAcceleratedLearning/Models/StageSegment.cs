using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    public class StageSegment
    {
        #region Constructor

        public StageSegment()
            : base()
        {
            //empty
        }

        #endregion

        #region Properties

        public string SegmentName { get; set; } = string.Empty;

        public List<Phase> Phases { get; set; } = new List<Phase>();

        #endregion

        public void InitializeSegmentFromParameters(List<Tuple<string, string>> parameters, int loop_counter = 0)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                var pname = parameters[i].Item1;
                var pval = parameters[i].Item2;
                if (pname.Equals("name"))
                {
                    SegmentName = pval;
                }
            }
        }
    }
}
