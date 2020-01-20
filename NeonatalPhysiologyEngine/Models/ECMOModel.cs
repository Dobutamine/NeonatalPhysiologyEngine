using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class ECMOModel
    {
        Model currentModel;

        public ECMOModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the ECMO model.";
        }
    }
}
