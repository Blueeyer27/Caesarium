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

            string path = Directory.GetCurrentDirectory();
            path = path.Substring(0, path.Length - 10);

            foreach (var player in players)
            {
                player.Sprite = CreateObjectImage(path + @"\Images\Objects\skin" + player.skinNumber + ".png", 45, 45);
                player.Sprite.Source = new CroppedBitmap(player.Sprite.Source as BitmapSource, new Int32Rect(0, 0, 48, 48));
                AddBattleObject(0, 0, player.Sprite);
            }

            //players[0].Sprite = CreateObjectImage(@"\Images\Objects\admin.gif", 45, 45);
            //players[2].Sprite = CreateObjectImage(@"\Images\Objects\DD2_Warrior_Sprite.png", 45, 45);
            //players[1].Sprite = CreateObjectImage(@"\Images\Objects\admin.gif", 45, 45);
            //players[3].Sprite = CreateObjectImage(@"\Images\Objects\DD2_Warrior_Sprite.png", 45, 45);

            players[0].Lightning = CreateObjectImage(@"\Images\Skills\lightning.png", 500, 1200);
            players[0].Lightning.Clip = new RectangleGeometry { Rect = new Rect(0, 5, 80, 1200) };
            //BitmapSource bs = BitmapSource.Create()
            players[0].IceBarrier = CreateObjectImage(path + @"\Images\Skills\ice_barrier.png", 200, 350);
            players[0].IceBarrier.Source = new CroppedBitmap(players[0].IceBarrier.Source as BitmapSource, new Int32Rect(275, 0, 130, 90));
            //players[0].IceBarrier.Clip = new RectangleGeometry { Rect = new Rect(370, 0, 350, 230) };

            //player.Stretch = Stretch.None;

            //AddBattleObject(0, 0, players[0].Sprite);
            //AddBattleObject(0, 0, players[1].Sprite);
            //AddBattleObject(0, 0, players[2].Sprite);
            //AddBattleObject(0, 0, players[3].Sprite);
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

            actionKeys.Add("C", false);
            actionKeys.Add("V", false);
            actionKeys.Add("N", false);
            actionKeys.Add("B", false);
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
            var key = e.Key.ToString();

            if (actionKeys.ContainsKey(key))
                actionKeys[key] = false;
        }

        public void contentControl_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key.ToString();

            if (actionKeys.ContainsKey(key))
                actionKeys[key] = true;

            switch (e.Key)
            {
                case Key.Q:

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

        private double CountAngle(string xParam, string yParam)
        {
            int x = int.Parse(xParam) / step;
            int y = int.Parse(yParam) / step * -1;

            if (x == -1)
            {
                if (y == -1) return 45;
                if (y == 0) return 90;
                if (y == 1) return 135;
            }
            if (x == 0)
            {
                if (y == -1) return 0;
                if (y == 1) return 180;
            }
            if (x == 1)
            {
                if (y == -1) return 325;
                if (y == 0) return 270;
                if (y == 1) return 225;
            }

            return 0;
        }

        private void RefreshFieldObjects()
        {
            byte[] data = Encoding.Unicode.GetBytes("getObj:0;");
            ServerConnect.stream.Write(data, 0, data.Length);
            string positions = ReadServerAnswer().Trim();
            if (positions.Length > 0)
            {
                var objectData = positions.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                var posData = objectData[0].Split(new char[] { ':', '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (posData.Length > 0)
                {
                    var i = 0;
                    foreach (var player in players)
                    {
                        if (posData.Length <= i) break;
                        var x = int.Parse(posData[i]);
                        var y = int.Parse(posData[i + 1]);

                        player.AnimateMove(x, y);
                        MoveBattleObject(player.x, player.y, player.Sprite);
                        i += 2;
                    }
                }
                if (objectData.Length > 1 && objectData[1].Length > 0)
                {
                    var LightingData = objectData[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    RemoveBattleObject(players[0].Lightning);
                    RemoveBattleObject(players[0].IceBarrier);
                    players[0].Lightning.RenderTransform = new RotateTransform(CountAngle(LightingData[3], LightingData[4]));
                    AddBattleObject(int.Parse(LightingData[1]) + 25, int.Parse(LightingData[2]) + 25, players[0].Lightning);


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

            return builder.ToString().Trim();
        }
    }
}
