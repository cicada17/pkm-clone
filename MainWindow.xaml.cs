using System;
using System.Collections;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Globalization;


namespace _3080Go
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();

        private BitmapImage grassSrc;
        private BitmapImage treeSrc;
        private BitmapImage waterSrc;
        private BitmapImage playerSrc;
        private BitmapImage pmSrc;
        private BitmapImage pmballSrc;
        private BitmapImage gymSrc;

        private Map WorldMap;
        private ObservableCollection<BitmapImage> mapImage;
        private Player player;
        Gym mgym;
        Gym_battle battlegame;
        catch_poke_game catchgame;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();


        public MainWindow()
        {
            InitializeComponent();

            InitGame();
        }

        private void InitGame()
        {
            grassSrc = new BitmapImage(new Uri("Resources/grass.png", UriKind.Relative));
            treeSrc = new BitmapImage(new Uri("Resources/tree.png", UriKind.Relative));
            waterSrc = new BitmapImage(new Uri("Resources/water.png", UriKind.Relative));
            playerSrc = new BitmapImage(new Uri("Resources/player.png", UriKind.Relative));
            pmSrc = new BitmapImage(new Uri("Resources/pm.png", UriKind.Relative));
            pmballSrc = new BitmapImage(new Uri("Resources/pmball.png", UriKind.Relative));
            gymSrc = new BitmapImage(new Uri("Resources/gym.png", UriKind.Relative));

            mapImage = new ObservableCollection<BitmapImage>();
            MapPanel.ItemsSource = mapImage;

            player = new Player("Amber");

            WorldMap = new Map(20, 20, 40, 40);
            DrawMap();
            tabControl.SelectedIndex = 5;
        }

        private void PlayerNameInputTBx_GotFocus(object sender, RoutedEventArgs e)
        {
            PlayerNameInputInstrtnTB.Visibility = System.Windows.Visibility.Hidden;
        }

        private void PlayerNameInputTBx_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
                PlayerNameInputInstrtnTB.Visibility = System.Windows.Visibility.Visible;
        }

        private void GameStartButton_Click(object sender, RoutedEventArgs e)
        {
            string playerName = PlayerNameInputTBx.Text;
            if (playerName != "")
            {
                player = new Player(playerName);
                tabControl.SelectedIndex = 0;
                UpdateLayout();
                MapPanel.Focus();
                DrawMap();
                MapGetButton.Content = "Move by Arrow Keys or W/A/S/D to find Pokemon";
            }
        }
        
        private void DrawMap()
        {
            int[] Coordinates = WorldMap.getable();
            if (Coordinates != null)
            {
                if (WorldMap.catch_poke() != null)
                    MapGetButton.Content = "Catch " + WorldMap.catch_poke().Name;
                else if (WorldMap.get_item() != null)
                    MapGetButton.Content = "Get item";
            }
            else MapGetButton.Content = "Nothing around";

            gymbutton.Visibility = (WorldMap.getin_gymable() == -1) ? Visibility.Collapsed : Visibility.Visible;

            mapImage.Clear();
            int[] playerPos = WorldMap.Player_pos;
            int playerX = playerPos[0];
            int playerY = playerPos[1];
            int xrange = ((int)MapPanel.Width / 28)/2;
            int yrange = ((int)MapPanel.Height / 28)/2;
            for (int y = playerY - yrange; y <= playerY + yrange; y++)
                for (int x = playerX - xrange; x <= playerX + xrange; x++)
                {
                    if (x==playerX && y==playerY)
                    {
                        mapImage.Add(playerSrc);
                        continue;
                    }
                    if (0 <= x && x < WorldMap.LEN0 &&
                        0 <= y && y < WorldMap.LEN1)
                        switch (WorldMap.map_get(x, y))
                        {
                            case 0:
                                mapImage.Add(grassSrc);
                                break;
                            case 1:
                                mapImage.Add(pmSrc);
                                break;
                            case 2:
                                mapImage.Add(pmballSrc);
                                break;
                            case 9:
                                mapImage.Add(gymSrc);
                                break;
                            case 10:
                                mapImage.Add(waterSrc);
                                break;
                            case 11:
                                mapImage.Add(treeSrc);
                                break;
                            default:
                                mapImage.Add(grassSrc);
                                break;
                        }
                    else mapImage.Add(null);
                }
        }

        private void Map_onMove(object sender, KeyEventArgs e)
        {
            if (tabControl.SelectedIndex != 0) return;
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    WorldMap.move_up();
                    break;
                case Key.S:
                case Key.Down:
                    WorldMap.move_down();
                    break;
                case Key.A:
                case Key.Left:
                    WorldMap.move_left();
                    break;
                case Key.D:
                case Key.Right:
                    WorldMap.move_right();
                    break;
            }
            e.Handled = true;
            DrawMap();
        }

        private void Manage_Click(object sender, RoutedEventArgs e)
        {
            // read player info
            playerNameTB.Text = "Name: "+player.Name;
            playerLevelTB.Text = "Level: " + player.Level.ToString();
            playerXPTB.Text = "XP: " + player.XP.ToString();
            // read item info
            for (int i = 0; i < 7; i++)
                (ItemGrid.Children[i + 1] as TextBlock).Text = datalist.Item[i] + ": " + player.item_had[i];
            // read pm info
            DrawPlayerPokemonsIn(ManagePMListBox);
            tabControl.SelectedIndex = 1;
            // init button and textbox
            InitManageButtons();
        }

        private void DrawPlayerPokemonsIn(ListBox listbox)
        {
            listbox.Items.Clear();
            foreach (Pokemon pm in player.All_poke)
            {
                StackPanel pmPanel = new StackPanel();
                pmPanel.Margin = new Thickness(10);
                pmPanel.Orientation = Orientation.Horizontal;
                listbox.Items.Add(pmPanel);

                Image pmImg = new Image();
                pmImg.Width = pmImg.Height = 80;
                pmImg.Source = new BitmapImage(new Uri(String.Format("Resources/pm{0}.png",
                    pm.Poke_no), UriKind.Relative));
                pmPanel.Children.Add(pmImg);

                StackPanel infoPanel = new StackPanel();
                infoPanel.Margin = new Thickness(30, 0, 0, 0);
                infoPanel.Width = 120;
                pmPanel.Children.Add(infoPanel);

                TextBlock pmname = new TextBlock();
                pmname.Text = "Name: " + pm.Name;
                infoPanel.Children.Add(pmname);

                TextBlock pmLevel = new TextBlock();
                pmLevel.Text = "Lv: " + pm.Lv.ToString();
                infoPanel.Children.Add(pmLevel);

                TextBlock pmCP = new TextBlock();
                pmCP.Text = "CP: " + pm.CP.ToString();
                infoPanel.Children.Add(pmCP);

                TextBlock pmHP = new TextBlock();
                pmHP.Text = String.Format("HP: {0}/{1}", pm.NowHP, pm.maxHP);
                infoPanel.Children.Add(pmHP);

                StackPanel skillPanel = new StackPanel();
                pmPanel.Children.Add(skillPanel);

                TextBlock emptyline = new TextBlock();
                emptyline.Text = "";
                skillPanel.Children.Add(emptyline);

                TextBlock skilltitle = new TextBlock();
                skilltitle.Text = "Skill Set:";
                skillPanel.Children.Add(skilltitle);

                for (int i = 0; i < pm.Skills.Length; i++)
                {
                    TextBlock skill = new TextBlock();
                    skill.Text = String.Format("{0}. {1}", i+1, datalist.Skill_Name(pm.Skills[i]));
                    skillPanel.Children.Add(skill);
                }
            }
            
        }

        private void MapGetButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorldMap.get_item()!=null)
            {
                int itemId = WorldMap.get_item()[0];
                int itemNum = WorldMap.get_item()[1];
                player.get_item(itemId, itemNum);
                WorldMap.despawn(WorldMap.getable()[0], WorldMap.getable()[1]);
                DrawMap();
                MapGetButton.Content = String.Format("You pick up {0} {1}", itemNum, datalist.Item[itemId]);
            }
            else
            {
                if (player.item_had[3]==0)
                {
                    MapGetButton.Content = "No pokeballs!";
                }
                Pokemon pmToCatch = WorldMap.catch_poke();
                if (pmToCatch != null)
                {
                    tabControl.SelectedIndex = 3;
                    CatchGameInstrTB.Visibility = System.Windows.Visibility.Visible;
                    catchGameInputTBx.Visibility = System.Windows.Visibility.Collapsed;
                    PreCatchPanel.Visibility = System.Windows.Visibility.Visible;
                    PokeballRB.Content = String.Format("Pokeball({0})", player.item_had[3]);
                    GreatballRB.Content = String.Format("Greatball({0})", player.item_had[4]);
                    GreatballRB.IsEnabled = (player.item_had[4] != 0);
                    RazzberryCBx.Content = String.Format("Razzberry({0})", player.item_had[5]);
                    RazzberryCBx.IsEnabled = (player.item_had[5] != 0);
                    PokeballRB.IsChecked = true;
                    RazzberryCBx.IsChecked = false;

                    TimerTB.Visibility = Visibility.Hidden;
                    ScoreTB.Visibility = Visibility.Hidden;
                    PMtoCatchImage.Source = new BitmapImage(new Uri(String.Format("Resources/pm{0}.png",
                        pmToCatch.Poke_no), UriKind.Relative));
                    PMtoCatchNameTB.Text = pmToCatch.Name;
                    PMtoCatchLvTB.Text = "Lv: " + pmToCatch.Lv.ToString();
                    PMtoCatchCPTB.Text = "CP: " + pmToCatch.CP.ToString();
                    CatchMessageTB.Text = "You encountered " + pmToCatch.Name + "!";
                    TimerTB.Text = "Time:   ";
                    ScoreTB.Text = "Score:  ";
                }
            }
        }

        private void CatchStartButton_Click(object sender, RoutedEventArgs e)
        {
            bool usedGreatBall = (GreatballRB.IsChecked == true);
            bool usedRazzberry = (RazzberryCBx.IsChecked == true);
            PreCatchPanel.Visibility = System.Windows.Visibility.Collapsed;
            catchGameInputTBx.Visibility = System.Windows.Visibility.Visible;
            Pokemon pmToCatch = WorldMap.catch_poke();

            if (usedGreatBall)
                player.GreatBall();
            else player.Pokeball();
            if (usedRazzberry)
                player.Razz_Berry();
            StartNewCatchGame(pmToCatch, usedGreatBall, usedRazzberry);
        }

        private void StartNewCatchGame(Pokemon pmToCatch, bool usedGreatBall, bool usedRazzberry)
        {
            catchgame = new catch_poke_game(1, pmToCatch, usedGreatBall, usedRazzberry);

            tabControl.SelectedIndex = 3;
            UpdateLayout();


            WordToTypeTB.Text = catchgame.Word_now;
            TimerTB.Visibility = Visibility.Visible;
            TimerTB.Text = "Time: " + ((int)catchgame.TimeRemained).ToString() + "s";
            ScoreTB.Visibility = Visibility.Visible;
            ScoreTB.Text = "Score: " + catchgame.Score.ToString();
            catchGameInputTBx.Text = "";

            catchGameInputTBx.Focus();
            // timer start
            dispatcherTimer.Tick += UpdatePerTick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }

        private async void EndCatchGame(int catchgameStatus)
        {
            Pokemon pmToCatch = catchgame.Poke_catching;
            if (catchgameStatus == 1)
            {
                CatchMessageTB.Text = String.Format("You caught {0}! Going back to map...",
                    pmToCatch.Name);
                player.Add_poke(pmToCatch);
            }
            else
            {
                CatchMessageTB.Text = String.Format("{0} fleed! Going back to map...",
                    pmToCatch.Name);
            }

            dispatcherTimer.Tick -= UpdatePerTick;
            dispatcherTimer.Stop();

            catchgame = null;
            catchGameInputTBx.Text = "";
            WordToTypeTB.Text = "";
            CatchGameInstrTB.Visibility = System.Windows.Visibility.Hidden;

            // wait 2.5 seconds before going back to map
            await Task.Delay(2500);
            tabControl.SelectedIndex = 0;
            UpdateLayout();
            MapPanel.Focus();
            WorldMap.despawn(WorldMap.getable()[0], WorldMap.getable()[1]);
            DrawMap();
        }

        private void UpdatePerTick(object sender, EventArgs e)
        {
            catchgame.Tick();

            // update the game counters
            TimerTB.Text = "Time: " + ((int)catchgame.TimeRemained).ToString() + "s";
            // update gui when game ends
            int catchgameStatus = catchgame.Status();
            if (catchgameStatus==-1)
                EndCatchGame(-1);
        }

        private void catchGameInputTBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (catchgame == null) return;
            bool WordChanged = Math.Abs(catchgame.MatchOnWordTyped((sender as TextBox).Text))==1;

            if (catchgame.Status() == 1)
                EndCatchGame(1);
            else if (WordChanged)
            {
                (sender as TextBox).Text = "";
                WordToTypeTB.Text = catchgame.Word_now;
                ScoreTB.Text = "Score: " + catchgame.Score.ToString();
            }
        }

        private void EvolveButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ManagePMListBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                player.Evolve(selectedIndex);

                DrawPlayerPokemonsIn(ManagePMListBox);
                candyNumTB.Text = "Candy: " + player.item_had[6];
                playerLevelTB.Text = "Level: " + player.Level.ToString();
                playerXPTB.Text = "XP: " + player.XP.ToString() + " /1000";
                ManagePMListBox.SelectedIndex = selectedIndex;
            }
        }

        private void PowerUpButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ManagePMListBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                player.Powerup(selectedIndex);

                DrawPlayerPokemonsIn(ManagePMListBox);
                candyNumTB.Text = "Candy: " + player.item_had[6];
                playerLevelTB.Text = "Level: " + player.Level.ToString();
                playerXPTB.Text = "XP: " + player.XP.ToString() + " /1000";
                ManagePMListBox.SelectedIndex = selectedIndex;
            }
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ManagePMListBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                player.Sell_poke(selectedIndex);
                DrawPlayerPokemonsIn(ManagePMListBox);
                playerLevelTB.Text = "Level: " + player.Level.ToString();
                playerXPTB.Text = "XP: " + player.XP.ToString() + " /1000";
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ManagePMListBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                RenamePanel.Visibility = System.Windows.Visibility.Visible;
                RenameTextBox.Focus();
            }
        }

        private void ConfirmRenameButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ManagePMListBox.SelectedIndex;
            string newname = RenameTextBox.Text;
            if (newname != "")
            {
                player.All_poke[ManagePMListBox.SelectedIndex].Name = newname;
                DrawPlayerPokemonsIn(ManagePMListBox);
                RenameTextBox.Text = "";
                RenamePanel.Visibility = System.Windows.Visibility.Hidden;
            }
            ManagePMListBox.SelectedIndex = selectedIndex;
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 0;
            UpdateLayout();
            MapPanel.Focus();
        }

        private void Gym_Click(object sender, RoutedEventArgs e)
        {
            if (WorldMap.getin_gymable() == -1) return;
            mgym = WorldMap.Gyms[WorldMap.getin_gymable()-1];
            tabControl.SelectedIndex = 4;
            DrawPlayerPokemonsIn(ChooseBattlePMListBox);
        }

        private async void GymBattleLeadIn(Gym_battle battle)
        {
            Pokemon guestPM = battle.GuestPM;
            Pokemon hostPM = battle.HostPM;
            BattleGuestHPTB.Text = "HP: " + guestPM.NowHP.ToString() + "/" + guestPM.maxHP.ToString();
            GuestPmPanel.DataContext = guestPM;
            AppendBattleLog("You challenged the Gym Leader!");
            HostPmPanel.Visibility = Visibility.Hidden;
            await Task.Delay(2000);

            HostPmPanel.DataContext = hostPM;
            HostPmPanel.Visibility = Visibility.Visible;
            BattleHostHPTB.Text = "HP: " + hostPM.NowHP.ToString() + "/" + hostPM.maxHP.ToString();
            AppendBattleLog("Gym Leader sent out " +battlegame.HostPM.Name + "!");
            await Task.Delay(2000);

            s1button.Visibility = System.Windows.Visibility.Visible;
            s2button.Visibility = System.Windows.Visibility.Visible;
            s1button.Content = datalist.Skill_Name(battle.GuestPM.Skills[0]);
            s2button.Content = datalist.Skill_Name(battle.GuestPM.Skills[1]);
            AppendBattleLog("What do you do?\n");
        }

        private void s1button_Click(object sender, RoutedEventArgs e)
        {
            s1button.Visibility = System.Windows.Visibility.Hidden;
            s2button.Visibility = System.Windows.Visibility.Hidden;

            battlegame.NextRound(1);
            DrawRoundResult();
        }

        private void s2button_Click(object sender, RoutedEventArgs e)
        {
            s1button.Visibility = System.Windows.Visibility.Hidden;
            s2button.Visibility = System.Windows.Visibility.Hidden;

            battlegame.NextRound(2);
            DrawRoundResult();
        }

        private void AppendBattleLog(string log)
        {
            BattleInfoTBx.Text += log + '\n';
        }

        private async void DrawRoundResult()
        {
            string guestPMName = battlegame.GuestPM.Name;
            string hostPMName = battlegame.HostPM.Name;
            // Guest PM use skill
            AppendBattleLog(String.Format("{0}'s {1} uses {2}.",
                player.Name, guestPMName, battlegame.GuestLastSkill));
            await Task.Delay(500);
            // Damage to Host PM
            BattleHostHPTB.Text = "HP: " + battlegame.HostPM.NowHP.ToString() + 
                "/" + battlegame.HostPM.maxHP.ToString();
            if (battlegame.DamageToHost != 0)
                AppendBattleLog(String.Format("Gym Leader's {0} receives {1} damages!",
                    hostPMName, battlegame.DamageToHost));
            else
                AppendBattleLog(String.Format("Gym Leader's {0} evaded!",
                    hostPMName));
            await Task.Delay(1000);
            // If Host PM faints, guest wins
            if (battlegame.ResultStatus == Gym_battle.Result.HostFaint)
            {
                AppendBattleLog(String.Format("Gym Leader's {0} faints!",
                    hostPMName));
                AppendBattleLog("\nYou win!");
                await Task.Delay(1000);
            }
            // If Host PM not faints
            else
            {
                // Host PM use skill
                AppendBattleLog(String.Format("Gym Leader's {0} uses {1}.",
                    hostPMName, battlegame.HostLastSkill));
                await Task.Delay(500);
                // Damage to Guest PM
                BattleGuestHPTB.Text = "HP: " + battlegame.GuestPM.NowHP.ToString() + 
                    "/" + battlegame.GuestPM.maxHP.ToString();
                if (battlegame.DamageToGuest != 0)
                    AppendBattleLog(String.Format("{0}'s {1} receives {2} damages!",
                        player.Name, guestPMName, battlegame.DamageToGuest));
                else
                    AppendBattleLog(String.Format("{0}'s {1} evaded!",
                        player.Name, guestPMName));
                await Task.Delay(1000);
                // If Guest PM faints, guest loses
                if (battlegame.ResultStatus == Gym_battle.Result.GuestFaint)
                {
                    AppendBattleLog(String.Format("{0}'s {1} faints!",
                        player.Name, guestPMName));
                    AppendBattleLog("\nYou loses...");
                    await Task.Delay(1000);
                }
            }
            
            // After all moves completed, next round or end game
            if (battlegame.ResultStatus == Gym_battle.Result.NoFaint)
            {
                AppendBattleLog("\nWhat do you do?\n");                
                s1button.Visibility = System.Windows.Visibility.Visible;
                s2button.Visibility = System.Windows.Visibility.Visible;
            }
            else if (battlegame.ResultStatus != Gym_battle.Result.NoFaint)
            {
                AppendBattleLog("\nGoing back to map...");
                // wait 2.5 seconds before going back to map
                await Task.Delay(2500);
                battlegame = null;
                BattleInfoTBx.Text = "";
                tabControl.SelectedIndex = 0;
                UpdateLayout();
                MapPanel.Focus();
            }
        }

        private void BattleInfoTBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).ScrollToEnd();
        }

        private void GoBattleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseBattlePMListBox.SelectedIndex == -1) return;
            Pokemon pmSelected = player.All_poke[ChooseBattlePMListBox.SelectedIndex];
            tabControl.SelectedIndex = 2;
            s1button.Visibility = System.Windows.Visibility.Hidden;
            s2button.Visibility = System.Windows.Visibility.Hidden;
            battlegame = new Gym_battle(pmSelected, mgym.Bat_list_front, mgym);
            GymBattleLeadIn(battlegame);
        }
        

        private void HealButton_Click(object sender, RoutedEventArgs e)
        {
            if (ManagePMListBox.SelectedIndex != -1)
            {
                HealMenu.Visibility = (HealMenu.Visibility == Visibility.Visible) ? 
                    Visibility.Hidden: Visibility.Visible;
                PotionHealButton.Content = String.Format("({0}) {1}",
                    player.item_had[0], "Potion [Heal 5HP]");
                PotionHealButton.IsEnabled = (player.item_had[0] != 0);
                SPPotionHealButton.Content = String.Format("({0}) {1}",
                    player.item_had[1], "Super Potion [Heal 30HP]");
                SPPotionHealButton.IsEnabled = (player.item_had[1] != 0);
                ReviveHealButton.Content = String.Format("({0}) {1}",
                    player.item_had[2], "Revive [Heal half of HP]");
                ReviveHealButton.IsEnabled = (player.item_had[2] != 0);
            }
        }

        private void PotionHealButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ManagePMListBox.SelectedIndex;

            player.Potion(selectedIndex);

            PotionHealButton.Content = String.Format("{0}: {1}",
                datalist.Item[0], player.item_had[0]);
            potionNumTB.Text = datalist.Item[0] + ": " + player.item_had[0].ToString();

            DrawPlayerPokemonsIn(ManagePMListBox);
            ManagePMListBox.SelectedIndex = selectedIndex;
        }

        private void SPPotionHealButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ManagePMListBox.SelectedIndex;

            player.SuperPotion(selectedIndex);

            SPPotionHealButton.Content = String.Format("{0}: {1}",
                datalist.Item[1], player.item_had[1]);
            potionNumTB.Text = datalist.Item[1] + ": " + player.item_had[1].ToString();

            DrawPlayerPokemonsIn(ManagePMListBox);
            ManagePMListBox.SelectedIndex = selectedIndex;
        }

        private void ReviveHealButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ManagePMListBox.SelectedIndex;

            player.Revive(selectedIndex);

            ReviveHealButton.Content = String.Format("{0}: {1}",
                datalist.Item[2], player.item_had[2]);
            potionNumTB.Text = datalist.Item[2] + ": " + player.item_had[2].ToString();

            DrawPlayerPokemonsIn(ManagePMListBox);
            ManagePMListBox.SelectedIndex = selectedIndex;
        }

        private void ManagePMListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = (sender as ListBox).SelectedIndex;
            if (selectedIndex==-1)
            {
                // init buttons and textbox contents
                InitManageButtons();
            }
            else
            {
                RenameButton.IsEnabled = true;
                HealButton.IsEnabled = true;
                Pokemon pmSelected = player.All_poke[selectedIndex];
                PowerUpButton.Content = "Power Up\n[3 candies]";
                PowerUpButton.IsEnabled = (player.item_had[6] >= 3);
                int candyNum = datalist.Poke_Candy(pmSelected.Poke_no);
                if (candyNum != -1)
                {
                    EvolveButton.Content = String.Format("Evolve\n[{0} candies]", candyNum);
                    EvolveButton.IsEnabled = (player.item_had[6] >= candyNum);
                }
                else EvolveButton.Content = "Cannot Evovle";
            }
        }

        private void InitManageButtons()
        {
            RenameButton.IsEnabled = false;
            HealButton.IsEnabled = false;
            PowerUpButton.Content = "Power Up";
            PowerUpButton.IsEnabled = false;
            EvolveButton.Content = "Evolve";
            EvolveButton.IsEnabled = false;
            HealMenu.Visibility = Visibility.Hidden;
            RenamePanel.Visibility = Visibility.Hidden;
            RenameTextBox.Text = "";
        }

    }


    public class StringFormatToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string)
            {
                return string.Format(parameter.ToString(), value);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
