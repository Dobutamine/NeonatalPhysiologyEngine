using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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

        public Task<string> GetModelStatusMessageAsync()
        {
            return Task.Run(() =>
            {
                return JsonConvert.SerializeObject(currentModel.modelInterface.StatusMessage);
            });
        }
        public ModelInterface(Model cm)
        {
            currentModel = cm;
        }

        public Task<string> GetCompartmentDataAsync(string comp_name)
        {
            return Task.Run(() =>
            {
                foreach (BloodCompartment bc in currentModel.modelDefinition.blood_compartments)
                {
                    if (bc.name == comp_name)
                    {
                        return JsonConvert.SerializeObject(bc);
                    }
                }

                foreach (GasCompartment gc in currentModel.modelDefinition.gas_compartments)
                {
                    if (gc.name == comp_name)
                    {
                        return JsonConvert.SerializeObject(gc);
                    }
                }

                foreach (Container cont in currentModel.modelDefinition.containers)
                {
                    if (cont.name == comp_name)
                    {
                        return JsonConvert.SerializeObject(cont);
                    }
                }

                return "";


            });
        }

        public Task<string> GetConnectorDataAsync(string con_name)
        {
            return Task.Run(() =>
            {

                foreach (BloodConnector bc in currentModel.modelDefinition.blood_connectors)
                {
                    if (bc.name == con_name)
                    {
                        return JsonConvert.SerializeObject(bc);

                    }
                }

                foreach (GasConnector gc in currentModel.modelDefinition.gas_connectors)
                {
                    if (gc.name == con_name)
                    {
                        return JsonConvert.SerializeObject(gc);

                    }
                }

                foreach (Valve valve in currentModel.modelDefinition.valves)
                {
                    if (valve.name == con_name)
                    {
                        return JsonConvert.SerializeObject(valve);

                    }
                }

                foreach (Shunt shunt in currentModel.modelDefinition.shunts)
                {
                    if (shunt.name == con_name)
                    {
                        return JsonConvert.SerializeObject(shunt);

                    }
                }

                return "";


            });
        }

        public void SetCompartmentProperties(string comp_props)
        {
            var new_props = JsonConvert.DeserializeObject<CompProps>(comp_props);

            foreach (BloodCompartment bc in currentModel.modelDefinition.blood_compartments)
            {
                if (bc.name == new_props.name)
                {
                    bc.vol_unstressed_baseline = new_props.u_vol_base;
                    bc.is_enabled = new_props.is_enabled;
                    bc.vol_current_baseline = new_props.vol_base;
                    bc.el_baseline = new_props.el_base;
                    bc.el_contraction_baseline = new_props.el_cont;
                    bc.el_min_volume = new_props.el_min;
                    bc.el_max_volume = new_props.el_max;
                    bc.el_k1 = new_props.el_k1;
                    bc.el_k2 = new_props.el_k2;
                    bc.fvatp = new_props.fvatp;
                }
            }

            foreach (GasCompartment bc in currentModel.modelDefinition.gas_compartments)
            {
                if (bc.name == new_props.name)
                {
                    bc.vol_unstressed_baseline = new_props.u_vol_base;
                    bc.is_enabled = new_props.is_enabled;
                    bc.vol_current_baseline = new_props.vol_base;
                    bc.el_baseline = new_props.el_base;
                    bc.el_contraction_baseline = new_props.el_cont;
                    bc.el_min_volume = new_props.el_min;
                    bc.el_max_volume = new_props.el_max;
                    bc.el_k1 = new_props.el_k1;
                    bc.el_k2 = new_props.el_k2;
                }
            }

            foreach (Container bc in currentModel.modelDefinition.containers)
            {
                if (bc.name == new_props.name)
                {
                    bc.vol_unstressed_baseline = new_props.u_vol_base;
                    bc.is_enabled = new_props.is_enabled;
                    bc.vol_current_baseline = new_props.vol_base;
                    bc.el_baseline = new_props.el_base;
                    bc.el_min_volume = new_props.el_min;
                    bc.el_max_volume = new_props.el_max;
                    bc.el_k1 = new_props.el_k1;
                    bc.el_k2 = new_props.el_k2;
                }
            }



        }

        public void SetConnectorProperties(string con_props)
        {
            var new_props = JsonConvert.DeserializeObject<ConProps>(con_props);


            foreach (BloodConnector bc in currentModel.modelDefinition.blood_connectors)
            {
                if (bc.name == new_props.name)
                {
                    bc.is_enabled = new_props.is_enabled;
                    bc.no_backflow = new_props.no_backflow;
                    bc.res_forward_baseline = new_props.res_forward_baseline;
                    bc.res_backward_baseline = new_props.res_backward_baseline;
                    bc.in_baseline = new_props.in_baseline;
                    bc.in_k1 = new_props.in_k1;
                    bc.in_k2 = new_props.in_k2;
                }
            }

            foreach (GasConnector gc in currentModel.modelDefinition.gas_connectors)
            {
                if (gc.name == new_props.name)
                {
                    gc.is_enabled = new_props.is_enabled;
                    gc.no_backflow = new_props.no_backflow;
                    gc.res_forward_baseline = new_props.res_forward_baseline;
                    gc.res_backward_baseline = new_props.res_backward_baseline;
                    gc.in_baseline = new_props.in_baseline;
                    gc.in_k1 = new_props.in_k1;
                    gc.in_k2 = new_props.in_k2;
                }
            }

            foreach (Valve gc in currentModel.modelDefinition.valves)
            {
                if (gc.name == new_props.name)
                {
                    gc.is_enabled = new_props.is_enabled;
                    gc.no_backflow = new_props.no_backflow;
                    gc.res_forward_baseline = new_props.res_forward_baseline;
                    gc.res_backward_baseline = new_props.res_backward_baseline;
                    gc.in_baseline = new_props.in_baseline;
                    gc.in_k1 = new_props.in_k1;
                    gc.in_k2 = new_props.in_k2;
                }
            }

            foreach (Shunt gc in currentModel.modelDefinition.shunts)
            {
                if (gc.name == new_props.name)
                {
                    gc.is_enabled = new_props.is_enabled;
                    gc.no_backflow = new_props.no_backflow;
                    gc.res_forward_baseline = new_props.res_forward_baseline;
                    gc.res_backward_baseline = new_props.res_backward_baseline;
                    gc.in_baseline = new_props.in_baseline;
                    gc.in_k1 = new_props.in_k1;
                    gc.in_k2 = new_props.in_k2;
                }
            }
        }


        public Task<string> GetCompartmentNameListAsync(int comp_type)
        {
            return Task.Run(() =>
            {
                List<string> comp_list = new List<string>();

                foreach (BloodCompartment bc in currentModel.modelDefinition.blood_compartments)
                {
                    comp_list.Add(bc.name);
                }

                foreach (GasCompartment gc in currentModel.modelDefinition.gas_compartments)
                {
                    comp_list.Add(gc.name);
                }

                foreach (Container cont in currentModel.modelDefinition.containers)
                {
                    comp_list.Add(cont.name);
                }

                return JsonConvert.SerializeObject(comp_list);
            });
        }

        public Task<string> GetConnectorNameListAsync(int con_type)
        {
            return Task.Run(() =>
            {
                List<string> con_list = new List<string>();


                foreach (BloodConnector bc in currentModel.modelDefinition.blood_connectors)
                {
                    con_list.Add(bc.name);
                }

                foreach (GasConnector gc in currentModel.modelDefinition.gas_connectors)
                {
                    con_list.Add(gc.name);
                }

                foreach (Valve gc in currentModel.modelDefinition.valves)
                {
                    con_list.Add(gc.name);
                }

                foreach (Shunt gc in currentModel.modelDefinition.shunts)
                {
                    con_list.Add(gc.name);
                }


                return JsonConvert.SerializeObject(con_list);
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

                    if (gc.name == "ALL" || gc.name == "ALR" || gc.name == "NCA")
                    {
                        _data.Add(gc.name + "_p", gc.pressures);
                        _data.Add(gc.name + "_v", gc.volumes);
                    }
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

    class ConProps
    {
        public string name = "";
        public int is_enabled = 1;
        public string comp1 = "";
        public string comp2 = "";
        public int no_backflow = 0;
        public double res_forward_baseline = 0.1;
        public double res_backward_baseline = 0.1;
        public double in_baseline = 0;
        public double in_k1 = 0;
        public double in_k2 = 0;



    }
    class CompProps
    {
        public string name = "";
        public int is_enabled = 1;
        public int comp_type = 0;
        public double u_vol_base = 47;
        public double vol_base = 140;
        public double el_base = 1;
        public double el_cont = 1;
        public double el_min = -1000;
        public double el_max = 1000;
        public double el_k1 = 1;
        public double el_k2 = 1;
        public double fvatp = 0;


    }

}