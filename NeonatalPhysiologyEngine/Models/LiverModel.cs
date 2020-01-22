using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class LiverModel
    {
        Model currentModel;

        public LiverModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the liver model.";
        }

        public void ModelCycle()
        {

        }
    }
}
