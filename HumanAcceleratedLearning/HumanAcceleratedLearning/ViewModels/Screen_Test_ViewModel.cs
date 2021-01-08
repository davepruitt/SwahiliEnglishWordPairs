using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Models;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning.ViewModels
{
    class Screen_Test_ViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        private SessionModel _model = SessionModel.GetInstance();

        #endregion

        #region Constructor

        public Screen_Test_ViewModel ()
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
                if (_model.CurrentPhase is Phase_Test)
                {
                    var cp = _model.CurrentPhase as Phase_Test;
                    return cp.CurrentForeignLanguageCue;
                }

                return string.Empty;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string NativeResponse
        {
            get
            {
                if (_model.CurrentPhase is Phase_Test)
                {
                    var cp = _model.CurrentPhase as Phase_Test;
                    return cp.CurrentNativeLanguageResponse;
                }

                return string.Empty;
            }
            set
            {
                if (_model.CurrentPhase is Phase_Test)
                {
                    var cp = _model.CurrentPhase as Phase_Test;
                    cp.CurrentNativeLanguageResponse = value;
                }
            }
        }

        #endregion
    }
}
