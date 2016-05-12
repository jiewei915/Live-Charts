﻿//The MIT License(MIT)

//copyright(c) 2016 Alberto Rodriguez

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LiveCharts.Charts;

namespace LiveCharts.Wpf.Points
{
    internal class RowPointView : PointView, IRectanglePointView
    {
        public Rectangle Rectangle { get; set; }
        public CoreRectangle Data { get; set; }
        public double ZeroReference  { get; set; }

        public override void DrawOrMove(ChartPoint previousDrawn, ChartPoint current, int index, ChartCore chart)
        {
            if (IsNew)
            {
                Canvas.SetTop(Rectangle, Data.Top);
                Canvas.SetLeft(Rectangle, ZeroReference);

                Rectangle.Width = 0;
                Rectangle.Height = Data.Height;

                if (DataLabel != null)
                {
                    Canvas.SetTop(DataLabel, Data.Top);
                    Canvas.SetLeft(DataLabel, ZeroReference);
                }
            }
          
            Func<double> getY = () =>
            {
                var r = Data.Top + Data.Height / 2 - DataLabel.ActualHeight / 2;

                if (r < 0) r = 2;
                if (r + DataLabel.ActualHeight > chart.DrawMargin.Height)
                    r -= r + DataLabel.ActualHeight - chart.DrawMargin.Height + 2;

                return r;
            };

            Func<double> getX = () =>
            {
                double r;

                if (Data.Left < ZeroReference)
                {
                    r = Data.Left - DataLabel.ActualWidth - 5;
                    if (r < 0) r = Data.Left + 5;
                }
                else
                {
                    r = Data.Left + Data.Width + 5;
                    if (r + DataLabel.ActualWidth > chart.DrawMargin.Width)
                        r -= DataLabel.ActualWidth + 10;
                }

                return r;
            };

            if (chart.View.DisableAnimations)
            {
                Rectangle.Width = Data.Width;
                Rectangle.Height = Data.Height;

                Canvas.SetTop(Rectangle, Data.Top);
                Canvas.SetLeft(Rectangle, Data.Left);

                if (DataLabel != null)
                {
                    DataLabel.UpdateLayout();

                    Canvas.SetTop(DataLabel, getY());
                    Canvas.SetLeft(DataLabel, getX());
                }

                if (HoverShape != null)
                {
                    Canvas.SetTop(HoverShape, Data.Top);
                    Canvas.SetLeft(HoverShape, Data.Left);
                    HoverShape.Height = Data.Height;
                    HoverShape.Width = Data.Width;
                }

                return;
            }

            var animSpeed = chart.View.AnimationsSpeed;

            if (DataLabel != null)
            {
                DataLabel.UpdateLayout();

                DataLabel.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(getX(), animSpeed));
                DataLabel.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(getY(), animSpeed));
            }

            Canvas.SetTop(Rectangle, Data.Top);
            Rectangle.BeginAnimation(Canvas.LeftProperty,
                new DoubleAnimation(Data.Left, animSpeed));

            Rectangle.Height = Data.Height;
            Rectangle.BeginAnimation(FrameworkElement.WidthProperty,
                new DoubleAnimation(Data.Width, animSpeed));

            if (HoverShape != null)
            {
                Canvas.SetTop(HoverShape, Data.Top);
                Canvas.SetLeft(HoverShape, Data.Left);
                HoverShape.Height = Data.Height;
                HoverShape.Width = Data.Width;
            }
        }

        public override void RemoveFromView(ChartCore chart)
        {
            chart.View.RemoveFromDrawMargin(HoverShape);
            chart.View.RemoveFromDrawMargin(Rectangle);
            chart.View.RemoveFromDrawMargin(DataLabel);
        }
    }
}
