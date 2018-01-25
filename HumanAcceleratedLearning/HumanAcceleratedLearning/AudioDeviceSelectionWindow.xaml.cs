using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// Interaction logic for AudioDeviceSelectionWindow.xaml
    /// </summary>
    public partial class AudioDeviceSelectionWindow : Window
    {
        public AudioDeviceSelectionWindow()
        {
            InitializeComponent();
            DataContext = AudioDeviceSelectionViewModel.GetInstance();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AudioDeviceSelectionViewModel vm = DataContext as AudioDeviceSelectionViewModel;
            if (vm != null)
            {
                vm.ResultOK = true;
            }

            //Close the audio device selection window
            this.Close();
        }
    }
}
