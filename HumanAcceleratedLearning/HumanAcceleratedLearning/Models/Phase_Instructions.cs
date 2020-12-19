using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    class Phase_Instructions : Phase
    {
        #region Constructor

        public Phase_Instructions ()
            : base()
        {
            PhaseType = PhaseType.InstructionsScreen;
        }

        #endregion

        #region Properties

        public string TextSourceFileName { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        #endregion

        #region Overrides

        public override void Update()
        {
            if (DateTime.Now >= (StartTime + TimeSpan.FromMilliseconds(Duration)))
            {
                IsPhaseFinished = true;
            }
        }

        public int GetTotalSecondsRemaining()
        {
            int seconds_remaining = Convert.ToInt32(Math.Round(((StartTime + TimeSpan.FromMilliseconds(Duration)) - DateTime.Now).TotalSeconds));
            return seconds_remaining;
        }

        public override void InitializePhaseFromParameters(List<Tuple<string, string>> parameters, int loop_counter = 0)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                var pname = parameters[i].Item1;
                var pval = parameters[i].Item2;
                var pval_is_loop_param = pval.Equals("n");
                var pval_parse_success = Int32.TryParse(pval, out int pval_parse_result);

                if (pname.Equals("text"))
                {
                    TextSourceFileName = pval;
                    var full_path_to_file = HumanAcceleratedLearningConfiguration.GetInstance().ResourcesFolder + 
                        HumanAcceleratedLearningConfiguration.GetInstance().InstructionsPath + @"\" + TextSourceFileName;
                    if (File.Exists(full_path_to_file))
                    {
                        Text = File.ReadAllText(full_path_to_file);
                    }
                }
                else if (pname.Equals("duration"))
                {
                    Duration = (pval_is_loop_param) ? loop_counter : (pval_parse_success) ? pval_parse_result : 0;
                }
            }
        }

        #endregion
    }
}
