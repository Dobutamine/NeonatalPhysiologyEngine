using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class AVInteractionModel
    {
        Model currentModel;

        public AVInteractionModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the atrio-ventricular interaction model.";
        }

        public void ModelCycle()
        {

        }
    }
}
