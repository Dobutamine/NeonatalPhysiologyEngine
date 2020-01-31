using NeonatalPhysiologyEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NeonatalPhysiologyGUI.ViewModels
{
    public class MainWindowViewModel
    {
        Model currentModel;

        public MainWindowViewModel()
        {
            currentModel = new Model();

            LoadModel();

            currentModel.StartRealTimeModel();
        }

        public void LoadModel(string filename = "")
        {
            if (filename == "")
            {
                // load embedded file
                string json_file = ProcessEmbeddedJSON();

                if (json_file == null)
                {
                    Console.WriteLine("Failed to process embedded JSON file.");
                }
                else
                {
                    currentModel.LoadModelFromJSON(json_file);
                }

            }
            else
            {
                currentModel.LoadModelFromDisk(filename);
            }
        }
        
        string ProcessEmbeddedJSON()
        {
            try
            {
                Assembly _assembly = Assembly.GetExecutingAssembly();

                using (var reader = new System.IO.StreamReader(_assembly.GetManifestResourceStream("NeonatalPhysiologyGUI.normal_neonate.json")))
                {
                    return reader.ReadToEnd();
                };
            }
            catch
            {

            }
            return null;

        }
    }
}
