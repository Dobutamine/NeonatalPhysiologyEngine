using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine
{
    public class Container
    {
        public string name { get; set; }
        public int is_enabled { get; set; }
        public double vol_unstressed { get; set; }
        public double vol_unstressed_baseline { get; set; }
        public double vol_current { get; set; }
        public double vol_current_baseline { get; set; }
        public double pres_current { get; set; }
        public double container_pressure { get; set; }
        public double container_volume { get; set; }
        public double external_pressure { get; set; }
        public double el_baseline { get; set; }
        public double el_min_volume { get; set; }
        public double el_max_volume { get; set; }
        public double el_k1 { get; set; }
        public double el_k2 { get; set; }
        public string compartments { get; set; }
        Model currentModel;

        List<BloodCompartment> containedBloodCompartments = new List<BloodCompartment>();
        List<GasCompartment> containedGasCompartments = new List<GasCompartment>();
        


        public void InitContainer(Model cm)
        {
            external_pressure = 0;
            container_volume = 0;

            currentModel = cm;

            string[] containedCompartmentNames = compartments.Split("_");

            foreach (string comp_name in containedCompartmentNames)
            {
                foreach(BloodCompartment blood_comp in currentModel.modelDefinition.blood_compartments)
                {
                    if (comp_name == blood_comp.name)
                    {
                        containedBloodCompartments.Add(blood_comp);
                    }
                }

                foreach (GasCompartment gas_comp in currentModel.modelDefinition.gas_compartments)
                {
                    if (comp_name == gas_comp.name)
                    {
                        containedGasCompartments.Add(gas_comp);
                    }
                }
            }

            Console.WriteLine("Initialized container {0} with compartments {1}", name, compartments);
        }

        public void UpdateCompartment()
        {
            if (is_enabled == 1)
            {
                // add the enclosed compartments volume to the volume of the container
                container_volume = CalculateVolume();

                // calculate the recoil pressure of the compartment
                pres_current = CalculatePressure();

                // transfer the recoil pressure of the container to the enclosed compartments
                TransferContainerPressure();
            }
        }

        double CalculatePressure()
        {

            return (container_volume - vol_unstressed) * CalculateElastance() + external_pressure;
        }

        double CalculateElastance()
        {
            if (vol_current >= el_max_volume)
            {
                return el_baseline + el_k2 * Math.Pow((vol_current - el_max_volume), 3);
            }

            if (vol_current <= el_min_volume)
            {
                return el_baseline + el_k1 * Math.Pow((vol_current - el_min_volume), 3);
            }

            return el_baseline;
        }

        double CalculateVolume()
        {
            double total_volume = vol_current;

            foreach(BloodCompartment blood_comp in containedBloodCompartments)
            {
                total_volume += blood_comp.vol_current;
            }

            foreach (GasCompartment gas_comp in containedGasCompartments)
            {
                total_volume += gas_comp.vol_current;
            }

            return total_volume;

        }

        void TransferContainerPressure()
        {
            foreach (BloodCompartment blood_comp in containedBloodCompartments)
            {
                blood_comp.container_pressure = pres_current;
            }

            foreach (GasCompartment gas_comp in containedGasCompartments)
            {
                gas_comp.container_pressure = pres_current;
            }

        }


    }
}
