using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Windows;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// A view-model class used for selecting an audio input device
    /// </summary>
    public class AudioDeviceSelectionViewModel : NotifyPropertyChangedObject
    {
        #region Constructor

        private static AudioDeviceSelectionViewModel _instance = null;

        /// <summary>
        /// Constructor
        /// </summary>
        private AudioDeviceSelectionViewModel()
        {
            QueryAudioDevices();
        }

        public static AudioDeviceSelectionViewModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AudioDeviceSelectionViewModel();
            }

            return _instance;
        }

        #endregion

        #region Private methods

        private void QueryAudioDevices()
        {
            //Discover how many devices exist
            int num_devices = WaveIn.DeviceCount;

            //Get the info of each discovered audio device
            for (int i = 0; i < num_devices; i++)
            {
                WaveInCapabilities device_info = WaveIn.GetCapabilities(i);
                AudioDeviceViewModel new_device_vm = new AudioDeviceViewModel(device_info, i);
                _available_devices.Add(new_device_vm);
            }

            NotifyPropertyChanged("AvailableAudioDevices");
            NotifyPropertyChanged("NoAudioDevicesDetectedText");
            NotifyPropertyChanged("NoAudioDevicesDetectedTextVisibility");
        }

        #endregion

        #region Private properties

        private List<AudioDeviceViewModel> _available_devices = new List<AudioDeviceViewModel>();
        private int _selected_index = 0;
        private SimpleCommand _refresh_command;

        #endregion

        #region Public properties

        public bool ResultOK = false;

        /// <summary>
        /// Collection of view-models for each audio device
        /// </summary>
        public List<AudioDeviceViewModel> AvailableAudioDevices
        {
            get
            {
                return _available_devices;
            }
        }

        /// <summary>
        /// Index of the selected audio device
        /// </summary>
        public int SelectedAudioDeviceIndex
        {
            get
            {
                return _selected_index;
            }
            set
            {
                _selected_index = value;
            }
        }

        /// <summary>
        /// Text to display if no audio devices were found
        /// </summary>
        public string NoAudioDevicesDetectedText
        {
            get
            {
                if (_available_devices.Count == 0)
                {
                    return "No audio devices found";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Whether or not the "no audio devices" text is visible
        /// </summary>
        public Visibility NoAudioDevicesDetectedTextVisibility
        {
            get
            {
                if (_available_devices.Count == 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public SimpleCommand RefreshCommand
        {
            get
            {
                return _refresh_command ?? (_refresh_command = new SimpleCommand(() => ToggleRefresh(), true));
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Refreshes the list of audio devices that are available
        /// </summary>
        public void ToggleRefresh()
        {
            QueryAudioDevices();
        }

        #endregion
    }
}
