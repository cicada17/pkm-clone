using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3080Go
{
    // Navigation, Capture, Gym-Battle, Manage Pokemon/Collection Powerup, evolve, name or sell your Pokemon,  bonus 

    public class Player
    {
        private string name;
        private int xp;// implies levels
        private List<Pokemon> player_poke = new List<Pokemon>();
        private int[] item_list = new int[7];//num of items owned


        public Player(string name)
        {
            this.name = name;
            xp = 0;
            for (int i = 0; i < 7; i++) item_list[i] = 0;
            item_list[0] = 1;
            item_list[1] = 1;
            item_list[2] = 1;
            item_list[3] = 10;
            item_list[4] = 1;
            item_list[5] = 1;
            item_list[6] = 50;
        }

        public int Level
        {
            get { if (50 < xp / 10000) return 50; else return xp / 1000 + 1; }
        }

        public int XP { get { return xp; } }
        public string Name { get { return name; } }
        public int[] item_had { get { return item_list; } }
        public Pokemon[] All_poke { get { return player_poke.ToArray(); } }

        public Pokemon Poke(int position)
        {
            if (player_poke.Count > position) return player_poke[position];
            else return null;
        }

        public void Add_poke(Pokemon p)
        {
            p.downgrade(Level);
            player_poke.Add(p);
            xp += 100;
        }

        public void Sell_poke(int position)
        {   //important
            if (player_poke.Count > position) player_poke.RemoveAt(position);
            item_list[6] += 2;
            xp += 25;
        }

        public void get_item(int item, int num)
        {
            item_list[item] += num;
            xp += num;
        }

        public bool Potion(int position)
        {
            if (player_poke[position] == null) return false;
            if (item_list[0] < 1 || (player_poke[position]).maxHP == (player_poke[position]).NowHP) return false;
            item_list[0]--;
            xp += 10;
            if (player_poke.Count > position) (player_poke[position]).heal(5);
            return true;
        }

        public bool SuperPotion(int position)
        {
            if (player_poke[position] == null) return false;
            if (item_list[1] < 1 || (player_poke[position]).maxHP == (player_poke[position]).NowHP) return false;
            item_list[1]--;
            xp += 15;
            if (player_poke.Count > position) (player_poke[position]).heal(30);
            return true;
        }

        public bool Revive(int position)
        {
            if (player_poke[position] == null) return false;
            if (item_list[2] < 1 & (player_poke[position]).weak_status) return false;
            item_list[2]--;
            xp += 20;
            if (player_poke.Count > position) (player_poke[position]).heal((player_poke[position]).maxHP / 2);
            return true;
        }

        public bool Pokeball()
        {
            if (item_list[3] < 1) return false;
            item_list[3]--;
            return true;
        }
        public bool GreatBall()
        {
            if (item_list[4] < 1) return false;
            item_list[4]--;
            return true;
        }
        public bool Razz_Berry()
        {
            if (item_list[5] < 1) return false;
            item_list[5]--;
            return true;
        }

        public bool Evolve(int position)
        {
            if (player_poke[position] == null) return false;
            int temp = (player_poke[position]).Poke_no;
            if (datalist.Evolve_list(temp) == -1) return false;
            int temp2 = datalist.Poke_Candy(((player_poke[position]).Poke_no));
            if (item_list[6] < temp2) return false;
            if ((player_poke[position]).Poke_evolve())
            {
                item_list[6] -= temp2;
                xp += 200;
            }
            else return false;
            return true;
        }

        public bool Powerup(int position)
        {
            if (player_poke[position] == null) return false;
            if (item_list[6] < 3) return false;
            if ((player_poke[position]).Poke_powerup())
            {
                item_list[6] -= 3;
                xp += 50;
            }
            else return false;
            return true;
        }


    }

    public class Pokemon
    {
        private string name;
        private int poke;
        private int level;
        // 	private int cp;
        // 	private int hp;
        private int nowHP;
        private int[] skills = new int[2];
        static private Random ran = new Random();

        public string Name
        {
            get { return name; }
            set
            {
                if (value is string) name = value;
                else Console.WriteLine("Invalid input");
            }
        }

        public int CP { get { return level * datalist.Poke_CP(poke); } }
        public int maxHP { get { return CP / 5; } }
        public int Lv { get { return level; } }
        public int NowHP { get { return nowHP; } }
        public int[] Skills { get { return skills; } }
        public int Poke_no { get { return poke; } }

        public Pokemon(string name, int poke_no)
        {
            this.name = name;
            this.poke = poke_no;
            int temp = datalist.Poke_Max_Lv(poke_no);
            level = ran.Next(1, temp); //????
            this.nowHP = this.maxHP;
            this.Randomize_skills();
        }


        public Pokemon(int poke_no)
        {
            this.name = datalist.Poke_Name(poke_no);
            this.poke = poke_no;
            int temp = datalist.Poke_Max_Lv(poke_no);
            level = ran.Next(1, temp); //????
            this.nowHP = this.maxHP;
            this.Randomize_skills();
        }

        public Pokemon Pokemon_Strong()
        {
            int temp;
            int no = 1;
            for (temp = 0; temp <= 40; )
            {
                no = ran.Next(1, datalist.number_poke_all.Count() + 1);
                temp = datalist.Poke_Max_Lv(no);
            }
            int temp2 = ran.Next(40, temp + 1);
            return new Pokemon(no);
        }

        private void Randomize_skills()
        {
            int[] temp = datalist.Poke_Type(poke);
            List<int> all_skills = new List<int>();
            foreach (int i in temp)
            {
                int[] temp1 = datalist.Type_Skill(i);
                foreach (int j in temp1) all_skills.Add(j);
            }
            int[] All_Skills = all_skills.ToArray();
            int[] temp_skills = new int[2];
            temp_skills[0] = All_Skills[ran.Next(0, All_Skills.Length)];
            temp_skills[1] = temp_skills[0];
            while (temp_skills[1] == temp_skills[0])
            {
                temp_skills[1] = All_Skills[ran.Next(0, All_Skills.Length)];
            }
            skills = temp_skills;
        }

        public bool Poke_powerup()
        {
            if (datalist.Poke_Max_Lv(poke) > level)
            {
                level++;
                return true;
            }
            else return false;
        }

        public bool Poke_evolve()
        {
            int j = datalist.Evolve_list(poke);
            if (j != -1)
            {
                if (datalist.Poke_Name(poke) == name) name = datalist.Poke_Name(j);
                poke = j;
                Randomize_skills();
                return true;
            }
            return false;
        }

        public void hurt(int i)
        {
            if (nowHP >= i) nowHP -= i;
            else nowHP = 0;
        }

        public void heal(int i)
        {
            if (maxHP >= nowHP + i) nowHP += i;
            else nowHP = maxHP;
        }

        public bool weak_status
        {
            get { return (nowHP == 0); }
        }

        public void downgrade(int player_lv)
        {
            if (player_lv > level) level = (level % player_lv) + 1;
        }

    }

    public class Map
    {
        private int[,] map;
        private int len0;
        private int len1;
        private Dictionary<int, Pokemon> appear_poke = new Dictionary<int, Pokemon>();
        private Dictionary<int, int[]> appear_item = new Dictionary<int, int[]>();
        private Dictionary<int, int> spawn_time = new Dictionary<int, int>();
        static Random ran = new Random();
        private int x; //(player's position)
        private int y;
        private Gym[] gyms = new Gym[2];


        public int pos_hash(int x0, int y0) {
            return x0 + y0 * len0;
        }

        public int[] pos_unhash(int z)
        {
            return new int[] { z % len0, z / len0 };
        }

        public Gym[] Gyms { get {return gyms;}}


        //temp
        public Map(int x, int y, int len0, int len1)
        {
            if (len0 >= 10) this.len0 = len0; else this.len0 = 10;
            if (len1 >= 10) this.len1 = len1; else this.len1 = 10;

            map = new int[len0, len1];
            int[] temp_ar = Map_randomize();
            map[0, 0] = 0;//0 is road/grass, 1 is poke, 2 is items, 9 is gym, 10+ is unaccessable region like river. (10 pond, 11 tree)
            if (map[x, y] <= 8)
            {
                if (x >= len0 - 1) this.x = 0; else this.x = x;
                if (y >= len1 - 1) this.y = 0; else this.y = y;
            }
            else
            {
                int min_dis = len0 * len0 + len1 * len1;
                int tempx = x;
                int tempy = y;
                for (int i = 0; i < len0; i++)
                {
                    // in case originally in unaccessable region find the nearest save spot as a starting pt.
                    for (int j = 0; j < len1; j++)
                    {
                        if (map[i, j] <= 8)
                        {
                            int temp = (i - x) * (i - x) + (j - y) * (j - y);
                            if (temp < min_dis)
                            {
                                min_dis = temp;
                                tempx = i; tempy = j;
                            }
                        }
                    }
                }
                this.x = tempx; this.y = tempy;
            }
            gyms[0] = new Gym("Gym 1", temp_ar[0], temp_ar[1]);
            gyms[1] = new Gym("Gym 2", temp_ar[2], temp_ar[3]);
            for (int i = 0; i < 2; i++)
            {
                gyms[0].Addstrong();
                gyms[1].Addstrong();
            }
            for (int i = 0; i < 4; i++)
            {
                gyms[0].Addrandom();
                gyms[1].Addrandom();
            }

        }

        private int[] Map_randomize()
        {

            int size = Convert.ToInt32(Math.Floor(Math.Sqrt(((Convert.ToDouble(len0) - 2) * (Convert.ToDouble(len1) - 2))) / 7));


            for (int i = 0; i < len0; i++)
            {
                for (int j = 0; j < len1; j++)
                {
                    map[i, j] = 0;
                }
            }

            //gym x2 , pond x 2 , tree x len0 
            int r3 = ran.Next(1, len0 - 2);
            int r4 = ran.Next(1, len1 - 2);

            for (int i = 0; i < ((LEN0 * LEN1) / 10); i++)
            {
                r3 = ran.Next(1, len0 - 2);
                r4 = ran.Next(1, len1 - 2);
                map[r3, r4] = 11;
            }


            for (int i = 0; i < size*2; i++)
            {
                r3 = ran.Next(1, len0 - 2);
                r4 = ran.Next(1, len1 - 2);
                map[r3, r4] = 10; map[r3 + 1, r4] = 10; map[r3, r4 + 1] = 10; map[r3 + 1, r4 + 1] = 10;
            }

            int r1 = ran.Next(1, len0 - 2);
            int r2 = ran.Next(1, len1 - 2);
            r3 = ran.Next(1, len0 - 2);
            r4 = ran.Next(1, len1 - 2);

            for (; ((r3 <= r1 + 2) & (r3 >= r1-2)) || ((r4 <= r2 + 2) & (r4 >= r2-2)); )
            {
                r3 = ran.Next(1, len0 - 2);
                r4 = ran.Next(1, len1 - 2);
            }
            map[r1, r2] = 9; map[r1 + 1, r2] = 9; map[r1, r2 + 1] = 9; map[r1 + 1, r2 + 1] = 9; map[r1 + 2, r2] = 0; map[r1 + 2, r2 + 1] = 0;
            map[r3, r4] = 9; map[r3 + 1, r4] = 9; map[r3, r4 + 1] = 9; map[r3 + 1, r4 + 1] = 9; map[r3 + 2, r4] = 0; map[r3 + 2, r2 + 1] = 0;
            return new int[] { r1, r2, r3, r4 };
        }

        public int map_get(int x, int y) { return map[x, y]; }
        public int LEN0 { get { return len0; } }
        public int LEN1 { get { return len1; } }
        public int[] Player_pos { get { return new int[] { x, y }; } }



        public void move_left() { if (x > 0 && map[x - 1, y] <= 8) x--; tick(); }
        public void move_right() { if (x < len0 - 1 && map[x + 1, y] <= 8) x++; tick(); }
        public void move_up() { if (y > 0 && map[x, y - 1] <= 8) y--; tick(); }
        public void move_down() { if (y < len1 - 1 && map[x, y + 1] <= 8) y++; tick(); }

        public int[] getable()
        {  //out: {x,y,what} what =1 if poke, what =2 if item, otherwise null array
            int min_dis = len0 * len0 + len1 * len1;
            int[] nearest = { len0 + 1, len1 + 1, 0 };

            foreach (KeyValuePair<int, Pokemon> pair in appear_poke)
            {
                int temp = (x - (pos_unhash(pair.Key))[0]) * (x - (pos_unhash(pair.Key))[0]) + (y - (pos_unhash(pair.Key))[1]) * (y - (pos_unhash(pair.Key))[1]);
                if (temp < min_dis & temp <= 8)
                {
                    nearest[0] = (pos_unhash(pair.Key))[0];
                    nearest[1] = (pos_unhash(pair.Key))[1];
                    min_dis = temp;
                    nearest[2] = 1;
                }
            }
            foreach (KeyValuePair<int, int[]> pair in appear_item)
            {
                int temp = (x - (pos_unhash(pair.Key))[0]) * (x - (pos_unhash(pair.Key))[0]) + (y - (pos_unhash(pair.Key))[1]) * (y - (pos_unhash(pair.Key))[1]);
                if (temp < min_dis & temp <= 8)
                {
                    nearest[0] = (pos_unhash(pair.Key))[0];
                    nearest[1] = (pos_unhash(pair.Key))[1];
                    min_dis = temp;
                    nearest[2] = 2;
                }
            }
            if (nearest[2] != 0) return nearest;
            else return null;
        }

        public int getin_gymable()
        {		//return the gym1/2
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (x + i >= 0 & y + j >= 0 & x + i < len0 & y + j < len1) { 
                    if (map[x + i, y + j] == 9)
                    {
                        int[] rel_pos = {0,0};
                        for (int k = -1; k < 2; k++){
                            for (int l = -1; l < 2; l++)
                            {
                                if (k * l != 0)
                                {
                                    if (x + i + k >= 0 & x + j + l >= 0 & x + i + k < len0 & x + j + l < len1)
                                        if (map[x + i + k, x + j + l] == 9) {
                                            rel_pos[0] = (k - 1) / 2; rel_pos[1] = (l - 1) / 2;
                                        }
                                }
                            }
                        }
                        if (gyms[0].posX == x + i + rel_pos[0] & gyms[0].posY == y + j + rel_pos[1]) return 1;
                        else return 2;
                    }
                    }
                }
            }
            return -1;
        }

        public Pokemon catch_poke()
        { // no despwan, return ONLY the nearest pokemon itself
            int[] temp2 = getable();
            if (temp2 == null) return null;
            if (temp2[2] != 1) return null;
            int nearest = pos_hash( temp2[0], temp2[1] );
            Pokemon temp = new Pokemon(1);
            appear_poke.TryGetValue(nearest, out temp);
            return temp;
        }

        public int[] get_item()
        {  // no despwan, same as above
            int[] temp2 = getable();
            if (temp2 == null) return null;
            if (temp2[2] != 2) return null;
            int nearest = pos_hash(temp2[0], temp2[1]);
            int[] temp = { -1, 0 };
            appear_item.TryGetValue(nearest, out temp);
            return temp;
        }

        public void tick()
        {
            //despwan natually
            for (int i = (LEN0) * (LEN1); i >= 0; i-- )
            {
                int time;
                if (spawn_time.TryGetValue(i, out time))
                {
                    if (time == 1) despawn((pos_unhash(i))[0], (pos_unhash(i))[1]);
                    else spawn_time[i]--;
                }
            }
            for (int i = (LEN0 ) * (LEN1); i >= 0; i--)
            {
                int time;
                if (spawn_time.TryGetValue(i, out time))
                {
                    if (time == 1) despawn((pos_unhash(i))[0], (pos_unhash(i))[1]);
                    else spawn_time[i]--;
                }
            }
            //randomize spawning
            if (spawn_time.Count <= len1 * len0 / 2)
            {
                int r = ran.Next(1, 4);
                if (r > 1)
                {
                    int rx = ran.Next(0, len0);
                    int ry = ran.Next(0, len1);
                    for (; spawn_time.ContainsKey(pos_hash(rx, ry)) || (map[rx, ry] != 0); )
                    {
                        rx = ran.Next(0, len0);
                        ry = ran.Next(0, len1);

                    }
                    spawn(rx, ry); // not confirm what is spawning at this pt
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (gyms[i].No_poke < 1) gyms[i].Addstrong();
                else if (ran.Next(1,3) == 1 )
                {
                    if (gyms[i].No_poke < 2) gyms[i].Addstrong();
                    else if (gyms[i].No_poke < 6) gyms[i].Addrandom();
                }
                
                gyms[i].Heal();
            }

        }

        private void spawn(int x0, int y0)
        {
            double L = Math.Exp(-30);
            int k = 2;
            double p = ran.NextDouble();
            for (; p > L; k++)
            {
                p *= ran.NextDouble();
            }
            k--;
            int r = ran.Next(1, 101);
            spawn_time.Add(pos_hash (x0, y0 ), k);
            if (r > 99)
            {
                map[x0, y0] = 1;
                Pokemon temp = new Pokemon(1);
                Pokemon temp2 = temp.Pokemon_Strong();
                appear_poke.Add(pos_hash(x0, y0), temp2);
            }
            else if (r > 50)
            {
                int no = ran.Next(1, datalist.number_poke_all.Count() + 1);
                string s = datalist.Poke_Name(no);
                map[x0, y0] = 1;
                appear_poke.Add(pos_hash(x0, y0), new Pokemon(s, no));
            }
            else
            {
                int items = ran.Next(0, datalist.Item.Count());
                int num = ran.Next(1, 4);
                map[x0, y0] = 2;
                appear_item.Add(pos_hash(x0, y0), new int[] { items, num });
            }
        }

        public void despawn(int x0, int y0)
        {
            int type = map[x0, y0];
            int temp = pos_hash( x0, y0 );
            if (type < 1 || type > 8) return;
            else
            {
                appear_poke.Remove(temp);
                appear_item.Remove(temp);
                spawn_time.Remove(temp);
                map[x0, y0] = 0;
            }
        }

    }

    public class Gym
    {
        private Pokemon[] bat_poke = new Pokemon[6];
        private string name;
        private static Random ran = new Random();
        private int no_poke;
        private int pos_x;
        private int pos_y;

        public int posX { get { return pos_x;  } }
        public int posY { get { return pos_y; } }

        public int Status
        {
            get
            {
                int status = 0;
                foreach (Pokemon p in bat_poke) if (p != null) status += p.CP;
                return status;
            }
        }
        public Pokemon[] Bat_list_poke { get { return bat_poke; } }
        public int No_poke { get { return no_poke; } }
        public Pokemon Bat_list_front { get { return bat_poke[no_poke - 1]; } }
        public string Name{get{return name;}}

        public Gym(string name, int x, int y)
        {
            this.name = name;
            no_poke = 0;
            pos_x = x;
            pos_y = y;
        }

        public Gym(Pokemon p, int x, int y)
        {
            name = p.Name;
            no_poke = 0;
            Addpoke(p);
            pos_x = x;
            pos_y = y;
        }

        public Gym(string name, Pokemon p, int x, int y)
        {
            this.name = name;
            no_poke = 0;
            Addpoke(p);
            pos_x = x;
            pos_y = y;
        }


        public void Addpoke(Pokemon p)
        {
            if (no_poke < 6)
            {
                no_poke++;
                bool inserted = false;
                for (int i = no_poke - 1; i > 0; i--)
                {
                    if (p.CP > (bat_poke[i - 1]).CP) bat_poke[i] = bat_poke[i - 1];
                    else
                    {
                        bat_poke[i] = p;
                        inserted = true;
                        break;
                    }
                }
                if (!inserted) bat_poke[0] = p;
            }
        }

        public void Addrandom()
        {
            int temp = datalist.number_poke_all.Count();
            Addpoke(new Pokemon(ran.Next(1, temp + 1)));
        }

        public void Addstrong()
        {
            Pokemon temp = new Pokemon(1);
            Pokemon temp2 = temp.Pokemon_Strong();
            Addpoke(temp2);
        }

        public void Heal()
        {
            foreach (Pokemon p in bat_poke)
            {
                if (p != null)
                p.heal(30);
            }
        }

        public int Hurt(int i)
        {			// -1 = error, 0 = hurt only, 1 = dead
            if (no_poke <= 0) return -1;
            (bat_poke[no_poke - 1]).hurt(i);
            if ((bat_poke[no_poke - 1]).weak_status)
            {
                Remove();
                return 1;
            }
            return 0;
        }

        public void Remove()
        {
            if (no_poke > 0)
            {
                no_poke--;
                bat_poke[no_poke] = null;
            }
        }
    }

    public class Gym_battle
    {
        private Pokemon[] battling = new Pokemon[2];
        public Pokemon GuestPM { get; private set; }
        public Pokemon HostPM { get; private set; }
        private Gym Gym_battling;
        private int xp;
        static Random ran = new Random();
        private int[] guest_skills = new int[2];
        private int[] host_skills = new int[2];
        private int[] guest_skills_type = new int[2];
        private int[] host_skills_type = new int[2];
        private int[] guest_type;
        private int[] host_type;
        public string GuestLastSkill { get; private set; }
        public string HostLastSkill { get; private set; }
        public int DamageToGuest { get; private set; }
        public int DamageToHost { get; private set; }
        public Result ResultStatus { get; private set; }
        public enum Result
        {
            NoFaint,
            GuestFaint,
            HostFaint
        };


        public Gym_battle(Pokemon guest, Pokemon host, Gym place)
        {
            battling[0] = guest;
            battling[1] = host;
            Gym_battling = place;
            xp = 0;
            guest_skills = new int[] { ((battling[0]).Skills)[0], ((battling[0]).Skills)[1] };
            host_skills = new int[] { ((battling[1]).Skills)[0], ((battling[1]).Skills)[1] };
            guest_skills_type = new int[] { datalist.Skill_Type(guest_skills[0]), datalist.Skill_Type(guest_skills[1]) };
            host_skills_type = new int[] { datalist.Skill_Type(host_skills[0]), datalist.Skill_Type(host_skills[1]) };
            guest_type = datalist.Poke_Type((battling[0]).Poke_no);
            host_type = datalist.Poke_Type((battling[1]).Poke_no);
            this.GuestPM = Bat_Poke[0];
            this.HostPM = Bat_Poke[1];
            ResultStatus = Result.NoFaint;
        }

        public Pokemon[] Bat_Poke { get { return battling; } }
        public Gym Gym_now { get { return Gym_battling; } }
        public int XP_get { get { return xp; } }


        private int attack(bool left, int skill)
        { //true is left, false is right, skill 1 2 are guest poke skills
            if (skill != 1 & skill != 2) return -1;
            if (ran.Next(0, 4) == 0) return 0;
            int dam = (battling[0]).CP * datalist.Skill_PW(guest_skills[skill - 1]);
            int type = guest_skills_type[skill - 1];
            foreach (int i in host_type)
            {
                dam = (dam * datalist.Att_Def(type, i)) / 2;
            }
            if (ran.Next(0, 10) > 7) dam *= 2;
            (battling[1]).hurt(dam / 1000 +1);
            return ((dam / 1000) + 1);		// return int = damage to host, 0 means missed
        }

        private int[] defense(bool left)   // {dam, skills}
        {
            int r = ran.Next(0, 2);
            int att = host_skills[r];
            if (ran.Next(0, 4) == 0) return new int[] {0,att};

            int dam = (battling[1]).CP * datalist.Skill_PW(host_skills[r]);
            int type = host_skills_type[r];
            foreach (int i in guest_type)
            {
                dam = (dam * datalist.Att_Def(type, i)) / 2;
            }
            if (ran.Next(0, 10) > 7) dam *= 2;
            (battling[0]).hurt(dam / 1000 + 1);
            return new int[] { (dam / 1000)+1, att}; // return int = damage to guest, 0 means avoided
        }

        private int Status()
        {   // 1 is guest win, -1 is host win, 0 is in process
            if ((battling[1]).weak_status) return 1;
            if ((battling[0]).weak_status) return -1;
            return 0;
        }

        public void NextRound(int skillChosenFromSkillSet)
        {
            DamageToHost = attack(true, skillChosenFromSkillSet);
            if (Status() == 1)
            {
                ResultStatus = Result.HostFaint;
                Gym_battling.Remove();
                return;
            }
            int[] defenceResult = defense(true);
            if (Status() == -1)
                ResultStatus = Result.GuestFaint;

            DamageToGuest = defenceResult[0];
            GuestLastSkill = datalist.Skill_Name(GuestPM.Skills[skillChosenFromSkillSet - 1]);
            HostLastSkill = datalist.Skill_Name(defenceResult[1]);
        }

    }



    public class catch_poke_game
    {
        private static Random ran = new Random();

        // data used only in game logics
        private int TimeLimit = 15;
        private int tickPerSecond;
        private int tickCounter;
        private string[] wordLib = {"GOOD","BAD","HEALTH","IERG","ENGG","CUHK","LESSON","WINDOW","APPLE",
                              "DOG","FIREFOX","GLASS","JELLY","KING","MOTHER","OPEN","PEOPLE","QUEEN",
                              "RACE","SOUR","TIGER","UNIX","VIVALDI","YOYO","ZEBRA","GIRL","BOTTLE",
                              "HOUSE","IDLE","EAGLE","CERTAIN","LEAST","WINNING","APE","DONATE","FILM",
								"LIFE", "POKEMON", "WHY","FUNNY","PIKACHU", "NOTE", "FIRE"};
        private int size;
        private int score;
        private Pokemon poke;
        private string word_now;
        private bool greatball;
        private bool berry;
        // data also called by presenter


        public double TimeRemained
        {
            get { return TimeLimit - (double)tickCounter / (double)tickPerSecond; }
        }
        public int Score { get { return score; } }
        public string Word_now { get { return word_now; } }
        public Pokemon Poke_catching { get { return poke; } }

        public catch_poke_game(int tickPerSecond, Pokemon poke, bool gb, bool ber)
        {
            this.tickPerSecond = tickPerSecond;
            this.tickCounter = 0;
            this.score = 0;
            this.poke = poke;
            size = wordLib.Count();
            word_now = wordLib[ran.Next(0, size)];
            greatball = gb;
            berry = ber;
        }

        public void Reset()
        {
            this.score = 0;
            this.tickCounter = 0;
            word_now = wordLib[ran.Next(0, size)];
        }

        public int MatchOnWordTyped(string wordTyped)
        {
            int len = wordTyped.Length;
            for (int i = 0; i < len; i++)
                if (wordTyped[i] != word_now[i])
                {
                    score--;
                    word_now = wordLib[ran.Next(0, size)];
                    return -1;
                }
            if (wordTyped == word_now)
            {
                score++;
                word_now = wordLib[ran.Next(0, size)];
                return 1;
            }
            return 0;
        }

        public void Tick()
        {
            tickCounter++;
        }

        public int Status()
        {
            int fac = 5;
            if (berry) fac++;
            if (greatball) fac += 2;
            if (score * fac > poke.Lv) return 1;
            else if (TimeRemained <= 0) return -1;
            else return 0;
        }

        

    }


    public class catch_poke_nogame
    {
        private static Random ran = new Random();
        private Pokemon poke;
        private bool greatball;
        private bool berry;
        private bool caught;
        // data also called by presenter

        public Pokemon Poke_catching { get { return poke; } }
        public bool Caught { get { return caught; } }

        public catch_poke_nogame(Pokemon poke, bool gb, bool ber)
        {
            this.poke = poke;
            greatball = gb;
            berry = ber;
            int temp = 20;
            if (gb) temp += 20;
            if (ber) temp += 10;
            int temp2 = ran.Next(1, temp + poke.Lv + 1);
            if (temp2 >= poke.Lv) caught = true;
            else caught = false;
        }
    }


    static public class datalist
    {
        // all list stores here
        static private string[] item = { "Potion", "Super Potion", "Revive", "Pokeball", "GreatBall", "Razz Berry", "Candy" };
        static private int[,] attrack_defence = {{2,2,2,2,2,2,2},{2,1,4,2,1,2,1},{2,1,1,2,4,2,2},
	{2,4,1,2,1,2,2},{2,2,4,1,1,2,4},{2,2,2,1,4,4,2},{4,2,2,2,2,2,1}};   //attrack --factor(has x2)--> defence  i.e. 2 means 1. 1 means 0.5
        static private Dictionary<int, int> evolve_list = new Dictionary<int, int>();
        static private Dictionary<int, int> poke_cp = new Dictionary<int, int>();
        static private Dictionary<int, int> poke_max_lv = new Dictionary<int, int>();
        static private Dictionary<int, string> poke_name = new Dictionary<int, string>();
        static private Dictionary<int, int> poke_candy = new Dictionary<int, int>();
        static private Dictionary<int, int[]> poke_type = new Dictionary<int, int[]>();
        static private Dictionary<int, string> type_name = new Dictionary<int, string>();
        static private Dictionary<int, int[]> type_skill = new Dictionary<int, int[]>();
        static private Dictionary<int, string> skill_name = new Dictionary<int, string>(); //Todo
        static private Dictionary<int, int> skill_pw = new Dictionary<int, int>(); // todo

        static datalist()
        {
            string[] lines = System.IO.File.ReadAllLines(@"Poke.txt");
            string[] tokens;
            char[] delimiterChars = { '\t', ' ' };
            foreach (string line in lines)
            {
                tokens = line.Split();
                //foreach (string s in tokens) Console.WriteLine(s);
                evolve_list.Add(int.Parse(tokens[0]), int.Parse(tokens[1]));
                poke_cp.Add(int.Parse(tokens[0]), int.Parse(tokens[2]));
                if (int.Parse(tokens[3]) != 0) poke_max_lv.Add(int.Parse(tokens[0]), int.Parse(tokens[3]));
                poke_name.Add(int.Parse(tokens[0]), tokens[4]);
                if (int.Parse(tokens[5]) != 0) poke_candy.Add(int.Parse(tokens[0]), int.Parse(tokens[5]));
                int[] temp2 = new int[tokens.Length - 6];
                for (int i = 0; i < tokens.Length - 6; i++)
                    temp2[i] = int.Parse(tokens[6 + i]);
                poke_type.Add(int.Parse(tokens[0]), temp2);
            }
            lines = System.IO.File.ReadAllLines(@"Skills.txt");
            foreach (string line in lines)
            {
                tokens = line.Split();
                //foreach (string s in tokens) Console.WriteLine(s);
                string temp2 = "";
                for (int i = 0; i < tokens.Length - 2; i++)
                    temp2 = temp2 + tokens[1 + i] + " ";
                temp2.Trim();
                skill_name.Add(int.Parse(tokens[0]), temp2);
                skill_pw.Add(int.Parse(tokens[0]), int.Parse(tokens[tokens.Length - 1]));
            }
            lines = System.IO.File.ReadAllLines(@"Type.txt");
            foreach (string line in lines)
            {
                tokens = line.Split();
                //foreach (string s in tokens) Console.WriteLine(s);
                type_name.Add(int.Parse(tokens[0]), tokens[1]);
                int[] temp2 = new int[tokens.Length - 2];
                for (int i = 0; i < tokens.Length - 2; i++)
                    temp2[i] = int.Parse(tokens[2 + i]);
                type_skill.Add(int.Parse(tokens[0]), temp2);
            }

        }
        static public int[] number_poke_all { get { return poke_name.Keys.ToArray(); } }
        static public string[] Item { get { return item; } }
        static public int Att_Def(int attrack, int defence)
        {
            if (attrack > attrack_defence.GetLength(0) || attrack <= 0 || defence > attrack_defence.GetLength(1) || defence <= 0)
                return -1;
            else return attrack_defence[attrack - 1, defence - 1];
        }

        static public int Evolve_list(int i)
        {
            int j;
            if (evolve_list.TryGetValue(i, out j)) return j;
            else return -1;
        }
        static public int Poke_CP(int i)
        {
            int j;
            if (poke_cp.TryGetValue(i, out j)) return j;
            else return -1;
        }
        static public int Poke_Max_Lv(int i)
        {
            int j;
            if (poke_max_lv.TryGetValue(i, out j)) return j;
            else return -1;
        }
        static public string Poke_Name(int i)
        {
            string j;
            if (poke_name.TryGetValue(i, out j)) return j;
            else return null;
        }
        static public int Poke_Candy(int i)
        {
            int j;
            if (poke_candy.TryGetValue(i, out j)) return j;
            else return -1;
        }
        static public int[] Poke_Type(int i)
        {
            int[] j;
            if (poke_type.TryGetValue(i, out j)) return j;
            else return null;
        }
        static public string Type_Name(int i)
        {
            string j;
            if (type_name.TryGetValue(i, out j)) return j;
            else return null;
        }
        static public int[] Type_Skill(int i)
        {
            int[] j;
            if (type_skill.TryGetValue(i, out j)) return j;
            else return null;
        }
        static public int Skill_Type(int i)
        {
            foreach (KeyValuePair<int, int[]> pair in type_skill)
            {
                foreach (int j in pair.Value)
                {
                    if (i == j) return pair.Key;
                }
            }
            return -1;
        }
        static public string Skill_Name(int i)
        {
            string j;
            if (skill_name.TryGetValue(i, out j)) return j;
            else return null;
        }
        static public int Skill_PW(int i)
        {
            int j;
            if (skill_pw.TryGetValue(i, out j)) return j;
            else return -1;
        }

    }

}


