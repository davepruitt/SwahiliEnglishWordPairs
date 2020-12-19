using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    public class LanguageDictionary
    {
        #region Constructor

        public LanguageDictionary ()
        {
            //empty
        }

        #endregion

        #region Properties

        public LanguageClassification LanguageType { get; set; } = LanguageClassification.Undefined;

        public string LanguageName { get; set; } = string.Empty;

        public List<Tuple<string, string>> DictionaryWordPairs { get; set; } = new List<Tuple<string, string>>();

        #endregion
    }
}
