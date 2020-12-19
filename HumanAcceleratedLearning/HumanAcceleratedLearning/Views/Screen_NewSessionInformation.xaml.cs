using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using HumanAcceleratedLearning.ViewModels;

namespace HumanAcceleratedLearning.Views
{
    /// <summary>
    /// Interaction logic for Screen_NewSessionInformation.xaml
    /// </summary>
    public partial class Screen_NewSessionInformation : UserControl, INotifyPropertyChanged
    {
        public Screen_NewSessionInformation()
        {
            InitializeComponent();

            //Set the width and height
            Width = 600;
            Height = 500;

            //Create a view-model object for this user control
            DataContext = new Screen_NewSessionInformation_ViewModel();
        }

        private void ParticpantID_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ParticpantID_TextBox.SelectAll();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NotifyPropertyChanged("Result");
        }
        
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
