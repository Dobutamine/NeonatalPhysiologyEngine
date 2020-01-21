using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeonatalPhysiologyEngine.IO
{
    public class ModelInterface : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool _modelUpdated;
        public bool ModelUpdated
        {
            get
            {
                return _modelUpdated;
            }
            set
            {
                _modelUpdated = value;
                OnPropertyChanged();

            }
        }

        FormattableString _statusMessage;
        public FormattableString StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage == value)
                    return;

                _statusMessage = value;
                OnPropertyChanged();
                Console.WriteLine(StatusMessage);
            }

        }

        public int MonitoringMode { get; set; } = 1; // 0 = off, 1 = hires, 2 = lores
        
        Model currentModel;

        public ModelInterface(Model cm)
        {
            currentModel = cm;
        }

        public void UpdateHiResData()
        {

        }
    }
}
