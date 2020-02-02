using NeonatalPhysiologyEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NeonatalPhysiologyGUI.Helpers;
using NeonatalPhysiologyGUI.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeonatalPhysiologyGUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // animated diagram visibility
        AnimatedDiagram animatedDiagram;
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

        public void InitializeAnimatedDiagram(AnimatedDiagram p)
        {
            animatedDiagram = p;
            animatedDiagram.UpdateSkeleton();
        }


    }
}
