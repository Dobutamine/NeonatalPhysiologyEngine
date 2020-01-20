using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class OxygenationModel
    {
        Model currentModel;

        public OxygenationModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the oxygenation model.";
        }
    }
}
