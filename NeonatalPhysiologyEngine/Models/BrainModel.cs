using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class BrainModel
    {
        Model currentModel;

        public BrainModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the brain model.";
        }

        public void ModelCycle()
        {

        }
    }
}
