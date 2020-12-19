using HumanAcceleratedLearning.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    public static class ObjectLocation_FileHandler
    {
        /// <summary>
        /// Creates a study block file for the specified username
        /// </summary>
        public static StreamWriter CreateObjectLocationFileForWriting(string username)
        {
            string date_for_filename = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string file_name = username + "_objectlocation_" + date_for_filename + ".txt";

            string fully_qualified_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\";
            DirectoryInfo dir_info = new DirectoryInfo(fully_qualified_path);
            if (!dir_info.Exists)
            {
                dir_info.Create();
            }

            string fully_qualified_file = fully_qualified_path + file_name;

            StreamWriter writer = new StreamWriter(fully_qualified_file);
            
            return writer;
        }

        public static bool CheckIfObjectLocationFileExists (string username)
        {
            string fully_qualified_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\";
            DirectoryInfo dir_info = new DirectoryInfo(fully_qualified_path);
            if (dir_info.Exists)
            {
                var files = dir_info.EnumerateFiles();
                var f = files.Where(x => x.Name.Contains("objectlocation")).FirstOrDefault();
                return (f != null);
            }

            return false;
        }

        public static StreamReader OpenObjectLocationFileForReading (string username)
        {
            string fully_qualified_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\";
            DirectoryInfo dir_info = new DirectoryInfo(fully_qualified_path);
            if (dir_info.Exists)
            {
                var files = dir_info.EnumerateFiles();
                var f = files.Where(x => x.Name.Contains("objectlocation")).FirstOrDefault();
                if (f != null)
                {
                    StreamReader fid = new StreamReader(f.FullName);
                    return fid;
                }
            }

            return null;
        }

        public static string GetNameOfObjectLocationFile (string username)
        {
            string fully_qualified_path = HumanAcceleratedLearningConfiguration.GetInstance().PrimarySavePath + username + @"\";
            DirectoryInfo dir_info = new DirectoryInfo(fully_qualified_path);
            if (dir_info.Exists)
            {
                var files = dir_info.EnumerateFiles();
                var f = files.Where(x => x.Name.Contains("objectlocation")).FirstOrDefault();
                if (f != null)
                {
                    return f.FullName;
                }
            }

            return string.Empty;
        }

        public static void ReadObjectLocationFile (HumanSubject s)
        {
            if (CheckIfObjectLocationFileExists(s.UserName))
            {
                var file_name = GetNameOfObjectLocationFile(s.UserName);
                if (!string.IsNullOrEmpty(file_name))
                {
                    var file_lines = Helpers.ConfigurationFileLoader.LoadFileLines(file_name);
                    foreach (var l in file_lines)
                    {
                        string [] line_parts = l.Split(new char[] { ',' });
                        if (line_parts.Length == 3)
                        {
                            string img_name = line_parts[0].Trim();
                            bool xpos_success = Int32.TryParse(line_parts[1], out int xpos);
                            bool ypos_success = Int32.TryParse(line_parts[2], out int ypos);
                            if (xpos_success && ypos_success)
                            {
                                s.AllObjectImages.Add(img_name);
                                s.AllObjectImageLocations.Add(new Tuple<int, int>(xpos, ypos));
                            }
                        }
                    }
                }
            }
        }

        public static void WriteAllObjectLocationsToFile(HumanSubject s)
        {
            var fid = CreateObjectLocationFileForWriting(s.UserName);
            for (int i = 0; i < s.AllObjectImages.Count; i++)
            {
                WriteObjectLocationTrial(fid, s.AllObjectImages[i], s.AllObjectImageLocations[i].Item1, s.AllObjectImageLocations[i].Item2);
            }

            fid.Close();
        }
        
        /// <summary>
        /// Writes a study trial for the object location to the file
        /// </summary>
        public static void WriteObjectLocationTrial(StreamWriter fid, string image_name, int xpos, int ypos)
        {
            fid.WriteLine(image_name + ", " + xpos.ToString() + ", " + ypos.ToString());
            fid.Flush();
        }
    }
}
