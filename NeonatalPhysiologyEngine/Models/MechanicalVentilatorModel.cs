using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class MechanicalVentilatorModel
    {
        Model currentModel;

        public MechanicalVentilatorModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the mechanical ventilator model.";
        }

        public void ModelCycle()
        {

        }

    }
}
