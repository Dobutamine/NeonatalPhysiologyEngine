using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class GlobalHypoxiaModel
    {
        Model currentModel;

        public GlobalHypoxiaModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the global hypoxia model.";
        }
    }
}
