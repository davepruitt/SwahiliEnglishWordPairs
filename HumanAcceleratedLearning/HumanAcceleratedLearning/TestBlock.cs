using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Loads a test-block file
        /// </summary>
        public static TestBlock LoadTestBlockFile ( string file_name )
        {
            TestBlock result = new TestBlock();

            try
            {
                List<string> file_lines = ConfigurationFileLoader.LoadFileLines(file_name);
                if (file_lines.Count > 0)
                {
                    result.UserName = file_lines[0];

                    for (int i = 1; i < file_lines.Count; i++)
                    {
                        string[] this_line = file_lines[i].Trim().Split(new char[] { ',' });

                        TestBlockTrial t = new TestBlockTrial();
                        t.WordPairID = Int32.Parse(this_line[0]);
                        t.WordGroupID = (WordGroup)Int32.Parse(this_line[1]);
                        t.SwahiliWord = this_line[2];
                        t.EnglishInput = this_line[3];
                        t.Correct = Int32.Parse(this_line[4]);
                        t.PresentationTime = MathHelperMethods.ConvertMatlabDatenumToDateTime(Double.Parse(this_line[5]));
                        t.InputLatency = Double.Parse(this_line[6]);

                        result.Trials.Add(t);
                    }
                }
            }
            catch (Exception)
            {
                //empty
            }
            
            return result;
        }

        /// <summary>
        /// Loads every single test block file for a current user.
        /// </summary>
        public static List<TestBlock> LoadAllTestBlockFiles ( string username )
        {
            List<TestBlock> result = new List<TestBlock>();

            string search_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\Test\";
            DirectoryInfo dir_info = new DirectoryInfo(search_path);
            if (dir_info.Exists)
            {
                var files_in_search_path = dir_info.EnumerateFiles().ToList();

                if (files_in_search_path.Count > 0)
                {
                    foreach (var f in files_in_search_path)
                    {
                        var r = TestBlock.LoadTestBlockFile(f.FullName);
                        result.Add(r);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Loads all test block files that occured today for the specified user.
        /// </summary>
        public static List<TestBlock> LoadTestBlockFilesFromToday ( string username )
        {
            List<TestBlock> result = new List<TestBlock>();

            string search_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\Test\";
            DirectoryInfo dir_info = new DirectoryInfo(search_path);
            if (dir_info.Exists)
            {
                var files_in_search_path = dir_info.EnumerateFiles().ToList();

                if (files_in_search_path.Count > 0)
                {
                    var todays_files = files_in_search_path.Where(x => x.CreationTime.Date == DateTime.Today.Date).ToList();
                    
                    foreach (var f in todays_files)
                    {
                        var r = TestBlock.LoadTestBlockFile(f.FullName);
                        result.Add(r);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Loads the most recent test block for a user
        /// </summary>
        public static TestBlock LoadMostRecentTestBlockFile ( string username )
        {
            TestBlock result = null;

            string search_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\Test\";
            DirectoryInfo dir_info = new DirectoryInfo(search_path);
            if (dir_info.Exists)
            {
                var files_in_search_path = dir_info.EnumerateFiles().ToList();
                
                if (files_in_search_path.Count > 0)
                {
                    var most_recent_creation_time = files_in_search_path.Select(x => x.CreationTime).Max();
                    FileInfo most_recent_file = files_in_search_path.Where(x => x.CreationTime == most_recent_creation_time).FirstOrDefault();

                    if (most_recent_file != null)
                    {
                        result = TestBlock.LoadTestBlockFile(most_recent_file.FullName);
                    }
                }
            }

            return result;
        }

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
