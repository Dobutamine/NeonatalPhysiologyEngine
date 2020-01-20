using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine
{
    public class Diffusor
    {
        public string name { get; set; }
        public string compartments { get; set; }
        public int is_enabled { get; set; }
        public double diff_o2 { get; set; }
        public double diff_co2 { get; set; }

        BloodCompartment comp1_blood;
        BloodCompartment comp2_blood;
        GasCompartment comp1_gas;
        GasCompartment comp2_gas;

        string comp1_name = "";
        string comp2_name = "";

        Model currentModel;


        public void CalculateDiffusion()
        {

        }

        public void InitializeDiffusor(Model cm)
        {
            string[] comps = compartments.Split("_");
            comp1_name = comps[0];
            comp2_name = comps[1];

            currentModel = cm;

            foreach (BloodCompartment blood_comp in currentModel.modelDefinition.blood_compartments)
            {
                if (blood_comp.name == comp1_name)
                {
                    comp1_blood = blood_comp;
                }
                if (blood_comp.name == comp2_name)
                {
                    comp2_blood = blood_comp;
                }

            }

            foreach (GasCompartment  gas_comp in currentModel.modelDefinition.gas_compartments)
            {
                if (gas_comp.name == comp1_name)
                {
                    comp1_gas = gas_comp;
                }
                if (gas_comp.name == comp2_name)
                {
                    comp2_gas = gas_comp;
                }

            }

            currentModel.modelInterface.StatusMessage = $"Initialized diffusor {name} : compartment {comp1_name} to compartment {comp2_name}";


        }
    }
}
