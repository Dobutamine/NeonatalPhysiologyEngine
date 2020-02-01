using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NeonatalPhysiologyGUI.Helpers
{
    public static class JSONHelpers
    {
        public static string ProcessEmbeddedJSON(string jsonName)
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
    }
}
