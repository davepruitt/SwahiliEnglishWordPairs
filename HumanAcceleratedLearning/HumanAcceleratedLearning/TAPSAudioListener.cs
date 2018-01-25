using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Collections.ObjectModel;

namespace HumanAcceleratedLearning
{
    public class TAPSAudioListener
    {
        #region Private data members

        private WaveIn audio_input = new WaveIn();
        private object audio_signal_lock = new object();
        private ObservableCollection<float> audio_signal = new ObservableCollection<float>();
        private int max_signal_length = 8000;
        private DateTime last_tone_start = DateTime.MinValue;

        #endregion

        #region Constructor

        private static TAPSAudioListener _instance = null;
        private static object _instance_lock = new object();

        private TAPSAudioListener (int device_number)
        {
            if (AudioDeviceSelectionViewModel.GetInstance().AvailableAudioDevices.Count > 0 &&
                device_number < AudioDeviceSelectionViewModel.GetInstance().AvailableAudioDevices.Count)
            {
                int sample_rate = 8000;
                int channels = 1;

                audio_input.DeviceNumber = device_number;
                audio_input.DataAvailable += Audio_input_DataAvailable;
                audio_input.WaveFormat = new WaveFormat(sample_rate, channels);
                audio_input.StartRecording();
            }
        }

        /// <summary>
        /// Returns an instance of the TAPS audio listener.
        /// </summary>
        public static TAPSAudioListener GetInstance ()
        {
            int device_index = AudioDeviceSelectionViewModel.GetInstance().SelectedAudioDeviceIndex;

            if (_instance == null)
            {
                lock(_instance_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TAPSAudioListener(device_index);
                    }
                }
            }

            return _instance;
        }

        #endregion

        #region Callback function that receives audio data from the microphone

        private void Audio_input_DataAvailable(object sender, WaveInEventArgs e)
        {
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                float sample32 = Math.Max(sample / 32768f, 0f);

                lock(audio_signal_lock)
                {
                    audio_signal.Add(sample32);
                    if (audio_signal.Count > max_signal_length)
                    {
                        audio_signal.RemoveAt(0);
                    }
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the current audio data in the buffer
        /// </summary>
        public List<float> RetrieveCurrentAudioData ()
        {
            List<float> result;

            lock(audio_signal_lock)
            {
                result = audio_signal.ToList();
            }

            return result;
        }

        public bool IsAudioPlaying ()
        {
            //Grab the audio signal
            List<float> audio;
            lock (audio_signal_lock)
            {
                audio = audio_signal.ToList();
            }

            //Check to see if there is sound playing.
            //We do this by simply summing up each sample to see if it exceeds a certain threshold.
            //If it does, we assume there is sound on the audio line.
            //By trial-and-error, it seems like the sound level is usually about 0.1/sample.
            //If we sum up 100 samples (8000 samples/sec, so about 0.0125 seconds of sound) that is a value of 10.
            //So we use the value of 10 as our threshold for identifying whether sound is playing.
            //In this case, we are also thresholding the signal, so anything between 0 and 0.1 just becomes 0.  This
            //helps to eliminate noise that may be caused by manually hitting the sound cable.
            float sum_threshold = 10;
            float signal_sum = audio.Select(x => (x >= 0.1) ? 0.1f : 0f).ToList().Sum();
            if (signal_sum >= sum_threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DetectToneStart ()
        {
            bool result = false;

            double elapsed_time = (DateTime.Now - last_tone_start).TotalSeconds;
            if (elapsed_time >= 5.0)
            {
                result = IsAudioPlaying();
                if (result)
                {
                    last_tone_start = DateTime.Now;
                }
            }

            return result;
        }

        public void SubscribeToAudioSignalChanges (System.Collections.Specialized.NotifyCollectionChangedEventHandler d)
        {
            lock(audio_signal_lock)
            {
                audio_signal.CollectionChanged -= d;
                audio_signal.CollectionChanged += d;
            }
        }

        public void UnsubscribeToAudioSignalChanges (System.Collections.Specialized.NotifyCollectionChangedEventHandler d)
        {
            lock(audio_signal_lock)
            {
                audio_signal.CollectionChanged -= d;
            }
        }
        
        #endregion
    }
}
