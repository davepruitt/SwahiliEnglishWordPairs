using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// The model class for the whole app
    /// </summary>
    public class Model : NotifyPropertyChangedObject
    {
        /// <summary>
        /// This enum defines the different possible states of the program
        /// </summary>
        public enum ProgramState
        {
            NotOpen,
            OpenAndWaiting,
            ReadyToBegin,
            BlockBeginning,
            StudySession,
            DistractorPhase,
            TestSession,
            Completed
        }

        #region Singleton constructor

        private static Model _instance = null;
        private static object _instance_lock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        private Model()
        {
            //empty
        }

        public static Model GetInstance ()
        {
            if (_instance == null)
            {
                lock(_instance_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Model();
                    }
                }
            }

            return _instance;
        }

        #endregion

        #region Background property changed

        private List<string> propertyNames = new List<string>();
        private object propertyNamesLock = new object();

        private void BackgroundPropertyChanged(string name)
        {
            lock (propertyNamesLock)
            {
                propertyNames.Add(name);
            }
        }

        #endregion

        #region Private data members

        private ProgramState _state = ProgramState.NotOpen;
        private BackgroundWorker _background_thread = null;
        
        private string _username = string.Empty;
        private string _current_swahili_word = string.Empty;
        private string _current_english_word = string.Empty;
        private string _current_english_response = string.Empty;
        private string _message_text = string.Empty;
        private ObservableCollection<WordGroup> _displayed_word_groups = new ObservableCollection<WordGroup>();
        private object _displayed_word_groups_lock = new object();
        private bool _begin_test_flag = false;
        private int _current_block = 0;
        private BlockPiece _current_block_piece = BlockPiece.S;
        private int _seconds_remaining_of_distractor = 0;
        private bool _waiting_for_user_press = false;
        private string _time_remaining_until_next_admin_interaction = string.Empty;

        private DateTime _last_button_press_time = DateTime.MinValue;
        
        public HumanSubject CurrentSubject = null;

        #endregion

        #region Public data members

        public string TimeRemainingUntilNextAdminInteraction
        {
            get
            {
                return _time_remaining_until_next_admin_interaction;
            }
            set
            {
                _time_remaining_until_next_admin_interaction = value;
                BackgroundPropertyChanged("TimeRemainingUntilNextAdminInteraction");
            }
        }

        public bool WaitingForUserPress
        {
            get
            {
                return _waiting_for_user_press;
            }
            set
            {
                _waiting_for_user_press = value;
                BackgroundPropertyChanged("WaitingForUserPress");
            }
        }

        public DateTime LastButtonPressTime
        {
            get
            {
                return _last_button_press_time;
            }
            set
            {
                _last_button_press_time = value;
            }
        }

        public int SecondsRemainingOfDistractor
        {
            get
            {
                return _seconds_remaining_of_distractor;
            }
            private set
            {
                _seconds_remaining_of_distractor = value;
                BackgroundPropertyChanged("SecondsRemainingOfDistractor");
            }
        }

        public bool BeginTestFlag
        {
            get
            {
                return _begin_test_flag;
            }
            set
            {
                _begin_test_flag = value;
            }
        }

        /// <summary>
        /// The username
        /// </summary>
        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;

                //Create a new human subject with this username, or load in the subject's data.
                if (HumanSubject.DoesSubjectExist(_username))
                {
                    CurrentSubject = HumanSubject.LoadHumanSubject(_username);
                }
                else
                {
                    CurrentSubject = HumanSubject.CreateNewSubjectData(_username);
                    HumanSubject.SaveHumanSubject(CurrentSubject);
                }

                NotifyPropertyChanged("UserName");
            }
        }

        /// <summary>
        /// The current program state
        /// </summary>
        public ProgramState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                BackgroundPropertyChanged("State");
            }
        }

        /// <summary>
        /// The current message to the administrative user
        /// </summary>
        public string CurrentMessage
        {
            get
            {
                return _message_text;
            }
            private set
            {
                _message_text = value;
                BackgroundPropertyChanged("CurrentMessage");
            }
        }

        /// <summary>
        /// The current Swahili word that is being displayed to the user
        /// </summary>
        public string CurrentSwahiliWord
        {
            get
            {
                return _current_swahili_word;
            }
            private set
            {
                _current_swahili_word = value;
                BackgroundPropertyChanged("CurrentSwahiliWord");
            }
        }

        /// <summary>
        /// The current English word that is being displayed to the user
        /// </summary>
        public string CurrentEnglishWord
        {
            get
            {
                return _current_english_word;
            }
            set
            {
                _current_english_word = value;
                BackgroundPropertyChanged("CurrentEnglishWord");
            }
        }

        /// <summary>
        /// The english word response by the user during the testing phase
        /// </summary>
        public string CurrentEnglishResponse
        {
            get
            {
                return _current_english_response;
            }
            set
            {
                _current_english_response = value;
                NotifyPropertyChanged("CurrentEnglishResponse");
            }
        }

        /// <summary>
        /// The current block being run by the background thread
        /// </summary>
        public int CurrentBlockNumber
        {
            get
            {
                return _current_block;
            }
            private set
            {
                _current_block = value;
                BackgroundPropertyChanged("CurrentBlockNumber");
            }
        }

        #endregion

        #region Debugging Methods

        /// <summary>
        /// Adds a "word group" to the list that is associated with each word-pair that has been displayed to the user so far.
        /// </summary>
        private void AddWordGroupToDisplayedGroupsCollection (WordGroup g)
        {
            lock(_displayed_word_groups_lock)
            {
                _displayed_word_groups.Add(g);
            }
        }

        /// <summary>
        /// Returns a list of "word groups" associated with each word-pair that have been displayed to the user so far.
        /// </summary>
        public List<WordGroup> GetDisplayedWordGroupsCollection ()
        {
            List<WordGroup> result;
            lock(_displayed_word_groups_lock)
            {
                result = _displayed_word_groups.ToList();
            }

            return result;
        }

        /// <summary>
        /// Clears the list of displayed word groups
        /// </summary>
        public void ClearWordGroupToDisplayedGroupsCollection ()
        {
            lock (_displayed_word_groups_lock)
            {
                _displayed_word_groups.Clear();
            }
        }

        /// <summary>
        /// Allows another class to subscribe to changes on the "displayed word groups" collection.
        /// </summary>
        public void SubscribeToDisplayedWordGroupsCollectionChanges (System.Collections.Specialized.NotifyCollectionChangedEventHandler h)
        {
            lock(_displayed_word_groups_lock)
            {
                _displayed_word_groups.CollectionChanged -= h;
                _displayed_word_groups.CollectionChanged += h;
            }
        }

        #endregion

        #region Methods
        
        public void SetState (ProgramState k)
        {
            State = k;
            _background_thread.ReportProgress(0);
        }

        /// <summary>
        /// Starts a new session
        /// </summary>
        public void StartSession ()
        {
            if (_background_thread == null)
            {
                _background_thread = new BackgroundWorker();
                _background_thread.WorkerSupportsCancellation = true;
                _background_thread.WorkerReportsProgress = true;
                _background_thread.DoWork += RunSession;
                _background_thread.ProgressChanged += SessionProgressChanged;
                _background_thread.RunWorkerCompleted += CloseBackgroundThread;
            }

            if (_background_thread != null && !_background_thread.IsBusy)
            {
                _background_thread.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Cancels a session that is currently running
        /// </summary>
        public void StopSession ()
        {
            if (_background_thread != null)
            {
                _background_thread.CancelAsync();
            }
        }

        private void CloseBackgroundThread(object sender, RunWorkerCompletedEventArgs e)
        {
            //empty
        }

        private void SessionProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (propertyNamesLock)
            {
                foreach (var name in propertyNames)
                {
                    NotifyPropertyChanged(name);
                }

                propertyNames.Clear();
            }
        }

        private void RunSession(object sender, DoWorkEventArgs e)
        {
            while (!_background_thread.CancellationPending)
            {
                switch (State)
                {
                    case ProgramState.NotOpen:
                        break;
                    case ProgramState.OpenAndWaiting:
                        break;
                    case ProgramState.Completed:
                        break;
                    default:

                        int number_of_blocks = HumanAcceleratedLearningConfiguration.GetInstance().BlockCount;

                        for (int i = 0; i < number_of_blocks; i++)
                        {
                            //Display the block beginning
                            SetState(ProgramState.BlockBeginning);

                            //Clear the list of word groups being displayed
                            ClearWordGroupToDisplayedGroupsCollection();

                            CurrentBlockNumber = i + 1;
                            RunBlockBeginning();

                            var block_sequence = HumanAcceleratedLearningConfiguration.GetInstance().BlockSequence.ToList();

                            while (block_sequence.Count > 0 && !_background_thread.CancellationPending)
                            {
                                //Pop the next item in the sequence
                                var next_sequence_item = block_sequence[0];
                                block_sequence.RemoveAt(0);

                                //Set the next sequence item as the current block piece
                                _current_block_piece = next_sequence_item;

                                if (_current_block_piece == BlockPiece.S ||
                                    _current_block_piece == BlockPiece.Sn)
                                {
                                    SetState(ProgramState.StudySession);
                                    RunStudySession();
                                }
                                else if (_current_block_piece == BlockPiece.T ||
                                    _current_block_piece == BlockPiece.Tn)
                                {
                                    SetState(ProgramState.TestSession);
                                    RunTestSession();
                                }
                                else
                                {
                                    SetState(ProgramState.DistractorPhase);
                                    RunDistractorPhase();
                                }
                            }
                        }

                        SetState(ProgramState.Completed);

                        break;

                }
            }
          
        }

        private void RunBlockBeginning ()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            while (!_background_thread.CancellationPending)
            {
                //Break out of the loop if the timer has reached 30 seconds
                if (timer.Elapsed.TotalSeconds >= 5)
                {
                    break;
                }

                //Report progress to the UI
                _background_thread.ReportProgress(0);

                //Sleep the thread for a little while so we don't consume the processor
                Thread.Sleep(33);
            }

            return;
        }

        private List<int> GetCorrectWordsFromPreviousTests ()
        {
            List<TestBlock> tests_from_today = TestBlock.LoadTestBlockFilesFromToday(CurrentSubject.UserName);
            List<int> result = new List<int>();

            foreach (var t in tests_from_today)
            {
                var r = t.Trials.Where(x => x.Correct == 1).Select(x => x.WordPairID).ToList();
                result.AddRange(r);
            }

            return result;
        }

        private Tuple<List<int>, List<WordGroup>> GetWordsToUse ()
        {
            //Load in the latest testing session for this user and check what words the user got correct/incorrect
            List<int> all_words_ids = CurrentSubject.WordGroups.Keys.ToList();

            List<int> word_ids_to_use = new List<int>();
            List<WordGroup> word_groups_to_use = new List<WordGroup>();

            List<int> correct_word_ids = GetCorrectWordsFromPreviousTests();
            
            if (_current_block_piece == BlockPiece.S || 
                _current_block_piece == BlockPiece.T ||
                correct_word_ids.Count == 0)
            {
                word_ids_to_use = all_words_ids;
            }
            else
            {
                //In this case, the "block piece" is "Sn" or "Tn", AND the user has had correct guesses on previous tests blocks
                //Filter word out of the master list that the user got correct on the last test
                word_ids_to_use = Enumerable.Except(all_words_ids, correct_word_ids).ToList();
            }

            word_groups_to_use = new List<WordGroup>();
            for (int i = 0; i < word_ids_to_use.Count; i++)
            {
                word_groups_to_use.Add(CurrentSubject.WordGroups[word_ids_to_use[i]]);
            }

            //Return the words to use
            Tuple<List<int>, List<WordGroup>> result = new Tuple<List<int>, List<WordGroup>>(word_ids_to_use, word_groups_to_use);
            return result;
        }

        private void RunStudySession ()
        {
            //If the current subject has not been defined, return now.
            if (CurrentSubject == null)
            {
                return;
            }

            //Load in the latest testing session for this user and check what words the user got correct/incorrect
            var words_to_use_tuple = GetWordsToUse();
            List<int> word_ids_to_use = words_to_use_tuple.Item1;
            List<WordGroup> word_groups_to_use = words_to_use_tuple.Item2;

            //Create a file for this study session
            var fid = StudyBlock.CreateStudyBlockFile(CurrentSubject.UserName);

            //Get the current stage
            var stage = HumanAcceleratedLearningConfiguration.GetInstance().Stages[HumanAcceleratedLearningConfiguration.GetInstance().SelectedStageIndex];
            bool first = (CurrentBlockNumber % 2 == 1);
            bool vns_subject = false;
            if (stage.StageName.Contains("VNS"))
            {
                vns_subject = true;
            }
            
            stage = HumanAcceleratedLearningConfiguration.GetInstance().LoadStage(vns_subject, first);

            int stage_sequence_index = -1;
            bool act_on_sequence_element = true;
            
            double this_element_duration = 0;
            double this_element_elapsed_time_ms = Double.MaxValue;
            DateTime this_element_start_time = DateTime.MinValue;
            DateTime last_element_finish_time = DateTime.MinValue;
            DateTime last_tone_detection_time = DateTime.MinValue;
            DateTime next_admin_interaction_time = DateTime.MinValue;
            
            bool done = false;
            while (!done)
            {
                //Check to see if a tone is being played by the TAPS program
                bool is_tone_detected = TAPSAudioListener.GetInstance().DetectToneStart();

                //If a tone is detected, then we need to fit a paired VNS word into the schedule of words to display
                if (is_tone_detected)
                {
                    //Set the last tone detection time to now
                    last_tone_detection_time = DateTime.Now;

                    //Output to the study file indicating that tone occurred
                    StudyBlock.WriteStudyTone(fid, last_tone_detection_time);
                }

                if (act_on_sequence_element)
                {
                    act_on_sequence_element = false;
                    stage_sequence_index++;

                    if (stage_sequence_index >= stage.StageSequence.Count)
                    {
                        done = true;
                        break;
                    }
                    else
                    {
                        var current_element = stage.StageSequence[stage_sequence_index];

                        if (stage_sequence_index == 0)
                        {
                            this_element_start_time = DateTime.Now;
                        }
                        else
                        {
                            this_element_start_time = last_element_finish_time;
                        }
                        
                        var group = current_element.Item1;
                        this_element_duration = current_element.Item2;
                        
                        var indices_of_words_of_this_group = Enumerable.Range(0, word_groups_to_use.Count)
                            .Where(x => word_groups_to_use[x] == group).ToList();
                        int index_of_word_to_choose = -1;
                        if (indices_of_words_of_this_group.Count > 0)
                        {
                            int meta_index_of_word_to_choose = MathHelperMethods.RandomNumbers.Next(indices_of_words_of_this_group.Count);
                            index_of_word_to_choose = indices_of_words_of_this_group[meta_index_of_word_to_choose];
                        }

                        string swahili_word = string.Empty;
                        string english_word = string.Empty;
                        if (group == WordGroup.Message)
                        {
                            if (this_element_duration == 1)
                            {
                                CurrentMessage = "Please verify that TAPS is now running";
                            }
                            else
                            {
                                CurrentMessage = "Please verify that TAPS is now paused";
                            }

                            this_element_duration = 0;
                        }
                        else if (group == WordGroup.Gap || index_of_word_to_choose == -1)
                        {
                            swahili_word = string.Empty;
                            english_word = string.Empty;

                            //Set the GUI to display the words
                            CurrentSwahiliWord = swahili_word;
                            CurrentEnglishWord = english_word;
                        }
                        else
                        {
                            //Fetch the index of the word pair into the master list of word-pairs, as well as the word group
                            int word_pair_to_use = word_ids_to_use[index_of_word_to_choose];
                            WordGroup word_pair_group = word_groups_to_use[index_of_word_to_choose];

                            //Remove the word id and word group from the list
                            word_ids_to_use.RemoveAt(index_of_word_to_choose);
                            word_groups_to_use.RemoveAt(index_of_word_to_choose);

                            //Get the actual swahili and english text to display
                            Tuple<string, string> word_pair = HumanAcceleratedLearningConfiguration.GetInstance().SwahiliEnglishDictionary[word_pair_to_use];
                            swahili_word = word_pair.Item1;
                            english_word = word_pair.Item2;

                            AddWordGroupToDisplayedGroupsCollection(word_pair_group);

                            //Set the GUI to display the words
                            CurrentSwahiliWord = swahili_word;
                            CurrentEnglishWord = english_word;

                            //Save this trial
                            StudyBlock.WriteStudyTrial(fid, word_pair_to_use, (int)word_pair_group, (int)word_pair_group, DateTime.Now);
                        }
                    }
                }

                //Get the time elapsed since the last word-pair presentation
                this_element_elapsed_time_ms = (DateTime.Now - this_element_start_time).TotalMilliseconds;

                if (this_element_duration >= 0)
                {
                    if (this_element_elapsed_time_ms >= this_element_duration)
                    {
                        last_element_finish_time = this_element_start_time + TimeSpan.FromMilliseconds(this_element_duration);
                        act_on_sequence_element = true;
                    }
                }
                else
                {
                    //Calculate the amount of time until the next "stop point" that requires administrator action
                    double sum_time = 0;
                    var stop_points = stage.StageSequence.Select((y, x) => y.Item2 < 0 ? x : -1).Where(i => i != -1).ToList();
                    stop_points = stop_points.Where(x => x > stage_sequence_index).ToList();
                    if (stop_points.Count > 0)
                    {
                        int next_stop_point = stop_points[0];
                        sum_time = stage.StageSequence.Where((y, x) => x > stage_sequence_index && x < next_stop_point).Sum(y => y.Item2);
                    }
                    else
                    {
                        sum_time = stage.StageSequence.Where((y, x) => x > stage_sequence_index).Sum(y => y.Item2);
                    }
                    
                    if (this_element_duration == -1)
                    {
                        if (last_tone_detection_time >= this_element_start_time)
                        {
                            //Set the element start time to be the tone detection time
                            this_element_start_time = last_tone_detection_time;

                            //Set the element duration to a number
                            this_element_duration = HumanAcceleratedLearningConfiguration.GetInstance().TimeSpanBetweenToneAndVNS + HumanAcceleratedLearningConfiguration.GetInstance().VNSPresentationDelay;
                            
                            sum_time += this_element_duration;
                            next_admin_interaction_time = DateTime.Now.AddMilliseconds(sum_time);
                        }
                    }
                    else if (this_element_duration == -2)
                    {
                        //WaitingForUserPress = true;
                        
                        if (DateTime.Now >= (last_tone_detection_time.AddSeconds(17)) && DateTime.Now >= this_element_start_time)
                        {
                            //if (LastButtonPressTime >= this_element_start_time)
                            //WaitingForUserPress = false;
                            //this_element_start_time = LastButtonPressTime;
                            //this_element_duration = HumanAcceleratedLearningConfiguration.GetInstance().TimeSpanBetweenToneAndVNS + HumanAcceleratedLearningConfiguration.GetInstance().VNSPresentationDelay;
                            
                            this_element_start_time = DateTime.Now;
                            this_element_duration = HumanAcceleratedLearningConfiguration.GetInstance().TimeSpanBetweenToneAndVNS + HumanAcceleratedLearningConfiguration.GetInstance().VNSPresentationDelay;

                            sum_time += this_element_duration;
                            next_admin_interaction_time = DateTime.Now.AddMilliseconds(sum_time);
                        }
                    }
                }

                if (next_admin_interaction_time > DateTime.Now)
                {
                    var time_remaining = next_admin_interaction_time - DateTime.Now;
                    TimeRemainingUntilNextAdminInteraction = time_remaining.ToString(@"hh\:mm\:ss");
                }
                
                //Report progress to the main thread
                _background_thread.ReportProgress(0);

                //Check to see if cancellation is pending on this thread.
                if (_background_thread.CancellationPending)
                {
                    done = true;
                }

                //Sleep the thread so as not to consume the processor
                Thread.Sleep(33);
            }

            //Close the study session file
            fid.Close();
        }

        private void RunTestSession ()
        {
            //If the current subject has not been defined, return now.
            if (CurrentSubject == null)
            {
                return;
            }

            //Load in the latest testing session for this user and check what words the user got correct/incorrect
            var words_to_use_tuple = GetWordsToUse();
            List<int> word_ids = words_to_use_tuple.Item1;
            List<WordGroup> word_groups = words_to_use_tuple.Item2;
            
            //Create a file for this test session
            var fid = TestBlock.CreateTestBlockFile(CurrentSubject.UserName);

            int word_id = 0;
            WordGroup word_group = WordGroup.NoVNS;
            bool time_to_display_new_word = true;
            bool first_trial = true;
            double word_display_duration = HumanAcceleratedLearningConfiguration.GetInstance().StandardTestingTime / 1000.0;
            Stopwatch timer = new Stopwatch();
            DateTime word_pair_presentation_time = DateTime.MinValue;
            bool trial_output_complete = false;

            bool done = false;

            double total_test_duration = HumanAcceleratedLearningConfiguration.GetInstance().StandardTestingTime * word_ids.Count;
            DateTime test_finish_time = DateTime.Now.AddMilliseconds(total_test_duration);
            
            while (!done)
            {
                double elapsed_display_time = timer.Elapsed.TotalSeconds;
                if (elapsed_display_time >= word_display_duration)
                {
                    time_to_display_new_word = true;
                }

                if (time_to_display_new_word)
                {
                    //Output the results of the previous trial if necessary
                    if (!first_trial)
                    {
                        if (!trial_output_complete)
                        {
                            double latency_to_input = timer.Elapsed.TotalSeconds;
                            bool correct = (CurrentEnglishResponse == CurrentEnglishWord);
                            TestBlockTrial t = new TestBlockTrial()
                            {
                                Correct = Convert.ToInt32(correct),
                                EnglishInput = CurrentEnglishResponse,
                                InputLatency = latency_to_input,
                                PresentationTime = word_pair_presentation_time,
                                SwahiliWord = CurrentSwahiliWord,
                                WordPairID = word_id,
                                WordGroupID = word_group
                            };

                            TestBlockTrial.WriteTestBlockTrial(fid, t);
                        }
                    }

                    //Break out of the loop if the test block has completed
                    if (word_ids.Count == 0)
                    {
                        done = true;
                        break;
                    }

                    //Choose a word-pair to display
                    int index = MathHelperMethods.RandomNumbers.Next(word_ids.Count);
                    word_id = word_ids[index];
                    word_group = word_groups[index];

                    //Remove the words from the list
                    word_ids.RemoveAt(index);
                    word_groups.RemoveAt(index);

                    //Get the actual swahili/english text to display
                    var word_pair = HumanAcceleratedLearningConfiguration.GetInstance().SwahiliEnglishDictionary[word_id];
                    string swahili_word = word_pair.Item1;
                    string english_word = word_pair.Item2;

                    AddWordGroupToDisplayedGroupsCollection(word_group);

                    //Set the GUI to display the words
                    CurrentSwahiliWord = swahili_word;
                    CurrentEnglishWord = english_word;
                    _current_english_response = string.Empty;
                    BackgroundPropertyChanged("CurrentEnglishResponse");

                    //Start a timer
                    timer.Restart();
                    time_to_display_new_word = false;
                    first_trial = false;
                    trial_output_complete = false;
                    word_pair_presentation_time = DateTime.Now;
                }

                if (!trial_output_complete)
                {
                    if (CurrentEnglishResponse == CurrentEnglishWord)
                    {
                        double latency_to_input = timer.Elapsed.TotalSeconds;
                        TestBlockTrial t = new TestBlockTrial()
                        {
                            Correct = 1,
                            EnglishInput = CurrentEnglishResponse,
                            InputLatency = latency_to_input,
                            SwahiliWord = CurrentSwahiliWord,
                            PresentationTime = word_pair_presentation_time,
                            WordGroupID = word_group,
                            WordPairID = word_id
                        };

                        TestBlockTrial.WriteTestBlockTrial(fid, t);

                        trial_output_complete = true;
                    }
                }

                //Break out of the loop if the user has cancelled the background thread
                if (_background_thread.CancellationPending)
                {
                    break;
                }

                if (test_finish_time > DateTime.Now)
                {
                    var time_remaining = test_finish_time - DateTime.Now;
                    TimeRemainingUntilNextAdminInteraction = time_remaining.ToString(@"hh\:mm\:ss");
                }

                //Update the GUI thread
                _background_thread.ReportProgress(0);

                //Sleep the thread so we don't consume all of the processor
                Thread.Sleep(33);
            }

            //Close the test block file
            fid.Close();
        }

        private void RunDistractorPhase ()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            while (!_background_thread.CancellationPending)
            {
                //Break out of the loop if the timer has reached 30 seconds
                if (timer.Elapsed.TotalSeconds >= 30)
                {
                    break;
                }
                
                SecondsRemainingOfDistractor = Convert.ToInt32(30.0f - timer.Elapsed.TotalSeconds);

                TimeRemainingUntilNextAdminInteraction = SecondsRemainingOfDistractor.ToString();

                //Report progress to the UI
                _background_thread.ReportProgress(0);

                //Sleep the thread for a little while so we don't consume the processor
                Thread.Sleep(33);
            }
            
            return;
        }

        #endregion
    }
}
