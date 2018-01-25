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
    /// Interaction logic for SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        public SessionWindow()
        {
            InitializeComponent();

            //Set the data context to a new view-model object
            DataContext = new SessionWindowViewModel();
        }
        
        private void Window_Closed(object sender, EventArgs e)
        {
            //empty
        }
        
        private void EnglishResponseTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (TestScreen.IsVisible)
            {
                bool success = EnglishResponseTextBox.Focus();
                var result = Keyboard.Focus(EnglishResponseTextBox);
            }
        }
    }
}
