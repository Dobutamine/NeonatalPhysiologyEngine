using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.Models
{
    public class ECGModel
    {
        Model currentModel;

        readonly double[] normalQRSWave = { -8, 3, 96, -104, 4, 9, 0, 0 };
        readonly double[] ventQRSWave = { -25, 20, 50, -150, 20, 20, 20, 15, 10, 10, 10 };

        double sinusNodePeriod = 1000.0;
        double sinusNodeCounter = 0.0;

        bool pqRunning = false;
        double pqCounter = 0;
        double pWaveSignalCounter = 0;
        double prevPSignal = 0;

        bool qtRunning = false;
        double qtCounter = 0;
        double qtc = 0;
        double tWaveSignalCounter = 0;
        double prevTSignal = 0;

        bool qrsRunning = false;
        double qrsCounter = 0;
        double qrsWaveSignalCounter = 0;

        bool ventricularActivation = false;
        bool atrialActivation = false;
        double ecgSignal = 0;


        public ECGModel(Model cm)
        {
            currentModel = cm;

            currentModel.modelInterface.StatusMessage = $"Initialized the ECG model.";
        }

        public void ModelCycle()
        {
            CalculateECGTimings();
        }

        void CalculateECGTimings()
        {
            // first set the sinus node timer in seconds
            if (currentModel.modelDefinition.ecg["heart_rate"] > 0)
            {
                sinusNodePeriod = 60.0 / currentModel.modelDefinition.ecg["heart_rate"];
            } else
            {
                sinusNodePeriod = 60.0;
            }

            // check whether it is time for a new heartbeat from the sinus node
            if (sinusNodeCounter > sinusNodePeriod && pqRunning == false)
            {
                // reset the sinus node counter
                sinusNodeCounter = 0;
                // reset the pq counter
                pqCounter = 0;
                // flag that the pq time is running
                pqRunning = true;
                // set the atrium activation active
                atrialActivation = true;
            }

            // check whether the pq time is finished.
            if (pqCounter > currentModel.modelDefinition.ecg["pq_time"] && pqRunning)
            {
                // reset the pq counter
                pqCounter = 0;
                // switch off the pq is running flag
                pqRunning = false;
                // start the qrs counter
                qrsCounter = 0;
                // flag that the qrs is running
                qrsRunning = true;
                // set the atrium passive
                atrialActivation = false;
                // set the ventricles active
                ventricularActivation = true;
            }

            // check whether the qrs time is finished
            if (qrsCounter > currentModel.modelDefinition.ecg["qrs_time"] && qrsRunning)
            {
                // reset the qrs counter
                qrsCounter = 0;
                // switch off the qrs is running flag
                qrsRunning = false;
                // start the qt counter
                qtCounter = 0;
                // flag that the qt is running
                qtRunning = true;
                // set the ventricles passive
                ventricularActivation = false;
            }

            // calculate the corrected qt time
            qtc = CalculateQTCTime();

            // check whether the qt time is finished.


        }

        double CalculateQTCTime()
        {
            if (currentModel.modelDefinition.ecg["heart_rate"] > 0)
            {
                return currentModel.modelDefinition.ecg["qt_time"] * Math.Sqrt(60.0 / currentModel.modelDefinition.ecg["heart_rate"]);
            }

            return currentModel.modelDefinition.ecg["qt_time"] * Math.Sqrt(60.0 / 10.0);

        }

    }
}
