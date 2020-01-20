using NeonatalPhysiologyEngine.IO;
using NeonatalPhysiologyEngine.Models;
using System;

namespace NeonatalPhysiologyEngine
{
    public class Model
    {
        public ModelDefinition modelDefinition;
        public ModelInterface modelInterface;
        public ECGModel ecg;
        public AutonomicNervousSystemModel ans;
        public SpontaneousBreathingModel breathing;
        public AcidBaseModel ab;
        public OxygenationModel oxy;
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

                // instantiate the autonomic nervous system model
                ans = new AutonomicNervousSystemModel(this);

                // instantiate the spontaneous breathing model
                breathing = new SpontaneousBreathingModel(this);

                // instantiate the acidbase model
                ab = new AcidBaseModel(this);

                // instantiate the oxygenation model
                oxy = new OxygenationModel(this);

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

        public void CalculateModel(int duration)
        {

        }

        void ModelCycle()
        {

        }

    }


}
