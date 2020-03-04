using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            }

        }

        Model currentModel;

        public ModelInterface(Model cm) => currentModel = cm;

        public async Task<string> GetCompartmentVolumesAsync(int id = 1) => await Task.Run(() => { 
            Dictionary<string,double> _volumes = new Dictionary<string, double>();
            foreach (BloodCompartment bc in currentModel.modelDefinition.blood_compartments)
            {
                _volumes.Add(bc.name, bc.vol_current);        
            }
            foreach (GasCompartment gc in currentModel.modelDefinition.gas_compartments)
            {
                _volumes.Add(gc.name, gc.vol_current);        
            }
            return JsonConvert.SerializeObject( _volumes );
        });

        public async Task<string> GetConnectorFlowsAsync(int id = 1) => await Task.Run(() => { 
            Dictionary<string,double> _flows = new Dictionary<string, double>();
            foreach (BloodConnector bc in currentModel.modelDefinition.blood_connectors)
            {
                _flows.Add(bc.name, bc.real_flow);        
            }
            foreach (GasConnector gc in currentModel.modelDefinition.gas_connectors)
            {
                _flows.Add(gc.name, gc.real_flow);        
            }
            return JsonConvert.SerializeObject( _flows );
        });  

    }

}
