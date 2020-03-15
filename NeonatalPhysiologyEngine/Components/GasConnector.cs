using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine
{
    public class GasConnector : IConnector
    {
        public string name { get; set; } = "";
        public string compartments { get; set; } = "";
        public int is_enabled { get; set; } = 0;
        public int no_backflow { get; set; } = 0;
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

        public GasCompartment comp1 { get; set; }
        public GasCompartment comp2 { get; set; }

        Model currentModel;

        public void InitGasConnector(Model cm)
        {

            // find the two gascompartment names which this connector connects
            string[] comps = compartments.Split("_");
            comp1_name = comps[0];
            comp2_name = comps[1];

            // store reference to the model
            currentModel = cm;

            // find the gas compartments which are connected by this connector
            foreach (GasCompartment comp in currentModel.modelDefinition.gas_compartments)
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

            currentModel.modelInterface.StatusMessage = $"Initialized gas compartment connector {name} by connecting {comp1.name} to {comp2.name}";

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
                if (is_enabled == 1)
                {
                    // find the flow direction
                    if (comp1.pres_current > comp2.pres_current)
                    {
                        // calculate the flow
                        current_flow = (comp1.pres_current - comp2.pres_current) / res_current;
                        // remove gas from comp1
                        comp1.GasOut(current_flow * currentModel.modelDefinition.modeling_stepsize);
                        // add gas to comp2
                        comp2.GasIn(current_flow * currentModel.modelDefinition.modeling_stepsize, comp1);
                        // store the flow
                        real_flow = current_flow;
                    }
                    else
                    {
                        // if no backflow is set then set the flow to zero
                        if (no_backflow == 1)
                        {
                            current_flow = 0;
                            real_flow = 0;
                        }
                        else
                        {
                            // calculate the flow
                            current_flow = (comp2.pres_current - comp1.pres_current) / res_current;
                            // add gas to comp1
                            comp1.GasIn(current_flow * currentModel.modelDefinition.modeling_stepsize, comp2);
                            // remove gas from comp2
                            comp2.GasOut(current_flow * currentModel.modelDefinition.modeling_stepsize);
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
