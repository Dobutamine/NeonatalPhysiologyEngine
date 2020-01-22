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
            AA = currentModel.FindModelComponent<BloodCompartment>("AA");
        }

        public void ModelCycle()
        {
            currentModel.acidbase.CalcAcidBaseFromTCO2(AA);
            currentModel.oxy.CalcOxygenationFromTO2(AA);

        }
    }
}
