using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    public class HumanSubject
    {
        #region Public data members

        public string UserName = string.Empty;
        public DateTime StartDate = DateTime.MinValue;
        public Dictionary<int, WordGroup> WordGroups = new Dictionary<int, WordGroup>();

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

        #region Static methods

        /// <summary>
        /// Creates an object for a new human subject
        /// </summary>
        public static HumanSubject CreateNewSubjectData (string username)
        {
            HumanSubject result = new HumanSubject();

            var configuration_data = HumanAcceleratedLearningConfiguration.GetInstance();

            result.UserName = username;
            result.StartDate = DateTime.Now;

            Stage selected_stage = HumanAcceleratedLearningConfiguration.GetInstance().Stages[HumanAcceleratedLearningConfiguration.GetInstance().SelectedStageIndex];
            //int total_p = selected_stage.StageSequence.Where(x => x.Item1 == WordGroup.PairedVNS).Count();
            //int total_u = selected_stage.StageSequence.Where(x => x.Item1 == WordGroup.InterleavedVNS).Count();
            //int total_n = selected_stage.StageSequence.Where(x => x.Item1 == WordGroup.NoVNS).Count();
            int total_p = HumanAcceleratedLearningConfiguration.GetInstance().WordCount_Paired;
            int total_u = HumanAcceleratedLearningConfiguration.GetInstance().WordCount_Interleaved;
            int total_n = HumanAcceleratedLearningConfiguration.GetInstance().WordCount_Unpaired;

            List<int> sorted_list = new List<int>();
            var p_list = Enumerable.Repeat((int)WordGroup.PairedVNS, total_p).ToList();
            var u_list = Enumerable.Repeat((int)WordGroup.InterleavedVNS, total_u).ToList();
            var n_list = Enumerable.Repeat((int)WordGroup.NoVNS, total_n).ToList();

            sorted_list.AddRange(p_list);
            sorted_list.AddRange(u_list);
            sorted_list.AddRange(n_list);
            
            //Get a random permutation of the list
            List<int> random_list = MathHelperMethods.ShuffleList(sorted_list);
            
            //Generate a list of integers the length of the size of the dictionary, from 0 to n-1:
            List<int> word_ids = HumanAcceleratedLearningConfiguration.GetInstance().GetIDsOfAllIncludedWords();
            
            for (int i = 0; i < random_list.Count; i++)
            {
                int this_word_id = word_ids[i];
                WordGroup this_word_group = (WordGroup)random_list[i];

                //Add this index to the word grouping dictionary for this patient
                result.WordGroups[this_word_id] = this_word_group;
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

                s.UserName = lines[0].Trim();

                double matlab_datenum = 0;
                bool success = Double.TryParse(lines[1], out matlab_datenum);
                if (success)
                {
                    s.StartDate = MathHelperMethods.ConvertMatlabDatenumToDateTime(matlab_datenum);
                }

                for (int i = 2; i < lines.Count; i++)
                {
                    string[] kv_pair = lines[i].Split(new char[] { ',' }, 2);
                    int key = 0;
                    int value = 0;
                    bool success2 = Int32.TryParse(kv_pair[0], out key);
                    bool success3 = Int32.TryParse(kv_pair[1], out value);
                    if (success2 && success3)
                    {
                        s.WordGroups[key] = (WordGroup)value;
                    }
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

            foreach (var kvp in s.WordGroups)
            {
                writer.WriteLine(kvp.Key.ToString() + ", " + ((int)kvp.Value).ToString());
            }

            writer.Close();
        }

        #endregion
    }
}
