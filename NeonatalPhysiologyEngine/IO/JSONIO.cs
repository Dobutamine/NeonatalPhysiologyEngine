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

            var jsonString = File.ReadAllText(filename);
            var jsonModel = JsonSerializer.Deserialize<ModelDefinition>(jsonString, options);

            return jsonModel;
            
            
        }
    }
}
