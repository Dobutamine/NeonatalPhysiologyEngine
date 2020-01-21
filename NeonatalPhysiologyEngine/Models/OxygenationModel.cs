using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class OxygenationModel
    {
        // This class models the oxygen properties of te plasma..

        // Independent input variables are : 
        // TO2 or PO2, temperature, DPG, pH, base excess and hemoglobin values

        public double PO2_P { get; set; }
        public double O2_P { get; set; }
        public double TO2_P { get; set; }
        public double SO2_P { get; set; }

        // private variable
        readonly double alpha_o2p = 0.0095;
        readonly double mmoltoml = 22.2674;                        // convert mmol to ml  (1 mmol of gas occupies 22.2674 ml)
        readonly double lefto2 = 0;
        readonly double righto2 = 100;
        readonly double brentAccuracy = 1e-8;
        readonly int maxIterations = 50;

        // brent function extra arguments as the numerics library Brent function can't take extra arguments.
        double to2_brent;

        // ODC variables
        double ph_odc;
        double be_odc;
        double dpg_odc;
        double temp_odc;
        double hb_odc;

        public void CalcOxygenationFromTO2(BloodCompartment bloodComp)
        {
            // brent function extra argument as the numerics library brent function can't take extra arguments.
            to2_brent = bloodComp.to2;

            // ODC variables
            ph_odc = Math.Abs(bloodComp.ph) > 0.0 ? bloodComp.ph : 7.40;
            be_odc = Math.Abs(bloodComp.be) > 0.0 ? bloodComp.be : 0.0;

            temp_odc = bloodComp.temp;
            dpg_odc = bloodComp.dpg;
            hb_odc = bloodComp.hemoglobin;

            // find the po2 where the difference between the to2 and the target to2 is zero mmol/l and the pO2 in kPa
            double po2 = BrentRootfinder(lefto2, righto2, brentAccuracy, maxIterations, out int iterationsUsed, out _);

            if (iterationsUsed < maxIterations)
            {
                CalcOxygenation(bloodComp.to2, po2);

                // sometimes multiple solutions are available, if the difference between the last step is too high it is probably a incorrect value, then ignore it
                if (Math.Abs(bloodComp.prev_so2 - SO2_P) < 0.05 || bloodComp.prev_so2 == 0.0 || bloodComp.error_counter > 10.0)
                {
                    bloodComp.so2 = SO2_P;       // fraction
                    bloodComp.to2 = TO2_P;       // in mmol/l
                    bloodComp.co2 = O2_P;        // in mmol/l
                    bloodComp.po2 = PO2_P;       // in kPa
                    bloodComp.error_counter = 0;
                    bloodComp.prev_so2 = SO2_P;
                }
                else
                {
                    bloodComp.error_counter++;
                }
            }
        }

        public void CalcOxygenationFromPO2(BloodCompartment bloodComp)
        {
            // ODC parameters
            ph_odc = bloodComp.ph;
            be_odc = bloodComp.be;
            temp_odc = bloodComp.temp;
            dpg_odc = bloodComp.dpg;
            hb_odc = bloodComp.hemoglobin;

            SO2_P = Odc(bloodComp.po2);
            double to2ml = (0.02325 * bloodComp.po2 + (1.36 * (bloodComp.hemoglobin / 0.6206) * SO2_P)) * 10.0;
            TO2_P = to2ml / mmoltoml;
            O2_P = PO2_P * alpha_o2p;
            PO2_P = bloodComp.po2;

            bloodComp.so2 = SO2_P;
            bloodComp.to2 = TO2_P;
            bloodComp.co2 = O2_P;
            bloodComp.po2 = PO2_P;

        }

        void CalcOxygenation(double to2, double po2)
        {
            PO2_P = po2;
            TO2_P = to2;
            O2_P = PO2_P * alpha_o2p;
            SO2_P = Odc(PO2_P);

        }

        double Odc(double po2)
        {
            double a = 1.04 * (7.4 - ph_odc) + 0.005 * be_odc + 0.07 * (dpg_odc - 5.0);
            double b = 0.055 * ((temp_odc + 273.15) - 310.15);
            double y0 = 1.875;   // was 1.875
            double x0 = 1.875 + a + b;  // was 1.946
            double h0 = 3.5 + a;  // was 3.5
            double k = 0.5343;
            double x = Math.Log(po2, Math.E);
            double y = (x - x0) + h0 * Math.Tanh(k * (x - x0)) + y0;

            return 1.0 / (Math.Pow(Math.E, -y) + 1.0);
        }

        double Dto2(double po2)
        {
            double so2 = Odc(po2);
            double to2 = (0.02325 * po2 + (1.36 * (hb_odc / 0.6206) * so2)) * 10.0;   // in ml O2 per liter
            double dto2 = to2_brent - (to2 / mmoltoml);                               // to2 in ml O2/l needs to be converted to mmol/l  

            return dto2;
        }

        double BrentRootfinder(double left, double right, double tolerance, double maxIterations, out int iterationsUsed, out double errorEstimate)
        {
            double c, d, e, fa, fb, fc, tol, m, p, q, r, s;

            // set up aliases to match Brent's notation
            double a = left; double b = right; double t = tolerance;
            iterationsUsed = 0;

            fa = Dto2(a);
            fb = Dto2(b);

        label_int:
            c = a; fc = fa; d = e = b - a;

        label_ext:
            if (Math.Abs(fc) < Math.Abs(fb))
            {
                a = b; b = c; c = a;
                fa = fb; fb = fc; fc = fa;
            }

            iterationsUsed++;

            tol = 2.0 * t * Math.Abs(b) + t;
            errorEstimate = m = 0.5 * (c - b);
            if (Math.Abs(m) > tol && fb != 0.0) // exact comparison with 0 is OK here
            {
                // See if bisection is forced
                if (Math.Abs(e) < tol || Math.Abs(fa) <= Math.Abs(fb))
                {
                    d = e = m;
                }
                else
                {
                    s = fb / fa;
                    if (a == c)
                    {
                        // linear interpolation
                        p = 2.0 * m * s; q = 1.0 - s;
                    }
                    else
                    {
                        // Inverse quadratic interpolation
                        q = fa / fc; r = fb / fc;
                        p = s * (2.0 * m * q * (q - r) - (b - a) * (r - 1.0));
                        q = (q - 1.0) * (r - 1.0) * (s - 1.0);
                    }
                    if (p > 0.0)
                        q = -q;
                    else
                        p = -p;
                    s = e; e = d;
                    if (2.0 * p < 3.0 * m * q - Math.Abs(tol * q) && p < Math.Abs(0.5 * s * q))
                        d = p / q;
                    else
                        d = e = m;
                }
                a = b; fa = fb;
                if (Math.Abs(d) > tol)
                    b += d;
                else if (m > 0.0)
                    b += tol;
                else
                    b -= tol;
                if (iterationsUsed == maxIterations)
                    return b;

                fb = Dto2(b);
                if ((fb > 0.0 && fc > 0.0) || (fb <= 0.0 && fc <= 0.0))
                    goto label_int;
                else
                    goto label_ext;
            }
            else
                return b;
        }


    }
}
