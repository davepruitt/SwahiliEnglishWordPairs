using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Models;

namespace HumanAcceleratedLearning.ViewModels
{
    public class Screen_BlockNumber_ViewModel
    {
        #region Private data members

        SessionModel _model = SessionModel.GetInstance();

        #endregion

        #region Constructor

        public Screen_BlockNumber_ViewModel()
        {
            //empty
        }

        #endregion

        #region Properties

        public string BlockNumber
        {
            get
            {
                var block_phase = _model.CurrentPhase as Phase_Block;
                if (block_phase != null)
                {
                    return block_phase.BlockNumber.ToString();
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
