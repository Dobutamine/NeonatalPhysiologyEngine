using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class MechanicalVentilatorModel
    {
        Model currentModel;

        GasCompartment VENTIN;
        GasCompartment TUBINGIN;
        GasCompartment YPIECE;
        GasCompartment TUBINGOUT;
        GasCompartment VENTOUT;

        GasConnector VENTIN_TUBINGIN;
        GasConnector TUBINGIN_YPIECE;
        GasConnector YPIECE_NCA;
        GasConnector YPIECE_TUBINGOUT;
        GasConnector TUBINGOUT_VENTOUT;

        bool inspiration = true;
        bool expiration = false;

        double current_pip = 0;
        double insp_counter = 0;
        double exp_counter = 0;

        double running_tidal_volume = 0;
        double tidal_volume = 0;


        public MechanicalVentilatorModel(Model cm)
        {
            currentModel = cm;

            // find the relevant compartments and connectors
            VENTIN = currentModel.FindModelComponent<GasCompartment>("VENTIN");
            TUBINGIN = currentModel.FindModelComponent<GasCompartment>("TUBINGIN");
            YPIECE = currentModel.FindModelComponent<GasCompartment>("YPIECE");
            TUBINGOUT = currentModel.FindModelComponent<GasCompartment>("TUBINGOUT");
            VENTOUT = currentModel.FindModelComponent<GasCompartment>("VENTOUT");

            VENTIN_TUBINGIN = currentModel.FindModelComponent<GasConnector>("VENTIN_TUBINGIN");
            TUBINGIN_YPIECE = currentModel.FindModelComponent<GasConnector>("TUBINGIN_YPIECE");
            YPIECE_NCA = currentModel.FindModelComponent<GasConnector>("YPIECE_NCA");
            YPIECE_TUBINGOUT = currentModel.FindModelComponent<GasConnector>("YPIECE_TUBINGOUT");
            TUBINGOUT_VENTOUT = currentModel.FindModelComponent<GasConnector>("TUBINGOUT_VENTOUT");


            currentModel.modelInterface.StatusMessage = $"Initialized the mechanical ventilator model.";

        }

        void VolumeControl()
        {

        }
        public void ModelCycle()
        {
            double t = currentModel.modelDefinition.modeling_stepsize;

            double p_atm = currentModel.modelDefinition.metabolism["p_atm"];

            if (inspiration)
            {
                // open the inspiratory valve
                VENTIN_TUBINGIN.res_forward_baseline = 2.5;
                VENTIN_TUBINGIN.res_backward_baseline = 2.5;

                // close the expiratory valve
                TUBINGOUT_VENTOUT.res_forward_baseline = 1000.0;
                TUBINGOUT_VENTOUT.res_backward_baseline = 1000.0;

                // if the pressure exceeds the maximal peak inspiratory pressure then close the inspiratory valve
                if (TUBINGIN.pres_current > current_pip + p_atm)
                {
                    // close the inspiratory valve
                    VENTIN_TUBINGIN.res_forward_baseline = 1000.0;
                    VENTIN_TUBINGIN.res_backward_baseline = 1000.0;
                }

                // increase the inspiration timer
                insp_counter += t;
            }

            if (expiration)
            {
                // open the expiratory valve
                TUBINGOUT_VENTOUT.res_forward_baseline = 2.5;
                TUBINGOUT_VENTOUT.res_backward_baseline = 2.5;

                if (TUBINGOUT.pres_current < currentModel.modelDefinition.ventilator["peep"] + p_atm)
                {
                    // close the expiratory valve
                    TUBINGOUT_VENTOUT.res_forward_baseline = 1000.0;
                    TUBINGOUT_VENTOUT.res_backward_baseline = 1000.0;
                }

                // increase the exhaled tidal volume
                running_tidal_volume += YPIECE_TUBINGOUT.real_flow * t;

                // increase the expiration timer
                exp_counter += t;
            }

            if (insp_counter > currentModel.modelDefinition.ventilator["t_in"])
            {
                insp_counter = 0;
                inspiration = false;
                expiration = true;
                tidal_volume = running_tidal_volume;
                running_tidal_volume = 0;
                if (currentModel.modelDefinition.ventilator["volume_controlled"] == 1 && tidal_volume > 0)
                {
                    VolumeControl();
                }
            }

            if (exp_counter > currentModel.modelDefinition.ventilator["t_ex"])
            {
                exp_counter = 0;
                expiration = false;
                inspiration = true;
            }


        }

    }
}
