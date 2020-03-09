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

        public ModelInterface(Model cm)
        {
            currentModel = cm;
        }

        public Task<string> GetModelStateAsync(int id = 1) 
        {
            return Task.Run(() =>
            {
                Dictionary<string,double[]> _data = new Dictionary<string, double[]>();
                
                _data.Add("heart_rate", new double[] {currentModel.modelDefinition.ecg["heart_rate"]});

                _data.Add("ecg_signal", new double[] {currentModel.modelDefinition.ecg["ecg_signal"]});

                foreach (BloodCompartment bc in currentModel.modelDefinition.blood_compartments)
                {
                    double[] newValue_bc = {bc.vol_current, bc.to2 };
                    _data.Add(bc.name, newValue_bc);        
                }
                foreach (GasCompartment gc in currentModel.modelDefinition.gas_compartments)
                {
                    double[] newValue_gc = {gc.vol_current, gc.to2 };
                    _data.Add(gc.name, newValue_gc);        
                }

                foreach (BloodConnector bcc in currentModel.modelDefinition.blood_connectors)
                {
                    double[] newValue_bcc = {bcc.real_flow };
                    _data.Add(bcc.name, newValue_bcc);        
                }
                foreach (Valve valve in currentModel.modelDefinition.valves)
                {
                    double[] newValue_valve = {valve.real_flow };
                    _data.Add(valve.name, newValue_valve);        
                }
                foreach (Shunt shunt in currentModel.modelDefinition.shunts)
                {
                    double[] newValue_shunt = {shunt.real_flow };
                    _data.Add(shunt.name, newValue_shunt);        
                }
                foreach (GasConnector gcc in currentModel.modelDefinition.gas_connectors)
                {
                    double[] newValue_gcc = {gcc.real_flow };
                    _data.Add(gcc.name, newValue_gcc);        
                }
                
                return JsonConvert.SerializeObject( _data );
            });     
        }
      
      
            
   


       
    }

}
