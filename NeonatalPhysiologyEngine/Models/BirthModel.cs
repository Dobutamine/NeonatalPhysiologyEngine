using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class BirthModel
    {
        Model currentModel;

        public BirthModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the birth model.";
        }

        public void ModelCycle()
        {

        }
    }
}
