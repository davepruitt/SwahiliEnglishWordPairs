using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows;
using System.Windows.Media;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// A view-model for the debugging view window.
    /// </summary>
    public class DebuggingViewModel : NotifyPropertyChangedObject
    {
        #region Privata data members

        PlotModel _plot_model = new PlotModel();
        PlotModel _word_groups_plot_model = new PlotModel();
        Model _program_model = Model.GetInstance();
        DateTime last_update = DateTime.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public DebuggingViewModel()
        {
            InitializePlot();
            InitializeAudioListener();
            InitializeWordGroupsPlot();

            _program_model.PropertyChanged += ExecuteReactionsToModelPropertyChanged;
            _program_model.SubscribeToDisplayedWordGroupsCollectionChanges(HandleChangesToWordGroupsCollectionOnModel);
        }

        #endregion

        #region Private methods

        private void InitializeWordGroupsPlot ()
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
                MarkerSize = 4
            };

            ScatterSeries interleaved_vns_series = new ScatterSeries()
            {
                MarkerStroke = OxyColors.Green,
                MarkerFill = OxyColors.Green,
                MarkerSize = 4
            };

            ScatterSeries no_vns_series = new ScatterSeries()
            {
                MarkerStroke = OxyColors.Blue,
                MarkerFill = OxyColors.Blue,
                MarkerSize = 4
            };

            WordGroupsPlot.Series.Add(paired_vns_series);
            WordGroupsPlot.Series.Add(interleaved_vns_series);
            WordGroupsPlot.Series.Add(no_vns_series);
        }

        private void HandleChangesToWordGroupsCollectionOnModel(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<WordGroup> groups = _program_model.GetDisplayedWordGroupsCollection();
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

        private void InitializeAudioListener ()
        {
            TAPSAudioListener.GetInstance().SubscribeToAudioSignalChanges(HandleIncomingAudio);
        }

        public void CloseAudioListener ()
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

        #region Public members

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
                int index_of_word = swahili_words.IndexOf(_program_model.CurrentSwahiliWord);
                if (index_of_word > -1)
                {
                    if (_program_model.CurrentSubject != null)
                    {
                        var word_group = _program_model.CurrentSubject.WordGroups[index_of_word];
                        var word_group_description = WordGroupConverter.ConvertToDescription(word_group);

                        string result = _program_model.CurrentSwahiliWord + @"/" + _program_model.CurrentEnglishWord
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
                return _program_model.CurrentMessage;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "CurrentMessage" })]
        public SolidColorBrush CurrentMessageColor
        {
            get
            {
                if (_program_model.CurrentMessage.Contains("paused"))
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else
                {
                    return new SolidColorBrush(Colors.Green);
                }
            }
        }

        #endregion
    }
}
