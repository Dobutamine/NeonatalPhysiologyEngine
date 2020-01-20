using System;
using NeonatalPhysiologyEngine;

namespace NeonatalPhysiologyConsole
{
    class Program
    {
        static Model TestModel;

        static void Main(string[] args)
        {
            string filepath = @"C:\Users\timan\Projects\NeonatalPhysiologyEngine\Model_definitions\normal_neonate.json";

            TestModel = new Model();
            TestModel.LoadModel(filepath);
            TestModel.InitModel();

            TestModel.modelInterface.PropertyChanged += ModelInterface_PropertyChanged;
        }

        private static void ModelInterface_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
    }
}
