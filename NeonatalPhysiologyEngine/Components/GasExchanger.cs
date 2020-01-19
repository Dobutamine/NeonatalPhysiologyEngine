using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine
{
    public class GasExchanger
    {
        public string name { get; set; }
        public string compartments { get; set; }
        public bool is_enabled { get; set; }
        public double diff_o2 { get; set; }
        public double diff_co2 { get; set; }

        BloodCompartment comp_blood;
        GasCompartment comp_gas;

        string comp_blood_name = "";
        string comp_gas_name = "";

        Model currentModel;

        public GasExchanger()
        {
            string[] comps = compartments.Split("_");
            comp_blood_name = comps[0];
            comp_gas_name = comps[1];
        }

        public void InitializeExchanger(Model cm)
        {
            currentModel = cm;

            foreach(BloodCompartment blood_comp in currentModel.modelDefinition.blood_compartments)
            {
                if (blood_comp.name == comp_blood_name)
                {
                    comp_blood = blood_comp;
                }
            }

            foreach (GasCompartment gas_comp in currentModel.modelDefinition.gas_compartments)
            {
                if (gas_comp.name == comp_gas_name)
                {
                    comp_gas = gas_comp;
                }
            }

            Console.WriteLine("Initialized gas exchange unit {0} : {1} to {2}", name, comp_blood.name, comp_gas.name);

        }

        public void CalculateGasexchange()
        {
            // first calculate the partial pressures of oxygen and carbon dioxide

            // calculate the flux
            double flux_o2 = (comp_blood.po2 * 7.50061683 - comp_gas.po2) * diff_o2 * currentModel.modelDefinition.modeling_stepsize;
            double flux_co2 = (comp_blood.pco2 * 7.50061683 - comp_gas.pco2) * diff_co2 * currentModel.modelDefinition.modeling_stepsize;

            // calculate the gas exchange. watch the units of the concetrations. The gas compartment is in mol/l and the blood compartment in mmol/l
            if (comp_blood.is_enabled)
            {
                double vol_current_l = comp_blood.vol_current / 1000.0;

                if (vol_current_l > 0)
                {
                    comp_blood.vol_current = comp_blood.vol_current - flux_o2 * 22.4 - flux_co2 * 22.4;

                    vol_current_l = comp_blood.vol_current / 1000.0;

                    comp_blood.to2 = (comp_blood.to2 * vol_current_l - flux_o2) / vol_current_l;
                    if (comp_blood.to2 < 0)
                    {
                        comp_blood.to2 = 0.01;
                    }

                    comp_blood.tco2 = (comp_blood.tco2 * vol_current_l - flux_co2) / vol_current_l;
                    if (comp_blood.tco2 < 0)
                    {
                        comp_blood.tco2 = 0.0;
                    }
                }
            }

            if (comp_gas.is_enabled)
            {
                double vol_current_l = comp_gas.vol_current / 1000.0;

                if (vol_current_l > 0)
                {
                    comp_gas.ctotal = ((comp_gas.ctotal * vol_current_l) + flux_o2 / 1000.0 + flux_co2 / 1000.0) / vol_current_l;

                    comp_gas.co2 = ((comp_gas.co2 * vol_current_l) + flux_o2 / 1000.0) / vol_current_l;
                    if (comp_gas.co2 < 0)
                    {
                        comp_gas.co2 = 0;
                    }

                    comp_gas.cco2 = ((comp_gas.cco2 * vol_current_l) + flux_co2 / 1000.0) / vol_current_l;
                    if (comp_gas.cco2 < 0)
                    {
                        comp_gas.cco2 = 0;
                    }

                    comp_gas.fo2 = comp_gas.co2 / comp_gas.ctotal;
                    comp_gas.fco2 = comp_gas.cco2 / comp_gas.ctotal;

                    comp_gas.po2 = comp_gas.fo2 * (comp_gas.pres_current - (comp_gas.pres_current * comp_gas.fh2o));
                    comp_gas.pco2 = comp_gas.fco2 * (comp_gas.pres_current - (comp_gas.pres_current * comp_gas.fh2o));

                }


            }
        }

    }
}
