using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine
{
    public class Valve
    {
        public string name { get; set; } = "";
        public string compartments { get; set; } = "";
        public bool is_enabled { get; set; } = false;
        public bool no_backflow { get; set; } = false;
        public double res_current { get; set; }
        public double res_forward_baseline { get; set; } = 10000;
        public double res_backward_baseline { get; set; } = 10000;
        public double in_current { get; set; } = 0.0;
        public double in_baseline { get; set; } = 1.0;
        public double in_k1 { get; set; } = 0.0;
        public double in_k2 { get; set; } = 0.0;

        public double current_flow { get; set; } = 0.0;
        public double real_flow { get; set; } = 0.0;

        string comp1_name = "";
        string comp2_name = "";

        public BloodCompartment comp1;
        public BloodCompartment comp2;

        Model currentModel;

        public Valve()
        {
            // find the two bloodcompartment names which this connector connects
            string[] comps = compartments.Split("_");
            comp1_name = comps[0];
            comp2_name = comps[1];
        }

        public void InitValve(Model cm)
        {
            // store reference to the model
            currentModel = cm;

            // find the blood compartments which are connected by this connector
            foreach (BloodCompartment comp in currentModel.modelDefinition.blood_compartments)
            {
                if (comp.name == comp1_name)
                {
                    comp1 = comp;
                }
                if (comp.name == comp2_name)
                {
                    comp2 = comp;
                }
            }

            Console.WriteLine("Initialized valve {0} by connecting {1} to {2}", name, comp1_name, comp2_name);

        }

        public void CalculateFlow()
        {
            if (comp1 != null && comp2 != null)
            {
                // calculate the current resistance
                res_current = CalculateResistance();

                // calculate the current inductance
                in_current = CalculateInductance();

                // check whether the connector is enabled
                if (is_enabled)
                {
                    // find the flow direction
                    if (comp1.pres_current > comp2.pres_current)
                    {
                        // calculate the flow
                        current_flow = (comp1.pres_current - comp2.pres_current) / res_current;
                        // remove blood from comp1
                        comp1.BloodOut(current_flow * currentModel.modelDefinition.modeling_stepsize);
                        // add blood to comp2
                        comp2.BloodIn(current_flow * currentModel.modelDefinition.modeling_stepsize, comp1);
                        // store the flow
                        real_flow = current_flow;
                    }
                    else
                    {
                        // if no backflow is set then set the flow to zero
                        if (no_backflow)
                        {
                            current_flow = 0;
                            real_flow = 0;
                        }
                        else
                        {
                            // calculate the flow
                            current_flow = (comp2.pres_current - comp1.pres_current) / res_current;
                            // add blood to comp1
                            comp1.BloodIn(current_flow * currentModel.modelDefinition.modeling_stepsize, comp2);
                            // remove blood from comp2
                            comp2.BloodOut(current_flow * currentModel.modelDefinition.modeling_stepsize);
                            // store the real flow
                            real_flow = -current_flow;
                        }
                    }
                }
            }
        }

        double CalculateResistance()
        {
            if (comp1.pres_current > comp2.pres_current)
            {
                return res_forward_baseline;
            }
            else
            {
                return res_backward_baseline;
            }
        }

        double CalculateInductance()
        {
            return 0;
        }

        void CalculateDiffusion()
        {

        }

    }
}
