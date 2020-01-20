using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;


namespace NeonatalPhysiologyEngine.IO
{
    public static class JSONIO
    {
        public static ModelDefinition ImportPatient(string filename)
        {
        
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            try
            {
                var jsonString = File.ReadAllText(filename);
                var jsonModel = JsonSerializer.Deserialize<ModelDefinition>(jsonString, options);

                return jsonModel;

            } catch (Exception ex)
            {
                return null;
            }
            
        }

        public static bool ExportPatient(string filename, ModelDefinition currentModel)
        {

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            try
            {
                var modelJson = JsonSerializer.Serialize(currentModel, options);
                File.WriteAllText(filename, modelJson);
                return true;

            } catch (Exception ex)
            {
                return false;
            }
           

           


        }

    }
}
