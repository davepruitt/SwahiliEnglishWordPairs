using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// A simple view-model class that is used for audio devices
    /// </summary>
    public class AudioDeviceViewModel : NotifyPropertyChangedObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AudioDeviceViewModel(WaveInCapabilities device_info, int index)
        {
            DeviceInfo = device_info;
            Index = index;
            ProductName = device_info.ProductName;
        }

        private string _product_name = string.Empty;

        public WaveInCapabilities DeviceInfo;

        public int Index = 0;

        public string ProductName
        {
            get
            {
                return _product_name;
            }
            set
            {
                _product_name = value;
                NotifyPropertyChanged("ProductName");
            }
        }
    }
}
