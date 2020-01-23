using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class AutonomicNervousSystemModel
    {
        Model currentModel;

        BloodCompartment AA;
        BloodCompartment BRAIN;
        BloodCompartment AD;
        BloodCompartment PA;
        BloodCompartment PV;

        double d_map_hp = 0;
        double d_map_cont = 0;
        double d_map_venpool = 0;
        double d_map_res = 0;

        double d_lungvol_hp = 0;
        double d_po2_hp = 0;
        double d_pco2_hp = 0;

        double d_po2_ve = 0;
        double d_pco2_ve = 0;
        double d_ph_ve = 0;

        double a_map = 0;
        double a_lungvol = 0;
        double a_po2 = 0;
        double a_pco2 = 0;
        double a_ph = 0;

        public AutonomicNervousSystemModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the autonomic nervous system model.";

            // get a reference to the relevant model components
            AA = currentModel.FindModelComponent<BloodCompartment>("AA");
            BRAIN = currentModel.FindModelComponent<BloodCompartment>("BRAIN");
            AD = currentModel.FindModelComponent<BloodCompartment>("AD");
            PA = currentModel.FindModelComponent<BloodCompartment>("PA");
            PV = currentModel.FindModelComponent<BloodCompartment>("PV");
        }

        public void ModelCycle()
        {
            // first we need the po2 and pco2 of the ascending aorta
            currentModel.acidbase.CalcAcidBaseFromTCO2(AA);
            currentModel.oxy.CalcOxygenationFromTO2(AA);

            // get the modeling interval
            double t = currentModel.modelDefinition.modeling_interval;

            // calculate the activation values
            a_map = ActivationCurve(AA.pres_current, currentModel.modelDefinition.ans["sa_map"], currentModel.modelDefinition.ans["th_map"], currentModel.modelDefinition.ans["op_map"]);
            a_lungvol = ActivationCurve(AA.pres_current, currentModel.modelDefinition.ans["sa_lungvol"], currentModel.modelDefinition.ans["th_lungvol"], currentModel.modelDefinition.ans["op_lungvol"]);
            a_po2 = ActivationCurve(AA.po2, currentModel.modelDefinition.ans["sa_po2"], currentModel.modelDefinition.ans["th_po2"], currentModel.modelDefinition.ans["op_po2"]);
            a_pco2 = ActivationCurve(AA.pco2, currentModel.modelDefinition.ans["sa_pco2"], currentModel.modelDefinition.ans["th_pco2"], currentModel.modelDefinition.ans["op_pco2"]);
            a_ph = ActivationCurve(AA.ph, currentModel.modelDefinition.ans["sa_ph"], currentModel.modelDefinition.ans["th_ph"], currentModel.modelDefinition.ans["op_ph"]);

            d_map_hp = t * ((1 / currentModel.modelDefinition.ans["tc_map_hp"]) * (-d_map_hp + a_map)) + d_map_hp;
            d_lungvol_hp = t * ((1 / currentModel.modelDefinition.ans["tc_lungvol_hp"]) * (-d_lungvol_hp + a_lungvol)) + d_lungvol_hp;
            d_po2_hp = t * ((1 / currentModel.modelDefinition.ans["tc_po2_hp"]) * (-d_po2_hp + a_po2)) + d_po2_hp;
            d_pco2_hp = t * ((1 / currentModel.modelDefinition.ans["tc_pco2_hp"]) * (-d_pco2_hp + a_pco2)) + d_pco2_hp;

            d_po2_ve = t * ((1 / currentModel.modelDefinition.ans["tc_po2_ve"]) * (-d_po2_ve + a_po2)) + d_po2_ve;
            d_pco2_ve = t * ((1 / currentModel.modelDefinition.ans["tc_pco2_ve"]) * (-d_pco2_ve + a_pco2)) + d_pco2_ve;
            d_ph_ve = t * ((1 / currentModel.modelDefinition.ans["tc_ph_ve"]) * (-d_ph_ve + a_ph)) + d_ph_ve;

            d_map_cont = t * ((1 / currentModel.modelDefinition.ans["tc_map_cont"]) * (-d_map_cont + a_map)) + d_map_cont;
            d_map_venpool = t * ((1 / currentModel.modelDefinition.ans["tc_map_venpool"]) * (-d_map_venpool + a_map)) + d_map_venpool;
            d_map_res = t * ((1 / currentModel.modelDefinition.ans["tc_map_res"]) * (-d_map_res + a_map)) + d_map_res;

            // # calculate the heartrate 
            currentModel.modelDefinition.ecg["heart_rate"] = 60000.0 / ((60000.0 / currentModel.modelDefinition.ecg["heart_rate_ref"]) + currentModel.modelDefinition.ans["g_map_hp"] * d_map_hp + currentModel.modelDefinition.ans["g_pco2_hp"] * d_pco2_hp + currentModel.modelDefinition.ans["g_po2_hp"] * d_po2_hp + currentModel.modelDefinition.ans["g_lungvol_hp"] * d_lungvol_hp);

            // calculate the target exhaled minute volume 
            currentModel.modelDefinition.breathing["target_minute_volume"] = currentModel.modelDefinition.breathing["ref_minute_volume"] + currentModel.modelDefinition.ans["g_ph_ve"] * d_ph_ve + currentModel.modelDefinition.ans["g_pco2_ve"] * d_pco2_ve + currentModel.modelDefinition.ans["g_po2_ve"] * d_po2_ve;

        }

        double ActivationCurve(double value, double saturation, double operating_point, double threshold)
        {
            double activation = 0;

            if (value >= saturation)       
            {
                activation = saturation - operating_point;
            } else
            {
                if (value <= threshold)     
                {
                    activation = threshold - operating_point;
                } else                     
                {
                    activation = value - operating_point;
                }
            }
            return activation;
        }
    }
}
