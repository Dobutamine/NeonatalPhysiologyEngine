using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeonatalPhysiologyGUI.Helpers;
using System.Text.Json;
using NeonatalPhysiologyGUI.AnimatedDiagramComponents;

namespace NeonatalPhysiologyGUI.Views
{
    /// <summary>
    /// Interaction logic for AnimatedDiagram.xaml
    /// </summary>
    public partial class AnimatedDiagram : UserControl
    {
        SKSurface mainSurface;
        SKSurface skeletonSurface;

        SKCanvas mainCanvas;
        SKCanvas skeletonCanvas;

        AnimatedDiagramDefinition animatedDiagramDefinition;


        readonly SKPaint skeletonPaint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = false,
            Color = SKColors.DarkGray,
            StrokeWidth = 10
        };

        bool initialized = false;

        public AnimatedDiagram()
        {
            InitializeComponent();

            initialized = true;

            string definition = LoadDiagramJSON();

            animatedDiagramDefinition = ImportAnimatedDiagramFromText(definition);

        }

        public void UpdateSkeleton()
        {
            canvasSkeleton.InvalidateVisual();
        }

        public void UpdateMainDiagram()
        {
            canvasMain.InvalidateVisual();
        }

        private void canvasSkeleton_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            skeletonSurface = e.Surface;
            skeletonCanvas = skeletonSurface.Canvas;

            if (initialized)
            {
                DrawSkeletonDiagram(skeletonCanvas, canvasSkeleton.CanvasSize.Width, canvasSkeleton.CanvasSize.Height);
            }
        }

        private void canvasMain_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            mainSurface = e.Surface;
            mainCanvas = mainSurface.Canvas;

            if (initialized)
            {
                DrawMainDiagram(mainCanvas, canvasMain.CanvasSize.Width, canvasMain.CanvasSize.Height);
            }
        }

        private void DrawMainDiagram(SKCanvas _canvas, float _width, float _height)
        {
            _canvas.Clear(SKColors.Transparent);
            _canvas.Translate(_width / 2, _height / 2);
        }

        private void DrawSkeletonDiagram(SKCanvas _canvas, float _width, float _height)
        {
            _canvas.Clear(SKColors.Transparent);
            _canvas.Translate(_width / 2f, _height / 2f);

            // draw main circle
            SKPoint circleLocation = new SKPoint(0, 0);
            float _radius = _width / 2.5f;
            if (_width > _height)
            {
                _radius = _height / 2.5f;
            }
            _canvas.DrawCircle(circleLocation, _radius, skeletonPaint);

        }

        public string LoadDiagramJSON()
        {
            string definition = JSONHelpers.ProcessEmbeddedJSON("NeonatalPhysiologyGUI.JSON.AnimatedDiagramLayout.json");

            return definition;

        }


        AnimatedDiagramDefinition ImportAnimatedDiagramFromText(string config_json)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            try
            {
                var jsonModel = JsonSerializer.Deserialize<AnimatedDiagramDefinition>(config_json, options);

                return jsonModel;

            }
            catch
            {
                return null;
            }

        }
    }



}
