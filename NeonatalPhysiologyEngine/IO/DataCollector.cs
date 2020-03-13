using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonatalPhysiologyEngine.IO {
    public class DataCollector {

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

        // lung and respiration
        GasCompartment OUT;
        GasCompartment NCA;
        GasCompartment ALL;
        GasCompartment ALR;
        GasCompartment VENTIN;
        GasCompartment VENTOUT;
        GasCompartment TUBINGIN;
        GasCompartment TUBINGOUT;
        GasCompartment YPIECE;
        Container CHEST_L;
        Container CHEST_R;

        GasConnector OUT_NCA;
        GasConnector NCA_ALL;
        GasConnector NCA_ALR;
        GasConnector VENTIN_TUBINGIN;
        GasConnector TUBINGIN_YPIECE;
        GasConnector YPIECE_NCA;
        GasConnector YPIECE_TUBINGOUT;
        GasConnector TUBINGOUT_VENTOUT;

        HemodynamicData hemodynamicData = new HemodynamicData ();
        CirculationData circulationData = new CirculationData ();
        RespirationData respirationData = new RespirationData ();
        LungData lungData = new LungData ();
        LabData labData = new LabData ();
        public VitalsData vitalsData = new VitalsData ();
        ECGData ecgData = new ECGData ();
        ECMOData ecmoData = new ECMOData ();
        ANSData ansData = new ANSData ();

        double current_running_time = 0;

        double update_counter_stepsize = 0;
        double update_counter_interval_1sec = 0;
        double update_counter_interval_3sec = 0;
        double update_counter_interval_5sec = 0;
        double update_counter_interval_10sec = 0;

        public double spo2_pre = 0;
        public double spo2_post = 0;
        public double abp_systole = 0;
        double temp_abp_systole = -1000;
        public double abp_diastole = 0;
        double temp_abp_diastole = 1000;
        public double pap_systole = 0;
        double temp_pap_systole = -1000;
        public double pap_diastole = 0;
        double temp_pap_diastole = 1000;
        public double et_co2 = 0;
        double temp_etco2 = -1000;
        public double resp_rate = 0;
        public double art_ph = 0;
        public double art_po2 = 0;
        public double art_pco2 = 0;
        public double art_hco3 = 0;
        public double art_be = 0;
        public double art_lactate = 0;

        public bool Recording { get; set; } = false;

        public DataCollector (Model cm) {
            currentModel = cm;

            // get the most common compartments and connectors
            LV = currentModel.FindModelComponent<BloodCompartment> ("LV");
            LA = currentModel.FindModelComponent<BloodCompartment> ("LA");
            RV = currentModel.FindModelComponent<BloodCompartment> ("RV");
            RA = currentModel.FindModelComponent<BloodCompartment> ("RA");
            AA = currentModel.FindModelComponent<BloodCompartment> ("AA");
            AD = currentModel.FindModelComponent<BloodCompartment> ("AD");
            PA = currentModel.FindModelComponent<BloodCompartment> ("PA");
            IVC = currentModel.FindModelComponent<BloodCompartment> ("IVC");
            SVC = currentModel.FindModelComponent<BloodCompartment> ("SVC");

            LV_AA = currentModel.FindModelComponent<Valve> ("LV_AA");
            LA_LV = currentModel.FindModelComponent<Valve> ("LA_LV");
            RV_PA = currentModel.FindModelComponent<Valve> ("RV_PA");
            RA_RV = currentModel.FindModelComponent<Valve> ("RA_RV");
            IVC_RA = currentModel.FindModelComponent<BloodConnector> ("IVC_RA");
            SVC_RA = currentModel.FindModelComponent<BloodConnector> ("SVC_RA");
            OFO = currentModel.FindModelComponent<Shunt> ("OFO");
            PDA = currentModel.FindModelComponent<Shunt> ("PDA");
            VSD = currentModel.FindModelComponent<Shunt> ("VSD");

            // get the most common compartments and connectors
            OUT = currentModel.FindModelComponent<GasCompartment> ("OUT");
            NCA = currentModel.FindModelComponent<GasCompartment> ("NCA");
            ALL = currentModel.FindModelComponent<GasCompartment> ("ALL");
            ALR = currentModel.FindModelComponent<GasCompartment> ("ALR");
            VENTIN = currentModel.FindModelComponent<GasCompartment> ("VENTIN");
            TUBINGIN = currentModel.FindModelComponent<GasCompartment> ("TUBINGIN");
            YPIECE = currentModel.FindModelComponent<GasCompartment> ("YPIECE");
            TUBINGOUT = currentModel.FindModelComponent<GasCompartment> ("TUBINGOUT");
            VENTOUT = currentModel.FindModelComponent<GasCompartment> ("VENTOUT");
            CHEST_L = currentModel.FindModelComponent<Container> ("CHEST_L");
            CHEST_R = currentModel.FindModelComponent<Container> ("CHEST_R");

            OUT_NCA = currentModel.FindModelComponent<GasConnector> ("OUT_NCA");
            NCA_ALL = currentModel.FindModelComponent<GasConnector> ("NCA_ALL");
            NCA_ALR = currentModel.FindModelComponent<GasConnector> ("NCA_ALR");
            VENTIN_TUBINGIN = currentModel.FindModelComponent<GasConnector> ("VENTIN_TUBINGIN");
            TUBINGIN_YPIECE = currentModel.FindModelComponent<GasConnector> ("TUBINGIN_YPIECE");
            YPIECE_NCA = currentModel.FindModelComponent<GasConnector> ("YPIECE_NCA");
            YPIECE_TUBINGOUT = currentModel.FindModelComponent<GasConnector> ("YPIECE_TUBINGOUT");
            TUBINGOUT_VENTOUT = currentModel.FindModelComponent<GasConnector> ("TUBINGOUT_VENTOUT");

            StartDatarecording ();

        }

        public void StartDatarecording () {
            ResetData ();

            Recording = true;

            current_running_time = 0;
        }

        public void StopDatarecording () {
            Recording = false;

        }

        public void BuildPVLoopLV () {

        }

        public void ModelCycle () {
            if (Recording) {
                // store data with a stepsize interval
                if (update_counter_stepsize > 1.0) {

                    update_counter_stepsize = 0;
                }

                // store data with a 1.0 second interval
                if (update_counter_interval_1sec > 1.0) {
                    art_ph = AA.ph;
                    art_po2 = AA.po2;
                    art_pco2 = AA.pco2;
                    art_hco3 = AA.hco3;
                    art_be = AA.be;
                    art_lactate = 2.0;

                    update_counter_interval_1sec = 0;
                }

                // store data with a 3.0 second interval
                if (update_counter_interval_3sec > 3.0) {
                    et_co2 = temp_etco2;
                    temp_etco2 = -1000;
                    update_counter_interval_3sec = 0;

                    abp_systole = temp_abp_systole;
                    temp_abp_systole = -1000;

                    abp_diastole = temp_abp_diastole;
                    temp_abp_diastole = 1000;

                    pap_systole = temp_pap_systole;
                    temp_pap_systole = -1000;

                    pap_diastole = temp_pap_diastole;
                    temp_pap_diastole = 1000;

                    spo2_pre = AA.so2;

                    spo2_post = AD.so2;

                    if (currentModel.modelDefinition.breathing["spont_breathing_enabled"] == 1) {
                        resp_rate = currentModel.modelDefinition.breathing["spont_resp_rate"];
                    }

                    if (currentModel.modelDefinition.ventilator["ventilator_enabled"] == 1) {
                        resp_rate = 60.0 / (currentModel.modelDefinition.breathing["t_in"] + currentModel.modelDefinition.breathing["t_ex"]);
                    }

                    update_counter_interval_3sec = 0;
                }

                // store data with a 5.0 second interval
                if (update_counter_interval_5sec > 1.0) {

                    update_counter_interval_5sec = 0;
                }

                // store data with a 10 second interval
                if (update_counter_interval_10sec > 1.0) {

                    update_counter_interval_10sec = 0;
                }

                if (AA.pres_current > temp_abp_systole) {
                    temp_abp_systole = AA.pres_current;
                }

                if (AA.pres_current < temp_abp_diastole) {
                    temp_abp_diastole = AA.pres_current;
                }

                if (PA.pres_current > temp_pap_systole) {
                    temp_pap_systole = PA.pres_current;
                }

                if (PA.pres_current < temp_pap_diastole) {
                    temp_pap_diastole = PA.pres_current;
                }

                if (NCA.pco2 > temp_etco2) {
                    temp_etco2 = NCA.pco2;
                }

                // increase the running time with a <modeling_stepsize> step
                current_running_time += currentModel.modelDefinition.modeling_stepsize;

                // increase the update counters
                update_counter_stepsize += currentModel.modelDefinition.modeling_stepsize;
                update_counter_interval_1sec += currentModel.modelDefinition.modeling_stepsize;
                update_counter_interval_3sec += currentModel.modelDefinition.modeling_stepsize;
                update_counter_interval_5sec += currentModel.modelDefinition.modeling_stepsize;
                update_counter_interval_10sec += currentModel.modelDefinition.modeling_stepsize;

            }

        }

        public void FindMinMax () {
            if (AA.pres_current > temp_abp_systole) {
                temp_abp_systole = AA.pres_current;
            }

        }

        public void ResetData () {
            HemodynamicData hemodynamicData = new HemodynamicData ();
            CirculationData circulationData = new CirculationData ();
            RespirationData respirationData = new RespirationData ();
            LungData lungData = new LungData ();
            LabData labData = new LabData ();
            VitalsData vitalsData = new VitalsData ();
            ECGData ecgData = new ECGData ();
            ECMOData ecmoData = new ECMOData ();
            ANSData ansData = new ANSData ();
        }

        void UpdateECGData () {
            ecgData.time.Add (current_running_time);
            ecgData.ecg_signal.Add (currentModel.modelDefinition.ecg["ecg_signal"]);
            ecgData.heartrate.Add (currentModel.modelDefinition.ecg["heart_rate"]);
            ecgData.rhythm_type.Add (currentModel.modelDefinition.ecg["rhythm_type"]);

        }

        void UpdateANSData () {

        }

        void UpdateVitalsData () {
            vitalsData.arterial_diastole.Add (abp_diastole);
            vitalsData.arterial_systole.Add (abp_systole);
            vitalsData.pap_diastole.Add (pap_diastole);
            vitalsData.pap_systole.Add (pap_systole);
            vitalsData.endtidal_co2.Add (et_co2);

            if (labData.arterial_so2.Count > 0) {
                vitalsData.so2_pre.Add (labData.arterial_so2.Last ());

            } else {

                vitalsData.so2_pre.Add (0);
            }

            if (currentModel.modelDefinition.breathing["spont_breathing_enabled"] == 1) {
                vitalsData.resp_rate.Add (currentModel.modelDefinition.breathing["spont_resp_rate"]);
            }

            if (currentModel.modelDefinition.ventilator["ventilator_enabled"] == 1) {
                var rate = 60 / (currentModel.modelDefinition.ventilator["t_in"] + currentModel.modelDefinition.ventilator["t_ex"]);

                vitalsData.resp_rate.Add (rate);
            }

        }

        void UpdateCirculationData () {

            // we try to make a shifting frame of circulation data
            circulationData.time.Add (current_running_time);
            circulationData.lv_pres.Add (LV.pres_current);
            circulationData.rv_pres.Add (RV.pres_current);
            circulationData.la_pres.Add (LA.pres_current);
            circulationData.ra_pres.Add (RA.pres_current);
            circulationData.aa_pres.Add (AA.pres_current);
            circulationData.pa_pres.Add (PA.pres_current);

            circulationData.lv_vol.Add (LV.vol_current);
            circulationData.rv_vol.Add (RV.vol_current);
            circulationData.la_vol.Add (LA.vol_current);
            circulationData.ra_vol.Add (RA.vol_current);

            circulationData.lv_aa_flow.Add (LV_AA.real_flow);
            circulationData.la_lv_flow.Add (LA_LV.real_flow);
            circulationData.rv_pa_flow.Add (RV_PA.real_flow);
            circulationData.ra_rv_flow.Add (RA_RV.real_flow);
            circulationData.ivc_flow.Add (IVC_RA.real_flow);
            circulationData.svc_flow.Add (SVC_RA.real_flow);

        }

        void UpdateLabData () {

        }

        void UpdateECMOData () {

        }

        void UpdateLungData () {

        }

        void UpdateHemodynamicData () {

        }

        void UpdateRespirationData () {

        }

    }

    public class HemodynamicData {
        public List<double> heartrate = new List<double> ();
        public List<double> stroke_volume_left = new List<double> ();
        public List<double> stroke_volume_right = new List<double> ();
        public List<double> cardiac_output = new List<double> ();

        public List<double> ivc_flow = new List<double> ();
        public List<double> svc_flow = new List<double> ();

        public List<double> abp_syst = new List<double> ();
        public List<double> abp_diast = new List<double> ();
        public List<double> abp_mean = new List<double> ();
        public List<double> pap_syst = new List<double> ();
        public List<double> pap_diast = new List<double> ();
        public List<double> pap_mean = new List<double> ();
        public List<double> cvp = new List<double> ();
    }

    public class CirculationData {
        public List<double> time = new List<double> ();

        public List<double> lv_pres = new List<double> ();
        public List<double> la_pres = new List<double> ();
        public List<double> rv_pres = new List<double> ();
        public List<double> ra_pres = new List<double> ();
        public List<double> aa_pres = new List<double> ();
        public List<double> pa_pres = new List<double> ();
        public List<double> ivc_pres = new List<double> ();
        public List<double> svc_pres = new List<double> ();

        public List<double> lv_vol = new List<double> ();
        public List<double> la_vol = new List<double> ();
        public List<double> rv_vol = new List<double> ();
        public List<double> ra_vol = new List<double> ();

        public List<double> lv_aa_flow = new List<double> ();
        public List<double> la_lv_flow = new List<double> ();
        public List<double> rv_pa_flow = new List<double> ();
        public List<double> ra_rv_flow = new List<double> ();
        public List<double> ivc_flow = new List<double> ();
        public List<double> svc_flow = new List<double> ();
    }

    public class RespirationData {
        public List<double> resp_rate = new List<double> ();
        public List<double> tidal_volume = new List<double> ();
        public List<double> minute_volume = new List<double> ();
        public List<double> p_mus = new List<double> ();

        public List<double> t_in = new List<double> ();
        public List<double> t_out = new List<double> ();
        public List<double> max_pip = new List<double> ();
        public List<double> peep = new List<double> ();

    }

    public class LungData {
        public List<double> time = new List<double> ();
        public List<double> out_pres = new List<double> ();
        public List<double> nca_pres = new List<double> ();
        public List<double> all_pres = new List<double> ();
        public List<double> alr_pres = new List<double> ();
        public List<double> ypiece_pres = new List<double> ();
        public List<double> tubingin_pres = new List<double> ();
        public List<double> tubingout_pres = new List<double> ();

        public List<double> all_vol = new List<double> ();
        public List<double> alr_vol = new List<double> ();

        public List<double> out_nca_flow = new List<double> ();
        public List<double> nca_all_flow = new List<double> ();
        public List<double> nca_alr_flow = new List<double> ();
        public List<double> ypiece_nca_flow = new List<double> ();
        public List<double> tubingin_ypiece_flow = new List<double> ();
        public List<double> ypiece_tubingout_flow = new List<double> ();
    }

    public class LabData {
        public List<double> time = new List<double> ();

        public List<double> arterial_ph = new List<double> ();
        public List<double> arterial_po2 = new List<double> ();
        public List<double> arterial_pco2 = new List<double> ();
        public List<double> arterial_hco3 = new List<double> ();
        public List<double> arterial_be = new List<double> ();
        public List<double> alveolar_po2 = new List<double> ();
        public List<double> alveolar_pco2 = new List<double> ();
        public List<double> end_tidal_pco2 = new List<double> ();

        public List<double> arterial_so2 = new List<double> ();
        public List<double> venous_so2 = new List<double> ();
        public List<double> postductal_so2 = new List<double> ();
        public List<double> preductal_so2 = new List<double> ();
        public List<double> arterial_lactate = new List<double> ();

    }

    public class VitalsData {
        public List<double> time = new List<double> ();

        public List<double> heartrate = new List<double> ();
        public List<double> resp_rate = new List<double> ();
        public List<double> so2_pre = new List<double> ();
        public List<double> so2_post = new List<double> ();
        public List<double> arterial_systole = new List<double> ();
        public List<double> arterial_diastole = new List<double> ();
        public List<double> pap_systole = new List<double> ();
        public List<double> pap_diastole = new List<double> ();
        public List<double> cvp = new List<double> ();
        public List<double> endtidal_co2 = new List<double> ();
        public List<double> temperature = new List<double> ();
    }

    public class ECGData {
        public List<double> time = new List<double> ();

        public List<double> ecg_signal = new List<double> ();
        public List<double> heartrate = new List<double> ();
        public List<double> rhythm_type = new List<double> ();

    }

    public class ECMOData {

    }

    public class ANSData {

    }

}