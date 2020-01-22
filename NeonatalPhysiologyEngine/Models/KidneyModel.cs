using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class KidneyModel
    {
        Model currentModel;

        public KidneyModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the kidney model.";
        }

        public void ModelCycle()
        {

        }
    }
}
