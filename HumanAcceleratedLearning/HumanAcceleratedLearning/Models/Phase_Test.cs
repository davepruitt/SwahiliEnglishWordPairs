using HumanAcceleratedLearning.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanAcceleratedLearning.Models
{
    public class Phase_Test : Phase
    {
        #region Constructor

        public Phase_Test()
            : base()
        {
            PhaseType = PhaseType.TestSession;
        }

        #endregion

        #region Properties

        public LanguageClassification Language { get; set; } = LanguageClassification.Undefined;

        public TestClassification TestType { get; set; } = TestClassification.Undefined;

        public string CurrentForeignLanguageCue { get; set; } = string.Empty;

        public string CurrentEnglishLanguageResponse { get; set; } = string.Empty;

        public string CurrentEnglishLanguageCorrectAnswer { get; set; } = string.Empty;

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
        protected DateTime test_start_time = DateTime.MinValue;
        protected int current_wordpair_index = 0;
        protected StreamWriter fid = null;
        protected TestBlockTrial current_trial = null;
        protected HumanSubject current_subject_obj = null;
        protected TimeSpan minimum_test_time = TimeSpan.FromSeconds(5.0);
        

        #endregion

        #region Overrides
        
        public override void Start(HumanSubject current_subject = null)
        {
            if (current_subject != null)
            {
                current_subject_obj = current_subject;

                //Get the list of words to use for this test block
                source_language_dictionary = HumanAcceleratedLearningConfiguration.GetInstance().LanguageDictionaries.Where(x => x.LanguageType == Language).FirstOrDefault();

                //Get the list of words that this participant has already gotten correct in previous test blocks
                var already_correct_words = current_subject.CorrectlyAnsweredWordList.Where(x => x.Item1 == Language).FirstOrDefault();
                if (already_correct_words != null && source_language_dictionary != null)
                {
                    var correct_word_list = already_correct_words.Item2;
                    var all_word_pairs = source_language_dictionary.DictionaryWordPairs.ToList();
                    var ordered_foreign_language_Words = all_word_pairs.Select(x => x.Item1).ToList();

                    //Remove the words from the list of word pairs that have already been answered correctly
                    for (int i = 0; i < correct_word_list.Count; i++)
                    {
                        int index = ordered_foreign_language_Words.IndexOf(correct_word_list[i]);
                        if (index > -1)
                        {
                            ordered_foreign_language_Words.RemoveAt(index);
                            all_word_pairs.RemoveAt(index);
                        }
                    }

                    //Shuffle the remaining words
                    shuffled_word_list = MathHelperMethods.ShuffleList(all_word_pairs);
                    last_word_display_time = DateTime.MinValue;
                    test_start_time = DateTime.Now;
                    current_wordpair_index = 0;

                    //Create a file for this test session
                    fid = TestBlock.CreateTestBlockFile(current_subject.UserName);

                    HasPhaseStarted = true;
                }
            }
        }

        public override void Update()
        {
            //Check to see if the participant has gotten the current word correct
            if (current_trial != null)
            {
                if (!current_trial.Correct)
                {
                    if (CurrentEnglishLanguageResponse.ToLower().Equals(CurrentEnglishLanguageCorrectAnswer.ToLower()))
                    {
                        current_trial.EnglishInput = CurrentEnglishLanguageResponse.ToLower();
                        current_trial.Correct = true;
                        current_trial.InputLatency = (DateTime.Now - current_trial.PresentationTime).TotalSeconds;
                    }
                }
            }

            //Check to see if enough time has passed such that we should move on to the next word
            if (DateTime.Now >= (last_word_display_time + TimeSpan.FromMilliseconds(this.Duration)))
            {
                //Save the current trial before moving on
                if (fid != null && current_trial != null)
                {
                    //If the participant didn't get this trial correct, note it as so.
                    if (!current_trial.Correct)
                    {
                        current_trial.EnglishInput = CurrentEnglishLanguageResponse;
                        current_trial.InputLatency = TimeSpan.FromMilliseconds(Duration).TotalSeconds;
                    }
                    else
                    {
                        //Otherwise, make sure the list of "correctly answered words" includes this latest word
                        var already_correct_words = current_subject_obj.CorrectlyAnsweredWordList.Where(x => x.Item1 == Language).FirstOrDefault();
                        already_correct_words.Item2.Add(CurrentForeignLanguageCue);
                    }
                    
                    //Write out this trial to a file
                    TestBlockTrial.WriteTestBlockTrial(fid, current_trial);
                }

                //Check to see if we are done with all of the words in the list
                if (current_wordpair_index >= shuffled_word_list.Count)
                {
                    //If there were no words in the list to begin with...
                    if (shuffled_word_list.Count == 0)
                    {
                        CurrentForeignLanguageCue = "No more words to test!";
                    }

                    if (DateTime.Now - test_start_time > minimum_test_time)
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
                }
                else
                {
                    //If we are not done yet...
                    //Grab the next word pair
                    CurrentForeignLanguageCue = shuffled_word_list[current_wordpair_index].Item1;
                    CurrentEnglishLanguageCorrectAnswer = shuffled_word_list[current_wordpair_index].Item2;
                    CurrentEnglishLanguageResponse = string.Empty;

                    //Set the time for when this word pair was displayed
                    last_word_display_time = DateTime.Now;

                    //Create a new trial object
                    current_trial = new TestBlockTrial();
                    current_trial.ForeignWord = CurrentForeignLanguageCue;
                    current_trial.PresentationTime = last_word_display_time;

                    //Increment the word pair index
                    current_wordpair_index++;
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
                else if (pname.Equals("language"))
                {
                    if (pval.Equals("japanese"))
                    {
                        Language = LanguageClassification.Japanese;
                    }
                    else if (pval.Equals("swahili"))
                    {
                        Language = LanguageClassification.Swahili;
                    }
                }
                else if (pname.Equals("type"))
                {
                    if (pval.Equals("full"))
                    {
                        TestType = TestClassification.All_Words;
                    }
                    else if (pval.Equals("partial"))
                    {
                        TestType = TestClassification.Partial_Words;
                    }
                }
            }
        }

        #endregion
    }
}
