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

        Dictionary<string, bool> actionKeys = new Dictionary<string, bool>();

        int step = 8;

        String[] spriteFilenames = new String[4];

        UserControl objectsField;
        public BattleControl()
        {

            InitializeComponent();
            InitializeBattleField();
            InitializeBattleObjects();
            InitializeKeys();

            players.Add(new PlayerInstance(new Image()));
            players.Add(new PlayerInstance(new Image()));
            players.Add(new PlayerInstance(new Image()));
            players.Add(new PlayerInstance(new Image()));

            

            players[0].Sprite = CreateObjectImage(@"\Images\Objects\admin.gif", 45, 45);
            players[2].Sprite = CreateObjectImage(@"\Images\Objects\DD2_Warrior_Sprite.png", 45, 45);
            players[1].Sprite = CreateObjectImage(@"\Images\Objects\admin.gif", 45, 45);
            players[3].Sprite = CreateObjectImage(@"\Images\Objects\DD2_Warrior_Sprite.png", 45, 45);

            players[0].Lightning = CreateObjectImage(@"\Images\Skills\lightning.png", 500, 1200);
            players[0].Lightning.Clip = new RectangleGeometry { Rect = new Rect(0, 5, 80, 1200) };
            //BitmapSource bs = BitmapSource.Create()
            players[0].IceBarrier = CreateObjectImage(@"C:\Users\Anton\Source\GitRepos\Caesarium_Main\Caesarium\CaesariumClient\Images\Skills\ice_barrier.png", 200, 350);
            players[0].IceBarrier.Source = new CroppedBitmap(players[0].IceBarrier.Source as BitmapSource, new Int32Rect(275, 0, 130, 90));
            //players[0].IceBarrier.Clip = new RectangleGeometry { Rect = new Rect(370, 0, 350, 230) };

            //player.Stretch = Stretch.None;

            AddBattleObject(0, 0, players[0].Sprite);
            AddBattleObject(0, 0, players[1].Sprite);
            AddBattleObject(0, 0, players[2].Sprite);
            AddBattleObject(0, 0, players[3].Sprite);


        }

        private void InitializeKeys()
        {
            actionKeys.Add("A", false);
            actionKeys.Add("S", false);
            actionKeys.Add("D", false);
            actionKeys.Add("W", false);
            actionKeys.Add("I", false);
            actionKeys.Add("J", false);
            actionKeys.Add("K", false);
            actionKeys.Add("L", false);

            actionKeys.Add("Q", false);
            actionKeys.Add("E", false);
            actionKeys.Add("U", false);
            actionKeys.Add("O", false);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MakeMove();
        }

        private void battleFieldGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Timer timer = new Timer(MakeAsyncMove, null, 0, 100);
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(MakeAction);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
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
            if (actionKeys.ContainsKey(e.Key.ToString()))
                actionKeys[e.Key.ToString()] = false;
        }

        int angle = 0;
        public void contentControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (actionKeys.ContainsKey(e.Key.ToString()))
                actionKeys[e.Key.ToString()] = true;

            switch (e.Key)
            {
                case Key.Q:
                    RemoveBattleObject(players[0].Lightning);
                    RemoveBattleObject(players[0].IceBarrier);
                    players[0].Lightning.RenderTransform = new RotateTransform(angle);
                    AddBattleObject(players[0].x + 18, players[0].y + 24, players[0].Lightning);
                    angle += 10;
                    break;
                case Key.E:
                    RemoveBattleObject(players[0].Lightning);
                    RemoveBattleObject(players[0].IceBarrier);
                    //players[0].Lightning.RenderTransform = new RotateTransform(angle);
                    AddBattleObject(players[0].x - 120, players[0].y - 50, players[0].IceBarrier);
                    //angle -= 10;
                    break;
            }
        }

        private void MakeAction(object sender, EventArgs e)
        {
            RefreshFieldObjects();

            StringBuilder makeMovesSb = new StringBuilder("");
            foreach (var key in actionKeys)
            {
                if (key.Value) makeMovesSb.Append(key.Key);
            }

            if (makeMovesSb.Length < 1) return;

            makeMovesSb.Insert(0, "action:");
            makeMovesSb.Append(";");
            //makeMovesQuery = "MakeMove:" + makeMovesQuery;
            byte[] data = Encoding.Unicode.GetBytes(makeMovesSb.ToString());
            ServerConnect.stream.Write(data, 0, data.Length);
        }

        private void RefreshFieldObjects()
        {
            byte[] data = Encoding.Unicode.GetBytes("getObj:0;");
            ServerConnect.stream.Write(data, 0, data.Length);

            string positions = ReadServerAnswer().Trim();
            if (positions.Length > 0)
            {
                var posData = positions.Split(new char[] { ':', '/' }, StringSplitOptions.RemoveEmptyEntries);

                var i = 0;
                foreach (var player in players)
                {
                    if (posData.Length <= i) break;
                    player.x = int.Parse(posData[i]);
                    player.y = int.Parse(posData[i + 1]);
                    MoveBattleObject(player.x, player.y, player.Sprite);
                    i += 2;
                }
            }
        }

        private string ReadServerAnswer()
        {
            byte[] data = new byte[64];
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
