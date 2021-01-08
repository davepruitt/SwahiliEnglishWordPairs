using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;

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
        public static void WriteStudyTrial (StreamWriter fid, string foreign_language_word, string native_word, DateTime time_presented)
        {
            double matlab_time = MathHelperMethods.ConvertDateTimeToMatlabDatenum(time_presented);
            fid.WriteLine(foreign_language_word.ToString() + ", " + native_word.ToString() + ", " + matlab_time.ToString());
            fid.Flush();
        }

        /// <summary>
        /// Writes a study trial for the object location to the file
        /// </summary>
        public static void WriteObjectLocationStudyTrial (StreamWriter fid, string image_name, double xpos, double ypos, DateTime time_presented)
        {
            double matlab_time = MathHelperMethods.ConvertDateTimeToMatlabDatenum(time_presented);
            fid.WriteLine("OBJECT, " + image_name + ", " + xpos.ToString("0.##") + ", " + ypos.ToString("0.##") + ", " + matlab_time.ToString());
            fid.Flush();
        }

        #endregion
    }
}
