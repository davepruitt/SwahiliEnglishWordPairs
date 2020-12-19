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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HumanAcceleratedLearning.ViewModels;

namespace HumanAcceleratedLearning.Views
{
    /// <summary>
    /// Interaction logic for Screen_AdminPage.xaml
    /// </summary>
    public partial class Screen_AdminPage : UserControl
    {
        public Screen_AdminPage()
        {
            InitializeComponent();
            DataContext = new Screen_AdminPage_ViewModel();
        }

        private void StartUserSessionButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as Screen_AdminPage_ViewModel;
            if (vm != null)
            {
                vm.HandleMainButtonClick();
            }
        }
    }
}
