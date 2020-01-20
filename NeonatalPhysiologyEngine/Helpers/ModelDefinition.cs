using NeonatalPhysiologyEngine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeonatalPhysiologyEngine
{
    public class ModelDefinition
    {
        public string __class__ { get; set; }
        public string __module__ { get; set; }
        public string name { get; set; }
        public float weight { get; set; }
        public float modeling_interval { get; set; }
        public float modeling_stepsize { get; set; }
        public Dictionary<string, double> metabolism { get; set; } = new Dictionary<string, double>
        {
            { "atp_need", 0.12 },
            { "resp_q", 0.8 },
            { "p_atm", 760.0 },
            { "outside_temp", 20.0 },
            { "body_temp", 37.0 },
            { "hemoglobin", 10.0 }
        };
        public Dictionary<string, double> ecg { get; set; } = new Dictionary<string, double>
        {
            {"heart_rate", 120.0},
            {"heart_rate_ref", 120.0},
            {"rhythm_type", 0},
            {"pq_time", 0.1},
            {"qrs_time", 0.075},
            {"qt_time", 0.4},
            {"cqt_time", 0.26},
            {"amp_p", 8.0},
            {"skew_p", 2.5},
            {"width_p", 20.0},
            {"amp_t", 18.0},
            {"skew_t", 1.5},
            {"width_t", 25.0},
            {"ncc_atrial", 0},
            {"ncc_ventricular", 0},
            {"ecg_signal", 0.0 }
        };
        public Dictionary<string, double> breathing { get; set; } = new Dictionary<string, double>
        {
            {"spont_breathing_enabled", 1},
            {"spont_resp_rate", 120.0},
            {"ref_minute_volume", 0},
            {"ref_tidal_volume", 0.1},
            {"target_minute_volume", 0.075},
            {"target_tidal_volume", 0.4},
            {"vtrr_ratio", 0.26},
            {"resp_muscle_pressure", 8.0}
        };
        public Dictionary<string, double> ventilator { get; set; } = new Dictionary<string, double>
        {
		   {"ventilator_enabled", 1},
           {"volume_controlled", 1},
           {"target_tidal_volume", 25.0},
           {"max_pip", 20.0},
           {"peep", 4.0},
           {"insp_flow", 8.0},
           {"exp_flow", 8.0},
           {"t_in", 0.4},
           {"t_ex", 1.0 }
        };
        public Dictionary<string, double> ans { get; set; } = new Dictionary<string, double>
        {
           {"th_lungvol", 40.0},
           {"op_lungvol", 50.0},
           {"sa_lungvol", 60.0},
           {"th_map", 40.0},
           {"op_map", 57.0},
           {"sa_map", 70.0},
           {"th_po2", 0.7},
           {"op_po2", 7.0},
           {"sa_po2", 7.0},
           {"th_pco2", 3.3},
           {"op_pco2", 5.4},
           {"sa_pco2", 9.3},
           {"th_ph", 7.0},
           {"op_ph", 7.39},
           {"sa_ph", 7.6},
           {"tc_po2_hp", 3.0},
           {"tc_map_hp", 2.0},
           {"tc_map_cont", 2.0},
           {"tc_map_venpool", 2.0},
           {"tc_map_res", 2.0},
           {"tc_po2_ve", 10.0},
           {"tc_pco2_hp", 2.0},
           {"tc_pco2_ve", 5.0},
           {"tc_ph_hp", 2.0},
           {"tc_ph_ve", 2.0},
           {"tc_lungvol_hp", 20.0},
           {"g_po2_ve", -50.0},
           {"g_ph_ve", 450.0},
           {"g_ph_hp", 0.0},
           {"g_pco2_ve", 250.0},
           {"g_pco2_hp", 0.0},
           {"g_po2_hp", -15.0},
           {"g_map_cont", 0.0},
           {"g_map_res", 0.0},
           {"g_map_venpool", 0.0},
           {"g_map_hp", 10.0},
           {"g_lungvol_hp", -0.72},
           {"lung_vol_hp_threshold", 0.25 }
        };
        public List<BloodCompartment> blood_compartments { get; set; } = new List<BloodCompartment>();
        public List<BloodConnector> blood_connectors { get; set; } = new List<BloodConnector>();
        public List<Valve> valves { get; set; } = new List<Valve>();
        public List<Shunt> shunts { get; set; } = new List<Shunt>();
        public List<GasCompartment> gas_compartments { get; set; } = new List<GasCompartment>();
        public List<GasConnector> gas_connectors { get; set; } = new List<GasConnector>();
        public List<Container> containers { get; set; } = new List<Container>();
        public List<Diffusor> diffusors { get; set; } = new List<Diffusor>();
        public List<GasExchanger> exchangers { get; set; } = new List<GasExchanger>();

    }
}
