using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace POEScouringRecipe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDragable = false;
        private bool isShowGridLine = false;
        private List<Tab> tabs = new List<Tab>();
        private Stash curStash = new Stash();
        private Random rd = new Random();
        private IKeyboardMouseEvents m_GlobalHook;
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 12; i++)
            {
                gridInventory.RowDefinitions.Add(new RowDefinition());
                gridInventory.ColumnDefinitions.Add(new ColumnDefinition());
            }
            tbSSID.Text = Properties.Settings.Default.ssid;

            // Mouse hook
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseClick += M_GlobalHook_MouseClick;
        }

        private void M_GlobalHook_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point p = gridInventory.PointToScreen(new Point(0d, 0d));

            int x = (e.X - (int)p.X) / (int)(gridInventory.ActualWidth / 12);
            int y = (e.Y - (int)p.Y) / (int)(gridInventory.ActualHeight / 12);

            if (x < 0 || x > 12 || y < 0 || y > 12)
                return;

            foreach (FrameworkElement c in gridInventory.Children)
            {
                Point point = (Point)c.Tag;
                if (point.Y == y && point.X == x)
                {
                    gridInventory.Children.Remove(c);
                    break;
                }

            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && isDragable)
                this.DragMove();
            //Point position = gridInventory.PointToScreen(new Point(0d, 0d));

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            //Window_Closing(null, null);
            Application.Current.Shutdown();

        }

        private void btnDragable_Click(object sender, RoutedEventArgs e)
        {
            isDragable = !isDragable;
            btnDragable.FontWeight = isDragable ? FontWeights.Bold : FontWeights.Normal;
        }

        private void tbSSID_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.ssid = tbSSID.Text;
            Properties.Settings.Default.Save();
        }

        private void btnFetch_Click(object sender, RoutedEventArgs e)
        {
            Stash s = GetStash(0);
            tabs = s.tabs;

            panelStashTab.Children.Clear();
            foreach (Tab t in tabs)
            {
                Button b = new Button();
                b.Click += StashTab_Click;
                b.Content = t.n;
                b.DataContext = t.i;
                b.Background = new SolidColorBrush(Color.FromRgb((byte)t.colour.r, (byte)t.colour.g, (byte)t.colour.b));
                b.Style = (Style)gridControl.Resources["style"];
                panelStashTab.Children.Add(b);
            }
            tbStatus.Text = $"Fetched {s.tabs.Count} stashs!";

        }

        private void StashTab_Click(object sender, RoutedEventArgs e)
        {
            curStash = GetStash(int.Parse((e.Source as Button).DataContext.ToString()));

            foreach (Control c in panelStashTab.Children)
            {
                c.FontWeight = FontWeights.Normal;
            }
            (e.Source as Button).FontWeight = FontWeights.Bold;
            tbStatus.Text = $"Loaded {curStash.items.Count} items!";
        }

        private Stash GetStash(int index)
        {
            HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("https://www.pathofexile.com/character-window/get-stash-items?league=metamorph&tabs=1&tabIndex=" + index + "&accountName=supernovalx6");
            rq.CookieContainer = new CookieContainer();
            rq.CookieContainer.Add(new Cookie("POESESSID", tbSSID.Text, "/", "pathofexile.com"));

            WebResponse resp =  rq.GetResponse();
            Stream receiveStream = resp.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string cont = readStream.ReadToEnd();

            return JsonConvert.DeserializeObject<Stash>(cont);
        }

        private void btnGridLine_Click(object sender, RoutedEventArgs e)
        {
            isShowGridLine = !isShowGridLine;
            gridInventory.ShowGridLines = isShowGridLine;
            btnGridLine.FontWeight = isShowGridLine ? FontWeights.Bold : FontWeights.Normal;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            this.Top = Properties.Settings.Default.t;
            this.Left = Properties.Settings.Default.l;
            this.Height = Properties.Settings.Default.h;
            this.Width = Properties.Settings.Default.w;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.t = this.Top;
            Properties.Settings.Default.l = this.Left;
            Properties.Settings.Default.h = this.Height;
            Properties.Settings.Default.w = this.Width;

            Properties.Settings.Default.Save();

            m_GlobalHook.MouseClick -= M_GlobalHook_MouseClick;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }

        private void HighLight(int x, int y)
        {
            Rectangle r = new Rectangle();
            bool xOdd = x % 2 != 0;
            bool yOdd = y % 2 != 0;
            Color c;
            if (xOdd)
            {
                if (yOdd)
                    c = Colors.Yellow;
                else
                    c = Colors.LimeGreen;
            }
            else
            {
                if (yOdd)
                    c = Colors.LimeGreen;
                else
                    c = Colors.Yellow;
            }
            r.Stroke = new SolidColorBrush(c);
            r.StrokeThickness = 4;
            r.Tag = new Point(x, y);
            gridInventory.Children.Add(r);
            Grid.SetRow(r, y);
            Grid.SetColumn(r, x);
        }

        private void ClearAllHighlight()
        {
            gridInventory.Children.Clear();
        }

        private void btnTransmute_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            ClearAllHighlight();
            foreach (Item i in curStash.items)
            {
                if (!isMagic(i.typeLine) && i.identified)
                {
                    HighLight(i.x, i.y);
                    count++;
                }
            }
            tbStatus.Text = $"Transmute {count}/{curStash.items.Count} items!";
        }

        private void btnRegal_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            ClearAllHighlight();
            foreach (Item i in curStash.items)
            {
                if (isRegalable(i))
                {
                    HighLight(i.x, i.y);
                    count++;
                }
            }
            tbStatus.Text = $"Regal {count}/{curStash.items.Count} items!";
        }

        private bool isMagic(string name)
        {
            return name.Split(' ').Length > 2;
        }

        private bool isRegalable(Item it)
        {
            bool isRing = it.typeLine.Contains("Ring");
            bool isAmulet = it.typeLine.Contains("Amulet");
            bool hasSuffix = it.typeLine.Contains("of");

            string[] s = it.typeLine.Split(' ');

            bool hasPrefix = s[1] != (isRing ? "Ring" : "Amulet");

            return hasPrefix ^ hasSuffix;
        }

        private void btnIdentify_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            ClearAllHighlight();
            foreach (Item i in curStash.items)
            {
                if (!i.identified)
                {
                    HighLight(i.x, i.y);
                    count++;
                }
            }
            tbStatus.Text = $"Identify {count}/{curStash.items.Count} items!";
        }

        private void btnSell_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            ClearAllHighlight();
            foreach (Item i in curStash.items)
            {
                if (i.name != "")
                {
                    HighLight(i.x, i.y);
                    count++;
                }
            }
            tbStatus.Text = $"Sell {count}/{curStash.items.Count} items!";
        }
    }
}
