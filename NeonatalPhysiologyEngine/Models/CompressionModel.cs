using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class CompressionModel
    {
        Model currentModel;

        public CompressionModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the compressions model.";
        }

        public void ModelCycle()
        {

        }
    }
}
