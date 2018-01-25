using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// Describes methods and properties of a study block
    /// </summary>
    public class StudyBlock
    {
        #region Static methods

        /// <summary>
        /// Creates a study block file for the specified username
        /// </summary>
        public static StreamWriter CreateStudyBlockFile ( string username )
        {
            string date_for_filename = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string file_name = username + "_" + date_for_filename + ".txt";

            string fully_qualified_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\Study\";
            DirectoryInfo dir_info = new DirectoryInfo(fully_qualified_path);
            if (!dir_info.Exists)
            {
                dir_info.Create();
            }

            string fully_qualified_file = fully_qualified_path + file_name;

            StreamWriter writer = new StreamWriter(fully_qualified_file);
            writer.WriteLine(username);

            return writer;
        }

        /// <summary>
        /// Writes a study trial to the file
        /// </summary>
        public static void WriteStudyTrial (StreamWriter fid, int word_pair_id, int word_group_id, int desired_word_group_id, DateTime time_presented)
        {
            double matlab_time = MathHelperMethods.ConvertDateTimeToMatlabDatenum(time_presented);
            fid.WriteLine(word_pair_id.ToString() + ", " + word_group_id.ToString() + ", " + desired_word_group_id.ToString() 
                + ", " + matlab_time.ToString());
        }

        /// <summary>
        /// Writes to the study file when a tone occurs
        /// </summary>
        public static void WriteStudyTone (StreamWriter fid, DateTime tone_timestamp)
        {
            double matlab_time = MathHelperMethods.ConvertDateTimeToMatlabDatenum(tone_timestamp);
            fid.WriteLine("TONE, " + matlab_time.ToString());
        }

        #endregion
    }
}
