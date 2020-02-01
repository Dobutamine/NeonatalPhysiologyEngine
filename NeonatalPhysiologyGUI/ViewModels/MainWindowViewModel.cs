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
            // instantiate a new
            currentModel = new Model();

            currentModel.modelInterface.PropertyChanged += ModelInterface_PropertyChanged;

            currentModel.LoadModelFromJSON(JSONHelpers.ProcessEmbeddedJSON("NeonatalPhysiologyGUI.JSON.NormalNeonate.json"));

        }

        private void ModelInterface_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StatusMessage")
            {

            }
        }
    }
}
