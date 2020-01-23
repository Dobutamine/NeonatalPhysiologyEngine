using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class SpontaneousBreathingModel
    {
        Model currentModel;

        double breath_timer_period = 6000;
        double breath_timer_counter = 0;
        
        GasCompartment ALL;
        GasCompartment ALR;
        Container CHEST_L;
        Container CHEST_R;

        double p_mus = 0;
        double amp = 5;
        double max_amp = 50;
        double breath_duration = 1000;

        List<double> volumes = new List<double>();
        double tidal_volume = 0;
        double minute_volume = 0;
        double volume_counter = 0;


        public SpontaneousBreathingModel(Model cm)
        {
            // reference the current model instance
            currentModel = cm;

            // get a reference to the model needed components
            ALL = currentModel.FindModelComponent<GasCompartment>("ALL");
            ALR = currentModel.FindModelComponent<GasCompartment>("ALR");
            CHEST_L = currentModel.FindModelComponent<Container>("CHEST_L");
            CHEST_R = currentModel.FindModelComponent<Container>("CHEST_R");

            // update the status
            currentModel.modelInterface.StatusMessage = $"Initialized the spontanenous breathing model.";
        }

        public void ModelCycle()
        {
            // check whether the spontaneous breathing is enabled
            if (currentModel.modelDefinition.breathing["spont_breathing_enabled"] == 1)
            {
                // get the current spontaneous resp rate 
                if (currentModel.modelDefinition.breathing["spont_resp_rate"] > 0)
                {
                    breath_timer_period = 60000 / currentModel.modelDefinition.breathing["spont_resp_rate"];
                } else
                {
                    breath_timer_period = 60000;
                }

                // calculate the resp rate and target tidal volume depending on the input of the ANS
                VTRRRatioController();

                // is it time for a new breath yet?
                if (breath_timer_counter > breath_timer_period)
                {
                    StartSpontaneousBreath();
                }

                // generate the respiratory muscle signal
                if (currentModel.modelDefinition.breathing["spont_resp_rate"] > 0)
                {
                    p_mus = GenerateMuscleSignal();
                } else
                {
                    p_mus = 0;
                }

                // transfer the respiratory muscle signal to the chest muscles
                CHEST_L.external_pressure = p_mus;
                CHEST_R.external_pressure = p_mus;

                // transfer the muscle pressure to the current model (just storage)
                currentModel.modelDefinition.breathing["resp_muscle_pressure"] = p_mus;

                // increase the breath timer counter
                breath_timer_counter += currentModel.modelDefinition.modeling_stepsize * 1000.0;

                // check the current volumes
                volume_counter += currentModel.modelDefinition.modeling_stepsize;
                if (volume_counter > 5.0)
                {
                    CalculateVolumes();
                }
                volumes.Add(ALL.vol_current + ALR.vol_current);

            }
        }
        
        void VTRRRatioController()
        {
            // calculate the spontaneous resp rate depending on the target minute volume (from ANS) and the set vt-rr ratio
            currentModel.modelDefinition.breathing["spont_resp_rate"] = Math.Sqrt(currentModel.modelDefinition.breathing["target_minute_volume"] / currentModel.modelDefinition.breathing["vtrr_ratio"]);

            // calculate the target tidal volume depending on the target resp rate and target minute volume (from ANS)
            currentModel.modelDefinition.breathing["target_tidal_volume"] = currentModel.modelDefinition.breathing["target_minute_volume"] / currentModel.modelDefinition.breathing["spont_resp_rate"];
        }

        void StartSpontaneousBreath()
        {
          
            // calculate the current tidal and minute volume
            CalculateVolumes();

            // has the target tidal volume been reached or exceeded?
            double d_tv = tidal_volume - currentModel.modelDefinition.breathing["target_tidal_volume"];

            // adjust the respiratory power to the resp muscles
            if (d_tv < -0.2)
            {
                amp -= 0.1 * d_tv;
                if (amp > max_amp)
                {
                    amp = max_amp;
                }
            }
            if (d_tv > 0.2)
            {
                amp -= 0.1 * d_tv;
                if (amp > max_amp)
                {
                    amp = max_amp;
                }
            }

            // reset the breathing timer
            breath_timer_counter = 1;
        }

        double GenerateMuscleSignal()
        {
            //  generate the respiratory muscle signal
            return -amp * Math.Exp((-25 * (Math.Pow((breath_timer_counter - breath_duration / 2), 2) / Math.Pow(breath_duration, 2))));
        }

        void CalculateVolumes()
        {
            // # calculate the current tidal and minute volumes
            tidal_volume = volumes.Max() - volumes.Min();
            minute_volume = tidal_volume * currentModel.modelDefinition.breathing["spont_resp_rate"];

            // empty the volumes array
            volumes = new List<double>();

            // reset the volumes counter
            volume_counter = 0;

        }
    }
}
