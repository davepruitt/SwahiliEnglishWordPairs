using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //empty
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

        #region Privata data members

        private string _configuration_file_name = "config.txt";
        private string _dictionary_file_name = "dictionary.tsv";
        private string _stage_path = "Stages";

        private bool _debugging_mode = false;
        private string _primary_save_path = string.Empty;
        
        private int _timespan_between_tone_and_vns = 5000;
        private int _vns_presentation_delay = -500;
        
        private int _standard_testing_time = 8000;
        private int _standard_post_testing_time = 2000;

        private int _block_count = 1;
        private int _selected_stage_index = 0;

        private List<Stage> _stages = new List<Stage>()
        {
            new Stage() { StageName = "VNS Subject - First Visit" },
            new Stage() { StageName = "Control Subject - First Visit" },
            new Stage() { StageName = "Test Only" }
        };

        #endregion

        #region Public properties

        public int WordCount_Paired = 25;
        public int WordCount_Interleaved = 25;
        public int WordCount_Unpaired = 25;

        public float TotalWordGroups = 1.0f;

        public List<Tuple<string, string>> SwahiliEnglishDictionary = new List<Tuple<string, string>>();

        public List<string> ExcludedSwahiliWords = new List<string>();
        
        public List<BlockPiece> BlockSequence = new List<BlockPiece>();

        public int BlockCount
        {
            get
            {
                return _block_count;
            }
            set
            {
                _block_count = value;
            }
        }

        public int StandardPostTestingTime
        {
            get
            {
                return _standard_post_testing_time;
            }
            set
            {
                _standard_post_testing_time = value;
            }
        }

        public int StandardTestingTime
        {
            get
            {
                return _standard_testing_time;
            }
            private set
            {
                _standard_testing_time = value;
            }
        }

        public int VNSPresentationDelay
        {
            get
            {
                return _vns_presentation_delay;
            }
            private set
            {
                _vns_presentation_delay = value;
            }
        }

        public int TimeSpanBetweenToneAndVNS
        {
            get
            {
                return _timespan_between_tone_and_vns;
            }
            private set
            {
                _timespan_between_tone_and_vns = value;
            }
        }

        public bool DebuggingMode
        {
            get
            {
                return _debugging_mode;
            }
            private set
            {
                _debugging_mode = value;
            }
        }

        public string PrimarySavePath
        {
            get
            {
                return _primary_save_path;
            }
            private set
            {
                _primary_save_path = value;
            }
        }

        public string StudySessionStagePath
        {
            get
            {
                return _stage_path;
            }
            set
            {
                _stage_path = value;
            }
        }

        public List<Stage> Stages
        {
            get
            {
                return _stages;
            }
            set
            {
                _stages = value;
            }
        }

        public int SelectedStageIndex
        {
            get
            {
                return _selected_stage_index;
            }
            set
            {
                _selected_stage_index = value;
            }
        }

        #endregion

        #region Public methods

        public List<int> GetIDsOfAllIncludedWords ()
        {
            List<int> result = new List<int>();
            for (int i = 0; i < SwahiliEnglishDictionary.Count; i++)
            {
                if (!ExcludedSwahiliWords.Contains(SwahiliEnglishDictionary[i].Item1))
                {
                    result.Add(i);
                }
            }

            return result;
        }

        private void LoadAllStages ()
        {
            List<Stage> result = new List<Stage>();
            var all_stage_files = Directory.GetFiles(this.StudySessionStagePath);
            foreach (var f in all_stage_files)
            {
                Stage r = Stage.LoadStage(f);
                result.Add(r);
            }

            this.Stages = result;
        }

        public Stage LoadStage ( bool vns, bool first )
        {
            string first_vns_name = "VNS_Stage_2.csv";
            string second_vns_name = "VNS_Stage_1.csv";
            string first_control_name = "Control_Stage_2.csv";
            string second_control_name = "Control_Stage_1.csv";

            string stage_to_load = string.Empty;
            if (vns && first)
            {
                stage_to_load = first_vns_name;
            }
            else if (vns && !first)
            {
                stage_to_load = second_vns_name;
            }
            else if (!vns && first)
            {
                stage_to_load = first_control_name;
            }
            else
            {
                stage_to_load = second_control_name;
            }

            stage_to_load = this.StudySessionStagePath + @"\" + stage_to_load;

            Stage result = Stage.LoadStage(stage_to_load);

            return result;
        }

        public void LoadConfigurationFile ()
        {
            //Load the configuration file
            List<string> file_lines = ConfigurationFileLoader.LoadConfigurationFile(_configuration_file_name);

            //Iterate through each line and parse out the data
            for (int i = 0; i < file_lines.Count; i++)
            {
                string this_line = file_lines[i];
                string[] split_string = this_line.Split(new char[] { ':' }, 2);
                string key = split_string[0].Trim();
                string value = split_string[1].Trim();

                int result = 0;
                bool success = false;
                
                if (key.Equals("Primary Save Path", StringComparison.InvariantCultureIgnoreCase))
                {
                    PrimarySavePath = value;
                }
                else if (key.Equals("Study Session Stage Path", StringComparison.InvariantCultureIgnoreCase))
                {
                    StudySessionStagePath = value;
                }
                else if (key.Equals("Time From Tone To VNS", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        TimeSpanBetweenToneAndVNS = result;
                    }
                }
                else if (key.Equals("VNS Presentation Delay", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        VNSPresentationDelay = result;
                    }
                }
                else if (key.Equals("Testing Time", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        StandardTestingTime = result;
                    }
                }
                else if (key.Equals("Post-Testing Time", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        StandardPostTestingTime = result;
                    }
                }
                else if (key.Equals("Debug", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        if (result == 0)
                        {
                            DebuggingMode = false;
                        }
                        else
                        {
                            DebuggingMode = true;
                        }
                    }
                }
                else if (key.Equals("Block Sequence", StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] parts = value.Split(new char[] { ',' });
                    foreach (string p in parts)
                    {
                        string p_trimmed = p.Trim();
                        BlockPiece block_piece = BlockPieceConverter.ConvertToBlockPiece(p_trimmed);
                        HumanAcceleratedLearningConfiguration.GetInstance().BlockSequence.Add(block_piece);
                    }
                }
                else if (key.Equals("Block Count", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        HumanAcceleratedLearningConfiguration.GetInstance().BlockCount = result;
                    }
                }
                else if (key.Equals("Excluded Words", StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] parts = value.Split(new char[] { ',' });
                    foreach (string p in parts)
                    {
                        string p_trimmed = p.Trim();
                        HumanAcceleratedLearningConfiguration.GetInstance().ExcludedSwahiliWords.Add(p_trimmed);
                    }
                }
                else if (key.Equals("Word Count Paired", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        HumanAcceleratedLearningConfiguration.GetInstance().WordCount_Paired = result;
                    }
                }
                else if (key.Equals("Word Count Interleaved", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        HumanAcceleratedLearningConfiguration.GetInstance().WordCount_Interleaved = result;
                    }
                }
                else if (key.Equals("Word Count Unpaired", StringComparison.InvariantCultureIgnoreCase))
                {
                    success = Int32.TryParse(value, out result);
                    if (success)
                    {
                        HumanAcceleratedLearningConfiguration.GetInstance().WordCount_Unpaired = result;
                    }
                }
            }

            //LoadAllStages();
        }

        public void LoadDictionary()
        {
            List<string> file_lines = ConfigurationFileLoader.LoadConfigurationFile(_dictionary_file_name);

            for (int i = 0; i < file_lines.Count; i++)
            {
                string this_line = file_lines[i];
                string[] split_string = this_line.Split(new char[] { '\t' }, 2);
                string key = split_string[0].Trim();
                string value = split_string[1].Trim();

                SwahiliEnglishDictionary.Add(new Tuple<string, string>(key, value));
            }
        }

        #endregion
    }
}
