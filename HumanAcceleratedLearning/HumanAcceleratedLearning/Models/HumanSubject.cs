using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning.Models
{
    public class HumanSubject
    {
        #region Public data members

        public string UserName = string.Empty;
        public DateTime StartDate = DateTime.MinValue;
        public Stage FirstVisitStage = null;
        public List<string> SegmentOrder = new List<string>();
        public Dictionary<string, List<string>> CorrectlyAnsweredWordList = new Dictionary<string, List<string>>();
        public List<string> AllObjectImages = new List<string>();
        public List<Tuple<int, int>> AllObjectImageLocations = new List<Tuple<int, int>>();
        public List<int> CorrectlyAnsweredImageList = new List<int>();
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public HumanSubject ()
        {
            //empty
        }

        #endregion

        #region Methods

        public void MakeSureCorrectlyAnsweredWordListExistsForSubject (string language_name)
        {
            if (!CorrectlyAnsweredWordList.Keys.Contains(language_name))
            {
                CorrectlyAnsweredWordList[language_name] = new List<string>();
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Creates an object for a new human subject
        /// </summary>
        public static HumanSubject CreateNewSubjectData (string username, Stage first_visit_stage)
        {
            HumanSubject result = new HumanSubject();

            var configuration_data = HumanAcceleratedLearningConfiguration.GetInstance();

            result.UserName = username;
            result.StartDate = DateTime.Now;
            result.FirstVisitStage = first_visit_stage;
            foreach (var seg in first_visit_stage.StageSegments)
            {
                result.SegmentOrder.Add(seg.SegmentName);
            }

            return result;
        }

        /// <summary>
        /// Checks to see if a patient already exists with a certain username
        /// </summary>
        public static bool DoesSubjectExist (string username)
        {
            bool does_exist = false;

            //Construct a file name to load
            string file_name = username + ".txt";

            //Construct a full path and file
            string fully_qualified_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + file_name;

            //Check to see if the patient data file exists
            FileInfo info = new FileInfo(fully_qualified_path);
            does_exist = info.Exists;

            return does_exist;
        }

        /// <summary>
        /// Loads data from a previously created human subject.
        /// </summary>
        public static HumanSubject LoadHumanSubject (string username)
        {
            HumanSubject s = new HumanSubject();

            //Construct a file name to load
            string file_name = username + ".txt";

            //Construct a full path and file
            string fully_qualified_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + file_name;

            //Check to see if the patient data file exists
            FileInfo info = new FileInfo(fully_qualified_path);
            
            //If the patient data file exists...
            if (info.Exists)
            {
                //Read the contents of the file
                List<string> lines = ConfigurationFileLoader.LoadFileLines(fully_qualified_path);

                //Read the user name
                s.UserName = lines[0].Trim();

                //Read the participant's start date
                double matlab_datenum = 0;
                bool success = Double.TryParse(lines[1], out matlab_datenum);
                if (success)
                {
                    s.StartDate = MathHelperMethods.ConvertMatlabDatenumToDateTime(matlab_datenum);
                }

                //Now read the order of the participant's segments
                for (int i = 2; i < lines.Count; i++)
                {
                    s.SegmentOrder.Add(lines[i].Trim());
                }
            }
            
            return s;
        }

        /// <summary>
        /// Saves a human subject to a data file
        /// </summary>
        /// <param name="s"></param>
        public static void SaveHumanSubject (HumanSubject s)
        {
            //Construct a file name for the patient data file
            string file_name = s.UserName + ".txt";

            //Construct a path for the save location
            string path_name = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath;

            //Create the directory if it doesn't exist
            DirectoryInfo info = new DirectoryInfo(path_name);
            if (!info.Exists)
            {
                info.Create();
            }

            //Open the file for writing
            string full_path_and_file = path_name + file_name;
            StreamWriter writer = new StreamWriter(full_path_and_file);

            writer.WriteLine(s.UserName);
            writer.WriteLine(MathHelperMethods.ConvertDateTimeToMatlabDatenum(s.StartDate).ToString());

            //Write out the names of each segment in the order that they were performed in the participant's first visit
            foreach (var seg in s.FirstVisitStage.StageSegments)
            {
                writer.WriteLine(seg.SegmentName);
            }
            
            //Close the file
            writer.Close();
        }

        #endregion
    }
}
