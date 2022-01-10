using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArtSoftDesktop
{
    public partial class DesktopEditor : Window
    {
        public DesktopEditor()
        {
            InitializeComponent();
        }

        public bool Result = false;

        public string Name
        {
            set
            {
                tbName.Text = value;
            }

            get
            {
                return tbName.Text;
            }
        }

        public string Color
        {
            set
            {
                tbColor.Text = value;
            }

            get
            {
                return tbColor.Text;
            }
        }

        public double? Left
        {
            set
            {
                tbLeft.Text = value.ToString();
            }

            get
            {
                if(tbLeft.Text != null && tbLeft.Text != "")
                {
                    return double.Parse(tbLeft.Text);
                }
                else
                {
                    return null;
                }
            }
        }

        public double? Top
        {
            set
            {
                tbTop.Text = value.ToString();
            }

            get
            {
                if (tbTop.Text != null && tbTop.Text != "")
                {
                    return double.Parse(tbTop.Text);
                }
                else
                {
                    return null;
                }
            }
        }

        public double? Width
        {
            set
            {
                tbWidth.Text = value.ToString();
            }

            get
            {
                if (tbWidth.Text != null && tbWidth.Text != "")
                {
                    return double.Parse(tbWidth.Text);
                }
                else
                {
                    return null;
                }
            }
        }

        public double? Height
        {
            set
            {
                tbHeight.Text = value.ToString();
            }

            get
            {
                if (tbHeight.Text != null && tbHeight.Text != "")
                {
                    return double.Parse(tbHeight.Text);
                }
                else
                {
                    return null;
                }
            }
        }

        public bool MenuVisible
        {
            set
            {
                cbMenuVisible.IsChecked = value;
            }

            get
            {
                return (bool)cbMenuVisible.IsChecked;
            }
        }

        public bool AutoClose
        {
            set
            {
                cbAutoClose.IsChecked = value;
            }

            get
            {
                return (bool)cbAutoClose.IsChecked;
            }
        }
        
        private bool Validate()
        {
            if (tbName.Text == null || tbName.Text == "")
            {
                MessageBox.Show("Name can't be empty", "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (tbColor.Text == null || tbColor.Text == "")
            {
                MessageBox.Show("Color can't be empty", "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            bool IsColorValid = false;
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(tbColor.Text);
                IsColorValid = true;
            }
            catch(Exception ex)
            {
            }

            if(!IsColorValid)
            {
                MessageBox.Show("Color is not proper", "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                Result = true;
                this.Close();
            }
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
