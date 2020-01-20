using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine
{
    public class BloodCompartment
    {
        public string name { get; set; }
        public int is_enabled { get; set; }
        public double vol_unstressed { get; set; }
        public double vol_unstressed_baseline { get; set; }
        public double vol_current { get; set; }
        public double vol_current_baseline { get; set; }
        public double pres_current { get; set; }
        public double container_pressure { get; set; }
        public double external_pressure { get; set; }
        public double el_baseline { get; set; }
        public double el_contraction { get; set; }
        public double el_contraction_baseline { get; set; }
        public double el_contraction_activation { get; set; }
        public double el_min_volume { get; set; }
        public double el_max_volume { get; set; }
        public double el_k1 { get; set; }
        public double el_k2 { get; set; }

        public double fvatp { get; set; }
        public double atp_factor { get; set; }
        public double to2 { get; set; }
        public double tco2 { get; set; }
        

        public double ph { get; set; }
        public double po2 { get; set; }
        public double pco2 { get; set; }
        public double hco3 { get; set; }
        public double be { get; set; }
        public double so2 { get; set; }
        public double co2 { get; set; }
        public double cco2 { get; set; }
        public double sid { get; set; }
        public double albumin { get; set; }
        public double phosphates { get; set; }
        public double hemoglobin { get; set; }
        public double dpg { get; set; }
        public double temp { get; set; }

        Model currentModel;

        public void InitBloodCompartment(Model cm)
        {
            // initialize starting values

            pres_current = 0;
            container_pressure = 0;
            external_pressure = 0;
            ph = 7.40;
            po2 = 10.0;
            pco2 = 46.0;
            hco3 = 24.3;
            be = 0.0;
            so2 = 90.0;
            albumin = 30.0;
            phosphates = 1.5;
            sid = 31.6;
            hemoglobin = 10.0;
            dpg = 5.0;
            temp = 37.0;

            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized blood compartment {name}.";
        }

        public void UpdateCompartment()
        {
            if (is_enabled == 1)
            {
                EnergyBalance();
                pres_current = CalculatePressure();
            }
        }

        public void BloodIn(double dvol, BloodCompartment compFrom)
        {
            vol_current += dvol;
            if (vol_current < 0)
            {
                vol_current = 0.01;
            }

            // convert from ml to litres
            double vol_current_l = vol_current / 1000.0;
            double dvol_l = dvol / 1000.0;

            // calculate the change in oxygen concentration
            double o2_infow = (compFrom.to2 - to2) * dvol_l;
            to2 = (to2 * vol_current_l + o2_infow) / vol_current_l;
            if (to2 < 0)
            {
                to2 = 0;
            }

            // calculate the change in carbon dioxide concentration
            double co2_infow = (compFrom.tco2 - tco2) * dvol_l;
            tco2 = (tco2 * vol_current_l + co2_infow) / vol_current_l;
            if (tco2 < 0)
            {
                tco2 = 0;
            }

        }

        public void BloodOut(double dvol)
        {
            vol_current -= dvol;
            if (vol_current < 0)
            {
                vol_current = 0.01;
            }
        }

        void EnergyBalance()
        {
            // get the local ATP need in molecules per second
            double atp_need = atp_factor * currentModel.modelDefinition.metabolism["atp_need"];

            // new we need to know how much molecules ATP we need in this step
            double atp_need_step = atp_need * currentModel.modelDefinition.modeling_stepsize;

            // get the number of oxygen molecules available in this compartment
            double o2_molecules_available = to2 * vol_current / 1000.0;

            // we state that 80% of these molecules are available for use
            double o2_molecules_available_for_use = 0.8 * o2_molecules_available;

            // how many molecules o2 do we need to burn in this step as 1 molecule of o2 gives 5 molecules of ATP
            double o2_to_burn = atp_need_step / 5.0;

            // how many needed ATP molecules can't be produced by aerobic respiration
            double anaerobic_atp = (o2_to_burn - (o2_molecules_available_for_use / 4.0)) * 5.0;

            // if negative then there are more o2 molecules available than needed and shut down anaerobic fermentation
            if (anaerobic_atp < 0)
            {
                anaerobic_atp = 0;
            }

            double o2_burned = o2_to_burn;
            // if we need to burn more than we have then burn all available o2 molecules
            if (o2_to_burn > o2_molecules_available_for_use)
            {
                // burn all available o2 molecules
                o2_burned = o2_molecules_available_for_use;
            }

            // as we burn o2 molecules we have to substract them from the total number of o2 molecules 
            o2_molecules_available -= o2_burned;

            // calculate the new TO2
            to2 = (o2_molecules_available / (vol_current / 1000.0));
            if (to2 < 0)
            {
                to2 = 0;
            }

            // we now know how much o2 molecules we've burnt so we also know how much co2 we generated depending on the respiratory quotient
            double co2_molecules_produced = o2_burned * currentModel.modelDefinition.metabolism["resp_q"] * CO2Storage();

            // add the co2 molecules to the total co2 molecules
            tco2 = (tco2 * (vol_current / 1000.0) + co2_molecules_produced) / (vol_current / 1000.0);
            if (tco2 < 0)
            {
                tco2 = 0;
            }

        }

        double CO2Storage()
        {
            double resp_q_factor = 1.0;

            // store co2 in tissues
            if (tco2 > 25.0)
            {
                resp_q_factor = 1.0 / (((tco2 - 25.0) * 5.0) + 1.0);
            }

            // free co2 from the tissues
            if (tco2 < 25.0)
            {
                resp_q_factor = 1.0 * (((25.0 - tco2) * 0.75) + 1.0);
            }
            if (tco2 < 24.0)
            {
                resp_q_factor = 1.0 * (((24.0 - tco2) * 2.0) + 1.0);
            }

            return resp_q_factor;
        }

        double CalculatePressure()
        {
            return (vol_current - vol_unstressed) * CalculateElastance() + container_pressure + external_pressure;
        }

        double CalculateElastance()
        {
            el_contraction = el_contraction_baseline * el_contraction_activation;

            if (vol_current >= el_max_volume)
            {
                return el_baseline + el_contraction + el_k2 * Math.Pow((vol_current - el_max_volume), 3);
            }

            if (vol_current <= el_min_volume)
            {
               return el_baseline + el_contraction + el_k1 * Math.Pow((vol_current - el_min_volume), 3);
            }

            return el_baseline + el_contraction;
        }
    }
}
