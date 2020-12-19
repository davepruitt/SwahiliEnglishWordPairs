using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning.Models
{
    public abstract class Phase : NotifyPropertyChangedObject
    {
        #region Constructor

        public Phase ()
        {
            //empty
        }

        #endregion

        #region Properties

        public PhaseType PhaseType { get; set; } = PhaseType.Unknown;

        public double Duration { get; set; } = 0;

        public DateTime StartTime { get; set; } = DateTime.MinValue;

        public bool IsPhaseFinished { get; set; } = false;

        public bool HasPhaseStarted { get; set; } = false;

        #endregion

        #region Virtual methods

        public virtual void Start (HumanSubject current_subject = null)
        {
            StartTime = DateTime.Now;
            HasPhaseStarted = true;
        }

        public virtual void Update ()
        {
            //empty
        }

        public virtual void InitializePhaseFromParameters (List<Tuple<string, string>> parameters, int loop_counter = 0)
        {
            //empty
        }

        #endregion
    }
}
