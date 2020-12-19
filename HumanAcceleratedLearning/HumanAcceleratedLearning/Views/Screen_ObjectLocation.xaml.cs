using HumanAcceleratedLearning.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace HumanAcceleratedLearning.Views
{
    /// <summary>
    /// Interaction logic for Screen_ObjectLocation.xaml
    /// </summary>
    public partial class Screen_ObjectLocation : UserControl
    {
        private bool drag_started = false;

        public Screen_ObjectLocation(int session_type)
        {
            InitializeComponent();

            if (session_type == 0)
            {
                var vm = new Screen_ObjectLocation_Study_ViewModel();
                vm.PropertyChanged += HandleViewModel_PropertyChanged;

                DataContext = vm;
            }
            else if (session_type == 1)
            {
                var vm = new Screen_ObjectLocation_Test_ViewModel();
                vm.PropertyChanged += HandleViewModel_PropertyChanged;

                DataContext = vm;
            }
        }

        private void HandleViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.DataContext is Screen_ObjectLocation_Study_ViewModel)
            {
                var vm = this.DataContext as Screen_ObjectLocation_Study_ViewModel;
                if (vm != null)
                {
                    if (vm.ColumnWidth == 0 && vm.RowHeight == 0)
                    {
                        vm.SetScreenDimensionsInModel(Convert.ToInt32(this.MainObjectLocationGrid.ActualWidth),
                            Convert.ToInt32(this.MainObjectLocationGrid.ActualHeight));
                    }

                    double left = Math.Max(0, vm.CurrentImageLocationX - (vm.ColumnWidth * 0.75));
                    double top = Math.Max(0, vm.CurrentImageLocationY - vm.RowHeight);

                    BitmapImage bmp = new BitmapImage(new Uri(vm.CurrentImagePath, UriKind.Absolute));
                    MainObjectLocationImage.Source = bmp;

                    MainObjectLocationBorder.Width = vm.ColumnWidth * 1.5;
                    MainObjectLocationBorder.Height = vm.RowHeight * 2.0;
                    MainObjectLocationBorder.Margin = new Thickness(left, top, 0, 0);
                }
            }
            else if (this.DataContext is Screen_ObjectLocation_Test_ViewModel)
            {
                var vm = this.DataContext as Screen_ObjectLocation_Test_ViewModel;
                if (vm != null)
                {
                    {
                        vm.SetScreenDimensionsInModel(Convert.ToInt32(this.MainObjectLocationGrid.ActualWidth),
                            Convert.ToInt32(this.MainObjectLocationGrid.ActualHeight));
                    }

                    drag_started = false;

                    double left = Math.Max(0, vm.CurrentImageLocationX - (vm.ColumnWidth * 0.75));
                    double top = Math.Max(0, vm.CurrentImageLocationY - vm.RowHeight);

                    BitmapImage bmp = new BitmapImage(new Uri(vm.CurrentImagePath, UriKind.Absolute));
                    MainObjectLocationImage.Source = bmp;

                    MainObjectLocationBorder.Width = vm.ColumnWidth * 1.5;
                    MainObjectLocationBorder.Height = vm.RowHeight * 2.0;
                    MainObjectLocationBorder.Margin = new Thickness(left, top, 0, 0);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is Screen_ObjectLocation_Study_ViewModel)
            {
                var vm = this.DataContext as Screen_ObjectLocation_Study_ViewModel;
                if (vm != null)
                {
                    vm.SetScreenDimensionsInModel(Convert.ToInt32(this.MainObjectLocationGrid.ActualWidth),
                        Convert.ToInt32(this.MainObjectLocationGrid.ActualHeight));
                }
            }
            else if (this.DataContext is Screen_ObjectLocation_Test_ViewModel)
            {
                var vm = this.DataContext as Screen_ObjectLocation_Test_ViewModel;
                if (vm != null)
                {
                    vm.SetScreenDimensionsInModel(Convert.ToInt32(this.MainObjectLocationGrid.ActualWidth),
                        Convert.ToInt32(this.MainObjectLocationGrid.ActualHeight));
                }
            }
        }

        private void MainObjectLocationBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drag_started = true;

            var vm = this.DataContext as Screen_ObjectLocation_Test_ViewModel;
            if (vm != null)
            {
                vm.SetMouseReleased(false);
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag_started)
            {
                var pos = Mouse.GetPosition(MainObjectLocationBorder);
                var cx = MainObjectLocationBorder.Margin.Left;
                var cy = MainObjectLocationBorder.Margin.Top;

                var vm = this.DataContext as Screen_ObjectLocation_Test_ViewModel;
                if (vm != null)
                {
                    vm.CurrentImageLocationX += pos.X - (vm.ColumnWidth * 0.75);
                    vm.CurrentImageLocationY += pos.Y - vm.RowHeight;

                    MainObjectLocationBorder.Margin = new Thickness(cx + pos.X - (vm.ColumnWidth * 0.75), cy + pos.Y - vm.RowHeight, 0, 0);
                }
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drag_started = false;

            var vm = this.DataContext as Screen_ObjectLocation_Test_ViewModel;
            if (vm != null)
            {
                vm.SetMouseReleased(true);
            }
        }
    }
}
