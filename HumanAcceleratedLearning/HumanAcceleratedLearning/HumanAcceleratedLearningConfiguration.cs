using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Models;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// A singleton class that handles configuration data for the program
    /// </summary>
    public class HumanAcceleratedLearningConfiguration
    {
        #region Singleton

        private static HumanAcceleratedLearningConfiguration _instance = null;
        private static object _instance_lock = new object();

        private HumanAcceleratedLearningConfiguration()
        {
            LoadConfigurationFile();
        }

        public static HumanAcceleratedLearningConfiguration GetInstance()
        {
            if (_instance == null)
            {
                lock(_instance_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new HumanAcceleratedLearningConfiguration();
                    }
                }
            }

            return _instance;
        }

        #endregion

        #region Variables and properties

        private string ConfigurationFileName = "config.txt";

        public string ResourcesFolder = @"Resources\";

        public string InstructionsPath { get; private set; } = "Instructions";
        public string DictionaryPath { get; private set; } = "Dictionaries";
        public string StagePath { get; private set; } = "Stages";
        public string ObjectLocationImagesPath { get; private set; } = "Interference Images";

        #endregion

        #region Public properties

        public string PrimarySavePath { get; set; } = string.Empty;
        public List<Stage> Stages { get; set; } = new List<Stage>();
        public List<LanguageDictionary> LanguageDictionaries { get; set; } = new List<LanguageDictionary>();
        
        #endregion

        #region Public methods
        
        public void LoadAllStages ()
        {
            List<Stage> result = new List<Stage>();
            var all_stage_files = Directory.GetFiles(ResourcesFolder + StagePath);
            foreach (var f in all_stage_files)
            {
                Stage r = Stage.LoadStageFromFile(f);
                result.Add(r);
            }

            //Set the class property with the result
            Stages = result;
        }

        public void LoadConfigurationFile ()
        {
            //Load the configuration file
            List<string> file_lines = ConfigurationFileLoader.LoadConfigurationFile(ResourcesFolder + ConfigurationFileName);

            //Iterate through each line and parse out the data
            for (int i = 0; i < file_lines.Count; i++)
            {
                string this_line = file_lines[i];
                string[] split_string = this_line.Split(new char[] { ':' }, 2);
                string key = split_string[0].Trim();
                string value = split_string[1].Trim();
                
                if (key.Equals("Primary Save Path", StringComparison.InvariantCultureIgnoreCase))
                {
                    PrimarySavePath = value;
                }
            }
        }

        public void LoadLanguageDictionaries()
        {
            List<LanguageDictionary> result = new List<LanguageDictionary>();
            var all_dictionary_files = Directory.GetFiles(ResourcesFolder + DictionaryPath);
            foreach (var f in all_dictionary_files)
            {
                //Get the file info for this file
                FileInfo f_info = new FileInfo(f);

                //Instantiate a new language dictionary object
                LanguageDictionary d = new LanguageDictionary();

                string filename = Path.GetFileNameWithoutExtension(f_info.Name);
                List<string> separated_parts = filename.Split(new char[] { '_' }).ToList();

                d.ForeignLanguageName = separated_parts[0].ToLower();
                if (separated_parts.Count >= 3)
                {
                    d.NativeLanguageName = separated_parts[2].ToLower();
                }
                else
                {
                    d.NativeLanguageName = "english";
                }

                if (separated_parts.Count >= 4)
                {
                    d.MetaData = separated_parts[3].ToLower();
                }
                
                //Load the dictionary file
                List<string> file_lines = ConfigurationFileLoader.LoadConfigurationFile(f);
                for (int i = 0; i < file_lines.Count; i++)
                {
                    string this_line = file_lines[i];
                    string[] split_string = this_line.Split(new char[] { '\t' }, 2);
                    string key = split_string[0].Trim();
                    string value = split_string[1].Trim();

                    d.DictionaryWordPairs.Add(new Tuple<string, string>(key, value));
                }

                result.Add(d);
            }

            //Set the class property with the result
            LanguageDictionaries = result;
        }

        #endregion
    }
}
