using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning
{
    public class TestBlockTrial
    {
        #region Public data members
        
        public string ForeignWord = string.Empty;
        public string EnglishInput = string.Empty;
        public bool Correct = false;
        public DateTime PresentationTime = DateTime.MinValue;
        public double InputLatency = double.NaN;
        
        #endregion

        #region Constructor

        public TestBlockTrial()
        {
            //empty
        }

        #endregion

        #region Static methods

        public static void WriteTestBlockTrial ( StreamWriter fid, TestBlockTrial t )
        {
            double matlab_time_presented = MathHelperMethods.ConvertDateTimeToMatlabDatenum(t.PresentationTime);
            fid.WriteLine(t.ForeignWord + ", " + t.EnglishInput + ", " + t.Correct.ToString() + ", " + 
                matlab_time_presented.ToString() + ", " + t.InputLatency.ToString());
            fid.Flush();
        }

        #endregion
    }
}
