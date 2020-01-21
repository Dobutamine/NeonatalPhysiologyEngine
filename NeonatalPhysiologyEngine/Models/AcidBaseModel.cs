using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class AcidBaseModel
    {
        // This class models the acid base properties of te plasma according to the Stewart-Figge principles.

        // Independent input variables are : 
        // pCO2 or TCO2, SID (Strong Ion Difference), albumin concentration, inorganic phosphates concentration and hemoglobin (all in mmol/l).

        // private variables -> state variables
        readonly double alpha_co2p = 0.230;
        readonly double kw = Math.Pow(10, -13.6) * 1000.0;
        readonly double kc = Math.Pow(10, -6.1) * 1000.0;
        readonly double kd = Math.Pow(10, -10.22) * 1000.0;

        // rootFinding parameters
        readonly double lefthp = Math.Pow(10.0, -8) * 1000.0;
        readonly double righthp = Math.Pow(10.0, -6) * 1000.0;
        readonly double brentAccuracy = 1e-8;
        readonly int maxIterations = 50;

        // local state variables
        double _tco2;
        double _sid;
        double _albumin;
        double _phosphates;
        double _hb;

        double pH = 0;
        double tco2co2 = 0;
        double co2p = 0;
        double hco3p = 0;
        double co3p = 0;
        double ohp = 0;
        double albbase = 0;
        double phosbase = 0;

        public void CalcAcidBaseFromTCO2(BloodCompartment bloodcomp)
        {
            // store the independent variables into local fields
            _tco2 = bloodcomp.tco2;
            _sid = bloodcomp.sid;
            _albumin = bloodcomp.albumin;
            _phosphates = bloodcomp.phosphates;
            _hb = bloodcomp.hemoglobin;

            // find the Hp concentration where the netcharge of the plasma is zero

            double Hp = BrentRootfinder(lefthp, righthp, brentAccuracy, maxIterations, out int iterationsUsed, out _);

            if (iterationsUsed < maxIterations)
            {
                bloodcomp.ph = -Math.Log10(Hp / 1000.0);
                // dissolved co2
                bloodcomp.cco2 = co2p;
                // co2 bound to bicarbonate
                bloodcomp.hco3 = hco3p;
                // co2 bound to carbamate
                bloodcomp.co3 = co3p;
                // calculate the base excess
                bloodcomp.be = (bloodcomp.hco3 - 24.4 + (2.3 * _hb + 7.7) * (bloodcomp.ph - 7.4)) * (1.0 - 0.023 * _hb);   // mmol/l -> van Slyke equation to determine the base excess  
                // calculate the pCO2
                bloodcomp.pco2 = bloodcomp.cco2 / alpha_co2p;
            }
        }

        double CalcPlasmaNetchargeFromTCO2(double hp)
        {
            pH = -Math.Log10(hp / 1000.0);
            tco2co2 = 1 + (kc / hp) + ((kc * kd) / Math.Pow(hp, 2.0));
            co2p = _tco2 / tco2co2;
            hco3p = (kc * co2p) / hp;
            co3p = (kd * hco3p) / hp;
            ohp = kw / hp;
            albbase = _albumin * (0.378 / (1.0 + Math.Pow(10.0, (7.1 - pH))));
            phosbase = _phosphates / (1.0 + Math.Pow(10.0, (6.8 - pH)));

            double netcharge = _sid + hp - hco3p - (2.0 * co3p) - ohp - albbase - phosbase;

            return netcharge;

        }

        double CalcPlasmaNetchargeFromPCO2(double hp, double _pco2)
        {
            double pH = -Math.Log10(hp / 1000.0);
            double co2p = _pco2 * alpha_co2p;
            double hco3p = (kc * co2p) / hp;
            double co3p = (kd * hco3p) / hp;
            double ohp = kw / hp;
            double albbase = _albumin * (0.378 / (1.0 + Math.Pow(10.0, (7.1 - pH))));
            double phosbase = _phosphates / (1.0 + Math.Pow(10.0, (6.8 - pH)));
            double netcharge = _sid + hp - hco3p - (2.0 * co3p) - ohp - albbase - phosbase;

            return netcharge;

        }

        double BrentRootfinder(double left, double right, double tolerance, double maxIterations, out int iterationsUsed, out double errorEstimate)
        {
            double c, d, e, fa, fb, fc, tol, m, p, q, r, s;

            // set up aliases to match Brent's notation
            double a = left; double b = right; double t = tolerance;
            iterationsUsed = 0;

            fa = CalcPlasmaNetchargeFromTCO2(a);
            fb = CalcPlasmaNetchargeFromTCO2(b);


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

                fb = CalcPlasmaNetchargeFromTCO2(b);
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
