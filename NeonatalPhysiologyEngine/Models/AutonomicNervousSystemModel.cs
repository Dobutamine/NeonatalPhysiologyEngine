using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class AutonomicNervousSystemModel
    {
        Model currentModel;

        public AutonomicNervousSystemModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the autonomic nervous system model.";
        }

        public void ModelCycle()
        {

        }
    }
}
