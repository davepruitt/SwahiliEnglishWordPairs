using HumanAcceleratedLearning.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    public class Phase_ObjectLocationStudy : Phase
    {
        #region Constructor

        public Phase_ObjectLocationStudy ()
            : base()
        {
            PhaseType = PhaseType.ObjectLocation_StudySession;
        }

        #endregion

        #region Private data members

        private HumanSubject _current_subject = null;
        private DateTime last_picture_display_time = DateTime.MinValue;
        private int current_image_index = 0;
        private StreamWriter fid = null;

        private double grid_columns = 8;
        private double grid_rows = 10;
        
        private List<int> shuffled_list_of_image_indices = new List<int>();

        #endregion

        #region Properties

        public string CurrentImagePath { get; set; } = string.Empty;

        public double ScreenWidth { get; set; } = 0;
        public double ScreenHeight { get; set; } = 0;

        public int CurrentImageLocationX_Column { get; set; } = 0;
        public int CurrentImageLocationY_Row { get; set; } = 0;
        public double CurrentImageLocationX { get; set; } = 0;
        public double CurrentImageLocationY { get; set; } = 0;
        
        public double ColumnWidth
        {
            get
            {
                return (ScreenWidth / grid_columns);
            }
        }

        public double RowHeight
        {
            get
            {
                return (ScreenHeight / grid_rows);
            }
        }

        public int CurrentImageIndex
        {
            get
            {
                return current_image_index;
            }
        }

        public int TotalImageCount
        {
            get
            {
                if (_current_subject != null)
                {
                    return _current_subject.AllObjectImages.Count;
                }

                return 0;
            }
        }

        #endregion

        #region Methods

        public override void Start(HumanSubject current_subject = null)
        {
            if (current_subject != null)
            {
                _current_subject = current_subject;

                if (current_subject.AllObjectImages.Count == 0 && current_subject.AllObjectImageLocations.Count == 0)
                {
                    var res_folder = HumanAcceleratedLearningConfiguration.GetInstance().ResourcesFolder;
                    var img_path = HumanAcceleratedLearningConfiguration.GetInstance().ObjectLocationImagesPath;
                    var all_images = Directory.GetFiles(res_folder + img_path).ToList();
                    
                    current_subject.AllObjectImages = all_images;
                    last_picture_display_time = DateTime.MinValue;
                    current_image_index = 0;

                    for (int i = 0; i < all_images.Count; i++)
                    {
                        var loc_x = MathHelperMethods.RandomNumbers.Next(1, Convert.ToInt32(grid_columns) - 1);
                        var loc_y = MathHelperMethods.RandomNumbers.Next(1, Convert.ToInt32(grid_rows) - 1);
                        current_subject.AllObjectImageLocations.Add(new Tuple<int, int>(loc_x, loc_y));
                    }
                    
                    //Save all of the object locations to a file
                    ObjectLocation_FileHandler.WriteAllObjectLocationsToFile(current_subject);
                }
                
                //Create a shuffled list of image indices for this study session
                var all_indices = Enumerable.Range(0, current_subject.AllObjectImages.Count).ToList();
                shuffled_list_of_image_indices = MathHelperMethods.ShuffleList(all_indices);

                //Create a file for this study session
                fid = StudyBlock.CreateStudyBlockFile(current_subject.UserName);

                HasPhaseStarted = true;
                StartTime = DateTime.Now;
            }
        }

        public override void Update()
        {
            if (_current_subject != null && this.ScreenWidth > 0 && this.ScreenHeight > 0)
            {
                if (_current_subject.AllObjectImages == null || (_current_subject.AllObjectImages != null && _current_subject.AllObjectImages.Count == 0))
                {
                    IsPhaseFinished = true;
                    return;
                }

                if (DateTime.Now >= (last_picture_display_time + TimeSpan.FromMilliseconds(this.Duration)))
                {
                    //Check to see if we are done
                    if (current_image_index >= _current_subject.AllObjectImages.Count)
                    {
                        //Set the flag indicating this phase is finished
                        IsPhaseFinished = true;

                        //Close the save file for this study block
                        if (fid != null)
                        {
                            fid.Close();
                        }

                        //Return from this function immediately
                        return;
                    }
                    else
                    {
                        //If we are not done yet...

                        var selected_image_index_from_shuffled_list = shuffled_list_of_image_indices[current_image_index];

                        //Grab the next image
                        CurrentImagePath = _current_subject.AllObjectImages[selected_image_index_from_shuffled_list];

                        //Calculate the new image location here...
                        CurrentImageLocationX_Column = _current_subject.AllObjectImageLocations[selected_image_index_from_shuffled_list].Item1;
                        CurrentImageLocationY_Row = _current_subject.AllObjectImageLocations[selected_image_index_from_shuffled_list].Item2;
                        CurrentImageLocationX = _current_subject.AllObjectImageLocations[selected_image_index_from_shuffled_list].Item1 * ColumnWidth;
                        CurrentImageLocationY = _current_subject.AllObjectImageLocations[selected_image_index_from_shuffled_list].Item2 * RowHeight;

                        //Set the time for when this image was displayed
                        last_picture_display_time = DateTime.Now;

                        //Increment the image index
                        current_image_index++;

                        //Save this trial
                        if (fid != null)
                        {
                            //double cm_x = (CurrentImageLocationX / dip_per_inch) * cm_per_inch;
                            //double cm_y = (CurrentImageLocationY / dip_per_inch) * cm_per_inch;

                            StudyBlock.WriteObjectLocationStudyTrial(fid, Path.GetFileName(CurrentImagePath),
                                CurrentImageLocationX, CurrentImageLocationY, last_picture_display_time);
                        }
                    }
                }
            }
        }

        public override void InitializePhaseFromParameters(List<Tuple<string, string>> parameters, int loop_counter = 0)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                var pname = parameters[i].Item1;
                var pval = parameters[i].Item2;
                var pval_is_loop_param = pval.Equals("n");
                var pval_parse_success = Int32.TryParse(pval, out int pval_parse_result);

                if (pname.Equals("duration"))
                {
                    Duration = (pval_is_loop_param) ? loop_counter : (pval_parse_success) ? pval_parse_result : 0;
                }
            }
        }

        #endregion
    }
}
