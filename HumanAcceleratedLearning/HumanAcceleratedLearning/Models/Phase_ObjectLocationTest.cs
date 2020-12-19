using HumanAcceleratedLearning.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    public class Phase_ObjectLocationTest : Phase
    {
        #region Constructor

        public Phase_ObjectLocationTest ()
            : base()
        {
            PhaseType = PhaseType.ObjectLocation_TestSession;
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

        private TestBlockTrial_ObjectLocation current_trial = null;

        //THIS VARIABLE COULD CHANGE DEPENDING ON THE SCREEN USED
        private double max_pixel_distance_for_correct
        {
            get
            {
                return (ColumnWidth * 1.5);
            }
        }

        private List<string> usable_images = new List<string>();
        private List<Tuple<int, int>> usable_image_positions = new List<Tuple<int, int>>();

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

        public bool MouseIsReleased { get; set; } = true;

        public int CurrentImageIndex
        {
            get
            {
                return current_image_index;
            }
        }

        public int ThisPhaseImageCount
        {
            get
            {
                return usable_images.Count;
            }
        }

        #endregion

        #region Methods

        public override void Start(HumanSubject current_subject = null)
        {
            if (current_subject != null)
            {
                _current_subject = current_subject;
                last_picture_display_time = DateTime.MinValue;
                current_image_index = 0;

                if (current_subject.AllObjectImages.Count == 0)
                {
                    ObjectLocation_FileHandler.ReadObjectLocationFile(current_subject);

                    /*var res_folder = HumanAcceleratedLearningConfiguration.GetInstance().ResourcesFolder;
                    var img_path = HumanAcceleratedLearningConfiguration.GetInstance().ObjectLocationImagesPath;
                    var all_images = Directory.GetFiles(res_folder + img_path).ToList();

                    current_subject.AllObjectImages = all_images;*/
                }

                if (current_subject.AllObjectImages.Count > 0)
                {
                    //Remove images that have already been correctly answered
                    usable_images = current_subject.AllObjectImages.Where((y, x) => !current_subject.CorrectlyAnsweredImageList.Contains(x)).ToList();
                    usable_image_positions = current_subject.AllObjectImageLocations.Where((y, x) => !current_subject.CorrectlyAnsweredImageList.Contains(x)).ToList();

                    //Create a shuffled list of image indices for this study session
                    var all_indices = Enumerable.Range(0, usable_images.Count).ToList();
                    shuffled_list_of_image_indices = MathHelperMethods.ShuffleList(all_indices);

                    //Create a file for this study session
                    fid = TestBlock.CreateTestBlockFile(current_subject.UserName);

                    HasPhaseStarted = true;
                    StartTime = DateTime.Now;
                }
            }
        }

        public override void Update()
        {
            if (_current_subject != null && this.ScreenWidth > 0 && this.ScreenHeight > 0)
            {
                if (usable_images == null || (usable_images != null && usable_images.Count == 0))
                {
                    IsPhaseFinished = true;
                    return;
                }

                //Check the status of the current trial
                if (current_trial != null)
                {
                    if ( MouseIsReleased ||
                         (DateTime.Now >= (last_picture_display_time + TimeSpan.FromMilliseconds(this.Duration)))
                       )
                    {
                        current_trial.PositionX = Convert.ToInt32(CurrentImageLocationX);
                        current_trial.PositionY = Convert.ToInt32(CurrentImageLocationY);

                        var current_distance = Math.Sqrt(
                            Math.Pow(current_trial.PositionX - current_trial.CorrectPositionX, 2) +
                            Math.Pow(current_trial.PositionY - current_trial.CorrectPositionY, 2));

                        current_trial.DistanceFromCorrectPosition = current_distance;
                        
                        if (current_trial.DistanceFromCorrectPosition <= max_pixel_distance_for_correct)
                        {
                            if (!current_trial.Correct)
                            {
                                current_trial.Correct = true;
                                current_trial.InputLatency = (DateTime.Now - current_trial.PresentationTime).TotalSeconds;
                            }
                        }
                        else
                        {
                            current_trial.Correct = false;
                            current_trial.InputLatency = -1;
                        }
                    }
                }

                if (DateTime.Now >= (last_picture_display_time + TimeSpan.FromMilliseconds(this.Duration)))
                {
                    //Save the current trial
                    if (current_trial != null)
                    {
                        //Add this image to the list of correctly answered images if necessary
                        if (current_trial.Correct)
                        {
                            var file_names_only = _current_subject.AllObjectImages.Select(x => Path.GetFileName(x)).ToList();
                            var idx = file_names_only.IndexOf(current_trial.ImageName);
                            if (idx > -1)
                            {
                                _current_subject.CorrectlyAnsweredImageList.Add(idx);
                            }
                        }

                        //Write out this trial to the file
                        TestBlockTrial_ObjectLocation.WriteTestBlockTrial(fid, current_trial);

                        //The current trial is finished. Set the current trial to null.
                        current_trial = null;
                    }

                    if (current_image_index >= usable_images.Count)
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
                        MouseIsReleased = true;
                        CurrentImagePath = usable_images[selected_image_index_from_shuffled_list];

                        //Calculate the new image location here...
                        CurrentImageLocationX_Column = Convert.ToInt32(grid_columns / 2);
                        CurrentImageLocationY_Row = Convert.ToInt32(grid_rows / 2);
                        CurrentImageLocationX = CurrentImageLocationX_Column * ColumnWidth;
                        CurrentImageLocationY = CurrentImageLocationY_Row * RowHeight;
                        
                        var correct_x = usable_image_positions[selected_image_index_from_shuffled_list].Item1 * ColumnWidth;
                        var correct_y = usable_image_positions[selected_image_index_from_shuffled_list].Item2 * RowHeight;

                        //Set the time for when this image was displayed
                        last_picture_display_time = DateTime.Now;

                        //Increment the image index
                        current_image_index++;
                        
                        //Create a new trial
                        current_trial = new TestBlockTrial_ObjectLocation()
                        {
                            ImageName = Path.GetFileName(CurrentImagePath),
                            PresentationTime = last_picture_display_time,
                            CorrectPositionX = Convert.ToInt32(correct_x),
                            CorrectPositionY = Convert.ToInt32(correct_y)
                        };
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
