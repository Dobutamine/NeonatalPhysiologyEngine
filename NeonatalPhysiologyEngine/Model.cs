using Microsoft.Extensions.FileProviders;
using NeonatalPhysiologyEngine.IO;
using NeonatalPhysiologyEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeonatalPhysiologyEngine
{
    public class Model
    {
        // declare the main modeling timer
        Timer modelingTimer;

        public ModelDefinition modelDefinition;
        public ModelInterface modelInterface;
        public DataCollector dataCollector;
        public ECGModel ecg;
        public OxygenationModel oxy;
        public AcidBaseModel acidbase;
        public AutonomicNervousSystemModel ans;
        public SpontaneousBreathingModel breathing;
        public ContractionModel contraction;
        public MechanicalVentilatorModel ventilator;
        public AVInteractionModel avinteraction;
        public GlobalHypoxiaModel hypoxia;
        public DrugModel drugs;
        public BirthModel birth;
        public BrainModel brain;
        public KidneyModel kidneys;
        public LiverModel liver;
        public MaternalPlacentaModel maternalPlacenta;
        public ECMOModel ecmo;
        public CompressionModel compressions;

        public Model()
        {
            // instantiate a model interface for this model instance which takes care of all communication with the outside world
            modelInterface = new ModelInterface(this)
            {
                // first message stating the model instantiated
                StatusMessage = $"Neonatal Physiology Engine version 1.0."
            };

        }

        public bool ModelLoaded { get; set; } = false;

        public async Task<bool> LoadModelFromDiskAsync(string file_name) => await Task.Run(() => { return LoadModelFromDisk(file_name); });
        public async Task<bool> LoadModelFromJSONAsync(string json_file) => await Task.Run(() => { return LoadModelFromJSON(json_file); });
        public async Task<bool> SaveModelToDiskAsync(string file_name) => await Task.Run(() => { return SaveModel(file_name); });
        public async Task<string> CalculateModelAsync(int duration = 60) => await Task.Run(() => { return CalculateModel(duration); });

        bool LoadModelFromDisk(string filename)
        {
            modelDefinition = JSONIO.ImportPatientFromFileOnDisk(filename);

            if (modelDefinition != null)
            {
                modelInterface.StatusMessage = $"Imported patient {filename}. {Environment.NewLine}";
                // initialize the loaded model
                InitModel();

                ModelLoaded = true;

            } else
            {
                modelInterface.StatusMessage = $"Failed to import patient {filename}. {Environment.NewLine}";

                ModelLoaded = true;
            }

            return ModelLoaded;
            
        }
        bool LoadModelFromJSON(string json_file)
        {
            modelDefinition = JSONIO.ImportPatientFromText(json_file);

            if (modelDefinition != null)
            {
                modelInterface.StatusMessage = $"Imported patient : {modelDefinition.name}.";
                // initialize the loaded model
                InitModel();

                ModelLoaded = true;

            } else
            {
                modelInterface.StatusMessage = $"Failed to import patient.";

                ModelLoaded = false;
            }

            return ModelLoaded;
        }       
        bool SaveModel(string filename)
        {
            if (JSONIO.ExportPatient(filename, modelDefinition) && modelDefinition != null)
            {
                modelInterface.StatusMessage = $"Exported patient {filename}. {Environment.NewLine}";

                return true;

            } else
            {
                modelInterface.StatusMessage = $"Failed to export patient to {filename}. {Environment.NewLine}";

                return false;
            }
            
        }
        public T FindModelComponent<T>(string comp_name)
        {
            foreach (BloodCompartment bloodComp in modelDefinition.blood_compartments)
            {
                if (comp_name == bloodComp.name)
                {
                    return (T)Convert.ChangeType(bloodComp, typeof(T));
                }
            }

            foreach (BloodConnector bloodCon in modelDefinition.blood_connectors)
            {
                if (comp_name == bloodCon.name)
                {
                    return (T)Convert.ChangeType(bloodCon, typeof(T));
                }
            }

            foreach (Valve valve in modelDefinition.valves)
            {
                if (comp_name == valve.name)
                {
                    return (T)Convert.ChangeType(valve, typeof(T));
                }
            }

            foreach (Shunt shunt in modelDefinition.shunts)
            {
                if (comp_name == shunt.name)
                {
                    return (T)Convert.ChangeType(shunt, typeof(T));
                }
            }


            foreach (GasCompartment gasComp in modelDefinition.gas_compartments)
            {
                if (comp_name == gasComp.name)
                {
                    return (T)Convert.ChangeType(gasComp, typeof(T));
                }
            }

            foreach (GasConnector gasCon in modelDefinition.gas_connectors)
            {
                if (comp_name == gasCon.name)
                {
                    return (T)Convert.ChangeType(gasCon, typeof(T));
                }
            }

            foreach (Container container in modelDefinition.containers)
            {
                if (comp_name == container.name)
                {
                    return (T)Convert.ChangeType(container, typeof(T));
                }
            }

            foreach (GasExchanger gasExchanger in modelDefinition.exchangers)
            {
                if (comp_name == gasExchanger.name)
                {
                    return (T)Convert.ChangeType(gasExchanger, typeof(T));
                }
            }

            foreach (Diffusor diffusor in modelDefinition.diffusors)
            {
                if (comp_name == diffusor.name)
                {
                    return (T)Convert.ChangeType(diffusor, typeof(T));
                }
            }


            return (T)Convert.ChangeType(null, typeof(T));

        }  
        public void InitModel()
        {
            // initialize all the model components and update the status message in de modelinterface class for notification purposes
            if (modelDefinition != null)
            {
                modelInterface.StatusMessage = $"Initializing model components.";

                modelInterface.StatusMessage = $"Initializing blood compartments.";
                foreach (BloodCompartment bloodComp in modelDefinition.blood_compartments)
                {
                    bloodComp.InitBloodCompartment(this);
                }

                modelInterface.StatusMessage = $"Initializing blood compartment connectors.";
                foreach (BloodConnector bloodCon in modelDefinition.blood_connectors)
                {
                    bloodCon.InitBloodConnector(this);
                }

                modelInterface.StatusMessage = $"Initializing valves.";
                foreach (Valve valve in modelDefinition.valves)
                {
                    valve.InitValve(this);
                }

                modelInterface.StatusMessage = $"Initializing shunts.";
                foreach (Shunt shunt in modelDefinition.shunts)
                {
                    shunt.InitShunt(this);
                }

                modelInterface.StatusMessage = $"Initializing gas compartments.";
                foreach (GasCompartment gasComp in modelDefinition.gas_compartments)
                {
                    gasComp.InitGasCompartment(this);
                }

                modelInterface.StatusMessage = $"Initializing gas compartment connectors.";
                foreach (GasConnector gasCon in modelDefinition.gas_connectors)
                {
                    gasCon.InitGasConnector(this);
                }

                modelInterface.StatusMessage = $"Initializing gasexchangers.";
                foreach (GasExchanger gasExchanger in modelDefinition.exchangers)
                {
                    gasExchanger.InitializeExchanger(this);
                }

                modelInterface.StatusMessage = $"Initializing diffusors.";
                foreach (Diffusor diffusor in modelDefinition.diffusors)
                {
                    diffusor.InitializeDiffusor(this);
                }

                modelInterface.StatusMessage = $"Initializing containers.";
                foreach (Container container in modelDefinition.containers)
                {
                    container.InitContainer(this);
                }

                modelInterface.StatusMessage = $"Initializing submodels.";

                // instantiate the ecg model
                ecg = new ECGModel(this);

                // instantiate the acidbase model
                acidbase = new AcidBaseModel();

                // instantiate the oxygenation model
                oxy = new OxygenationModel();

                // instantiate the autonomic nervous system model
                ans = new AutonomicNervousSystemModel(this);

                // instantiate the spontaneous breathing model
                breathing = new SpontaneousBreathingModel(this);

                // instantiate the contraction model of the heart
                contraction = new ContractionModel(this);

                // instantiate the mechanical ventilator model
                ventilator = new MechanicalVentilatorModel(this);

                // instantiate the av-interaction model
                avinteraction = new AVInteractionModel(this);

                // instantiate the global hypoxia model
                hypoxia = new GlobalHypoxiaModel(this);

                // instantiate the birth model
                birth = new BirthModel(this);

                // instantiate the brain model
                brain = new BrainModel(this);

                // instantiate the drug model
                drugs = new DrugModel(this);

                // instantiate the kidney model
                kidneys = new KidneyModel(this);

                // instantiate the liver model
                liver = new LiverModel(this);

                // instantiate the maternal placenta model
                maternalPlacenta = new MaternalPlacentaModel(this);

                // instantiate the ecmo model
                ecmo = new ECMOModel(this);

                // instantiate the compressions model
                compressions = new CompressionModel(this);

                // instantiate the datacollector
                dataCollector = new DataCollector(this);
            }
            else
            {
                modelInterface.StatusMessage = $"Failed to initialize the model.{Environment.NewLine}";
            }
           
        }
        public void StartRealTimeModel()
        {
            // start the modeling timer if the model loaded correctly
            if (modelDefinition != null)
            {
                if (modelingTimer != null)
                {
                    modelingTimer.Dispose();
                    modelInterface.StatusMessage = $"Realtime model restarting.";
                }
                else
                {
                    modelInterface.StatusMessage = $"Realtime model starting.";
                }

                // restart the modeling timer
                int modelTime = (int)(modelDefinition.modeling_interval * 1000);
                modelingTimer = new Timer(ModelCycle, null, 0, modelTime);
            }
        }
        public void StopRealTimeModel()
        {
            // stop the modeling timer
            if (modelingTimer != null)
            {
                modelInterface.StatusMessage = $"Realtime model stopped.";
                modelingTimer.Dispose();
            }
        }
        string CalculateModel(int duration = 60)
        {
            string report = $"";

            // first delay the realtime model
            if (modelingTimer != null)
            {
                modelingTimer.Change(0, duration * 1000);
            }

            // declare a stopwatch to measure the execution times
            Stopwatch s_modelCycle = new Stopwatch();

            // declare a list holding all execution times to measure the average execution time
            List<double> executionTimes = new List<double>();

            // duration in seconds
            int noNeededSteps = (int)(duration / modelDefinition.modeling_interval);

            // print a status message
            report += $"Calculating model for {duration} seconds in {noNeededSteps} steps. {Environment.NewLine}";

            // calculate the model steps
            for (int i = 0; i < noNeededSteps; i++)
            {
                // start the stopwatch for measuring the model cycle executing time
                s_modelCycle.Start();

                // do the model cycle steps
                ModelCycle(true);

                // report the execution time of the model cycle
                executionTimes.Add(s_modelCycle.ElapsedMilliseconds);

                // stop the stopwatch 
                s_modelCycle.Reset();
            }

            // report performance 
            double performance_total = executionTimes.Sum();
            double performance_step_mean = executionTimes.Average();

            report += $"> Model step interval     : {Math.Round(modelDefinition.modeling_interval * 1000,2)} ms. {Environment.NewLine}";
            report += $"> Model step size         : {Math.Round(modelDefinition.modeling_stepsize * 1000,2)} ms. {Environment.NewLine}";
            report += $"> Model run calculated in : {Math.Round(performance_total,3)} ms.{Environment.NewLine}";
            report += $"> Average model step in   : {Math.Round(performance_step_mean,3)} ms.{Environment.NewLine}";

            // restart the timer if it was already running.
            if (modelingTimer != null)
            {
                modelingTimer.Change(0, (int)(modelDefinition.modeling_interval * 1000));
            }
            
            return report;
        }
        void ModelCycle(object state)
        {
            // calculate the number of frames
            int frames = (int)(modelDefinition.modeling_interval / modelDefinition.modeling_stepsize);

            // model cycles with normal step size which don'require high resolution (saves CPU time)
            ecg.ModelCycle();

            // model cycles with smaller step size for higher resolution results
            for (int i = 0; i < frames; i++)
            {
                // calculate the activation curves for the contraction of the heart chambers
                contraction.ModelCycle();

                // calculate all the flows over the valves
                foreach(Valve valve in modelDefinition.valves)
                {
                    valve.CalculateFlow();
                }

                // calculate all the flows between the blood compartments
                foreach(BloodConnector bloodCon in modelDefinition.blood_connectors)
                {
                    bloodCon.CalculateFlow();
                }

                // calculate all the flows between the blood compartments over the shunts
                foreach (Shunt shunt in modelDefinition.shunts)
                {
                    shunt.CalculateFlow();
                }

                // calculate all the flows between the gas compartments
                foreach (GasConnector gasCon in modelDefinition.gas_connectors)
                {
                    gasCon.CalculateFlow();
                }

                // calculate the container pressures
                foreach (Container container in modelDefinition.containers)
                {
                    container.UpdateCompartment();
                }

                // calculate all the pressures of the blood compartments
                foreach (BloodCompartment bloodComp in modelDefinition.blood_compartments)
                {
                    bloodComp.UpdateCompartment();
                }

                // calculate all the pressures of the blood compartments
                foreach (GasCompartment gasComp in modelDefinition.gas_compartments)
                {
                    gasComp.UpdateCompartment(modelDefinition.metabolism["p_atm"]);
                }

                // do the gasexchange 
                foreach(GasExchanger gasExchanger in modelDefinition.exchangers)
                {
                    gasExchanger.CalculateGasexchange();
                }

                // calculate the diffusors
                foreach(Diffusor diffusor in modelDefinition.diffusors)
                {
                    diffusor.CalculateDiffusion();
                }

                // spontaneous breathing
                breathing.ModelCycle();

                // ventilator
                ventilator.ModelCycle();

                // av interaction model
                avinteraction.ModelCycle();

                // birth model
                birth.ModelCycle();

                // compression model
                compressions.ModelCycle();
            
                // ECMO model
                ecmo.ModelCycle();

                // maternal placenta model
                maternalPlacenta.ModelCycle();

                // update the data collector
                dataCollector.ModelCycle();

            }
            
            // brain model
            brain.ModelCycle();

            // drug model
            drugs.ModelCycle();

            // Global hypoxia
            hypoxia.ModelCycle();

            // kidney model
            kidneys.ModelCycle();

            // liver mode
            liver.ModelCycle();

            // autonomic nervous system model
            ans.ModelCycle();

            // signal the modelinterface class that a model run has been completed
            modelInterface.ModelUpdated = true; 

        }

    }


}
