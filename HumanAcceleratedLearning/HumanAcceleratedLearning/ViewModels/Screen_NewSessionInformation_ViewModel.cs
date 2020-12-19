using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;
using HumanAcceleratedLearning.Models;

namespace HumanAcceleratedLearning.ViewModels
{
    public class Screen_NewSessionInformation_ViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        SessionModel _model = SessionModel.GetInstance();
        HumanAcceleratedLearningConfiguration _config = HumanAcceleratedLearningConfiguration.GetInstance();

        private ObservableCollection<string> _segment_list = new ObservableCollection<string>();

        #endregion

        #region Constructor

        public Screen_NewSessionInformation_ViewModel ()
        {
            _model.PropertyChanged += ExecuteReactionsToModelPropertyChanged;

            UpdateSegmentNames();
        }

        #endregion

        #region Private methods

        private void UpdateSegmentNames ()
        {
            Stage s = HumanAcceleratedLearningConfiguration.GetInstance().Stages[_model.SelectedStageIndex];
            var segment_list = s.StageSegments.Select(x => x.SegmentName).ToList();

            //Remove the "collection changed" event handler temporarily
            _segment_list.CollectionChanged -= HandleDragDrop_CollectionChanged;

            //Clear the list and add items for the new stage
            _segment_list.Clear();
            foreach (var seg in segment_list)
            {
                _segment_list.Add(seg);
            }

            //Re-add the event handler
            _segment_list.CollectionChanged += HandleDragDrop_CollectionChanged;
        }

        private void HandleDragDrop_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Get the ordered list of segment names according to how the user wants to order them
            var seg_names = _segment_list.ToList();

            //Get the current stage
            Stage s = HumanAcceleratedLearningConfiguration.GetInstance().Stages[_model.SelectedStageIndex];
            
            if (seg_names.Count == s.StageSegments.Count)
            {
                List<StageSegment> new_stage_segment_list = new List<StageSegment>();
                foreach (var this_seg_name in seg_names)
                {
                    var this_seg = s.StageSegments.Where(x => x.SegmentName.Equals(this_seg_name)).FirstOrDefault();
                    if (this_seg != null)
                    {
                        new_stage_segment_list.Add(this_seg);
                    }
                }

                s.StageSegments = new_stage_segment_list;
            }
        }

        #endregion

        #region Properties

        [ReactToModelPropertyChanged(new string[] { "UserName" })]
        public string SubjectIdentifier
        {
            get
            {
                return _model.UserName;
            }
            set
            {
                _model.UserName = ViewHelperMethods.CleanInput(value).Trim().ToUpper();
            }
        }

        public List<string> SessionTypeItemsList
        {
            get
            {
                return _config.Stages.Select(x => x.StageName).ToList();
            }
        }

        [ReactToModelPropertyChanged(new string[] { "SelectedStageIndex" })]
        public ObservableCollection<string> SegmentNamesList
        {
            get
            {
                return _segment_list;
            }
            set
            {
                _segment_list = value;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "SelectedStageIndex" })]
        public int SessionTypeSelectedIndex
        {
            get
            {
                return _model.SelectedStageIndex;
            }
            set
            {
                _model.SelectedStageIndex = value;

                UpdateSegmentNames();
            }
        }

        [ReactToModelPropertyChanged(new string[] { "UserName" })]
        public bool IsNextButtonEnabled
        {
            get
            {
                return (!string.IsNullOrEmpty(_model.UserName));
            }
        }

        [ReactToModelPropertyChanged(new string[] { "UserName" })]
        public string ParticipantID_MessageText
        {
            get
            {
                if (!string.IsNullOrEmpty(_model.UserName))
                {
                    if (HumanSubject.DoesSubjectExist(_model.UserName))
                    {
                        return "This is an existing participant.";
                    }
                    else
                    {
                        return "This is a new participant.";
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [ReactToModelPropertyChanged(new string[] { "SelectedStageIndex" })]
        public string SessionType_MessageText
        {
            get
            {
                return HumanAcceleratedLearningConfiguration.GetInstance().Stages[_model.SelectedStageIndex].StageDescription;
            }
        }

        #endregion
    }
}
