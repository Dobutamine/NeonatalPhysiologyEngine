using NeonatalPhysiologyEngine.IO;
using System;

namespace NeonatalPhysiologyEngine
{
    public class Model
    {
        public ModelDefinition modelDefinition;

        public Model()
        {
            Console.WriteLine("Neonatal Physiology Engine version 1.0.");
            Console.WriteLine("");
        }

        public void LoadModel(string filename)
        {
            modelDefinition = JSONIO.ImportPatient(filename);
            Console.WriteLine("Imported patient {0} ", filename);
        }

        public void SaveModel(string filename)
        {

        }

        public void InitModel()
        {
            Console.WriteLine("");
            Console.WriteLine("Initializing blood compartments.");
            Console.WriteLine("");
            foreach(BloodCompartment bloodComp in modelDefinition.blood_compartments)
            {
                bloodComp.InitBloodCompartment(this);
            }
            
            Console.WriteLine("");
            Console.WriteLine("Initializing blood compartment connectors.");
            Console.WriteLine("");
            foreach (BloodConnector bloodCon in modelDefinition.blood_connectors)
            {
                bloodCon.InitBloodConnector(this);
            }
            
            Console.WriteLine("");
            Console.WriteLine("Initializing valves.");
            Console.WriteLine("");
            foreach (Valve valve in modelDefinition.valves)
            {
                valve.InitValve(this);
            }
            
            Console.WriteLine("");
            Console.WriteLine("Initializing shunts.");
            Console.WriteLine("");
            foreach (Shunt shunt in modelDefinition.shunts)
            {
                shunt.InitShunt(this);
            }

            Console.WriteLine("");
            Console.WriteLine("Initializing gas compartments.");
            Console.WriteLine("");
            foreach (GasCompartment gasComp in modelDefinition.gas_compartments)
            {
                gasComp.InitGasCompartment(this);
            }

            Console.WriteLine("");
            Console.WriteLine("Initializing gas compartment connectors.");
            Console.WriteLine("");
            foreach (GasConnector gasCon in modelDefinition.gas_connectors)
            {
                gasCon.InitGasConnector(this);
            }

            Console.WriteLine("");
            Console.WriteLine("Initializing gasexchangers.");
            Console.WriteLine("");
            foreach (GasExchanger gasExchanger in modelDefinition.exchangers)
            {
                gasExchanger.InitializeExchanger(this);
            }

            Console.WriteLine("");
            Console.WriteLine("Initializing diffusors.");
            Console.WriteLine("");
            foreach (Diffusor diffusor in modelDefinition.diffusors)
            {
                diffusor.InitializeDiffusor(this);
            }

            Console.WriteLine("");
            Console.WriteLine("Initializing containers.");
            Console.WriteLine("");
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
