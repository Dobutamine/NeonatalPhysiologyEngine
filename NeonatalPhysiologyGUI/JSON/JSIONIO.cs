using NeonatalPhysiologyGUI.AnimatedDiagramComponents;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace NeonatalPhysiologyGUI.Helpers
{
    public static class JSIONIO
    {
        public static string GetEmbeddedJSON(string jsonName)
        {
            try
            {
                Assembly _assembly = Assembly.GetExecutingAssembly();

                using (var reader = new System.IO.StreamReader(_assembly.GetManifestResourceStream(jsonName)))
                {
                    return reader.ReadToEnd();
                };
            }
            catch
            {

            }
            return null;

        }

        public static AnimatedDiagramDefinition LoadAnimatedDiagramDefinitionFromJSON(string json_file)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var definition = JsonSerializer.Deserialize<AnimatedDiagramDefinition>(json_file, options);

            return definition;

        }

        public static AnimatedDiagramDefinition LoadAnimatedDiagramDefinitionFromDisk(string filename)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            try
            {
                var jsonString = System.IO.File.ReadAllText(filename);
                var definition = JsonSerializer.Deserialize<AnimatedDiagramDefinition>(jsonString, options);
                return definition;
            }
            catch
            {
                return null;
            }

        }

        public static bool ExportAnimatedDiagramDefinition(string filename, AnimatedDiagramDefinition add)
        {

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            try
            {
                var json = JsonSerializer.Serialize(add, options);
                System.IO.File.WriteAllText(filename, json);
                return true;

            }
            catch
            {
                return false;
            }





        }


    }
}
