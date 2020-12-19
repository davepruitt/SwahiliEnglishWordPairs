using HumanAcceleratedLearning.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    public class TestBlockTrial_ObjectLocation
    {
        #region Data

        public string ImageName = string.Empty;
        public int PositionX = 0;
        public int PositionY = 0;
        public int CorrectPositionX = 0;
        public int CorrectPositionY = 0;
        public double DistanceFromCorrectPosition = 0;
        public bool Correct = false;
        public DateTime PresentationTime = DateTime.MinValue;
        public double InputLatency = double.NaN;

        #endregion

        #region Constructor

        public TestBlockTrial_ObjectLocation ()
        {
            //empty
        }

        #endregion

        #region Static methods

        public static void WriteTestBlockTrial(StreamWriter fid, TestBlockTrial_ObjectLocation t)
        {
            string pos_x = t.PositionX.ToString();
            string pos_y = t.PositionY.ToString();
            string cpos_x = t.CorrectPositionX.ToString();
            string cpos_y = t.CorrectPositionY.ToString();
            string dist = t.DistanceFromCorrectPosition.ToString("0.##");
            string correct_bool = t.Correct.ToString();
            string latency = t.InputLatency.ToString("0.##");
            double matlab_time_presented = MathHelperMethods.ConvertDateTimeToMatlabDatenum(t.PresentationTime);

            fid.WriteLine(t.ImageName + ", " + pos_x + ", " + pos_y + ", " + cpos_x + ", " + cpos_y + ", " + dist + ", " + correct_bool + ", " +
                latency + ", " + matlab_time_presented.ToString());
            fid.Flush();
        }

        #endregion
    }
}
