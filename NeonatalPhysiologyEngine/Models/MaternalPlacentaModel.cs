using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class MaternalPlacentaModel
    {
        Model currentModel;

        public MaternalPlacentaModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the maternal placenta model.";
        }

        public void ModelCycle()
        {

        }
    }
}
