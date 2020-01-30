using System;
using System.Reflection;
using NeonatalPhysiologyEngine;

namespace NeonatalPhysiologyCommandLine
{
    class Program
    {
        static Model test_model = new Model();

        static void Main(string[] args)
        {

            LoadModel();

            test_model.CalculateModel(60);

 

        }
        static void LoadModel(string filename = "" )
        {
            if (filename == "" )
            {
                // load embedded file
                string json_file = ProcessEmbeddedJSON();

                if (json_file == null)
                {
                    Console.WriteLine("Failed to process embedded JSON file.");
                }
                else
                {
                    test_model.LoadModelFromJSON(json_file);
                }

            } else
            {
                test_model.LoadModelFromDisk(filename);
            }
        }
        static string ProcessEmbeddedJSON()
        {
            try
            {
                Assembly _assembly = Assembly.GetExecutingAssembly();

                using (var reader = new System.IO.StreamReader(_assembly.GetManifestResourceStream("NeonatalPhysiologyCommandLine.normal_neonate.json")))
                {
                    return reader.ReadToEnd();
                };
            } catch
            {
               
            }
            return null;

        }

    }
}
