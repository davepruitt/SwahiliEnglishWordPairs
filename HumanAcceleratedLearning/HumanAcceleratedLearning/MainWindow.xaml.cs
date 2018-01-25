using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Load in configuration data
            HumanAcceleratedLearningConfiguration.GetInstance().LoadConfigurationFile();

            //Load in the Swahili/English dictionary
            HumanAcceleratedLearningConfiguration.GetInstance().LoadDictionary();

            //Allow the user to select an audio device
            AudioDeviceSelectionWindow audio_device_selection_window = new AudioDeviceSelectionWindow();
            audio_device_selection_window.ShowDialog();

            //Initialize the audio recording
            TAPSAudioListener.GetInstance();

            DataContext = new SetupScreenViewModel();
        }

        private void UsernameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression exp = UsernameTextBox.GetBindingExpression(TextBox.TextProperty);
                exp.UpdateSource();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var vm = this.DataContext as SetupScreenViewModel;
            if (vm != null)
            {
                vm.StopSession();
            }
        }
    }
}
