using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Models;
using HumanAcceleratedLearning.Helpers;
using HumanAcceleratedLearning.Converters;

namespace HumanAcceleratedLearning.ViewModels
{
    public class Screen_AdminPage_ViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        SessionModel _model = SessionModel.GetInstance();
        ParticipantWindow participant_window = new ParticipantWindow();

        #endregion

        #region Constructor

        public Screen_AdminPage_ViewModel ()
        {
            _model.PropertyChanged += ExecuteReactionsToModelPropertyChanged;
        }

        #endregion

        #region Properties

        public string ParticipantIdentifier
        {
            get
            {
                return _model.UserName;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string CurrentWordPairText
        {
            get
            {
                if (_model.CurrentPhase is Phase_ObjectLocationStudy)
                {
                    return "Current object location";
                }
                else
                {
                    return "Current word pair";
                }
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string ForeignLanguageWord
        {
            get
            {
                if (_model.CurrentPhase is Phase_Study)
                {
                    var cp = _model.CurrentPhase as Phase_Study;
                    return cp.CurrentForeignLanguageWord;
                }
                else if (_model.CurrentPhase is Phase_Test)
                {
                    var cp = _model.CurrentPhase as Phase_Test;
                    return cp.CurrentForeignLanguageCue;
                }
                else if (_model.CurrentPhase is Phase_ObjectLocationStudy)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationStudy;
                    return ("(" + cp.CurrentImageLocationX_Column.ToString() + ", " + cp.CurrentImageLocationY_Row.ToString() + ")");
                }

                return string.Empty;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string EnglishLanguageWord
        {
            get
            {
                if (_model.CurrentPhase is Phase_Study)
                {
                    var cp = _model.CurrentPhase as Phase_Study;
                    return cp.CurrentEnglishLanguageWord;
                }
                else if (_model.CurrentPhase is Phase_Test)
                {
                    var cp = _model.CurrentPhase as Phase_Test;
                    return cp.CurrentEnglishLanguageCorrectAnswer;
                }
                else if (_model.CurrentPhase is Phase_ObjectLocationStudy)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationStudy;
                    
                    string px_x = Convert.ToInt32(cp.CurrentImageLocationX).ToString();
                    string px_y = Convert.ToInt32(cp.CurrentImageLocationY).ToString();
                    return ("px: (" + px_x + ", " + px_y + ")");
                }

                return string.Empty;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public int EnglishLanguageWordFontSize
        {
            get
            {
                if (_model.CurrentPhase is Phase_ObjectLocationStudy)
                {
                    return 18;
                }
                else return 96;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string CurrentBlockProgress
        {
            get
            {
                if (_model.CurrentPhase is Phase_Study)
                {
                    var cp = _model.CurrentPhase as Phase_Study;

                    var words_completed = cp.CurrentPhase_NumberOfWordsCompleted.ToString();
                    var total_words = cp.TotalWords.ToString();
                    return (words_completed + @" / " + total_words);
                }
                else if (_model.CurrentPhase is Phase_Test)
                {
                    var cp = _model.CurrentPhase as Phase_Test;
                    var words_completed = cp.CurrentPhase_NumberOfWordsCompleted.ToString();
                    var total_words = cp.TotalWords.ToString();
                    return (words_completed + @" / " + total_words);
                }
                else if (_model.CurrentPhase is Phase_Distraction)
                {
                    var cp = _model.CurrentPhase as Phase_Distraction;
                    return cp.GetTotalSecondsRemaining().ToString();
                }
                else if (_model.CurrentPhase is Phase_Block)
                {
                    var cp = _model.CurrentPhase as Phase_Block;
                    return cp.GetTotalSecondsRemaining().ToString();
                }
                else if (_model.CurrentPhase is Phase_Instructions)
                {
                    var cp = _model.CurrentPhase as Phase_Instructions;
                    return cp.GetTotalSecondsRemaining().ToString();
                }
                else if (_model.CurrentPhase is Phase_ObjectLocationStudy)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationStudy;
                    return (cp.CurrentImageIndex.ToString() + " / " + cp.TotalImageCount.ToString());
                }
                else if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    return (cp.CurrentImageIndex.ToString() + " / " + cp.ThisPhaseImageCount.ToString());
                }

                return string.Empty;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "__new_frame__" })]
        public string CurrentBlockName
        {
            get
            {
                string result = string.Empty;

                if (_model.CurrentPhase != null)
                {
                    result = PhaseTypeConverter.ConvertPhaseTypeToDescription(_model.CurrentPhase.PhaseType);

                    if (_model.CurrentPhase is Phase_Test)
                    {
                        result += " - " + LanguageClassificationConverter.ConvertLanguageClassificationToDescription((_model.CurrentPhase as Phase_Test).Language);
                    }
                    else if (_model.CurrentPhase is Phase_Study)
                    {
                        result += " - " + LanguageClassificationConverter.ConvertLanguageClassificationToDescription((_model.CurrentPhase as Phase_Study).Language);
                    }
                }

                return result;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "RemainingStudyPhases" })]
        public string RemainingStudyBlockCount
        {
            get
            {
                return _model.RemainingStudyPhases.ToString();
            }
        }

        [ReactToModelPropertyChanged(new string[] { "RemainingTestPhases" })]
        public string RemainingTestBlockCount
        {
            get
            {
                return _model.RemainingTestPhases.ToString();
            }
        }

        [ReactToModelPropertyChanged(new string[] { "ProgramState" })]
        public string AdminButtonContent
        {
            get
            {
                string result = string.Empty;

                if (_model.ProgramState == SessionModel.ProgramStateEnumeration.ParticipantWindowNotOpen)
                {
                    result = "Open participant window";
                }
                else if (_model.ProgramState == SessionModel.ProgramStateEnumeration.NotStarted)
                {
                    result = "Start";
                }
                else if (_model.ProgramState == SessionModel.ProgramStateEnumeration.Paused)
                {
                    result = "Unpause";
                }
                else if (_model.ProgramState == SessionModel.ProgramStateEnumeration.Finished)
                {
                    result = "Finished";
                }
                else
                {
                    result = "Pause";
                }
                
                return result;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "ProgramState" })]
        public bool IsMainButtonEnabled
        {
            get
            {
                return (_model.ProgramState != SessionModel.ProgramStateEnumeration.Finished);
            }
        }

        #endregion

        #region Methods

        public void HandleMainButtonClick ()
        {
            if (_model.ProgramState == SessionModel.ProgramStateEnumeration.ParticipantWindowNotOpen)
            {
                _model.SetState(SessionModel.ProgramStateEnumeration.NotStarted);

                participant_window.Show();
                participant_window.Focus();
            }
            else if (_model.ProgramState == SessionModel.ProgramStateEnumeration.NotStarted)
            {
                _model.SetState(SessionModel.ProgramStateEnumeration.BeginSession);
                _model.StartSession();
            }
            else if (_model.ProgramState == SessionModel.ProgramStateEnumeration.Paused)
            {
                _model.SetState(SessionModel.ProgramStateEnumeration.UpdateCurrentPhase);
            }
            else if (_model.ProgramState == SessionModel.ProgramStateEnumeration.UpdateCurrentPhase)
            {
                _model.SetState(SessionModel.ProgramStateEnumeration.Paused);
            }

            NotifyPropertyChanged("AdminButtonContent");
        }

        #endregion
    }
}
