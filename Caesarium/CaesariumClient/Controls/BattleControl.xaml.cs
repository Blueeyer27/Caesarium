using CaesariumClient.Controls.Battle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CaesariumClient.Controls
{
    /// <summary>
    /// Interaction logic for BattleControl.xaml
    /// </summary>
    public partial class BattleControl : UserControl
    {
        List<PlayerInstance> players = new List<PlayerInstance>();

        Dictionary<string, bool> moveKeys = new Dictionary<string, bool>();

        int step = 8;

        UserControl objectsField;
        public BattleControl()
        {
            InitializeComponent();
            InitializeBattleField();
            InitializeBattleObjects();

            players.Add(new PlayerInstance(new Image()));
            players.Add(new PlayerInstance(new Image()));

            moveKeys.Add("A", false);
            moveKeys.Add("S", false);
            moveKeys.Add("D", false);
            moveKeys.Add("W", false);
            moveKeys.Add("I", false);
            moveKeys.Add("J", false);
            moveKeys.Add("K", false);
            moveKeys.Add("L", false);

            players[0].Sprite = CreateObjectImage(@"\Images\Objects\admin.gif", 45, 45);
            players[1].Sprite = CreateObjectImage(@"\Images\Objects\DD2_Warrior_Sprite.png", 45, 45);

            //player.Stretch = Stretch.None;

            AddBattleObject(0, 0, players[0].Sprite);
            AddBattleObject(0, 0, players[1].Sprite);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
            //MakeMove();
        }

        private void battleFieldGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Timer timer = new Timer(MakeAsyncMove, null, 0, 100);
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(MakeMove);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();
        }

        private Image CreateObjectImage(string uri, int height = -1, int width = -1)
        {
            var dir = Directory.GetCurrentDirectory();
            Image Mole = new Image();
            ImageSource MoleImage = new BitmapImage(new Uri(uri, UriKind.Relative));

            Mole.Source = MoleImage;
            if (height > 0) Mole.Height = height;
            if (width > 0) Mole.Width = width;

            Mole.HorizontalAlignment = HorizontalAlignment.Left;
            Mole.VerticalAlignment = VerticalAlignment.Top;

            return Mole;
        }

        private void contentControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += contentControl_KeyDown;
            window.KeyUp += contentControl_KeyUp;
        }

        private void contentControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (moveKeys.ContainsKey(e.Key.ToString()))
                moveKeys[e.Key.ToString()] = false;
        }

        public void contentControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (moveKeys.ContainsKey(e.Key.ToString()))
                moveKeys[e.Key.ToString()] = true;
        }

        private string makeMovesQuery;
        private void MakeMove(object sender, EventArgs e)
        {
            RefreshFieldObjects();

            makeMovesQuery = "";
            foreach (var key in moveKeys)
            {
                if (key.Value) makeMovesQuery += key.Key;
            }

            if (makeMovesQuery.Length < 1) return;

            makeMovesQuery = "MakeMove:" + makeMovesQuery;
            byte[] data = Encoding.Unicode.GetBytes(makeMovesQuery);
            ServerConnect.stream.Write(data, 0, data.Length);
            makeMovesQuery = "";
        }

        private void RefreshFieldObjects()
        {
            //TODO: getFieldObjects:0 !!!
            byte[] data = Encoding.Unicode.GetBytes("getFieldObjects:0");
            ServerConnect.stream.Write(data, 0, data.Length);

            //TODO:
            string positions = ReadServerAnswer();
            if (positions.Length > 0)
            {
                var posData = positions.Split(new char[] { ':', '{', '}' }, StringSplitOptions.RemoveEmptyEntries);

                var player = players[int.Parse(posData[0])];
                MoveBattleObject(int.Parse(posData[1]), int.Parse(posData[2]), player.Sprite);

                player = players[int.Parse(posData[3])];
                MoveBattleObject(int.Parse(posData[4]), int.Parse(posData[5]), player.Sprite);
            }
        }

        private string ReadServerAnswer()
        {
            byte[] data = new byte[512];
            StringBuilder builder = new StringBuilder("");
            int bytes = 0;

            while (ServerConnect.stream.DataAvailable)
            {
                bytes = ServerConnect.stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } 

            return builder.ToString();
        }
    }
}
