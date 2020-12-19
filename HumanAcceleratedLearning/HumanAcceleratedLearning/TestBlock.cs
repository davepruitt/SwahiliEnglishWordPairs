using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning
{
    public class TestBlock
    {
        #region Public data members

        public string UserName = string.Empty;
        public List<TestBlockTrial> Trials = new List<TestBlockTrial>();
        
        #endregion

        #region Constructor

        public TestBlock()
        {
            //empty
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Creates a new test block file
        /// </summary>
        public static StreamWriter CreateTestBlockFile ( string username )
        {
            //Construct a file name to write
            string date_for_filename = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string file_name = username + "_" + date_for_filename + ".txt";

            //Construct a full path and file
            string fully_qualified_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\Test\";

            //See if the directory already exists.  If not, then make it.
            DirectoryInfo dir_info = new DirectoryInfo(fully_qualified_path);
            if (!dir_info.Exists)
            {
                //Create the directory
                dir_info.Create();
            }

            //Create a new file for this text block
            string fully_qualified_file = fully_qualified_path + file_name;
            StreamWriter writer = new StreamWriter(fully_qualified_file);

            //Write the username as the file header
            writer.WriteLine(username);

            //Return the StreamWriter object so that it can be used
            return writer;
        }

        #endregion
    }
}
