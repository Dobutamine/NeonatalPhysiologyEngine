using System;
using System.Collections.Generic;
using System.Text;
using NeonatalPhysiologyEngine;


namespace NeonatalPhysiologyGUI.ViewModels
{
    public class MainViewModel
    {
        double screenx = 1280;
        double screeny = 800;
        double dpi = 1.5;

        Model model;

        public MainViewModel(double _screenx, double _screeny, double _dpi_scale)
        {
            screenx = _screenx;
            screeny = _screeny;
            dpi = _dpi_scale;

            model = new Model();

        }
    }
}
