using NeonatalPhysiologyEngine;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonatalPhysiologyGUI.AnimatedDiagram
{
    public interface ICompartment
    {
        double vol_current { get; set; }
        double to2 { get; set; }
    }

    public class AnimatedCompartment
    {
        // list of all compartmentn contained in this animated compartment
        public List<ICompartment> compartments = new List<ICompartment>();

        SKPaint circleOut;

        SKPaint paint;

        SKPaint textPaint;

        SKPaint textPaint2;

        public float OffsetXFactor = 2.5f;
        public SKPoint offset = new SKPoint
        {
            X = 8,
            Y = 8

        };

        public string Name { get; set; } = "X";
        public bool IsVisible { get; set; } = true;
        public bool IsVessel { get; set; } = false;

        public float ScaleRelative { get; set; } = 50;
        public float Dpi { get; set; } = 1;
        public float PositionInDegrees { get; set; } = 0;
        public float StartInDegrees { get; set; } = 0;
        public float EndInDegrees { get; set; } = 0;
        public float Direction { get; set; } = 1;
        public float Speed { get; set; } = 0.05f;
      
        public float RadiusXOffset { get; set; } = 1;
        public float RadiusYOffset { get; set; } = 1;
     

        public AnimatedCompartment(float _dpi)
        {
            Dpi = _dpi;

            circleOut = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true,
                Color = SKColors.Orange,
                StrokeWidth = 5,
            };

            paint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = false,
                Color = SKColors.Blue,
                StrokeWidth = 10
            };

            textPaint = new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName("Arial Bold"),
                Style = SKPaintStyle.Fill,
                FakeBoldText = true,
                IsAntialias = true,
                Color = SKColors.White,
                IsStroke = false,
                TextSize = 14f * Dpi
            };

            textPaint2 = new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName("Arial Bold"),
                Style = SKPaintStyle.Fill,
                FakeBoldText = true,
                IsAntialias = true,
                Color = SKColors.White,
                IsStroke = false,
                TextSize = 12f * Dpi
            };
     
        }

        public void AddCompartment(ICompartment c)
        {
            compartments.Add(c);
        }

        public void DrawCompartment(SKCanvas canvas, float _radX, float _radY)
        {
            
            float scale = _radX * ScaleRelative;
            float radius = _radX / 2.5f;

            if (_radX > _radY)
            {
                scale = _radY * ScaleRelative;
                radius = _radY / 2.5f;
            }

            // calculate the total volume and average spO2 if lumping is the case
            float totalVolume = 0;
            float totalSpO2 = 0;
            foreach (ICompartment c in compartments)
            {
                totalVolume += (float)c.vol_current;
                totalSpO2 += (float)c.to2;
            }

            paint.Color = AnimatedElementHelper.CalculateBloodColor(totalSpO2 / compartments.Count);

            float r = AnimatedElementHelper.RadiusCalculator(totalVolume, scale);

            if (IsVessel)
            {
                // calculate position
                SKPoint locationOrigen = AnimatedElementHelper.GetPosition(StartInDegrees, radius, RadiusXOffset, RadiusYOffset);
                SKPoint locationTarget = AnimatedElementHelper.GetPosition(EndInDegrees, radius, RadiusXOffset, RadiusYOffset);

                SKRect mainRect = new SKRect(0, 0, 0, 0)
                {
                    Left = (float)Math.Sin(270 * 0.0174532925) * RadiusXOffset * radius,
                    Top = (float)Math.Cos(180 * 0.0174532925) * RadiusYOffset * radius,
                    Right = (float)Math.Sin(90 * 0.0174532925) * RadiusXOffset * radius,
                    Bottom = (float)Math.Cos(0 * 0.0174532925) * RadiusYOffset * radius
                };

                using (SKPath path = new SKPath())
                {
                    path.AddArc(mainRect, StartInDegrees, Math.Abs(StartInDegrees - EndInDegrees) * Direction);
                    circleOut.Color = paint.Color;

                    circleOut.StrokeWidth = r;
                    canvas.DrawPath(path, circleOut);

                    offset.X = Math.Abs(locationOrigen.X - locationTarget.X) / OffsetXFactor;
                    canvas.DrawTextOnPath(Name, path, offset, textPaint2);
                }
            }
            else
            {
                // calculate position
                SKPoint location = AnimatedElementHelper.GetPosition(PositionInDegrees, radius, RadiusXOffset, RadiusYOffset);

                float twidth = textPaint.MeasureText(Name);

                paint.StrokeWidth = 10;
                canvas.DrawCircle(location, r, paint);
                canvas.DrawText(Name, location.X - twidth / 2, location.Y + 7, textPaint);
            }
        }
    }
}
