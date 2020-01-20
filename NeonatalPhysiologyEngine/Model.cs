using NeonatalPhysiologyEngine.IO;
using System;

namespace NeonatalPhysiologyEngine
{
    public class Model
    {
        public ModelDefinition modelDefinition;
        public ModelInterface modelInterface;

        public Model()
        {
            // instantiate a model interface for this model instance which takes care of all communication with the outside world
            modelInterface = new ModelInterface(this);
            modelInterface.StatusMessage = $"Neonatal Physiology Engine version 1.0. {Environment.NewLine}";
        }

        public void LoadModel(string filename)
        {
            modelDefinition = JSONIO.ImportPatient(filename);
            modelInterface.StatusMessage = $"Imported patient {filename}. {Environment.NewLine}";
        }

        public void SaveModel(string filename)
        {

        }

        public void InitModel()
        {
            
            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing blood compartments.{Environment.NewLine}";
            foreach(BloodCompartment bloodComp in modelDefinition.blood_compartments)
            {
                bloodComp.InitBloodCompartment(this);
            }

            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing blood compartment connectors.{Environment.NewLine}";
            foreach (BloodConnector bloodCon in modelDefinition.blood_connectors)
            {
                bloodCon.InitBloodConnector(this);
            }

            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing valves.{Environment.NewLine}";
            foreach (Valve valve in modelDefinition.valves)
            {
                valve.InitValve(this);
            }

            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing shunts.{Environment.NewLine}";
            foreach (Shunt shunt in modelDefinition.shunts)
            {
                shunt.InitShunt(this);
            }

            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing gas compartments.{Environment.NewLine}";
            foreach (GasCompartment gasComp in modelDefinition.gas_compartments)
            {
                gasComp.InitGasCompartment(this);
            }

            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing gas compartment connectors.{Environment.NewLine}";
            foreach (GasConnector gasCon in modelDefinition.gas_connectors)
            {
                gasCon.InitGasConnector(this);
            }

            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing gasexchangers.{Environment.NewLine}";
            foreach (GasExchanger gasExchanger in modelDefinition.exchangers)
            {
                gasExchanger.InitializeExchanger(this);
            }

            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing diffusors.{Environment.NewLine}";
            foreach (Diffusor diffusor in modelDefinition.diffusors)
            {
                diffusor.InitializeDiffusor(this);
            }

            modelInterface.StatusMessage = $"{Environment.NewLine}Initializing containers.{Environment.NewLine}";
            foreach (Container container in modelDefinition.containers)
            {
                container.InitContainer(this);
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
