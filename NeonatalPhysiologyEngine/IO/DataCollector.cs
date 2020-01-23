using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine.IO
{
    public class DataCollector
    {
 
        Model currentModel;

        // circulation
        BloodCompartment LV;
        BloodCompartment LA;
        BloodCompartment RV;
        BloodCompartment RA;
        BloodCompartment AA;
        BloodCompartment AD;
        BloodCompartment PA;
        BloodCompartment IVC;
        BloodCompartment SVC;

        Valve LV_AA;
        Valve LA_LV;
        Valve RA_RV;
        Valve RV_PA;

        Shunt OFO;
        Shunt VSD;
        Shunt PDA;

        BloodConnector SVC_RA;
        BloodConnector IVC_RA;

        CirculationData circulationData = new CirculationData();
        RespirationData respirationData = new RespirationData();
        BloodgasData bloodgasData = new BloodgasData();
        VitalsData vitalsData = new VitalsData();
        ECGData ecgData = new ECGData();
        ECMOData ecmoData = new ECMOData();


        double current_time = 0;

        public DataCollector(Model cm)
        {
            currentModel = cm;

            // get the most common compartments and connectors
            LV = currentModel.FindModelComponent<BloodCompartment>("LV");
            LA = currentModel.FindModelComponent<BloodCompartment>("LA");
            RV = currentModel.FindModelComponent<BloodCompartment>("RV");
            RA = currentModel.FindModelComponent<BloodCompartment>("RA");
            AA = currentModel.FindModelComponent<BloodCompartment>("AA");
            AD = currentModel.FindModelComponent<BloodCompartment>("AD");
            PA = currentModel.FindModelComponent<BloodCompartment>("PA");
            IVC = currentModel.FindModelComponent<BloodCompartment>("IVC");
            SVC = currentModel.FindModelComponent<BloodCompartment>("SVC");

            LV_AA = currentModel.FindModelComponent<Valve>("LV_AA");
            LA_LV = currentModel.FindModelComponent<Valve>("LA_LV");
            RV_PA = currentModel.FindModelComponent<Valve>("RV_PA");
            RA_RV = currentModel.FindModelComponent<Valve>("RA_RV");
            IVC_RA = currentModel.FindModelComponent<BloodConnector>("IVC_RA");
            SVC_RA = currentModel.FindModelComponent<BloodConnector>("SVC_RA");

        }
        
        public void ResetData()
        {
            circulationData = new CirculationData();

        }

        void UpdateCirculationData()
        {
            circulationData.time.Add(current_time);
            circulationData.lv_pres.Add(LV.pres_current);
            circulationData.rv_pres.Add(RV.pres_current);
            circulationData.la_pres.Add(LA.pres_current);
            circulationData.ra_pres.Add(RA.pres_current);
            circulationData.aa_pres.Add(AA.pres_current);
            circulationData.pa_pres.Add(PA.pres_current);

        }
        public void UpdateHires()
        {
            UpdateCirculationData();

            current_time += currentModel.modelDefinition.modeling_stepsize;

        }

        public void UpdateLoRes()
        {

        }
    }

    public class CirculationData
    {
        public List<double> time = new List<double>();
        public List<double> lv_pres = new List<double>();
        public List<double> la_pres = new List<double>();
        public List<double> rv_pres = new List<double>();
        public List<double> ra_pres = new List<double>();
        public List<double> aa_pres = new List<double>();
        public List<double> pa_pres = new List<double>();
        public List<double> ivc_pres = new List<double>();
        public List<double> svc_pres = new List<double>();
    }

    public class RespirationData
    {

    }

    public class BloodgasData
    {

    }

    public class VitalsData
    {

    }

    public class ECGData
    {

    }

    public class ECMOData
    {

    }

}
