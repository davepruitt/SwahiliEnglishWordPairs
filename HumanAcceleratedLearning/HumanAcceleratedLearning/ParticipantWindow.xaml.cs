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
using HumanAcceleratedLearning.Models;
using HumanAcceleratedLearning.Views;

namespace HumanAcceleratedLearning
{
    /// <summary>
    /// Interaction logic for ParticipantWindow.xaml
    /// </summary>
    public partial class ParticipantWindow : Window
    {
        public ParticipantWindow()
        {
            InitializeComponent();

            //Subscribe to changes on the model
            SessionModel.GetInstance().PropertyChanged += ReactToSessionModelPropertyChanged;

            //Determine whether to maximize the window or not
            double w = System.Windows.SystemParameters.PrimaryScreenWidth;
            double h = System.Windows.SystemParameters.PrimaryScreenHeight;
            if (w >= (h * 2))
            {
                this.WindowState = WindowState.Normal;
                this.Width = 1024;
                this.Height = 768;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }

            //Initialize the UI
            InitializeUserInterface();
        }

        private void InitializeUserInterface ()
        {
            ParticipantWindow_MainGrid.Children.Add(new Screen_Welcome());
        }

        private void ReactToSessionModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var model = SessionModel.GetInstance();
            var program_state = model.ProgramState;

            if (e.PropertyName.Equals("ProgramState"))
            {
                if (program_state == SessionModel.ProgramStateEnumeration.Finished)
                {
                    ParticipantWindow_MainGrid.Children.Clear();
                    ParticipantWindow_MainGrid.Children.Add(new Screen_Closing());
                }
            }
            else if (e.PropertyName.Equals("CurrentPhase"))
            {
                ParticipantWindow_MainGrid.Children.Clear();

                switch (model.CurrentPhase.PhaseType)
                {
                    case PhaseType.InstructionsScreen:
                        ParticipantWindow_MainGrid.Children.Add(new Screen_Instructions());
                        break;
                    case PhaseType.BlockScreen:
                        ParticipantWindow_MainGrid.Children.Add(new Screen_BlockNumber());
                        break;
                    case PhaseType.DistractionScreen:
                        ParticipantWindow_MainGrid.Children.Add(new Screen_Distraction());
                        break;
                    case PhaseType.StudySession:
                        ParticipantWindow_MainGrid.Children.Add(new Screen_Study());
                        break;
                    case PhaseType.TestSession:
                        ParticipantWindow_MainGrid.Children.Add(new Screen_Test());
                        break;
                    case PhaseType.ObjectLocation_StudySession:
                        ParticipantWindow_MainGrid.Children.Add(new Screen_ObjectLocation(0));
                        break;
                    case PhaseType.ObjectLocation_TestSession:
                        ParticipantWindow_MainGrid.Children.Add(new Screen_ObjectLocation(1));
                        break;
                }
            }
        }
    }
}
