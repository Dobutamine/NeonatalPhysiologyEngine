using NeonatalPhysiologyEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NeonatalPhysiologyGUI.Helpers;

namespace NeonatalPhysiologyGUI.ViewModels
{
    public class MainWindowViewModel
    {
        Model currentModel;

        public MainWindowViewModel()
        {
            currentModel = new Model();

            LoadModel();

        }

        public void LoadModel(string filename = "")
        {
            if (filename == "")
            {
                // load embedded file
                string json_file = JSONHelpers.ProcessEmbeddedJSON("NeonatalPhysiologyGUI.JSON.NormalNeonate.json");

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
        
        
    }
}
