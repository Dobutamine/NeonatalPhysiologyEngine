using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public Task<string> GetCompartmentDataAsync(string comp_name)
        {
            return Task.Run(() =>
            {
                BloodCompartment bc = currentModel.FindModelComponent<BloodCompartment>(comp_name);
                return JsonConvert.SerializeObject(bc);
            });
        }

        public Task<string> GetModelStatusMessageAsync()
        {
            return Task.Run(() =>
            {
                return JsonConvert.SerializeObject(currentModel.modelInterface.StatusMessage);
            });
        }

        public Task<string> GetCompartmentNameListAsync(int comp_type)
        {
            return Task.Run(() =>
            {
                List<string> comp_list = new List<string>();

                switch (comp_type)
                {
                    case 0: // blood
                        foreach (BloodCompartment bc in currentModel.modelDefinition.blood_compartments)
                        {
                            comp_list.Add(bc.name);
                        }
                        break;
                    case 1: // gas
                        foreach (GasCompartment gc in currentModel.modelDefinition.gas_compartments)
                        {
                            comp_list.Add(gc.name);
                        }
                        break;
                    case 2: // container
                        foreach (Container cont in currentModel.modelDefinition.containers)
                        {
                            comp_list.Add(cont.name);
                        }
                        break;
                    default:
                        break;
                }

                return JsonConvert.SerializeObject(comp_list);
            });
        }



        public Task<string> GetModelStateAsync()
        {
            return Task.Run(() =>
            {
                Dictionary<string, double[]> _data = new Dictionary<string, double[]>();

                // monitor signals
                _data.Add("heart_rate", new double[] { Math.Round(currentModel.modelDefinition.ecg["heart_rate"], 0) });

                _data.Add("ecg_signal", new double[] { currentModel.modelDefinition.ecg["ecg_signal"] });

                _data.Add("abp_systole", new double[] { Math.Round(currentModel.dataCollector.abp_systole, 1) });

                _data.Add("abp_diastole", new double[] { Math.Round(currentModel.dataCollector.abp_diastole, 1) });

                _data.Add("pap_systole", new double[] { Math.Round(currentModel.dataCollector.pap_systole, 1) });

                _data.Add("pap_diastole", new double[] { Math.Round(currentModel.dataCollector.pap_diastole, 1) });

                _data.Add("et_co2", new double[] { Math.Round(currentModel.dataCollector.et_co2, 1) });

                _data.Add("spo2_pre", new double[] { Math.Round(currentModel.dataCollector.spo2_pre, 2) });

                _data.Add("spo2_post", new double[] { Math.Round(currentModel.dataCollector.spo2_post, 2) });

                _data.Add("resp_rate", new double[] { Math.Round(currentModel.dataCollector.resp_rate, 1) });

                _data.Add("temp", new double[] { Math.Round(currentModel.modelDefinition.metabolism["body_temp"], 1) });

                _data.Add("art_ph", new double[] { Math.Round(currentModel.dataCollector.art_ph, 2) });

                _data.Add("art_po2", new double[] { Math.Round(currentModel.dataCollector.art_po2, 1) });

                _data.Add("art_pco2", new double[] { Math.Round(currentModel.dataCollector.art_pco2, 1) });

                _data.Add("art_hco3", new double[] { Math.Round(currentModel.dataCollector.art_hco3, 0) });

                _data.Add("art_be", new double[] { Math.Round(currentModel.dataCollector.art_be, 1) });

                _data.Add("art_lactate", new double[] { Math.Round(currentModel.dataCollector.art_lactate, 1) });

                // blood compartment signals
                foreach (BloodCompartment bc in currentModel.modelDefinition.blood_compartments)
                {
                    double[] newValue_bc = { Math.Round(bc.vol_current, 2), Math.Round(bc.pres_current, 2), Math.Round(bc.to2, 2) };
                    _data.Add(bc.name, newValue_bc);

                    if (bc.name == "LV" || bc.name == "RV" || bc.name == "LA" || bc.name == "RA")
                    {
                        _data.Add(bc.name + "_p", bc.pressures);
                        _data.Add(bc.name + "_v", bc.volumes);
                    }

                }
                foreach (GasCompartment gc in currentModel.modelDefinition.gas_compartments)
                {
                    double[] newValue_gc = { Math.Round(gc.vol_current, 2), Math.Round(gc.to2, 2), Math.Round(gc.pco2, 2) };
                    _data.Add(gc.name, newValue_gc);
                }

                foreach (BloodConnector bcc in currentModel.modelDefinition.blood_connectors)
                {
                    double[] newValue_bcc = { Math.Round(bcc.real_flow, 2) };
                    _data.Add(bcc.name, newValue_bcc);
                }
                foreach (Valve valve in currentModel.modelDefinition.valves)
                {
                    double[] newValue_valve = { Math.Round(valve.real_flow, 2) };
                    _data.Add(valve.name, newValue_valve);
                }
                foreach (Shunt shunt in currentModel.modelDefinition.shunts)
                {
                    double[] newValue_shunt = { Math.Round(shunt.real_flow, 2) };
                    _data.Add(shunt.name, newValue_shunt);
                }
                foreach (GasConnector gcc in currentModel.modelDefinition.gas_connectors)
                {
                    double[] newValue_gcc = { Math.Round(gcc.real_flow, 2) };
                    _data.Add(gcc.name, newValue_gcc);
                }

                return JsonConvert.SerializeObject(_data);
            });
        }

    }

}