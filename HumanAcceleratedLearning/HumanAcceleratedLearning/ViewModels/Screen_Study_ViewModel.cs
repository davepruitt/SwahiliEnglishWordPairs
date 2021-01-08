using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Models;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning.ViewModels
{
    public class Screen_Study_ViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        SessionModel _model = SessionModel.GetInstance();

        #endregion

        #region Constructor

        public Screen_Study_ViewModel ()
        {
            _model.PropertyChanged += ExecuteReactionsToModelPropertyChanged;
        }

        #endregion

        #region Properties

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string ForeignWord
        {
            get
            {
                if (_model.CurrentPhase is Phase_Study)
                {
                    var cp = _model.CurrentPhase as Phase_Study;
                    return cp.CurrentForeignLanguageWord;
                }
                
                return string.Empty;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string NativeWord
        {
            get
            {
                if (_model.CurrentPhase is Phase_Study)
                {
                    var cp = _model.CurrentPhase as Phase_Study;
                    return cp.CurrentNativeLanguageWord;
                }
                
                return string.Empty;
            }
        }

        #endregion
    }
}
