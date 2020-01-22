using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class ContractionModel
    {
        Model currentModel;

        double tas_left = 0;
        double tas_right = 0;
        double tvs_left = 0;
        double tvs_right = 0;

        double aaf_left = 0;
        double aaf_right = 0;
        double vaf_left = 0;
        double vaf_right = 0;

        BloodCompartment RA;
        BloodCompartment LA;
        BloodCompartment RV;
        BloodCompartment LV;

        public ContractionModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the contraction model.";

            // find a reference to the heart chamber compartments
            RA = currentModel.FindModelComponent<BloodCompartment>("RA");
            LA = currentModel.FindModelComponent<BloodCompartment>("LA");
            RV = currentModel.FindModelComponent<BloodCompartment>("RV");
            LV = currentModel.FindModelComponent<BloodCompartment>("LV");
        }

        public void ModelCycle()
        {
            // get the modeling stepsize
            double t = currentModel.modelDefinition.modeling_stepsize;

            // get the durations of the atrial and ventricular elastance activation functions in seconds
            tas_right = currentModel.modelDefinition.ecg["pq_time"];
            tas_left = currentModel.modelDefinition.ecg["pq_time"];
            tvs_right = currentModel.modelDefinition.ecg["cqt_time"];
            tvs_left = currentModel.modelDefinition.ecg["cqt_time"];

            // varying elastance activation function of the atria
            double ncc_atrial = currentModel.modelDefinition.ecg["ncc_atrial"];

            if (ncc_atrial >= 0 && ncc_atrial < (tas_left / t))
            {
                aaf_left = Math.Pow(Math.Sin(Math.PI * (ncc_atrial / tas_left) * t), 2);
            } else
            {
                aaf_left = 0;
            }

            if (ncc_atrial >= 0 && ncc_atrial < (tas_right / t))
            {
                aaf_right = Math.Pow(Math.Sin(Math.PI * (ncc_atrial / tas_right) * t), 2);
            }
            else
            {
                aaf_right = 0;
            }

            // varying elastance activation function of the ventricles
            double ncc_ventricular = currentModel.modelDefinition.ecg["ncc_ventricular"];

            if (ncc_ventricular >= 0 && ncc_ventricular < (tvs_left / t))
            {
                vaf_left = Math.Pow(Math.Sin(Math.PI * (ncc_ventricular / tvs_left) * t), 2);
            }
            else
            {
                vaf_left = 0;
            }

            if (ncc_ventricular >= 0 && ncc_ventricular < (tvs_right / t))
            {
                vaf_right = Math.Pow(Math.Sin(Math.PI * (ncc_ventricular / tvs_right) * t), 2);
            }
            else
            {
                vaf_right = 0;
            }

            // increase the atrial and ventricular activation function timers
            ncc_atrial += 1;
            ncc_ventricular += 1;
            
            // store the counters in the modeldefinition class
            currentModel.modelDefinition.ecg["ncc_atrial"] = ncc_atrial;
            currentModel.modelDefinition.ecg["ncc_ventricular"] = ncc_ventricular;

            // transfer the activation function to the heart compartments
            RA.el_contraction_activation = aaf_right;
            RV.el_contraction_activation = vaf_right;
            LA.el_contraction_activation = aaf_left;
            LV.el_contraction_activation = vaf_right;

        }
    }
}
