using NeonatalPhysiologyEngine.IO;
using NeonatalPhysiologyEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NeonatalPhysiologyEngine
{
    public class Model
    {
        public ModelDefinition modelDefinition;
        public ModelInterface modelInterface;
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

        public Model(string filename = "")
        {
            // instantiate a model interface for this model instance which takes care of all communication with the outside world
            modelInterface = new ModelInterface(this)
            {
                // first message stating the model instantiated
                StatusMessage = $"Neonatal Physiology Engine version 1.0. {Environment.NewLine}"
            };

            // load model if a filename is provided
            if (filename != "")
            {
                LoadModel(filename);
            }

        }

        public void LoadModel(string filename)
        {
            modelDefinition = JSONIO.ImportPatient(filename);
            if (modelDefinition != null)
            {
                modelInterface.StatusMessage = $"Imported patient {filename}. {Environment.NewLine}";

                // initialize the loaded model
                InitModel();

            } else
            {
                modelInterface.StatusMessage = $"Failed to import patient {filename}. {Environment.NewLine}";
            }
            
        }

        public void SaveModel(string filename)
        {
            if (JSONIO.ExportPatient(filename, modelDefinition) && modelDefinition != null)
            {
                modelInterface.StatusMessage = $"Exported patient {filename}. {Environment.NewLine}";
            } else
            {
                modelInterface.StatusMessage = $"Failed to export patient to {filename}. {Environment.NewLine}";
            }
            
        }

        public void InitModel()
        {
            // initialize all the model components and update the status message in de modelinterface class for notification purposes
            if (modelDefinition != null)
            {
                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing model components.";

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing blood compartments.";
                foreach (BloodCompartment bloodComp in modelDefinition.blood_compartments)
                {
                    bloodComp.InitBloodCompartment(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing blood compartment connectors.";
                foreach (BloodConnector bloodCon in modelDefinition.blood_connectors)
                {
                    bloodCon.InitBloodConnector(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing valves.";
                foreach (Valve valve in modelDefinition.valves)
                {
                    valve.InitValve(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing shunts.";
                foreach (Shunt shunt in modelDefinition.shunts)
                {
                    shunt.InitShunt(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing gas compartments.";
                foreach (GasCompartment gasComp in modelDefinition.gas_compartments)
                {
                    gasComp.InitGasCompartment(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing gas compartment connectors.";
                foreach (GasConnector gasCon in modelDefinition.gas_connectors)
                {
                    gasCon.InitGasConnector(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing gasexchangers.";
                foreach (GasExchanger gasExchanger in modelDefinition.exchangers)
                {
                    gasExchanger.InitializeExchanger(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing diffusors.";
                foreach (Diffusor diffusor in modelDefinition.diffusors)
                {
                    diffusor.InitializeDiffusor(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing containers.";
                foreach (Container container in modelDefinition.containers)
                {
                    container.InitContainer(this);
                }

                modelInterface.StatusMessage = $"{Environment.NewLine}Initializing submodels.";

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
            }
            else
            {
                modelInterface.StatusMessage = $"Failed to initialize the model.{Environment.NewLine}";
            }
           
        }

        public void StartRealTimeModel()
        {

        }

        public void StopRealTimeModel()
        {

        }

        public void CalculateModel(int duration = 10)
        {
            // declare a stopwatch to measure the execution times
            Stopwatch s_modelCycle = new Stopwatch();

            // declare a list holding all execution times to measure the average execution time
            List<double> executionTimes = new List<double>();

            // duration in seconds
            int noNeededSteps = (int)(duration / modelDefinition.modeling_interval);

            // print a status message
            modelInterface.StatusMessage = $"{Environment.NewLine} Calculating model for {duration} seconds in {noNeededSteps} steps. {Environment.NewLine}";

            // calculate the model steps
            for (int i = 0; i < noNeededSteps; i++)
            {
                // start the stopwatch for measuring the model cycle executing time
                s_modelCycle.Start();

                // do the model cycle steps
                ModelCycle();

                // report the execution time of the model cycle
                executionTimes.Add(s_modelCycle.ElapsedMilliseconds);

                // stop the stopwatch 
                s_modelCycle.Reset();
            }

            // report performance 
            double performance_total = executionTimes.Sum();
            double performance_step_mean = executionTimes.Average();

            modelInterface.StatusMessage = $"> Model step interval     : {Math.Round(modelDefinition.modeling_interval * 1000,2)} ms.";
            modelInterface.StatusMessage = $"> Model step size         : {Math.Round(modelDefinition.modeling_stepsize * 1000,2)} ms.";
            modelInterface.StatusMessage = $"> Model run calculated in : {Math.Round(performance_total,3)} ms.";
            modelInterface.StatusMessage = $"> Average model step in   : {Math.Round(performance_step_mean,3)} ms.";
        }

        void ModelCycle()
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

                // spontaneous breathing
                breathing.ModelCycle();

                // ventilator
                ventilator.ModelCycle();

                // update the data collector in the model interface class if monitoring is true
                if (modelInterface.MonitoringMode == 1)
                {
                    modelInterface.UpdateHiResData();
                }
                
            }

            // autonomic nervous system
            ans.ModelCycle();


        }

    }


}
