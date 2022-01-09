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
    public partial class AddControl : Window
    {
        public AddControl()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddControlItem addControlItem;

            List<AddControlItem> addControlItems = new List<AddControlItem>();

            addControlItem = new AddControlItem();
            addControlItem.Name = "ShortCutControl";
            addControlItem.Type = "ArtSoftDesktop.ShortCutControl";
            addControlItems.Add(addControlItem);

            addControlItem = new AddControlItem();
            addControlItem.Name = "FrameControl";
            addControlItem.Type = "ArtSoftDesktop.FrameControl";
            addControlItems.Add(addControlItem);

            dgAddControl.ItemsSource = addControlItems;
            dgAddControl.Items.Refresh();
            dgAddControl.UpdateLayout();

            dgAddControl.SelectedIndex = 0;
        }

        public Type Control;

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(dgAddControl.SelectedItem != null)
            {
                AddControlItem item = (AddControlItem)dgAddControl.SelectedItem;
                var type = Type.GetType(item.Type);
                Control = type;
                this.Close();
            }
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class AddControlItem
    {
        public string Name { set; get; }
        public string Type { set; get; }
    }
}
