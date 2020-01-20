using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class AcidBaseModel
    {
        Model currentModel;

        public AcidBaseModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the acid-base model.";
        }
    }
}
