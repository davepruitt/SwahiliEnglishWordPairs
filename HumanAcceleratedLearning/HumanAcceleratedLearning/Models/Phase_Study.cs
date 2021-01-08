using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning.Models
{
    public class Phase_Study : Phase
    {
        #region Constructor

        public Phase_Study()
            : base()
        {
            PhaseType = PhaseType.StudySession;
        }

        #endregion

        #region Properties

        public string ForeignLanguage { get; set; } = string.Empty;

        public string NativeLanguage { get; set; } = string.Empty;
        
        public int Spacing { get; set; } = 0;

        public string CurrentForeignLanguageWord { get; set; } = string.Empty;

        public string CurrentNativeLanguageWord { get; set; } = string.Empty;

        public int CurrentPhase_NumberOfWordsCompleted
        {
            get
            {
                return current_wordpair_index;
            }
        }

        public int TotalWords
        {
            get
            {
                if (shuffled_word_list != null)
                {
                    return shuffled_word_list.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Protected variables

        protected LanguageDictionary source_language_dictionary = null;
        protected List<Tuple<string, string>> shuffled_word_list = null;
        protected DateTime last_word_display_time = DateTime.MinValue;
        protected int current_wordpair_index = 0;
        protected StreamWriter fid = null;

        #endregion

        #region Overrides
        
        public override void Start(HumanSubject current_subject = null)
        {
            if (current_subject != null)
            {
                //Get the list of words to use for this study block
                var hal_config = HumanAcceleratedLearningConfiguration.GetInstance();
                var available_dictionaries = hal_config.LanguageDictionaries;
                source_language_dictionary = available_dictionaries.Where(x => 
                    x.ForeignLanguageName.Equals(ForeignLanguage) &&
                    x.NativeLanguageName.Equals(NativeLanguage)
                    ).FirstOrDefault();

                //Shuffle the words
                if (source_language_dictionary != null)
                {
                    var word_list = source_language_dictionary.DictionaryWordPairs.ToList();
                    shuffled_word_list = MathHelperMethods.ShuffleList(word_list);
                    last_word_display_time = DateTime.MinValue;
                    current_wordpair_index = 0;
                    
                    //Create a file for this study session
                    fid = StudyBlock.CreateStudyBlockFile(current_subject.UserName);

                    HasPhaseStarted = true;
                }
            }
        }

        public override void Update()
        {
            //If the shuffled word list is null or has 0 words, immediately exit
            if (shuffled_word_list == null || (shuffled_word_list != null && shuffled_word_list.Count == 0))
            {
                IsPhaseFinished = true;
                return;
            }

            //If it is not yet time to display a new word pair...
            //Check to see if it is time to make the current word pair disappear...
            if (DateTime.Now >= (last_word_display_time + TimeSpan.FromMilliseconds(this.Duration)))
            {
                CurrentForeignLanguageWord = string.Empty;
                CurrentNativeLanguageWord = string.Empty;
            }

            if (DateTime.Now >= (last_word_display_time + TimeSpan.FromMilliseconds(this.Duration + this.Spacing)))
            {
                //Check to see if we are done
                if (current_wordpair_index >= shuffled_word_list.Count)
                {
                    //Set the flag indicating this phase is finished
                    IsPhaseFinished = true;

                    //Close the save file for this study block
                    if (fid != null)
                    {
                        fid.Close();
                    }
                    
                    //Return from this function immediately
                    return;
                }
                else
                {
                    //If we are not done yet...
                    //Grab the next word pair
                    CurrentForeignLanguageWord = shuffled_word_list[current_wordpair_index].Item1;
                    CurrentNativeLanguageWord = shuffled_word_list[current_wordpair_index].Item2;

                    //Set the time for when this word pair was displayed
                    last_word_display_time = DateTime.Now;

                    //Increment the word pair index
                    current_wordpair_index++;

                    //Save this trial
                    if (fid != null)
                    {
                        StudyBlock.WriteStudyTrial(fid, CurrentForeignLanguageWord, CurrentNativeLanguageWord, last_word_display_time);
                    }
                }
            }
        }

        public override void InitializePhaseFromParameters(List<Tuple<string, string>> parameters, int loop_counter = 0)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                var pname = parameters[i].Item1;
                var pval = parameters[i].Item2;
                var pval_is_loop_param = pval.Equals("n");
                var pval_parse_success = Int32.TryParse(pval, out int pval_parse_result);

                if (pname.Equals("duration"))
                {
                    Duration = (pval_is_loop_param) ? loop_counter : (pval_parse_success) ? pval_parse_result : 0;
                }
                else if (pname.Equals("spacing"))
                {
                    Spacing = (pval_is_loop_param) ? loop_counter : (pval_parse_success) ? pval_parse_result : 0;
                }
                else if (pname.Equals("language"))
                {
                    ForeignLanguage = pval.ToLower();
                    NativeLanguage = "english";
                }
                else if (pname.Equals("foreign"))
                {
                    ForeignLanguage = pval.ToLower();
                }
                else if (pname.Equals("native"))
                {
                    NativeLanguage = pval.ToLower();
                }
            }
        }

        #endregion
    }
}
