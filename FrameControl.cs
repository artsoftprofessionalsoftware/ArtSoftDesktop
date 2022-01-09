using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArtSoftDesktop
{
    public class FrameControl: Control
    {
        public double Top
        {
            set
            {
                Canvas.SetTop(this, value);
            }

            get
            {
                return Canvas.GetTop(this);
            }
        }

        public double Left
        {
            set
            {
                Canvas.SetLeft(this, value);
            }

            get
            {
                return Canvas.GetLeft(this);
            }
        }

        string _color;
        
        public string Color
        {
            set
            {
                _color = value;
                this.InvalidateVisual();
            }

            get
            {
                return _color;
            }
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(this.Color);
            Pen pen = new Pen(brush, 1);
            Rect rect = new Rect(0, 0, this.Width, this.Height);
            drawingContext.DrawRectangle(brush, pen, rect);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        //protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    base.OnMouseRightButtonDown(e);
        //    MessageBox.Show("Right button clicked");
        //}
    }
}
