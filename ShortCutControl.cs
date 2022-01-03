using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

namespace ArtSoftDesktop
{
    public class ShortCutControl : Border
    {
        Image _image;
        //Label _label;
        TextBlock _textBlock;
        StackPanel _panel;

        public ShortCutControl()
        {
            _image = new Image();
            _image.Width = 40;
            _image.Height = 40;

            _textBlock = new TextBlock();
            _textBlock.MaxWidth = 70;
            _textBlock.TextWrapping = TextWrapping.Wrap;
            _textBlock.TextAlignment = TextAlignment.Center;
            _textBlock.Margin = new Thickness(5, 0, 5, 5);

            _panel = new StackPanel();
            _panel.Children.Add(_image);
            _panel.Children.Add(_textBlock);

            this.Child = _panel;

            this.BorderThickness = new System.Windows.Thickness(2);
            this.CornerRadius = new System.Windows.CornerRadius(5);
            this.Padding = new System.Windows.Thickness(0, 8, 0, 0);
            this.MinWidth = 80;
        }

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

        public string File { set; get; }

        public string Title
        {
            set
            {
                _textBlock.Text = value;
            }

            get
            {
                return _textBlock.Text;
            }
        }

        public ImageSource Source
        {
            set
            {
                _image.Source = value;
            }

            get
            {
                return _image.Source;
            }
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            var bordBrush = new SolidColorBrush(Color.FromArgb(255, 200, 210, 255));
            this.BorderBrush = bordBrush;

            var backBrush = new SolidColorBrush(Color.FromArgb(255, 245, 250, 255));
            this.Background = backBrush;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.BorderBrush = null;
            this.Background = null;
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ClickCount == 2)
            {
                Process process = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = File;
                Process prc = Process.Start(psi);
            }
        }
    }
}
