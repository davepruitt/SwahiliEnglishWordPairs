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
    /// Interaction logic for DebuggingView.xaml
    /// </summary>
    public partial class DebuggingView : Window
    {
        public DebuggingView()
        {
            InitializeComponent();
            DataContext = new DebuggingViewModel();
        }
    }
}
