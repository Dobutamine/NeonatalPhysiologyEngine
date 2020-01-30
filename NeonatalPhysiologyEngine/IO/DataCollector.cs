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


        double current_time_hires = 0;

        double current_time_lores = 0;

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

            circulationData = new CirculationData();

            respirationData = new RespirationData();

        }
        
        public void ResetData()
        {
            circulationData = new CirculationData();

            respirationData = new RespirationData();


        }

        void UpdateCirculationData()
        {
            circulationData.time.Add(current_time_hires);

            circulationData.lv_pres.Add(LV.pres_current);
            circulationData.rv_pres.Add(RV.pres_current);
            circulationData.la_pres.Add(LA.pres_current);
            circulationData.ra_pres.Add(RA.pres_current);
            circulationData.aa_pres.Add(AA.pres_current);
            circulationData.pa_pres.Add(PA.pres_current);

            circulationData.lv_vol.Add(LV.vol_current);
            circulationData.rv_vol.Add(RV.vol_current);
            circulationData.la_vol.Add(LA.vol_current);
            circulationData.ra_vol.Add(RA.vol_current);

            circulationData.lv_aa_flow.Add(LV_AA.real_flow);
            circulationData.la_lv_flow.Add(LA_LV.real_flow);
            circulationData.rv_pa_flow.Add(RV_PA.real_flow);
            circulationData.ra_rv_flow.Add(RA_RV.real_flow);
            circulationData.ivc_flow.Add(IVC_RA.real_flow);
            circulationData.svc_flow.Add(SVC_RA.real_flow);

        }

        void UpdateLungData()
        {

        }

        void UpdateLabData()
        {

        }

        void UpdateVitalsData()
        {

        }

        public void UpdateHires()
        {
            UpdateCirculationData();

            UpdateLungData();

            current_time_hires += currentModel.modelDefinition.modeling_stepsize;

        }

        public void UpdateLoRes()
        {
            UpdateVitalsData();

            UpdateLabData();

            current_time_lores += currentModel.modelDefinition.modeling_interval;
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

        public List<double> lv_vol = new List<double>();
        public List<double> la_vol = new List<double>();
        public List<double> rv_vol = new List<double>();
        public List<double> ra_vol = new List<double>();

        public List<double> lv_aa_flow = new List<double>();
        public List<double> la_lv_flow = new List<double>();
        public List<double> rv_pa_flow = new List<double>();
        public List<double> ra_rv_flow = new List<double>();
        public List<double> ivc_flow = new List<double>();
        public List<double> svc_flow = new List<double>();
    }

    public class RespirationData
    {
        public List<double> time = new List<double>();

        public List<double> out_pres = new List<double>();
        public List<double> nca_pres = new List<double>();
        public List<double> all_pres = new List<double>();
        public List<double> alr_pres = new List<double>();
        public List<double> ypiece_pres = new List<double>();
        public List<double> tubingin_pres = new List<double>();
        public List<double> tubingout_pres = new List<double>();

        public List<double> all_vol = new List<double>();
        public List<double> alr_vol = new List<double>();

        public List<double> out_nca_flow = new List<double>();
        public List<double> nca_all_flow = new List<double>();
        public List<double> nca_alr_flow = new List<double>();
        public List<double> ypiece_nca_flow = new List<double>();
        public List<double> tubingin_ypiece_flow = new List<double>();
        public List<double> ypiece_tubingout_flow = new List<double>();
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
