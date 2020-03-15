using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine
{
    public class GasCompartment
    {
        public string name { get; set; }
        public int is_enabled { get; set; }
        public int has_fixed_composition { get; set; }
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

        public double fo2 { get; set; }
        public double fn2 { get; set; }
        public double fco2 { get; set; }
        public double fother { get; set; }
        public double fh2o { get; set; }

        public double po2 { get; set; }
        public double pn2 { get; set; }
        public double pco2 { get; set; }
        public double pother { get; set; }
        public double ph2o { get; set; }
        public double patm { get; set; }
        public double ctotal { get; set; }
        public double co2 { get; set; }
        public double cn2 { get; set; }
        public double cco2 { get; set; }
        public double cother { get; set; }
        public double ch2o { get; set; }
        public double to2 { get; set; }
        public double temp { get; set; }
        double gas_constant = 62.36367;
        Model currentModel;
        public void InitGasCompartment(Model cm)
        {
            pres_current = 0;
            container_pressure = 0;
            external_pressure = 0;
            patm = 760;
            temp = 37;
            pres_current = CalculatePressure();
            CalculateComposition();

            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized gas compartment {name}.";
        }

        public void UpdateCompartment(double atmospheric_pressure)
        {
            if (is_enabled == 1)
            {
                patm = atmospheric_pressure;
                pres_current = CalculatePressure();
                to2 = po2;
            }
        }

        public void GasIn(double dvol, GasCompartment compFrom)
        {
            if (has_fixed_composition == 0)
            {
                // change the volume
                vol_current += dvol;
                if (vol_current < 0)
                {
                    vol_current = 0.01;
                }

                // convert the concentrations
                double vol_current_l = vol_current / 1000.0;
                double dvol_l = dvol / 1000.0;

                // change the gas concentrations
                double dtotal = dvol_l * (compFrom.ctotal - ctotal);
                ctotal = ((ctotal * vol_current_l) + dtotal) / vol_current_l;

                double dco2 = dvol_l * (compFrom.co2 - co2);
                co2 = ((co2 * vol_current_l) + dco2) / vol_current_l;

                double dcco2 = dvol_l * (compFrom.cco2 - cco2);
                cco2 = ((cco2 * vol_current_l) + dcco2) / vol_current_l;

                double dcn2 = dvol_l * (compFrom.cn2 - cn2);
                cn2 = ((cn2 * vol_current_l) + dcn2) / vol_current_l;

                double dcother = dvol_l * (compFrom.cother - cother);
                cother = ((cother * vol_current_l) + dcother) / vol_current_l;

                // update the composition
                fo2 = co2 / ctotal;
                fco2 = cco2 / ctotal;
                fn2 = cn2 / ctotal;
                fother = cother / ctotal;

                po2 = fo2 * (pres_current - (pres_current * fh2o));
                pco2 = fco2 * (pres_current - (pres_current * fh2o));
                pn2 = fn2 * (pres_current - (pres_current * fh2o));
                pother = fother * (pres_current - (pres_current * fh2o));

            }
            else
            {
                CalculateComposition();
            }
        }

        public void GasOut(double dvol)
        {
            if (has_fixed_composition == 0)
            {
                vol_current -= dvol;
                if (vol_current < 0)
                {
                    vol_current = 0.01;
                }
            }
        }

        double CalculatePressure()
        {
            return (vol_current - vol_unstressed) * CalculateElastance() + container_pressure + external_pressure + patm;
        }
        double CalculateElastance()
        {

            if (vol_current >= el_max_volume)
            {
                return el_baseline + el_k2 * Math.Pow((vol_current - el_max_volume), 3);
            }

            if (vol_current <= el_min_volume)
            {
                return el_baseline + el_k1 * Math.Pow((vol_current - el_min_volume), 3);
            }

            return el_baseline;
        }

        void CalculateComposition()
        {
            double vol_current_l = vol_current / 1000.0;

            // calculate the concentration at this pressure, volume and temperatuur in mol/l
            ctotal = ((pres_current * vol_current_l) / (gas_constant * (273.15 + temp))) / vol_current_l;

            // calculate the partial pressures depending on the concentrations
            co2 = fo2 * ctotal;
            cco2 = fco2 * ctotal;
            cn2 = fn2 * ctotal;
            cother = fother * ctotal;
            ch2o = fh2o * ctotal;

            po2 = fo2 * (pres_current - (pres_current * fh2o));
            pco2 = fco2 * (pres_current - (pres_current * fh2o));
            pn2 = fn2 * (pres_current - (pres_current * fh2o));
            pother = fother * (pres_current - (pres_current * fh2o));

        }
    }
}
