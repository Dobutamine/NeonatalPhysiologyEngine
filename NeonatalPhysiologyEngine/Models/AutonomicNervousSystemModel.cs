using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class AutonomicNervousSystemModel
    {
        Model currentModel;

        BloodCompartment AA;

        public AutonomicNervousSystemModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the autonomic nervous system model.";

            // find a reference to the heart chamber compartments
            foreach (BloodCompartment bloodComp in currentModel.modelDefinition.blood_compartments)
            {
                if (bloodComp.name == "AA")
                {
                    AA = bloodComp;
                }
            }
        }

        public void ModelCycle()
        {
            currentModel.acidbase.CalcAcidBaseFromTCO2(AA);

        }
    }
}
