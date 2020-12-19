using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Models;

namespace HumanAcceleratedLearning.ViewModels
{
    public class Screen_Instructions_ViewModel
    {
        SessionModel _model = SessionModel.GetInstance();

        #region Constructor

        public Screen_Instructions_ViewModel ()
        {
            //empty
        }

        #endregion

        #region Properties

        public string InstructionsText
        {
            get
            {
                var instructions_phase = _model.CurrentPhase as Phase_Instructions;
                if (instructions_phase != null)
                {
                    return instructions_phase.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        #endregion
    }
}
