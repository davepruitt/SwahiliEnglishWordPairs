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
using HumanAcceleratedLearning.Models;
using HumanAcceleratedLearning.Views;

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
            HumanAcceleratedLearningConfiguration.GetInstance();
            HumanAcceleratedLearningConfiguration.GetInstance().LoadAllStages();
            HumanAcceleratedLearningConfiguration.GetInstance().LoadLanguageDictionaries();

            //Determine whether to maximize the window or not
            double w = System.Windows.SystemParameters.PrimaryScreenWidth;
            double h = System.Windows.SystemParameters.PrimaryScreenHeight;
            if (w >= (h * 2))
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }

            //Set up the UI
            Initialize_UserInterface();
        }

        private void Initialize_UserInterface()
        {
            //Create the setup session view
            Screen_NewSessionInformation new_session_ui = new Screen_NewSessionInformation();
            new_session_ui.PropertyChanged += Handle_NewSessionInformation_Result;
            MainWindowContentRegion.Children.Add(new_session_ui);
        }

        private void Handle_NewSessionInformation_Result(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Clear the main window content region of its children
            MainWindowContentRegion.Children.Clear();

            //Display the UI piece in which we run the session
            Screen_AdminPage admin_page_ui = new Screen_AdminPage();
            MainWindowContentRegion.Children.Add(admin_page_ui);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (Window w in System.Windows.Application.Current.Windows)
            {
                if (w != this)
                {
                    w.Close();
                }
            }

            //Stop the session if need be
            var model = SessionModel.GetInstance();
            model.StopSession();
        }
    }
}
