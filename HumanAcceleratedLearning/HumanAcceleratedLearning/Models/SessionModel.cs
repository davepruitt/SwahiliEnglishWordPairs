using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning.Models
{
    /// <summary>
    /// The model class for the whole app
    /// </summary>
    public class SessionModel : NotifyPropertyChangedObject
    {
        #region Singleton constructor

        private static SessionModel _instance = null;
        private static object _instance_lock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        private SessionModel()
        {
            //empty
        }

        public static SessionModel GetInstance()
        {
            if (_instance == null)
            {
                lock (_instance_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SessionModel();
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

        #region Program state enumeration

        public enum ProgramStateEnumeration
        {
            ParticipantWindowNotOpen,
            NotStarted,
            BeginSession,
            ExecuteNewPhase,
            UpdateCurrentPhase,
            FinishCurrentPhase,
            Finished,
            Paused
        }

        #endregion

        #region Private data members

        private BackgroundWorker _background_thread = null;

        private int _selected_stage_index = 0;
        private string _username = string.Empty;
        private ProgramStateEnumeration _program_state = ProgramStateEnumeration.ParticipantWindowNotOpen;
        private HumanSubject _current_subject = null;

        private int CurrentSegmentIndex = 0;
        private int CurrentPhaseIndex = 0;
        private Phase _current_phase = null;

        private int _remaining_study_phases = 0;
        private int _remaining_test_phases = 0;

        #endregion

        #region Public data members

        public int RemainingStudyPhases
        {
            get
            {
                return _remaining_study_phases;
            }
            set
            {
                _remaining_study_phases = value;
                BackgroundPropertyChanged("RemainingStudyPhases");
            }
        }

        public int RemainingTestPhases
        {
            get
            {
                return _remaining_test_phases;
            }
            set
            {
                _remaining_test_phases = value;
                BackgroundPropertyChanged("RemainingTestPhases");
            }
        }

        /// <summary>
        /// The current participant object
        /// </summary>
        public HumanSubject CurrentSubject
        {
            get
            {
                return _current_subject;
            }
            private set
            {
                _current_subject = value;
            }
        }

        /// <summary>
        /// The current state of the program
        /// </summary>
        public ProgramStateEnumeration ProgramState
        {
            get
            {
                return _program_state;
            }
            private set
            {
                _program_state = value;

                BackgroundPropertyChanged("ProgramState");
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
                NotifyPropertyChanged("UserName");
            }
        }

        /// <summary>
        /// The index of the selected stage in the stage list
        /// </summary>
        public int SelectedStageIndex
        {
            get
            {
                return _selected_stage_index;
            }
            set
            {
                _selected_stage_index = value;

                NotifyPropertyChanged("SelectedStageIndex");
            }
        }

        /// <summary>
        /// The current phase of the program
        /// </summary>
        public Phase CurrentPhase
        {
            get
            {
                return _current_phase;
            }
            set
            {
                _current_phase = value;
                BackgroundPropertyChanged("CurrentPhase");
            }
        }
        
        #endregion

        #region Methods

        public void SetState (ProgramStateEnumeration s)
        {
            ProgramState = s;
        }
        
        /// <summary>
        /// Starts a new session
        /// </summary>
        public void StartSession()
        {
            if (_background_thread == null)
            {
                _background_thread = new BackgroundWorker();
                _background_thread.WorkerSupportsCancellation = true;
                _background_thread.WorkerReportsProgress = true;
                _background_thread.DoWork += RunSession;
                _background_thread.ProgressChanged += SessionProgressChanged;
                _background_thread.RunWorkerCompleted += FinishUpAfterBackgroundThreadIsClosed;

                _background_thread.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Cancels a session that is currently running
        /// </summary>
        public void StopSession()
        {
            if (_background_thread != null)
            {
                _background_thread.CancelAsync();
            }
        }

        private void FinishUpAfterBackgroundThreadIsClosed(object sender, RunWorkerCompletedEventArgs e)
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

            NotifyPropertyChanged("__new_frame__");
        }

        private void RunSession(object sender, DoWorkEventArgs e)
        {
            //Select the stage we want to run
            Stage selected_stage = HumanAcceleratedLearningConfiguration.GetInstance().Stages[SelectedStageIndex];

            //Create a new human subject with this username, or load in the subject's data.
            if (!HumanSubject.DoesSubjectExist(UserName))
            {
                //If this is a new subject...
                
                //Create an object holding the data for this human subject
                CurrentSubject = HumanSubject.CreateNewSubjectData(_username, selected_stage);

                //Save the data to a file
                HumanSubject.SaveHumanSubject(CurrentSubject);
            }
            else
            {
                //If this is an existing subject, load the subject's data from a file
                CurrentSubject = HumanSubject.LoadHumanSubject(UserName);
            }
            
            //Set a flag indicating we are not done yet
            bool done = false;
            
            //Loop until the program is either cancelled or we are done
            while (!done)
            {
                //Handle the current state of the session
                done = HandleSessionState(selected_stage);

                //If the user wants to close down the program or cancel the session, set the flag indicating we are done
                if (_background_thread.CancellationPending)
                {
                    done = true;
                }

                //Report progress of the background thread
                _background_thread.ReportProgress(0);

                //Sleep the background thread a little bit so we don't consume the entire CPU
                Thread.Sleep(33);
            }

        }

        private void CalculateNumberOfRemainingPhases (Stage selected_stage, StageSegment current_segment)
        {
            int s = 0;
            int t = 0;

            //Get the current stage segment
            var segment_order = CurrentSubject.SegmentOrder;
            for (int i = CurrentSegmentIndex; i < CurrentSubject.SegmentOrder.Count; i++)
            {
                var this_segment_name = segment_order[i];
                var this_segment = selected_stage.StageSegments.Where(x => x.SegmentName.Equals(this_segment_name)).FirstOrDefault();

                if (this_segment == current_segment)
                {
                    try
                    {
                        var remaining_phases = this_segment.Phases.GetRange(CurrentPhaseIndex + 1, this_segment.Phases.Count - CurrentPhaseIndex - 1).ToList();
                        s += remaining_phases.Where(x => x is Phase_Study).ToList().Count;
                        t += remaining_phases.Where(x => x is Phase_Test).ToList().Count;
                    }
                    catch (Exception)
                    {
                        //empty
                    }
                }
                else
                {
                    try
                    {
                        s += this_segment.Phases.Where(x => x is Phase_Study).ToList().Count;
                        t += this_segment.Phases.Where(x => x is Phase_Test).ToList().Count;
                    }
                    catch (Exception)
                    {
                        //empty
                    }
                }
            }

            RemainingStudyPhases = s;
            RemainingTestPhases = t;
        }

        private bool HandleSessionState (Stage selected_stage)
        {
            if (CurrentSegmentIndex >= CurrentSubject.SegmentOrder.Count)
            {
                ProgramState = ProgramStateEnumeration.Finished;
                return true;
            }

            //Get the current stage segment
            var segment_order = CurrentSubject.SegmentOrder;
            var current_segment_name = segment_order[CurrentSegmentIndex];
            var current_segment = selected_stage.StageSegments.Where(x => x.SegmentName.Equals(current_segment_name)).FirstOrDefault();

            //Do some simple math which will be displayed in the GUI
            CalculateNumberOfRemainingPhases(selected_stage, current_segment);

            switch (ProgramState)
            {
                case ProgramStateEnumeration.BeginSession:

                    //Do anything we need to do to initiate the session here

                    //Set the new program state
                    ProgramState = ProgramStateEnumeration.ExecuteNewPhase;

                    break;
                case ProgramStateEnumeration.ExecuteNewPhase:

                    //Set the current phase
                    CurrentPhase = current_segment.Phases[CurrentPhaseIndex];

                    //Do anything we need to do here to actually execute the phase
                    CurrentPhase.Start(CurrentSubject);
                    
                    //Set the next program state
                    ProgramState = ProgramStateEnumeration.UpdateCurrentPhase;

                    break;
                case ProgramStateEnumeration.UpdateCurrentPhase:

                    //Update the current phase
                    CurrentPhase.Update();

                    //Check to see if the phase has finished, and if so, proceed to the next program state
                    if (CurrentPhase.IsPhaseFinished)
                    {
                        ProgramState = ProgramStateEnumeration.FinishCurrentPhase;
                    }
                    
                    break;
                case ProgramStateEnumeration.FinishCurrentPhase:

                    //Update the current phase index
                    CurrentPhaseIndex++;

                    //Check to see if we are done with the current segment
                    if (CurrentPhaseIndex >= current_segment.Phases.Count)
                    {
                        //Reset the current phase index
                        CurrentPhaseIndex = 0;

                        //Set the new segment index
                        CurrentSegmentIndex++;

                        //Check to see if we have finished all segments
                        if (CurrentSegmentIndex >= selected_stage.StageSegments.Count)
                        {
                            //If so, we are completely done
                            ProgramState = ProgramStateEnumeration.Finished;

                            //Break out of this portion of the switch statement
                            return true;
                        }
                    }

                    //Set the new state
                    ProgramState = ProgramStateEnumeration.ExecuteNewPhase;

                    break;
                case ProgramStateEnumeration.Finished:
                    return true;
                default:
                    //In other program states, we do nothing
                    break;
            }

            return false;
        }

        #endregion
    }
}
