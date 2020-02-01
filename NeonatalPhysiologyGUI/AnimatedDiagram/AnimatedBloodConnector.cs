using NeonatalPhysiologyEngine;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonatalPhysiologyGUI.AnimatedDiagram
{
    public class AnimatedBloodConnector
    {

        public List<BloodConnector> connectors = new List<BloodConnector>();
        public BloodCompartment sizeCompartment;



        SKPaint circleOut;

        SKPaint paint;

        SKPaint textPaint;

        public string Name { get; set; } = "";
        public bool IsVisible = true;
        public bool Gradient { get; set; } = false;
        public float Dpi { get; set; } = 1f;
        public float ScaleRelative { get; set; } = 18;
        public float Degrees { get; set; } = 0;
        public float StartAngle { get; set; } = 0;
        public float EndAngle { get; set; } = 0;
        public float Direction { get; set; } = 1;
        public float Speed { get; set; } = 0.05f;
        public float Width { get; set; } = 30;
        public float XOffset { get; set; } = 0;
        public float YOffset { get; set; } = 0;
        public float RadiusXOffset { get; set; } = 1;
        public float RadiusYOffset { get; set; } = 1;

        int averageCounter = 0;
        float averageFlow = 0;
        float tempAverageFlow = 0;

        float strokeWidth = 15;
        float currentStrokeWidth = 0;
        float currentAngle = 0;
        float strokeStepsize = 0.1f;

        
        public AnimatedBloodConnector(float _dpi)
        {
            Dpi = _dpi;

             circleOut = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = SKColors.Orange,
                StrokeWidth = 5,
            };


            paint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.BlanchedAlmond,

                StrokeWidth = 10
            };

            textPaint = new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName("Arial Bold"),
                FakeBoldText = true,
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = SKColors.White,
                IsStroke = false,
                TextSize = 14f * Dpi


            };

            Width = 30 * Dpi;
        }
        public void AddConnector(BloodConnector c)
        {
            connectors.Add(c);
        }


        public void DrawConnector(SKCanvas canvas, float _radX, float _radY)
        {
            float totalFlow = 0;
            float totalSpO2 = 0;
            float totalSpO2To = 0;
            float totalSpO2From = 0;
            float currentVolume = 0;
            float radius = 0;

            float scale = _radX * ScaleRelative ;
            radius = _radX / 2.5f;

            if (_radX > _radY)
            {
                scale = _radY * ScaleRelative ;
                radius = _radY / 2.5f;
            }


            // calculate the total volume and average spO2 if lumping is the case
            foreach (BloodConnector c in connectors)
            {
                totalFlow += (float)c.current_flow * Speed;
                if (totalFlow >= 0)
                {
                    totalSpO2 += (float)c.comp1.to2;
                    if (Gradient)
                    {
                        totalSpO2From += (float)c.comp1.to2;
                        totalSpO2To += (float)c.comp2.to2;
                    }
                    else
                    {
                        totalSpO2From += (float)c.comp1.to2;
                        totalSpO2To += (float)c.comp1.to2;
                    }
                }
                else
                {
                    totalSpO2 += (float)c.comp2.to2;
                    if (Gradient)
                    {
                        totalSpO2From += (float)c.comp2.to2;
                        totalSpO2To += (float)c.comp1.to2;
                    }
                    else
                    {
                        totalSpO2From += (float)c.comp2.to2;
                        totalSpO2To += (float)c.comp2.to2;
                    }

                }
            }


            tempAverageFlow += totalFlow;

            if (averageCounter > 100)
            {
                averageFlow = Math.Abs(tempAverageFlow) / averageCounter;
                tempAverageFlow = 0;
                averageCounter = 0;
            }
            averageCounter++;


            SKColor colorTo = AnimatedElementHelper.CalculateBloodColor(totalSpO2To / connectors.Count);
            SKColor colorFrom = AnimatedElementHelper.CalculateBloodColor(totalSpO2From / connectors.Count);

            currentAngle += totalFlow * Direction;
            if (Math.Abs(currentAngle) > Math.Abs(StartAngle - EndAngle))
            {
                currentAngle = 0;
            }

            if (sizeCompartment != null)
            {
                currentVolume = (float)sizeCompartment.vol_current;
                circleOut.StrokeWidth = AnimatedElementHelper.RadiusCalculator(currentVolume, scale);
            }
            else
            {
                strokeWidth = averageFlow * Width;
                if (strokeWidth > 30) strokeWidth = 30;
                if (strokeWidth < 2) strokeWidth = 2;

                strokeStepsize = (strokeWidth - currentStrokeWidth) / 10;
                currentStrokeWidth += strokeStepsize;
                if (Math.Abs(currentStrokeWidth - strokeWidth) < Math.Abs(strokeStepsize))
                {
                    strokeStepsize = 0;
                    currentStrokeWidth = strokeWidth;
                }

                circleOut.StrokeWidth = currentStrokeWidth;
            }


            // calculate position
            SKRect mainRect = new SKRect(0, 0, 0, 0)
            {
                Left = (float)Math.Sin(270 * 0.0174532925) * RadiusXOffset * radius + XOffset,
                Right = (float)Math.Sin(90 * 0.0174532925) * RadiusXOffset * radius + XOffset,
                Top = (float)Math.Cos(180 * 0.0174532925) * RadiusYOffset * radius + YOffset,
                Bottom = (float)Math.Cos(0 * 0.0174532925) * RadiusYOffset * radius + YOffset
            };

            circleOut.Shader = SKShader.CreateSweepGradient(
                new SKPoint(0f, 0f),
                new SKColor[] { colorFrom, colorTo },
                new float[] { StartAngle / 360f, EndAngle / 360f }
            );

            using (SKPath path = new SKPath())
            {
                path.AddArc(mainRect, StartAngle, Math.Abs(StartAngle - EndAngle));
                canvas.DrawPath(path, circleOut);
            }

            SKPoint location1 = AnimatedElementHelper.GetPosition(StartAngle + currentAngle, radius, RadiusXOffset, RadiusYOffset);
            canvas.DrawCircle(location1.X + XOffset, location1.Y + YOffset, 7, paint);





        }






    }
}
