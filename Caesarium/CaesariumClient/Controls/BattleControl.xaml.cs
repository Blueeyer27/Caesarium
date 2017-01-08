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
        List<Skill> displayedSkills = new List<Skill>();

        Dictionary<string, bool> actionKeys = new Dictionary<string, bool>();

        int step = 8;

        String[] spriteFilenames = new String[4];

        string appPath;

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

            appPath = Directory.GetCurrentDirectory();
            appPath = appPath.Substring(0, appPath.Length - 10);

            foreach (var player in players)
            {
                player.AllSpriteStates = CreateObjectImage(appPath + @"\Images\Objects\skin" + player.skinNumber + ".png", 144, 192);
                player.DeadSprite = CreateObjectImage(appPath + @"\Images\Objects\dead.png", 48, 48);

                player.Sprite = CreateObjectImage(appPath + @"\Images\Objects\skin" + player.skinNumber + ".png", 48, 48);
                player.Sprite.Source = new CroppedBitmap(player.AllSpriteStates.Source as BitmapSource, new Int32Rect(0, 0, 48, 48));

                AddBattleObject(0, 0, player.Sprite);            
            }
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

        private void battleFieldGrid_Loaded(object sender, RoutedEventArgs e)
        {
            CreateTimer(MakeAction, 20);
            CreateTimer(CheckDisplayedSkills, 30);
        }

        private void CheckDisplayedSkills(object sender, EventArgs e)
        {
            var skillsCopy = new List<Skill>();
            foreach (var skill in displayedSkills)
            {
                if (skill.CanRemoveAnimation())
                {
                    RemoveBattleObject(skill.Sprite);
                }
                else
                {
                    skill.Animate();
                    skillsCopy.Add(skill);
                }
            }

            displayedSkills = skillsCopy;
        }

        private void CreateTimer(Action<object, EventArgs> action, int milliseconds)
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(action);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds);
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
                else if (y == 0) return 90;
                else if (y == 1) return 135;
            }
            else if (x == 0)
            {
                if (y == -1) return 0;
                else if (y == 1) return 180;
            }
            else if (x == 1)
            {
                if (y == -1) return 325;
                else if (y == 0) return 270;
                else if (y == 1) return 225;
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

                        if (!player.Dead)
                        {
                            if (x < 0 || y < 0) player.Dead = true;
                            else
                            {
                                player.AnimateMove(x, y);
                                MoveBattleObject(player.x, player.y, player.Sprite);
                            }
                        }

                        i += 2;
                    }
                }

                for (var i = 1; i < objectData.Length; i++)
                {
                    if (objectData[i].Length <= 0) continue;

                    var skill = objectData[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (skill.Length < 6) continue;

                    switch (skill[0])
                    {
                        case "light":
                            Skill lightning = new Skill(CreateObjectImage(appPath + @"\Images\Skills\lightning.png", int.Parse(skill[5]), 40), 249, 59, 6);
                            lightning.Sprite = CreateObjectImage(appPath + @"\Images\Skills\lightning.png", int.Parse(skill[5]), 40);
                            lightning.Sprite.Stretch = Stretch.Fill;
                            lightning.SetAngle(CountAngle(skill[3], skill[4]), 20);

                            displayedSkills.Add(lightning);
                            AddBattleObject(int.Parse(skill[1]), int.Parse(skill[2]) + 25, lightning.Sprite);
                            break;
                        case "barr":
                            Skill iceBarrier = new Skill(CreateObjectImage(appPath + @"\Images\Skills\ice_barrier.png", 90, 136), 90, 136, 5);
                            iceBarrier.Sprite = CreateObjectImage(appPath + @"\Images\Skills\ice_barrier.png", 200, 200);
                            iceBarrier.Sprite.Stretch = Stretch.Fill;

                            displayedSkills.Add(iceBarrier);
                            AddBattleObject(int.Parse(skill[1]) - 70, int.Parse(skill[2]) - 70, iceBarrier.Sprite);
                            break;
                    }
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
