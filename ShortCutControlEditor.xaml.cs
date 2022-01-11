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
    public partial class ShortCutControlEditor : Window
    {
        public ShortCutControlEditor()
        {
            InitializeComponent();
        }

        ShortCutControl _control;

        public bool Result = false;

        public void Init(ShortCutControl control)
        {
            tbTop.Text = control.Top.ToString();
            tbLeft.Text = control.Left.ToString();
            tbFile.Text = control.File;
            tbTitle.Text = control.Title;
            _control = control;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                _control.Top = double.Parse(tbTop.Text);
                _control.Left = double.Parse(tbLeft.Text);
                _control.File = tbFile.Text;
                _control.Title = tbTitle.Text;
                Result = true;
                this.Close();
            }
        }

        private bool Validate()
        {
            if(tbFile.Text == null || tbFile.Text == "")
            {
                MessageBox.Show("File can't be empty", "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (tbTitle.Text == null || tbTitle.Text == "")
            {
                MessageBox.Show("Name can't be empty", "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
