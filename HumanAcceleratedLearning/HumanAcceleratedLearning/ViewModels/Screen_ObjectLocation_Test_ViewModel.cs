using HumanAcceleratedLearning.Helpers;
using HumanAcceleratedLearning.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HumanAcceleratedLearning.ViewModels
{
    public class Screen_ObjectLocation_Test_ViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        SessionModel _model = SessionModel.GetInstance();
        private string _current_img_path = string.Empty;
        private BitmapImage _current_img = null;

        #endregion

        #region Constructor

        public Screen_ObjectLocation_Test_ViewModel ()
        {
            _model.PropertyChanged += this.ExecuteReactionsToModelPropertyChanged;
        }

        #endregion

        #region Overrides

        protected override void ExecuteReactionsToModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.ExecuteReactionsToModelPropertyChanged(sender, e);

            if (_model.CurrentPhase is Phase_ObjectLocationTest)
            {
                var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                
                var vm_file = Path.GetFileName(this.CurrentImagePath);
                var m_file = Path.GetFileName(cp.CurrentImagePath);
                if (!m_file.Equals(vm_file))
                {
                    var full_path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, cp.CurrentImagePath);
                    FileAttributes attr = File.GetAttributes(full_path);
                    if (!attr.HasFlag(FileAttributes.Directory))
                    {
                        FileInfo f_info = new FileInfo(full_path);
                        if (f_info.Exists)
                        {
                            this.CurrentImagePath = full_path;
                        }
                    }
                }
            }
        }

        #endregion

        #region Properties

        public BitmapImage CurrentImageSource
        {
            get
            {
                return _current_img;
            }
            set
            {
                _current_img = value;
                NotifyPropertyChanged("CurrentImageSource");
            }
        }

        public string CurrentImagePath
        {
            get
            {
                return _current_img_path;
            }
            set
            {
                _current_img_path = value;
                NotifyPropertyChanged("CurrentImagePath");
            }
        }

        public double CurrentImageLocationX
        {
            get
            {
                if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    return cp.CurrentImageLocationX;
                }

                return 0;
            }
            set
            {
                if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    cp.CurrentImageLocationX = value;
                }
            }
        }

        public double CurrentImageLocationY
        {
            get
            {
                if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    return cp.CurrentImageLocationY;
                }

                return 0;
            }
            set
            {
                if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    cp.CurrentImageLocationY = value;
                }
            }
        }

        public double CurrentImageWidth
        {
            get
            {
                if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    return (cp.ColumnWidth * 1.5);
                }

                return 0;
            }
        }

        public double CurrentImageHeight
        {
            get
            {
                if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    return (cp.RowHeight * 2.0);
                }

                return 0;
            }
        }

        public double ColumnWidth
        {
            get
            {
                if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    return cp.ColumnWidth;
                }

                return 0;
            }
        }

        public double RowHeight
        {
            get
            {
                if (_model.CurrentPhase is Phase_ObjectLocationTest)
                {
                    var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                    return cp.RowHeight;
                }

                return 0;
            }
        }

        #endregion

        #region Methods

        public void SetScreenDimensionsInModel(int w, int h)
        {
            if (_model.CurrentPhase is Phase_ObjectLocationTest)
            {
                //Set the width and height in the phase model
                var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                cp.ScreenWidth = w;
                cp.ScreenHeight = h;
            }
        }

        #endregion

        #region Methods

        public void SetMouseReleased (bool r)
        {
            if (_model.CurrentPhase is Phase_ObjectLocationTest)
            {
                var cp = _model.CurrentPhase as Phase_ObjectLocationTest;
                cp.MouseIsReleased = r;
            }
        }

        #endregion
    }
}
