using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using System.Windows.Media;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// A view-model for the setup screen
    /// </summary>
    public class SetupScreenViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        Model _model = Model.GetInstance();
        PlotModel _plot_model = new PlotModel();
        PlotModel _word_groups_plot_model = new PlotModel();
        DateTime last_update = DateTime.MinValue;
        SessionWindow _session_window = new SessionWindow();
        SimpleCommand _admin_button_command;

        #endregion

        #region Constructor

        public SetupScreenViewModel()
        {
            InitializePlot();
            InitializeAudioListener();
            InitializeWordGroupsPlot();

            ProgramModel.PropertyChanged += ExecuteReactionsToModelPropertyChanged;
            ProgramModel.SubscribeToDisplayedWordGroupsCollectionChanges(HandleChangesToWordGroupsCollectionOnModel);

            this.StartSession();
        }

        #endregion

        #region Private methods

        private void InitializeWordGroupsPlot()
        {
            //Add an x-axis and y-axis to the plot
            LinearAxis x_axis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 151,
                MinimumRange = 151
            };
            
            LinearAxis y_axis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 4,
                MinimumRange = 4
            };

            WordGroupsPlot.Axes.Add(x_axis);
            WordGroupsPlot.Axes.Add(y_axis);

            //Add an area series to the plot
            ScatterSeries paired_vns_series = new ScatterSeries()
            {
                MarkerStroke = OxyColors.Red,
                MarkerFill = OxyColors.Red,
                MarkerSize = 4,
                MarkerType = MarkerType.Triangle
            };

            ScatterSeries interleaved_vns_series = new ScatterSeries()
            {
                MarkerStroke = OxyColors.Green,
                MarkerFill = OxyColors.Green,
                MarkerSize = 4,
                MarkerType = MarkerType.Triangle
            };

            ScatterSeries no_vns_series = new ScatterSeries()
            {
                MarkerStroke = OxyColors.Blue,
                MarkerFill = OxyColors.Blue,
                MarkerSize = 4,
                MarkerType = MarkerType.Triangle
            };

            WordGroupsPlot.Series.Add(paired_vns_series);
            WordGroupsPlot.Series.Add(interleaved_vns_series);
            WordGroupsPlot.Series.Add(no_vns_series);
        }

        private void HandleChangesToWordGroupsCollectionOnModel(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<WordGroup> groups = ProgramModel.GetDisplayedWordGroupsCollection();
            List<int> group_ints = groups.Select(x => (int)x).ToList();

            //Clear all series
            foreach (ScatterSeries s in WordGroupsPlot.Series)
            {
                s.Points.Clear();
            }

            //Fill the series with new points
            for (int i = 0; i < group_ints.Count; i++)
            {
                int x_val = i + 1;
                int y_val = group_ints[i] + 1;
                ScatterSeries s = WordGroupsPlot.Series[group_ints[i]] as ScatterSeries;
                s.Points.Add(new ScatterPoint(x_val, y_val));
            }

            WordGroupsPlot.InvalidatePlot(true);
        }
        
        private void HandleIncomingAudio(object sender, NotifyCollectionChangedEventArgs e)
        {
            TimeSpan elapsed_time = DateTime.Now - last_update;
            if (elapsed_time.TotalMilliseconds >= 33)
            {
                var audio_data = TAPSAudioListener.GetInstance().RetrieveCurrentAudioData();

                var area_series = Plot.Series.FirstOrDefault() as AreaSeries;
                if (area_series != null)
                {
                    List<DataPoint> points = audio_data.Select((y, x) => new DataPoint(x, y)).ToList();
                    area_series.Points.Clear();
                    area_series.Points.AddRange(points);
                    Plot.InvalidatePlot(true);
                }

                last_update = DateTime.Now;
                NotifyPropertyChanged("SoundDetectedVisibility");
            }
        }

        private void InitializeAudioListener()
        {
            TAPSAudioListener.GetInstance().SubscribeToAudioSignalChanges(HandleIncomingAudio);
        }

        public void CloseAudioListener()
        {
            TAPSAudioListener.GetInstance().UnsubscribeToAudioSignalChanges(HandleIncomingAudio);
        }

        private void InitializePlot()
        {
            //Add an x-axis and y-axis to the plot
            LinearAxis x_axis = new LinearAxis()
            {
                Position = AxisPosition.Bottom
            };

            LinearAxis y_axis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 1,
                MinimumRange = 1
            };

            Plot.Axes.Add(x_axis);
            Plot.Axes.Add(y_axis);

            //Add an area series to the plot
            AreaSeries plot_series = new AreaSeries()
            {
                Color = OxyColors.CornflowerBlue
            };
            Plot.Series.Add(plot_series);
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

        #region Methods

        public void StartSession ()
        {
            ProgramModel.StartSession();
        }

        public void StopSession ()
        {
            _session_window.Close();
            ProgramModel.StopSession();
        }

        public void RunAdminButtonCommand ()
        {
            switch (ProgramModel.State)
            {
                case Model.ProgramState.NotOpen:

                    ProgramModel.SetState(Model.ProgramState.OpenAndWaiting);
                    _session_window.Show();
                    _session_window.Focus();

                    break;
                case Model.ProgramState.OpenAndWaiting:

                    ProgramModel.SetState(Model.ProgramState.ReadyToBegin);
                    _session_window.Focus();

                    break;
                case Model.ProgramState.StudySession:

                    ProgramModel.LastButtonPressTime = DateTime.Now;
                    _session_window.Focus();

                    break;

                case Model.ProgramState.Completed:

                    ProgramModel.SetState(Model.ProgramState.NotOpen);
                    _session_window.Close();

                    break;
            }
        }

        #endregion

        #region Properties

        public SimpleCommand AdminButtonCommand
        {
            get
            {
                return _admin_button_command ?? (_admin_button_command = new SimpleCommand(() => RunAdminButtonCommand(), true));
            }
        }

        /// <summary>
        /// Whether or not the "sound detected" text is visible in the debugging GUI
        /// </summary>
        public Visibility SoundDetectedVisibility
        {
            get
            {
                if (TAPSAudioListener.GetInstance().IsAudioPlaying())
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Displays debugging information about the current word/pair
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "CurrentSwahiliWord", "CurrentEnglishWord" })]
        public string CurrentWordInformation
        {
            get
            {
                var config = HumanAcceleratedLearningConfiguration.GetInstance();
                List<string> swahili_words = config.SwahiliEnglishDictionary.Select(x => x.Item1).ToList();
                int index_of_word = swahili_words.IndexOf(ProgramModel.CurrentSwahiliWord);
                if (index_of_word > -1)
                {
                    if (ProgramModel.CurrentSubject != null)
                    {
                        var word_group = ProgramModel.CurrentSubject.WordGroups[index_of_word];
                        var word_group_description = WordGroupConverter.ConvertToDescription(word_group);

                        string result = ProgramModel.CurrentSwahiliWord + @"/" + ProgramModel.CurrentEnglishWord
                            + " is part of the group: " + word_group_description;
                        return result;
                    }
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Displays an message to the administrative user
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "CurrentMessage" })]
        public string CurrentMessage
        {
            get
            {
                return ProgramModel.CurrentMessage;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "TimeRemainingUntilNextAdminInteraction" })]
        public string TimerText
        {
            get
            {
                return ProgramModel.TimeRemainingUntilNextAdminInteraction;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "CurrentMessage" })]
        public SolidColorBrush CurrentMessageColor
        {
            get
            {
                if (ProgramModel.CurrentMessage.Contains("paused"))
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else
                {
                    return new SolidColorBrush(Colors.Green);
                }
            }
        }

        /// <summary>
        /// The plot model for the "word groups" plot
        /// </summary>
        public PlotModel WordGroupsPlot
        {
            get
            {
                return _word_groups_plot_model;
            }
            set
            {
                _word_groups_plot_model = value;
            }
        }

        /// <summary>
        /// The plot model
        /// </summary>
        public PlotModel Plot
        {
            get
            {
                return _plot_model;
            }
            set
            {
                _plot_model = value;
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

        public string BlockCount
        {
            get
            {
                return HumanAcceleratedLearningConfiguration.GetInstance().BlockCount.ToString();
            }
        }

        public string BlockSequence
        {
            get
            {
                var seq = HumanAcceleratedLearningConfiguration.GetInstance().BlockSequence;
                string result = string.Empty;
                foreach (var s in seq)
                {
                    result += BlockPieceConverter.ConvertToDescription(s);
                }

                return result;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "CurrentBlockNumber" })]
        public string CurrentBlockNumber
        {
            get
            {
                return ProgramModel.CurrentBlockNumber.ToString();
            }
        }

        [ReactToModelPropertyChanged(new string[] { "State" })]
        public string CurrentBlockSegment
        {
            get
            {
                string result = string.Empty;

                if (ProgramModel.State == Model.ProgramState.StudySession)
                {
                    result = "Study";
                }
                else if (ProgramModel.State == Model.ProgramState.DistractorPhase)
                {
                    result = "Distractor";
                }
                else if (ProgramModel.State == Model.ProgramState.TestSession)
                {
                    result = "Testing";
                }

                return result;
            }
        }

        public List<string> StageNames
        {
            get
            {
                return HumanAcceleratedLearningConfiguration.GetInstance().Stages.Select(x => x.StageName).ToList();
            }
        }

        public int StageSelectedIndex
        {
            get
            {
                return HumanAcceleratedLearningConfiguration.GetInstance().SelectedStageIndex;
            }
            set
            {
                HumanAcceleratedLearningConfiguration.GetInstance().SelectedStageIndex = value;

                if (HumanAcceleratedLearningConfiguration.GetInstance().SelectedStageIndex == 2)
                {
                    HumanAcceleratedLearningConfiguration.GetInstance().BlockCount = 1;
                    HumanAcceleratedLearningConfiguration.GetInstance().BlockSequence = new List<BlockPiece>()
                    {
                        BlockPiece.T
                    };

                    NotifyPropertyChanged("BlockSequence");
                    NotifyPropertyChanged("BlockCount");
                }
                else
                {
                    HumanAcceleratedLearningConfiguration.GetInstance().BlockSequence = new List<BlockPiece>();
                    HumanAcceleratedLearningConfiguration.GetInstance().LoadConfigurationFile();

                    NotifyPropertyChanged("BlockSequence");
                    NotifyPropertyChanged("BlockCount");
                }
            }
        }

        [ReactToModelPropertyChanged(new string[] { "State", "WaitingForUserPress" })]
        public string AdminButtonContent
        {
            get
            {
                string result = string.Empty;

                switch (ProgramModel.State)
                {
                    case Model.ProgramState.NotOpen:
                        result = "Open user window";
                        break;
                    case Model.ProgramState.OpenAndWaiting:
                        result = "Start user session";
                        break;
                    case Model.ProgramState.StudySession:
                        if (ProgramModel.WaitingForUserPress)
                        {
                            result = "Please click to continue session";
                        }
                        else
                        {
                            result = "Waiting for user session to finish";
                        }
                        break;
                    case Model.ProgramState.Completed:
                        result = "Session complete. Click to close user window.";
                        break;
                    default:
                        result = "Waiting for user session to finish";
                        break;
                }

                return result;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "State" })]
        public bool UserChangesEnabled
        {
            get
            {
                if (ProgramModel.State == Model.ProgramState.NotOpen)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [ReactToModelPropertyChanged(new string[] { "State", "WaitingForUserPress", "UserName" })]
        public bool AdminButtonEnabled
        {
            get
            {
                switch (ProgramModel.State)
                {
                    case Model.ProgramState.NotOpen:
                        if (!string.IsNullOrEmpty(ProgramModel.UserName))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Model.ProgramState.OpenAndWaiting:
                        return true;
                    case Model.ProgramState.StudySession:
                        if (ProgramModel.WaitingForUserPress)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Model.ProgramState.Completed:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public SolidColorBrush AdminButtonTextColor
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        #endregion
    }
}
