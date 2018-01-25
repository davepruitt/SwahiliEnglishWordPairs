using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// Describes a stage in the Swahili study program
    /// </summary>
    public class Stage
    {
        #region Private data members

        List<Tuple<WordGroup, double>> _stage_sequence = new List<Tuple<WordGroup, double>>();
        string _stage_name = string.Empty;

        #endregion

        #region Constructor

        public Stage ()
        {
            //empty
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the stage
        /// </summary>
        public string StageName
        {
            get
            {
                return _stage_name;
            }
            set
            {
                _stage_name = value;
            }
        }

        public List<Tuple<WordGroup, double>> StageSequence
        {
            get
            {
                return _stage_sequence;
            }
            set
            {
                _stage_sequence = value;
            }
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Loads a stage from a stage file
        /// </summary>
        /// <param name="file_name">The name of the stage file</param>
        /// <returns></returns>
        public static Stage LoadStage ( string file_name )
        {
            Stage result = new Stage();

            var file_lines = ConfigurationFileLoader.LoadFileLines(file_name);

            foreach (var line in file_lines)
            {
                var line_parts = line.Split(new char[] { ',' });

                WordGroup this_word_group = WordGroup.Gap;
                double duration = 0;

                if (line_parts[0].Equals("P"))
                {
                    this_word_group = WordGroup.PairedVNS;
                }
                else if (line_parts[0].Equals("U"))
                {
                    this_word_group = WordGroup.InterleavedVNS;
                }
                else if (line_parts[0].Equals("N"))
                {
                    this_word_group = WordGroup.NoVNS;
                }
                else if (line_parts[0].Equals("M"))
                {
                    this_word_group = WordGroup.Message;
                }
                else
                {
                    this_word_group = WordGroup.Gap;
                }

                bool success = Double.TryParse(line_parts[1], out duration);
                if (!success)
                {
                    if (line_parts[1].Equals("x"))
                    {
                        duration = -1;
                    }
                    else if (line_parts[1].Equals("y"))
                    {
                        duration = -2;
                    }
                }

                Tuple<WordGroup, double> new_item = new Tuple<WordGroup, double>(this_word_group, duration);
                result.StageSequence.Add(new_item);
            }

            result.StageName = Path.GetFileNameWithoutExtension(file_name);

            return result;
        }

        #endregion
    }
}
