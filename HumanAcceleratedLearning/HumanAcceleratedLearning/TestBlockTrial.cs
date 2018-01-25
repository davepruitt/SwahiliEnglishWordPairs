using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning
{
    public class TestBlockTrial
    {
        #region Public data members

        public int WordPairID;
        public WordGroup WordGroupID;
        public string SwahiliWord;
        public string EnglishInput;
        public int Correct;
        public DateTime PresentationTime;
        public double InputLatency;
        
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
            fid.WriteLine(t.WordPairID.ToString() + ", " + ((int)t.WordGroupID).ToString() + ", " + t.SwahiliWord + ", " +
                t.EnglishInput + ", " + t.Correct.ToString() + ", " + matlab_time_presented.ToString() + ", " +
                t.InputLatency.ToString());
        }

        #endregion
    }
}
