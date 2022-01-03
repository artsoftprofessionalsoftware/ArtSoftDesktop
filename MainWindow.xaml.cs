﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;

namespace ArtSoftDesktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Control control;

        XmlDocument prfXmlDoc;
        XmlNamespaceManager prfXmlNmSpce;
        XmlElement prfXmlElem;

        bool dirty = false;

        ProfileItem currentProfile;
        FrameworkElement currentControl;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProfiles();
            cnvMain.Focus();
        }

        void LoadProfiles()
        {
            try
            {
                //XmlReaderSettings settings = new XmlReaderSettings();
                //settings.Schemas.Add("http://www.profiles.com", "Profiles.xsd");
                //settings.ValidationType = ValidationType.Schema;

                //ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);

                //XmlReader rdr = XmlReader.Create("Profiles.xml", settings);
                prfXmlDoc = new XmlDocument();
                //xml.Load(rdr);
                prfXmlDoc.Load("Profiles.xml");

                prfXmlNmSpce = new XmlNamespaceManager(prfXmlDoc.NameTable);
                prfXmlNmSpce.AddNamespace("ps", "http://www.profiles.com");

                prfXmlElem = prfXmlDoc.DocumentElement;

                //rdr.Close();
                //rdr.Dispose();

                RefreshProfilesCombo();

                cbProfile.SelectedIndex = 0;

                XmlElement pn = (XmlElement)prfXmlElem.ChildNodes[0];
                int id = int.Parse(pn.SelectSingleNode("descendant::ps:id", prfXmlNmSpce).InnerText);
                string name = pn.SelectSingleNode("descendant::ps:name", prfXmlNmSpce).InnerText;
                ProfileItem pi = new ProfileItem();
                pi.Id = id;
                pi.Name = name;
                currentProfile = pi;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Console.WriteLine("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    Console.WriteLine("Warning {0}", e.Message);
                    break;
            }
        }

        private void RefreshProfilesCombo()
        {
            cbProfile.Items.Clear();

            foreach (XmlElement pn in prfXmlElem.ChildNodes)
            {
                int id = int.Parse(pn.SelectSingleNode("descendant::ps:id", prfXmlNmSpce).InnerText);
                string name = pn.SelectSingleNode("descendant::ps:name", prfXmlNmSpce).InnerText;
                ProfileItem pi = new ProfileItem();
                pi.Id = id;
                pi.Name = name;
                cbProfile.Items.Add(pi);
            }
        }

        private class ProfileItem
        {
            public int Id { set; get; }
            public string Name { set; get; }
        }

        private void miAddProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = prfXmlDoc.DocumentElement.NamespaceURI;

                XmlNode npn = prfXmlDoc.CreateNode(XmlNodeType.Element, "profile", uri);

                XmlNode idn = prfXmlDoc.CreateNode(XmlNodeType.Element, "id", uri);
                int nid = GetNewId();
                idn.InnerText = nid.ToString();
                npn.AppendChild(idn);

                XmlNode nmn = prfXmlDoc.CreateNode(XmlNodeType.Element, "name", uri);
                nmn.InnerText = "profile" + nid.ToString();
                npn.AppendChild(nmn);

                var doc = prfXmlDoc.DocumentElement;

                doc.AppendChild(npn);

                prfXmlDoc.Save("Profiles.xml");

                string dskFileName = "Desktop_" + nid.ToString().FillStart('0', 6) + ".xml";
                XmlDocument dskXmlDoc = new XmlDocument();
                string dskInitXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                dskInitXml += "<desktop xmlns=\"http://www.desktop.com\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">";
                dskInitXml += "</desktop>";
                dskXmlDoc.LoadXml(dskInitXml);
                dskXmlDoc.Save(dskFileName);

                RefreshProfilesCombo();

                cbProfile.SelectedIndex = cbProfile.Items.Count - 1;

                MessageBox.Show("Profile successfully added", "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void miAddControl_Click(object sender, RoutedEventArgs e)
        {
            AddControl();
        }

        private void cmiAddControl_Click(object sender, RoutedEventArgs e)
        {
            AddControl();
        }

        private void AddControl()
        {
            AddControl addControl = new AddControl();

            addControl.ShowDialog();

            if (addControl.Control == typeof(ShortCutControl))
            {
                ShortCutControl control = new ShortCutControl();
                control.Left = 20;
                control.Top = 20;
                control.Title = "";
                ShortCutControlEditor editor = new ShortCutControlEditor();
                editor.Init(control);
                editor.ShowDialog();
                if (editor.Result)
                {
                    control.Source = GetFileBitMap(control.File);
                    cnvMain.Children.Add(control);
                    ControlInit(control);
                    dirty = true;
                }
            }

            if (addControl.Control == typeof(TestControl))
            {
                TestControl control = new TestControl();
                control.Left = 20;
                control.Top = 20;
                control.Width = 100;
                control.Height = 20;
                control.Color = "gray";
                TestControlEditor editor = new TestControlEditor();
                editor.Init(control);
                editor.ShowDialog();
                if (editor.Result)
                {
                    cnvMain.Children.Add(control);
                    ControlInit(control);
                    dirty = true;
                }
            }
        }

        private void miEditControl_Click(object sender, RoutedEventArgs e)
        {
            if (currentControl != null)
            {
                EditControl(currentControl);
            }
        }

        private void EditControl(FrameworkElement control)
        {
            if (control.GetType() == typeof(TestControl))
            {
                TestControlEditor editor = new TestControlEditor();
                TestControl testControl = (TestControl)control;
                editor.Init(testControl);
                editor.ShowDialog();
                if (editor.Result)
                {
                    dirty = true;
                }
            }

            if (control.GetType() == typeof(ShortCutControl))
            {
                ShortCutControlEditor editor = new ShortCutControlEditor();
                ShortCutControl shortCutcontrol = (ShortCutControl)control;
                editor.Init(shortCutcontrol);
                editor.ShowDialog();
                if (editor.Result)
                {
                    dirty = true;
                }
            }
        }

        private void miDeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete profile", "ArtSoftDesktop", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (cbProfile.SelectedValue != null && cbProfile.SelectedIndex > 0)
                {
                    int id = int.Parse(cbProfile.SelectedValue.ToString());
                    var prf = prfXmlElem.SelectSingleNode("descendant::ps:profile[ps:id=" + id.ToString() + "]", prfXmlNmSpce);
                    prfXmlElem.RemoveChild(prf);

                    string dskFileName = "Desktop_" + id.ToString().FillStart('0', 6) + ".xml";
                    if (File.Exists(dskFileName))
                    {
                        File.Delete(dskFileName);
                    }

                    prfXmlDoc.Save("Profiles.xml");

                    RefreshProfilesCombo();

                    if (cbProfile.Items.Count > 0)
                    {
                        cbProfile.SelectedIndex = 0;
                        currentProfile = (ProfileItem)cbProfile.SelectedItem;
                    }
                }
            }
        }

        private void miDeleteControl_Click(object sender, RoutedEventArgs e)
        {
            if (currentControl != null)
            {
                DeleteControl(currentControl);
            }
        }

        private void editMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ContextMenu menu = (ContextMenu)item.Parent;
            FrameworkElement control = (FrameworkElement)menu.PlacementTarget;
            EditControl(control);
        }

        private void deleteMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ContextMenu menu = (ContextMenu)item.Parent;
            FrameworkElement control = (FrameworkElement)menu.PlacementTarget;
            DeleteControl(control);
        }

        private void DeleteControl(FrameworkElement control)
        {
            if (MessageBox.Show("Do you really want to delete control", "ArtSoftDesktop", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                cnvMain.Children.Remove(control);
                currentControl = null;
                dirty = true;
            }
        }

        private int GetNewId()
        {
            int nid = 0;

            foreach (XmlElement pn in prfXmlElem.ChildNodes)
            {
                int id = int.Parse(pn.SelectSingleNode("descendant::ps:id", prfXmlNmSpce).InnerText);
                nid = Math.Max(nid, id);
            }
            nid++;
            return nid;
        }

        private void cbProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dirty)
            {
                MessageBoxResult mbr = MessageBox.Show("Profile is changed. Do you want to save it?", "ArtSoftDesktop", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (mbr == MessageBoxResult.Cancel)
                {
                    return;
                }
                if (mbr == MessageBoxResult.Yes)
                {
                    if (!SaveProfile())
                    {
                        return;
                    }
                }
            }

            currentProfile = (ProfileItem)cbProfile.SelectedItem;
            int id = currentProfile.Id;

            LoadDesktop();

            dirty = false;
        }

        private void LoadDesktop()
        {
            try
            {
                int id = currentProfile.Id;

                var brush = new SolidColorBrush(Color.FromArgb(255, 235, 245, 255));
                cnvMain.Background = brush;

                cnvMain.Children.Clear();

                string dskFileName = "Desktop_" + id.ToString().FillStart('0', 6) + ".xml";

                XmlDocument dskXmlDoc = new XmlDocument();

                dskXmlDoc.Load(dskFileName);

                XmlElement dskXmlElem = dskXmlDoc.DocumentElement;

                foreach (XmlNode ctlNode in dskXmlElem.ChildNodes)
                {
                    string name = ctlNode.SelectSingleNode("descendant::ps:name", prfXmlNmSpce).InnerText;
                    double top = double.Parse(ctlNode.SelectSingleNode("descendant::ps:top", prfXmlNmSpce).InnerText);
                    double left = double.Parse(ctlNode.SelectSingleNode("descendant::ps:left", prfXmlNmSpce).InnerText);

                    var type = Type.GetType(name);
                    FrameworkElement control = (FrameworkElement)Activator.CreateInstance(type);
                    //control.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0, 0xFF, 0));
                    //Canvas.SetTop(control, top);
                    //Canvas.SetLeft(control, left);

                    if (control.GetType() == typeof(TestControl))
                    {
                        double width = double.Parse(ctlNode.SelectSingleNode("descendant::ps:width", prfXmlNmSpce).InnerText);
                        double height = double.Parse(ctlNode.SelectSingleNode("descendant::ps:height", prfXmlNmSpce).InnerText);
                        string color = ctlNode.SelectSingleNode("descendant::ps:color", prfXmlNmSpce).InnerText;
                        TestControl testControl = (TestControl)control;
                        testControl.Top = top;
                        testControl.Left = left;
                        testControl.Width = width;
                        testControl.Height = height;
                        testControl.Color = color;
                        cnvMain.Children.Add(control);
                    }

                    if (control.GetType() == typeof(ShortCutControl))
                    {
                        string file = ctlNode.SelectSingleNode("descendant::ps:file", prfXmlNmSpce).InnerText;
                        string title = ctlNode.SelectSingleNode("descendant::ps:title", prfXmlNmSpce).InnerText;
                        ShortCutControl shortCutControl = (ShortCutControl)control;
                        shortCutControl.Top = top;
                        shortCutControl.Left = left;
                        shortCutControl.File = file;
                        shortCutControl.Title = title;
                        shortCutControl.Source = GetFileBitMap(file);
                        cnvMain.Children.Add(control);
                    }

                    ControlInit(control);

                    canBeDragged = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ControlInit(FrameworkElement control)
        {
            control.MouseEnter += Control_OnMouseEnter;
            control.MouseDown += Control_OnMouseDown;
            control.MouseUp += Control_OnMouseUp;
            control.MouseMove += Control_OnMouseMove;

            ContextMenu contextMenu = new ContextMenu();
            MenuItem editMenuItem = new MenuItem();
            editMenuItem.Header = "Edit";
            editMenuItem.Click += editMenuItem_OnClick;
            contextMenu.Items.Add(editMenuItem);
            MenuItem deleteMenuItem = new MenuItem();
            deleteMenuItem.Header = "Delete";
            deleteMenuItem.Click += deleteMenuItem_OnClick;
            contextMenu.Items.Add(deleteMenuItem);

            control.ContextMenu = contextMenu;
        }

        private void cbEdit_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void cbEdit_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveProfile();
        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            SaveProfile();
        }

        private bool SaveProfile()
        {
            try
            {
                int id = currentProfile.Id;

                string dskFileName = "Desktop_" + id.ToString().FillStart('0', 6) + ".xml";

                XmlDocument dskXmlDoc = new XmlDocument();

                dskXmlDoc.Load(dskFileName);

                XmlElement dskXmlElem = dskXmlDoc.DocumentElement;

                List<XmlNode> nodes = new List<XmlNode>();

                foreach (XmlNode ctlNode in dskXmlElem.ChildNodes)
                {
                    nodes.Add(ctlNode);
                }

                foreach (XmlNode ctlNode in nodes)
                {
                    dskXmlElem.RemoveChild(ctlNode);
                }

                var uri = prfXmlDoc.DocumentElement.NamespaceURI;

                foreach (FrameworkElement control in cnvMain.Children)
                {
                    XmlNode cxn = dskXmlDoc.CreateNode(XmlNodeType.Element, "control", uri);

                    XmlNode npxn = dskXmlDoc.CreateNode(XmlNodeType.Element, "name", uri);
                    npxn.InnerText = control.GetType().FullName;
                    cxn.AppendChild(npxn);

                    double ctlx = Canvas.GetLeft(control);
                    double ctly = Canvas.GetTop(control);

                    XmlNode tpxn = dskXmlDoc.CreateNode(XmlNodeType.Element, "top", uri);
                    tpxn.InnerText = ctly.ToString();
                    cxn.AppendChild(tpxn);

                    XmlNode lpxn = dskXmlDoc.CreateNode(XmlNodeType.Element, "left", uri);
                    lpxn.InnerText = ctlx.ToString();
                    cxn.AppendChild(lpxn);

                    if (control.GetType() == typeof(TestControl))
                    {
                        TestControl testControl = (TestControl)control;

                        XmlNode wpxn = dskXmlDoc.CreateNode(XmlNodeType.Element, "width", uri);
                        wpxn.InnerText = testControl.Width.ToString();
                        cxn.AppendChild(wpxn);

                        XmlNode hpxn = dskXmlDoc.CreateNode(XmlNodeType.Element, "height", uri);
                        hpxn.InnerText = testControl.Height.ToString();
                        cxn.AppendChild(hpxn);

                        XmlNode cpxn = dskXmlDoc.CreateNode(XmlNodeType.Element, "color", uri);
                        cpxn.InnerText = testControl.Color.ToString();
                        cxn.AppendChild(cpxn);
                    }

                    if (control.GetType() == typeof(ShortCutControl))
                    {
                        ShortCutControl shortCutControl = (ShortCutControl)control;

                        XmlNode fxn = dskXmlDoc.CreateNode(XmlNodeType.Element, "file", uri);
                        fxn.InnerText = shortCutControl.File;
                        cxn.AppendChild(fxn);

                        XmlNode txn = dskXmlDoc.CreateNode(XmlNodeType.Element, "title", uri);
                        txn.InnerText = shortCutControl.Title;
                        cxn.AppendChild(txn);
                    }

                    dskXmlElem.AppendChild(cxn);
                }

                dskXmlDoc.Save(dskFileName);

                dirty = false;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ArtSoftDesktop", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
        }

        WindowState currentWindowState;

        private void cbFullScreen_Checked(object sender, RoutedEventArgs e)
        {
            currentWindowState = WindowState;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        private void cbFullScreen_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowState = currentWindowState;
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void cnvMain_MouseMove(object sender, MouseEventArgs e)
        {
            Point newMousePos = Mouse.GetPosition(cnvMain);

            if (newMousePos.Y <= 10 && (cbMenuAutoHide.IsChecked ?? false))
            {
                spMain.Visibility = Visibility.Visible;
            }

            if (newMousePos.Y > 10 && (cbMenuAutoHide.IsChecked ?? false))
            {
                spMain.Visibility = Visibility.Collapsed;
            }

            if (currentControl != null)
            {
                double curx = Canvas.GetLeft(currentControl);
                double cury = Canvas.GetTop(currentControl);

                if (canBeDragged && curMouseMode == MouseMode.Move)
                {
                    double xoff = newMousePos.X - curMousePos.X;
                    double yoff = newMousePos.Y - curMousePos.Y;
                    Canvas.SetLeft(currentControl, curx + xoff);
                    Canvas.SetTop(currentControl, cury + yoff);
                    dirty = true;
                }

                if (canBeDragged && curMouseMode == MouseMode.ResizeWE)
                {
                    double xoff = newMousePos.X - curMousePos.X;
                    //control.Width = curMousePos.X + xoff;
                    currentControl.Width = newMousePos.X - curx + 5;
                    dirty = true;
                }

                if (canBeDragged && curMouseMode == MouseMode.ResizeNS)
                {
                    double yoff = newMousePos.Y - curMousePos.Y;
                    //control.Height = control.Height + yoff;
                    currentControl.Height = newMousePos.Y - cury + 5;
                    dirty = true;
                }
            }

            curMousePos = newMousePos;
        }

        enum MouseMode
        {
            None,
            Move,
            ResizeNS,
            ResizeWE
        }

        bool canBeDragged = false;
        Point curMousePos;
        MouseMode curMouseMode = MouseMode.None;

        private void Control_OnMouseEnter(object sender, MouseEventArgs e)
        {
            //canBeDragged = false;
        }

        private void Control_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            canBeDragged = true;
            curMousePos = Mouse.GetPosition(cnvMain);

            FrameworkElement control = (FrameworkElement)sender;

            curMouseMode = MouseMode.None;

            if (e.ClickCount == 1)
            {
                curMouseMode = MouseMode.Move;
            }

            double clft = Canvas.GetLeft(control);
            double ctop = Canvas.GetTop(control);

            if (curMousePos.X >= clft + control.Width - 10 && curMousePos.X < clft + control.Width)
            {
                curMouseMode = MouseMode.ResizeWE;
            }

            if (curMousePos.Y >= ctop + control.Height - 10 && curMousePos.Y <= ctop + control.Height)
            {
                curMouseMode = MouseMode.ResizeNS;
            }

            currentControl = control;
        }

        private void Control_OnMouseUp(object sender, MouseEventArgs e)
        {
            canBeDragged = false;
            curMouseMode = MouseMode.None;
        }

        private void Control_OnMouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement control = (FrameworkElement)sender;

            double curx = Canvas.GetLeft(control);
            double cury = Canvas.GetTop(control);

            if (curMousePos.X >= curx + control.Width - 10 && curMousePos.X < curx + control.Width)
            {
                Mouse.SetCursor(Cursors.SizeWE);
            }

            if (curMousePos.Y >= cury + control.Height - 10 && curMousePos.Y <= cury + control.Height)
            {
                Mouse.SetCursor(Cursors.SizeNS);
            }
        }

        private void miPaste_Click(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            {
                Paste();
            }

            if (e.Key == Key.A && System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            {
                AddControl();
            }

            if (e.Key == Key.E && System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            {
                if(currentControl != null)
                {
                    EditControl(currentControl);
                }
            }

            if (e.Key == Key.Left && System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (currentControl != null)
                {
                    if (currentControl.GetType() == typeof(ShortCutControl))
                    {
                        ShortCutControl shortCutControl = (ShortCutControl)currentControl;
                        shortCutControl.Left -= 1;
                        dirty = true;
                    }
                }
            }

            if (e.Key == Key.Up && System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (currentControl != null)
                {
                    if(currentControl.GetType() == typeof(ShortCutControl))
                    {
                        ShortCutControl shortCutControl = (ShortCutControl)currentControl;
                        shortCutControl.Top -= 1;
                        dirty = true;
                    }
                }
            }

            if (e.Key == Key.Right && System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (currentControl != null)
                {
                    if (currentControl.GetType() == typeof(ShortCutControl))
                    {
                        ShortCutControl shortCutControl = (ShortCutControl)currentControl;
                        shortCutControl.Left += 1;
                        dirty = true;
                    }
                }
            }

            if (e.Key == Key.Down && System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (currentControl != null)
                {
                    if (currentControl.GetType() == typeof(ShortCutControl))
                    {
                        ShortCutControl shortCutControl = (ShortCutControl)currentControl;
                        shortCutControl.Top += 1;
                        dirty = true;
                    }
                }
            }

            if (e.Key == Key.Delete)
            {
                if (currentControl != null)
                {
                    DeleteControl(currentControl);
                }
            }
        }

        private void cmiLink_Click(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void Paste()
        {
            StringCollection fileDropList = Clipboard.GetFileDropList();
            if (fileDropList.Count > 0)
            {
                string file = fileDropList[0];
                Point position = new Point(10, 10);
                AddShortcut(file, position);
            }
        }

        private void cnvMain_Drop(object sender, DragEventArgs e)
        {
            Point position = e.GetPosition(cnvMain);
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string file = files[0];
                AddShortcut(file, position);
            }
        }

        private void AddShortcut(string file, Point position)
        {
            try
            {
                ShortCutControl control = new ShortCutControl();

                control.Top = position.Y;
                control.Left = position.X;
                control.File = file;

                string name = System.IO.Path.GetFileName(file);
                control.Title = name;

                control.Source = GetFileBitMap(file);

                control.MouseEnter += Control_OnMouseEnter;
                control.MouseDown += Control_OnMouseDown;
                control.MouseUp += Control_OnMouseUp;
                control.MouseMove += Control_OnMouseMove;

                ControlInit(control);

                cnvMain.Children.Add(control);

                dirty = true;
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to extract the icon from the binary");
            }
        }

        //private BitmapImage GetFileBitMap(string file)
        //{
        //    System.Drawing.Icon icon = (System.Drawing.Icon)null;

        //    icon = System.Drawing.Icon.ExtractAssociatedIcon(file);

        //    System.Drawing.Bitmap bitmap = icon.ToBitmap();

        //    MemoryStream memory = new MemoryStream();

        //    //bitmap.Save(@"i:\temp\image.bmp");

        //    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        //    memory.Position = 0;
        //    BitmapImage bitmapImage = new BitmapImage();
        //    bitmapImage.BeginInit();
        //    bitmapImage.StreamSource = memory;
        //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //    bitmapImage.EndInit();

        //    return bitmapImage;
        //}

        private ImageSource GetFileBitMap(string file)
        {
            System.Drawing.Icon icon = (System.Drawing.Icon)null;

            icon = System.Drawing.Icon.ExtractAssociatedIcon(file);

            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        private void miQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dirty)
            {
                MessageBoxResult mbr = MessageBox.Show("Profile is changed. Do you want to save it?", "ArtSoftDesktop", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (mbr == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                if (mbr == MessageBoxResult.Yes)
                {
                    if (!SaveProfile())
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
    }
}