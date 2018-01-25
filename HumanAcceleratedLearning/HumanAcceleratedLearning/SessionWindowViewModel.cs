using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// View-model class for the main window.
    /// </summary>
    public class SessionWindowViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        Model _model = Model.GetInstance();

        SimpleCommand _start_button_command;
        SimpleCommand _begin_test_command;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SessionWindowViewModel()
        {
            ProgramModel.PropertyChanged += ExecuteReactionsToModelPropertyChanged;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// The program model
        /// </summary>
        private Model ProgramModel
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines the visibility of the welcome screen
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "State" })]
        public Visibility WelcomeScreenVisibility
        {
            get
            {
                if (ProgramModel.State == Model.ProgramState.OpenAndWaiting)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Determines the visibility of the study screen
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "State" })]
        public Visibility StudyScreenVisibility
        {
            get
            {
                if (ProgramModel.State == Model.ProgramState.StudySession)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Determines the visibility of the testing screen.
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "State" })]
        public Visibility TestScreenVisibility
        {
            get
            {
                if (ProgramModel.State == Model.ProgramState.TestSession)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// The visibility of the transition screen inbetween the study and testing sessions.
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "State" })]
        public Visibility DistractionScreenGUI
        {
            get
            {
                if (ProgramModel.State == Model.ProgramState.DistractorPhase)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// The visibility of the block number screen
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "State" })]
        public Visibility BlockNumberGUI
        {
            get
            {
                if (ProgramModel.State == Model.ProgramState.BlockBeginning)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Visibility of the distractor text
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "SecondsRemainingOfDistractor", "State" })]
        public Visibility DistractorTextVisibility
        {
            get
            {
                if (ProgramModel.State == Model.ProgramState.DistractorPhase)
                {
                    if (ProgramModel.SecondsRemainingOfDistractor <= 5)
                    {
                        return Visibility.Visible;
                    }
                }

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// How much time is remaining of the distractor phase
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "SecondsRemainingOfDistractor" })]
        public string DistractorTimeRemaining
        {
            get
            {
                return ProgramModel.SecondsRemainingOfDistractor.ToString();
            }
        }

        /// <summary>
        /// The current block number being run
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "CurrentBlockNumber" })]
        public string BlockNumber
        {
            get
            {
                return ("Block " + ProgramModel.CurrentBlockNumber.ToString());
            }
        }

        /// <summary>
        /// The visibility of the closing screen
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "State" })]
        public Visibility ClosingScreenVisibility
        {
            get
            {
                if (ProgramModel.State == Model.ProgramState.Completed)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Command executed by the start button in the GUI
        /// </summary>
        public SimpleCommand StartButtonCommand
        {
            get
            {
                return _start_button_command ?? (_start_button_command = new SimpleCommand(() => Start(), true));
            }
        }

        public SimpleCommand BeginTestCommand
        {
            get
            {
                return _begin_test_command ?? (_begin_test_command = new SimpleCommand(() => BeginTest(), true));
            }
        }

        /// <summary>
        /// The current username
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "UserName" })]
        public string UserName
        {
            get
            {
                return ProgramModel.UserName;
            }
            set
            {
                ProgramModel.UserName = ViewHelperMethods.CleanInput(value).Trim().ToUpper();
            }
        }

        /// <summary>
        /// Whether or not to enable the start button
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "UserName" })]
        public bool IsStartButtonEnabled
        {
            get
            {
                if (!string.IsNullOrEmpty(ProgramModel.UserName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The color of the start button
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "UserName" })]
        public SolidColorBrush StartButtonColor
        {
            get
            {
                if (IsStartButtonEnabled)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Gray);
                }
            }
        }

        /// <summary>
        /// The current Swahili word being displayed
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "CurrentSwahiliWord" })]
        public string SwahiliWord
        {
            get
            {
                return ProgramModel.CurrentSwahiliWord;
            }
        }

        /// <summary>
        /// The current English word being displayed
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "CurrentEnglishWord" })]
        public string EnglishWord
        {
            get
            {
                return ProgramModel.CurrentEnglishWord;
            }
        }

        /// <summary>
        /// The english response by the user
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "CurrentEnglishResponse" })]
        public string EnglishResponse
        {
            get
            {
                return ProgramModel.CurrentEnglishResponse;
            }
            set
            {
                ProgramModel.CurrentEnglishResponse = value;
            }
        }

        /// <summary>
        /// The color of the english word the user is typing
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "CurrentEnglishResponse" })]
        public SolidColorBrush EnglishResponseColor
        {
            get
            {
                if (ProgramModel.CurrentEnglishResponse == ProgramModel.CurrentEnglishWord)
                {
                    return new SolidColorBrush(Colors.SpringGreen);
                }
                else
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Starts the study/test session program
        /// </summary>
        public void Start ()
        {
            ProgramModel.StartSession();
        }

        /// <summary>
        /// Cancels a session that is currently running
        /// </summary>
        public void Stop ()
        {
            ProgramModel.StopSession();
        }

        /// <summary>
        /// Begins the testing portion of the session.
        /// </summary>
        public void BeginTest()
        {
            ProgramModel.BeginTestFlag = true;
        }

        #endregion
    }
}
