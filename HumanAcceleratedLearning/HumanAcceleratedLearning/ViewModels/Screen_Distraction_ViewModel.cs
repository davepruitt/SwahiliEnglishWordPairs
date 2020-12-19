using HumanAcceleratedLearning.Helpers;
using HumanAcceleratedLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HumanAcceleratedLearning.ViewModels
{
    public class Screen_Distraction_ViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        SessionModel _model = SessionModel.GetInstance();

        #endregion

        #region Constructor

        public Screen_Distraction_ViewModel()
        {
            _model.PropertyChanged += ExecuteReactionsToModelPropertyChanged;
        }

        #endregion

        #region Properties
        
        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string DistractorTimeRemaining
        {
            get
            {
                var distraction_phase = _model.CurrentPhase as Phase_Distraction;
                if (distraction_phase != null && distraction_phase.HasPhaseStarted)
                {
                    return distraction_phase.GetTotalSecondsRemaining().ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public Visibility DistractorTextVisibility
        {
            get
            {
                var distraction_phase = _model.CurrentPhase as Phase_Distraction;
                if (distraction_phase != null && distraction_phase.HasPhaseStarted)
                {
                    if (distraction_phase.GetTotalSecondsRemaining() <= 10)
                    {
                        return Visibility.Visible;
                    }
                }

                return Visibility.Hidden;
            }
        }

        #endregion
    }
}
