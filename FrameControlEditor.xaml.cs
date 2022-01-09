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
    public partial class TestControlEditor : Window
    {
        public TestControlEditor()
        {
            InitializeComponent();
        }

        FrameControl _control;

        public bool Result = false;

        public void Init(FrameControl control)
        {
            tbTop.Text = control.Top.ToString();
            tbLeft.Text = control.Left.ToString();
            tbWidth.Text = control.Width.ToString();
            tbHeight.Text = control.Height.ToString();
            tbColor.Text = control.Color;
            _control = control;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _control.Top = double.Parse(tbTop.Text);
            _control.Left = double.Parse(tbLeft.Text);
            _control.Width = double.Parse(tbWidth.Text);
            _control.Height = double.Parse(tbHeight.Text);
            _control.Color = tbColor.Text;
            Result = true;
            this.Close();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
