using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class SpontaneousBreathingModel
    {
        Model currentModel;

        public SpontaneousBreathingModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the spontanenous breathing model.";
        }

        public void ModelCycle()
        {

        }
    }
}
