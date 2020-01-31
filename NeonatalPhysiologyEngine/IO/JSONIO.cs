using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace NeonatalPhysiologyEngine.IO
{
    public static class JSONIO
    {
        public static ModelDefinition ImportPatientFromFileOnDisk(string filename)
        {
        
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            try
            {
             

                var jsonString = System.IO.File.ReadAllText(filename);
                var jsonModel = JsonSerializer.Deserialize<ModelDefinition>(jsonString, options);

                return jsonModel;

            } catch
            {
                return null;
            }
            
        }

        public static ModelDefinition ImportPatientFromText(string config_json)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            try
            {
                var jsonModel = JsonSerializer.Deserialize<ModelDefinition>(config_json, options);

                return jsonModel;

            }
            catch
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
                System.IO.File.WriteAllText(filename, modelJson);
                return true;

            } catch
            {
                return false;
            }
           

           


        }

    }
}
