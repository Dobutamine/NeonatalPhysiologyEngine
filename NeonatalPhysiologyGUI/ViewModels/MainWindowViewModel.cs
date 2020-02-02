using NeonatalPhysiologyEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NeonatalPhysiologyGUI.Helpers;
using NeonatalPhysiologyGUI.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NeonatalPhysiologyGUI.AnimatedDiagramComponents;
using System.Text.Json;

namespace NeonatalPhysiologyGUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // animated diagram
        AnimatedDiagram animatedDiagram;
        AnimatedDiagramDefinition animatedDiagramDefinition;
        private bool diagramVisible = true;
        public bool DiagramVisible
        {
            get { return diagramVisible; }
            set { diagramVisible = value; OnPropertyChanged(); }
        }

        private bool vitalsVisible = true;
        public bool VitalsVisible
        {
            get { return vitalsVisible; }
            set { vitalsVisible = value; OnPropertyChanged(); }
        }

        private bool labVisible = true;
        public bool LabVisible
        {
            get { return labVisible; }
            set { labVisible = value; OnPropertyChanged(); }
        }

        private bool additionalVisible = false;
        public bool AdditionalVisible
        {
            get { return additionalVisible; }
            set { additionalVisible = value; OnPropertyChanged(); }
        }

        Model currentModel;

        public MainWindowViewModel(double _screen_x, double _screen_y, double _dpi)
        {
            // instantiate a new model
            currentModel = new Model();

            // add an event handler to track with the model
            currentModel.modelInterface.PropertyChanged += ModelInterface_PropertyChanged;

            // get the embedded JSON model configuration file
            string embeddedJSONModel = JSIONIO.GetEmbeddedJSON("NeonatalPhysiologyGUI.JSON.NormalNeonate.json");

            // pass the JSON file to the model to load it
            currentModel.LoadModelFromJSON(embeddedJSONModel);

            // get the embedded JSON animated model diagram definition
            string embeddedJSONAdd = JSIONIO.GetEmbeddedJSON("NeonatalPhysiologyGUI.JSON.AnimatedDiagramLayout.json");

            // load the animated diagram definition file
            animatedDiagramDefinition = JSIONIO.LoadAnimatedDiagramDefinitionFromJSON(embeddedJSONAdd);
        }

        private void ModelInterface_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StatusMessage")
            {

            }
        }

        public void InitializeAnimatedDiagram(AnimatedDiagram p)
        {
            animatedDiagram = p;
        }

       


    }
}
