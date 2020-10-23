using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TAPSONIC_TOP_Lilly
{
    public partial class Form1 : Form
    {
        static Encoding euckr = Encoding.GetEncoding(51949);

        static int time_px = 5;// 초당 가로 픽셀
        static int time_song = 180;// 음악 최대 시간

        static int[] buff_v = Enumerable.Repeat(0, time_song).ToArray();//버프값. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_t = Enumerable.Repeat(0, time_song).ToArray();//버프값 탭. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_l = Enumerable.Repeat(0, time_song).ToArray();//버프값 롱. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_s = Enumerable.Repeat(0, time_song).ToArray();//버프값 슬라. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_f = Enumerable.Repeat(0, time_song).ToArray();//버프값 플릭. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_va = Enumerable.Repeat(0, time_song).ToArray();//버프 기본값 및 콤보 적용
        static int[] buff_ta = Enumerable.Repeat(0, time_song).ToArray();//버프값 탭. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_la = Enumerable.Repeat(0, time_song).ToArray();//버프값 롱. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_sa = Enumerable.Repeat(0, time_song).ToArray();//버프값 슬라. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_fa = Enumerable.Repeat(0, time_song).ToArray();//버프값 플릭. 만약 타입별로 한다면 이걸 분할해야함.
        static bool[] buff_c = Enumerable.Repeat(false, time_song).ToArray();//콤보 유무

        static int sum = 0;// 
        static int sum_t = 0;// 
        static int sum_l = 0;// 
        static int sum_s = 0;// 
        static int sum_f = 0;// 
        static int cnt = 0;// 

        static List<Chra> ca = new List<Chra>();//모든캐릭
        static List<Chra> ct;//탭캐릭. 분류 코드로 자동 처리
        static List<Chra> cl;//탭캐릭. 분류 코드로 자동 처리
        static List<Chra> cs;//탭캐릭. 분류 코드로 자동 처리
        static List<Chra> cp;//탭캐릭. 분류 코드로 자동 처리

        static List<Song> so;//음악 목록
        Dictionary<string, List<Note>> openWith = new Dictionary<string, List<Note>>();

        static Font fnt = new Font("맑은 고딕", 16);
        //var fontf = new Font("맑은 고딕", 16, FontStyle.Regular, GraphicsUnit.Pixel);

        static int grp_w = time_song * time_px;// 그래픽 가로 크기
        static int grp_h_div = 4;//그래픽 배율 감소
        static int grp_h = 150;// 그래픽 세로 크기

        // 세로 그래프용
        static Bitmap bitmap = new Bitmap(grp_w, grp_h);
        static Graphics graphics = Graphics.FromImage(bitmap);

        // 가로 그래프용
        static Bitmap bitmap2 = new Bitmap(grp_w, grp_h);
        static Graphics graphics2 = Graphics.FromImage(bitmap2);
        static SolidBrush solidBrush = new SolidBrush(Color.FromArgb(0, 0, 0));

        /// <summary>
        /// 속성. 댄서 세션 보컬
        /// </summary>
        enum Att
        {
            dancer,
            session,
            vocal,
            none
        }

        /// <summary>
        /// 타입. 탭 롱 슬라이드 플릿
        /// </summary>
        enum Type
        {
            tap,
            @long,
            slide,
            Flick,
            none
        }

        /// <summary>
        /// 버프 종류. 만약 자버프 같은걸 넣을시 여기다가 해야할듯
        /// </summary>
        enum Buff
        {
            combo,
            buff, // (구)분류전
            buff_a,
            buff_t,
            buff_l,
            buff_s,
            buff_f,
            etc
        }

        struct Chra
        {
            public string name;
            public Att att;
            public Type type;
            public Buff buff;
            public int buff_v;
            public int off;
            public int on;

            public Chra(string name, Att att, Type type, Buff buff, int buff_v, int off, int on)
            {
                this.name = name;
                this.type = type;
                this.buff = buff;
                this.buff_v = buff_v;
                this.att = att;
                this.off = off;
                this.on = on;
            }

            public override string ToString() => $"{name},\t {type.ToString()},\t {att.ToString()},\t {buff.ToString()},\t {buff_v},\t {off},\t {on}";
        }

        struct Song
        {
            public string name;
            public Att att;
            public int time;

            public Song(string name, Att att, int time)
            {
                this.name = name;
                this.att = att;
                this.time = time;
            }

            public override string ToString() => $"{name},\t {att.ToString()},\t {time}";
        }


        struct Note
        {
            int time;
            Type type;
            int length;
            int cnt;
        }

        void readSongCsv()
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("SongNote");
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".csv") == 0)
                {
                    Debug.WriteLine("D:{0}\t{1}", File.Name, File.FullName);
                    readNoteCsv(File.Name);
                }
            }
        }

        List<Note> readNoteCsv(string s)
        {
            List<Note> lnt = new List<Note>();
            using (StreamReader sr = new StreamReader(@"SongNote\" + s))
            {
                while (!sr.EndOfStream)
                {
                    string[] temp = sr.ReadLine().Split('\t');        // Split() 메서드를 이용하여 ',' 구분하여 잘라냄
                    //주석 회피
                    if (temp[0].Substring(0, 1).Equals("#"))
                    {
                        continue;
                    }
                    //string s = sr.ReadLine();
                    //Console.WriteLine("C:{0}\t{1}\t{2}\t{3}", temp[0], temp[1], temp[2], temp[3]);
                    Debug.WriteLine("D:{0}\t{1}\t{2}\t{3}", temp[0], temp[1], temp[2], temp[3]);
                }
            }
            return lnt;
        }

        public Form1()
        {
            InitializeComponent();

            //PM> Install-Package System.Text.Encoding.CodePages -Version 4.7.1
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);// 망할 닷넷 같으니

            readSongCsv();

            setChra();
            setSong();

            trackBar1.Maximum = 180;
            trackBar1.Value = 120;
            trackBar1.LargeChange = 1;

            // 콤보박스 세팅

            comboBox1.DataSource = ct;
            comboBox1.DisplayMember = "name";
            comboBox1.SelectedIndex = 0;

            comboBox2.DataSource = cl;
            comboBox2.DisplayMember = "name";
            comboBox2.SelectedIndex = 0;

            comboBox3.DataSource = cs;
            comboBox3.DisplayMember = "name";
            comboBox3.SelectedIndex = 0;

            comboBox4.DataSource = cp;
            comboBox4.DisplayMember = "name";
            comboBox4.SelectedIndex = 0;

            comboBox5.DataSource = so;
            comboBox5.DisplayMember = "name";
            comboBox5.SelectedIndex = 0;

            //pictureBox1.BackColor = Color.Black;

            time_somgSet();

            print();
            print2();
        }

        private static void setChra()
        {           
            using (StreamReader sr = new StreamReader(@"chra\chra.csv", euckr))//EUC-KR
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        string[] s = sr.ReadLine().Split(',');        // Split() 메서드를 이용하여 ',' 구분하여 잘라냄

                        Debug.Write( s.Length, ";\t");
                        for (int i = 0; i < s.Length; i++)
                        {
                            Debug.Write( s[i], "\t");
                        }
                        Debug.WriteLine(";\t");

                        int cl = 0;

                        //주석 회피. 이름
                        if (s[cl].Length != 0 && s[cl].Substring(0, 1).Equals("#"))
                        {
                            continue;
                        }

                        cl++;

                        // 속성
                        if (s[cl].Length == 0) continue;

                        Att att = Att.none;
                        switch (s[cl].Substring(0, 1))
                        {
                            case "v":
                            case "V":
                                att = Att.vocal;
                                break;
                            case "d":
                            case "D":
                                att = Att.dancer;
                                break;
                            case "s":
                            case "S":
                                att = Att.session;
                                break;
                            default:
                                break;
                        }

                        cl++;

                        // 타입
                        if (s[cl].Length == 0) continue;                        

                        Type type = Type.none;
                        switch (s[cl].Substring(0, 1))
                        {
                            case "t":
                            case "T":
                                type = Type.tap;
                                break;
                            case "l":
                            case "L":
                                type = Type.@long;
                                break;
                            case "s":
                            case "S":
                                type = Type.slide;
                                break;
                            case "f":
                            case "F":
                                type = Type.Flick;
                                break;
                            default:
                                break;
                        }

                        cl++;

                        // 버프
                        if (s[cl].Length == 0) continue;                        

                        Buff buff = Buff.etc;
                        switch (s[cl].Substring(0, 1))
                        {
                            case "c":
                            case "C":
                                buff = Buff.combo;
                                break;
                            case "a":
                            case "A":
                                buff = Buff.buff_a;
                                break;
                            case "t":
                            case "T":
                                buff = Buff.buff_t;
                                break;
                            case "l":
                            case "L":
                                buff = Buff.buff_l;
                                break;
                            case "s":
                            case "S":
                                buff = Buff.buff_s;
                                break;
                            case "f":
                            case "F":
                                buff = Buff.buff_f;
                                break;
                            default:
                                break;
                        }

                        cl++;

                        ca.Add(new Chra(s[0], att, type, buff, int.Parse(s[4]), int.Parse(s[5]), int.Parse(s[6])));

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    Debug.WriteLine("ca.Last:{0}", ca.Last());
                    Debug.WriteLine("ca.Count:{0}", ca.Count);
                }

            }

            // 정렬
            ca.Sort(delegate (Chra x, Chra y)
            {
                return x.name.CompareTo(y.name);
            });

            ct = ca.Where(Chra => Chra.type == Type.tap).ToList();
            cl = ca.Where(Chra => Chra.type == Type.@long).ToList();
            cs = ca.Where(Chra => Chra.type == Type.slide).ToList();
            cp = ca.Where(Chra => Chra.type == Type.Flick).ToList();
        }

        private static void setSong()
        {

            // 음악 세팅
            so = new List<Song>();

            so.Add(new Song(" ", Att.none, 180));
            so.Add(new Song("A.I", Att.dancer, 144));
            so.Add(new Song("Always", Att.dancer, 100));
            so.Add(new Song("Angel", Att.dancer, 124));
            so.Add(new Song("Angel (Hard, Expert)", Att.dancer, 127));
            so.Add(new Song("Astral Supplication", Att.dancer, 121));
            so.Add(new Song("Astro Fight", Att.dancer, 111));
            so.Add(new Song("Bad Elixir", Att.dancer, 128));
            so.Add(new Song("Beat U Down", Att.dancer, 91));
            so.Add(new Song("Binary World", Att.dancer, 115));
            so.Add(new Song("Black Water", Att.dancer, 140));
            so.Add(new Song("Black Water (Hard, Expert)", Att.dancer, 141));
            so.Add(new Song("Brain Power", Att.dancer, 110));
            so.Add(new Song("Cherokee", Att.dancer, 125));
            so.Add(new Song("Child of Night", Att.dancer, 105));
            so.Add(new Song("confict", Att.dancer, 136));
            so.Add(new Song("confict (Hard, Expert)", Att.dancer, 138));
            so.Add(new Song("Cosmic Elevator", Att.dancer, 144));
            so.Add(new Song("Cypher Gate", Att.dancer, 100));
            so.Add(new Song("Delta Spike", Att.dancer, 115));
            so.Add(new Song("Elastic Star", Att.dancer, 109));
            so.Add(new Song("End Of Fight (Round 2)", Att.dancer, 131));
            so.Add(new Song("End of the Moonlight", Att.dancer, 109));
            so.Add(new Song("Fight Night", Att.dancer, 108));
            so.Add(new Song("First Kiss", Att.dancer, 100));
            so.Add(new Song("FTR", Att.dancer, 141));
            so.Add(new Song("GAI∀PEX", Att.dancer, 116));
            so.Add(new Song("Get Down", Att.dancer, 111));
            so.Add(new Song("GET OUT", Att.dancer, 124));
            so.Add(new Song("GET OUT ~Hip Noodle Mix~", Att.dancer, 137));
            so.Add(new Song("Gient Step", Att.dancer, 141));
            so.Add(new Song("Groovin'Up", Att.dancer, 122));
            so.Add(new Song("Halcyon", Att.dancer, 155));
            so.Add(new Song("HELIX", Att.dancer, 138));
            so.Add(new Song("Here in the moment", Att.dancer, 95));
            so.Add(new Song("Here in the moment (Hard, Expert)", Att.dancer, 100));
            so.Add(new Song("HEXAD", Att.dancer, 94));
            so.Add(new Song("Kamigakari", Att.dancer, 127));
            so.Add(new Song("KILLER BEE", Att.dancer, 125));
            so.Add(new Song("Liar", Att.dancer, 130));
            so.Add(new Song("Light House", Att.dancer, 117));
            so.Add(new Song("Long Vacation", Att.dancer, 127));
            so.Add(new Song("Loving U", Att.dancer, 114));
            so.Add(new Song("Mulch", Att.dancer, 107));
            so.Add(new Song("Nature Fortress", Att.dancer, 114));
            so.Add(new Song("NB RANGER", Att.dancer, 118));
            so.Add(new Song("Nephthys", Att.dancer, 113));
            so.Add(new Song("Over The Rainbow", Att.dancer, 110));
            so.Add(new Song("Placebo Dying", Att.dancer, 149));
            so.Add(new Song("Placebo Dying (Easy)", Att.dancer, 144));
            so.Add(new Song("Ray of Illuminati", Att.dancer, 130));
            so.Add(new Song("Ready Now", Att.dancer, 94));
            so.Add(new Song("Revival On", Att.dancer, 147));
            so.Add(new Song("RockSTAR", Att.dancer, 113));
            so.Add(new Song("Shoreline", Att.dancer, 89));
            so.Add(new Song("SigNalize", Att.dancer, 107));
            so.Add(new Song("Silent Clarity", Att.dancer, 123));
            so.Add(new Song("Sinister Evolution", Att.dancer, 122));
            so.Add(new Song("Soda Pop City", Att.dancer, 115));
            so.Add(new Song("SON OF SUN", Att.dancer, 102));
            so.Add(new Song("SON OF SUN (Hard, Expert)", Att.dancer, 104));
            so.Add(new Song("Space Corridor", Att.dancer, 102));
            so.Add(new Song("Space of Soul", Att.dancer, 144));
            so.Add(new Song("Splash Dreams", Att.dancer, 114));
            so.Add(new Song("SQUEEZE", Att.dancer, 135));
            so.Add(new Song("Stay with Me", Att.dancer, 121));
            so.Add(new Song("STOP", Att.dancer, 124));
            so.Add(new Song("Super Lovely", Att.dancer, 121));
            so.Add(new Song("Tales of Princess Chocolate", Att.dancer, 140));
            so.Add(new Song("Tales of Princess Chocolate (Easy)", Att.dancer, 136));
            so.Add(new Song("The Feeling", Att.dancer, 123));
            so.Add(new Song("The Sun Rising to Hope", Att.dancer, 118));
            so.Add(new Song("The Sun Rising to Hope (Hard, Expert)", Att.dancer, 120));
            so.Add(new Song("Thor", Att.dancer, 108));
            so.Add(new Song("Tok! Tok! Tok!", Att.dancer, 134));
            so.Add(new Song("U.A.D", Att.dancer, 122));
            so.Add(new Song("Uranium", Att.dancer, 134));
            so.Add(new Song("ViViT", Att.dancer, 125));
            so.Add(new Song("VORTEX", Att.dancer, 122));
            so.Add(new Song("VORTEX (Easy)", Att.dancer, 117));
            so.Add(new Song("Watch Your Step", Att.dancer, 94));
            so.Add(new Song("WhiteBlue", Att.dancer, 111));
            so.Add(new Song("너랑 있으면", Att.dancer, 123));
            so.Add(new Song("혜성(comet)", Att.dancer, 149));
            so.Add(new Song("A Song of Sixpence", Att.vocal, 89));
            so.Add(new Song("Across Starlight", Att.vocal, 144));
            so.Add(new Song("ASHITA NO ASHIOTO", Att.vocal, 158));
            so.Add(new Song("Ayatorite mau", Att.vocal, 125));
            so.Add(new Song("Burn It Down", Att.vocal, 101));
            so.Add(new Song("Bye Bye Love ~Nu Jazz Mix~", Att.vocal, 135));
            so.Add(new Song("Colours of Sorrow", Att.vocal, 95));
            so.Add(new Song("Cosmic Fantasy Lovesong", Att.vocal, 110));
            so.Add(new Song("Deborah ", Att.vocal, 121));
            so.Add(new Song("EGG", Att.vocal, 119));
            so.Add(new Song("Eternal Memory ~ 소녀의 꿈~", Att.vocal, 129));
            so.Add(new Song("Fallen Angel", Att.vocal, 117));
            so.Add(new Song("Fallen Angel (Hard, Expert)", Att.vocal, 120));
            so.Add(new Song("Fallin' In LUV", Att.vocal, 129));
            so.Add(new Song("Feel", Att.vocal, 127));
            so.Add(new Song("For the Ikarus", Att.vocal, 131));
            so.Add(new Song("Give Me 5", Att.vocal, 116));
            so.Add(new Song("GoodBye", Att.vocal, 113));
            so.Add(new Song("Hello Pinky", Att.vocal, 128));
            so.Add(new Song("I Want You", Att.vocal, 105));
            so.Add(new Song("In My Heart", Att.vocal, 98));
            so.Add(new Song("Jump to Summer", Att.vocal, 152));
            so.Add(new Song("Ladymade Star", Att.vocal, 102));
            so.Add(new Song("Leave me Alone", Att.vocal, 111));
            so.Add(new Song("LENA Revolution", Att.vocal, 124));
            so.Add(new Song("LET UP", Att.vocal, 131));
            so.Add(new Song("Love☆Panic", Att.vocal, 127));
            so.Add(new Song("Lover (BS Style)", Att.vocal, 86));
            so.Add(new Song("Luv Flow", Att.vocal, 129));
            so.Add(new Song("Luv is Ture", Att.vocal, 110));
            so.Add(new Song("Mellow D Fantasy", Att.vocal, 133));
            so.Add(new Song("Melody", Att.vocal, 99));
            so.Add(new Song("Memoirs", Att.vocal, 131));
            so.Add(new Song("My Jealousy", Att.vocal, 149));
            so.Add(new Song("Never Say", Att.vocal, 122));
            so.Add(new Song("One the Love", Att.vocal, 132));
            so.Add(new Song("Only for you", Att.vocal, 136));
            so.Add(new Song("Only for you (Hard, Expert)", Att.vocal, 139));
            so.Add(new Song("Over The Horizon", Att.vocal, 145));
            so.Add(new Song("Private Party", Att.vocal, 133));
            so.Add(new Song("Raise Me Up", Att.vocal, 123));
            so.Add(new Song("RED", Att.vocal, 123));
            so.Add(new Song("Remember", Att.vocal, 105));
            so.Add(new Song("Rhythm", Att.vocal, 114));
            so.Add(new Song("Running girl", Att.vocal, 105));
            so.Add(new Song("Saturday", Att.vocal, 132));
            so.Add(new Song("Secret", Att.vocal, 110));
            so.Add(new Song("Secret Dejavu", Att.vocal, 122));
            so.Add(new Song("Shadow Flower", Att.vocal, 115));
            so.Add(new Song("Showtime", Att.vocal, 123));
            so.Add(new Song("Silver Bullet", Att.vocal, 145));
            so.Add(new Song("sO mUCH iN LUV", Att.vocal, 130));
            so.Add(new Song("Someday", Att.vocal, 106));
            so.Add(new Song("Streetlight", Att.vocal, 119));
            so.Add(new Song("Sunny Side", Att.vocal, 148));
            so.Add(new Song("Sweet Shining Shooting Star", Att.vocal, 96));
            so.Add(new Song("Take On Me", Att.vocal, 122));
            so.Add(new Song("The Clear Blue Sky", Att.vocal, 87));
            so.Add(new Song("Trip", Att.vocal, 111));
            so.Add(new Song("Twinkle Star", Att.vocal, 111));
            so.Add(new Song("U-NIVUS", Att.vocal, 129));
            so.Add(new Song("Visual Dream II (In Fiction)", Att.vocal, 148));
            so.Add(new Song("Wake Up feat.a-m", Att.vocal, 130));
            so.Add(new Song("Wintermute", Att.vocal, 135));
            so.Add(new Song("Y (CE STYLE)", Att.vocal, 101));
            so.Add(new Song("You & Me", Att.vocal, 102));
            so.Add(new Song("Your Own Miracle", Att.vocal, 77));
            so.Add(new Song("너를 꿈꾸며", Att.vocal, 124));
            so.Add(new Song("바람에게 부탁해", Att.vocal, 107));
            so.Add(new Song("바람의 기억", Att.vocal, 146));
            so.Add(new Song("별, 하늘, 그대", Att.vocal, 160));
            so.Add(new Song("별, 하늘, 그대 (Hard, Expert)", Att.vocal, 164));
            so.Add(new Song("별빛정원", Att.vocal, 114));
            so.Add(new Song("비행", Att.vocal, 158));
            so.Add(new Song("설레임", Att.vocal, 124));
            so.Add(new Song("아침형 인간", Att.vocal, 134));
            so.Add(new Song("유령", Att.vocal, 117));
            so.Add(new Song("작은 꿈", Att.vocal, 154));
            so.Add(new Song("징글벨", Att.vocal, 93));
            so.Add(new Song("징글벨 (Hard, Expert)", Att.vocal, 98));
            so.Add(new Song("Bingo", Att.session, 108));
            so.Add(new Song("Black Swan", Att.session, 109));
            so.Add(new Song("BlythE", Att.session, 129));
            so.Add(new Song("Boom!", Att.session, 90));
            so.Add(new Song("Brandnew Days", Att.session, 142));
            so.Add(new Song("BUCK WILD", Att.session, 115));
            so.Add(new Song("Can We Talk (Broken Dog Leg Mix)", Att.session, 136));
            so.Add(new Song("CANNON 2017", Att.session, 148));
            so.Add(new Song("D2", Att.session, 111));
            so.Add(new Song("Dear My Lady", Att.session, 92));
            so.Add(new Song("Devil's Whispering", Att.session, 135));
            so.Add(new Song("DIVINE SERVICE", Att.session, 122));
            so.Add(new Song("Don't Die", Att.session, 124));
            so.Add(new Song("Don't Die (Hard, Expert)", Att.session, 126));
            so.Add(new Song("Dream it", Att.session, 131));
            so.Add(new Song("Driving Lazy Bee", Att.session, 89));
            so.Add(new Song("Dual Strikers", Att.session, 106));
            so.Add(new Song("Enemy Storm", Att.session, 122));
            so.Add(new Song("Eternal Damnation", Att.session, 134));
            so.Add(new Song("Fentanest", Att.session, 121));
            so.Add(new Song("Festa Nova", Att.session, 121));
            so.Add(new Song("Fly Away", Att.session, 128));
            so.Add(new Song("Get On Top", Att.session, 126));
            so.Add(new Song("Glory Day", Att.session, 131));
            so.Add(new Song("Grid System", Att.session, 136));
            so.Add(new Song("Haikai, Kingyobachi no sokokara", Att.session, 130));
            so.Add(new Song("Heaven's vengeance", Att.session, 136));
            so.Add(new Song("HELIOS", Att.session, 129));
            so.Add(new Song("Higher", Att.session, 105));
            so.Add(new Song("Humming Bird", Att.session, 117));
            so.Add(new Song("Jibun kanpai", Att.session, 126));
            so.Add(new Song("Keys to the World", Att.session, 125));
            so.Add(new Song("Korobeiniki", Att.session, 112));
            so.Add(new Song("La Campabella : Nu Rave", Att.session, 108));
            so.Add(new Song("Lemonade", Att.session, 133));
            so.Add(new Song("Mess it Up", Att.session, 129));
            so.Add(new Song("Midnight Blood", Att.session, 137));
            so.Add(new Song("Miles", Att.session, 124));
            so.Add(new Song("Miles (Hard, Expert)", Att.session, 129));
            so.Add(new Song("Mind Control", Att.session, 143));
            so.Add(new Song("OBLIVION", Att.session, 118));
            so.Add(new Song("OBLIVION (Easy)", Att.session, 117));
            so.Add(new Song("Out Law", Att.session, 119));
            so.Add(new Song("Persona", Att.session, 137));
            so.Add(new Song("Rage of Demon", Att.session, 113));
            so.Add(new Song("Rolling On the Duck", Att.session, 139));
            so.Add(new Song("Runaway", Att.session, 124));
            so.Add(new Song("Ruti'n", Att.session, 99));
            so.Add(new Song("Ruti'n Electro Remix", Att.session, 115));
            so.Add(new Song("Ruti'n Electro Remix (Easy)", Att.session, 113));
            so.Add(new Song("SIN", Att.session, 114));
            so.Add(new Song("StarFish", Att.session, 132));
            so.Add(new Song("Sunset Rider", Att.session, 128));
            so.Add(new Song("SuperSonic", Att.session, 104));
            so.Add(new Song("The Party", Att.session, 125));
            so.Add(new Song("Toys Festival", Att.session, 136));
            so.Add(new Song("Urban Night", Att.session, 143));
            so.Add(new Song("VERUM - oremus", Att.session, 133));
            so.Add(new Song("Waiting for you", Att.session, 125));
            so.Add(new Song("Weather Auction", Att.session, 113));
            so.Add(new Song("Ya! Party!", Att.session, 97));
            so.Add(new Song("고백, 꽃, 늑대 Part.2", Att.session, 95));
            so.Add(new Song("고백, 꽃, 늑대 Part.2 (Hard, Expert)", Att.session, 108));
            so.Add(new Song("바람에게 부탁해 Live Mix", Att.session, 104));
            so.Add(new Song("태권부리", Att.session, 119));
            so.Add(new Song("피아노 협주곡 1번", Att.session, 120));

            // 정렬
            so.Sort(delegate (Song x, Song y)
            {
                return x.name.CompareTo(y.name);
            });
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            chraSelectedIndexChanged(sender, ct, label1);
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            chraSelectedIndexChanged(sender, cl, label2);
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            chraSelectedIndexChanged(sender, cs, label3);
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            chraSelectedIndexChanged(sender, cp, label4);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            time_somgSet();
            charBuffCalc();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1) return;
            if (string.IsNullOrWhiteSpace(((ComboBox)sender).Text)) return;
            time_song = ((Song)((ComboBox)sender).SelectedItem).time;
            label0.Text = string.Format("{0:D2}:{1:D2}", time_song / 60, time_song % 60);
            trackBar1.Value = time_song;
            ((ComboBox)sender).BackColor = getColorAtt(((Song)((ComboBox)sender).SelectedItem).att);
            charBuffCalc();

        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            boxColor<Chra>(sender, e);
        }
        private void comboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            boxColor<Chra>(sender, e);
        }
        private void comboBox3_DrawItem(object sender, DrawItemEventArgs e)
        {
            boxColor<Chra>(sender, e);
        }
        private void comboBox4_DrawItem(object sender, DrawItemEventArgs e)
        {
            boxColor<Chra>(sender, e);
        }
        private void comboBox5_DrawItem(object sender, DrawItemEventArgs e)
        {
            boxColor<Song>(sender, e);
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {

        }

        private void comboBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {

        }

        /// <summary>
        /// 캐릭 및 곡을 종합해서 합산 계산
        /// </summary>
        void charBuffCalc()
        {
            // 값 초기화
            sum = 0;
            sum_t = 0;
            sum_l = 0;
            sum_s = 0;
            sum_f = 0;
            cnt = 0;
            buff_va = Enumerable.Repeat(0, time_song).ToArray();
            buff_ta = Enumerable.Repeat(0, time_song).ToArray();
            buff_la = Enumerable.Repeat(0, time_song).ToArray();
            buff_sa = Enumerable.Repeat(0, time_song).ToArray();
            buff_fa = Enumerable.Repeat(0, time_song).ToArray();
            buff_v = Enumerable.Repeat(0, time_song).ToArray();
            buff_t = Enumerable.Repeat(0, time_song).ToArray();
            buff_l = Enumerable.Repeat(0, time_song).ToArray();
            buff_s = Enumerable.Repeat(0, time_song).ToArray();
            buff_f = Enumerable.Repeat(0, time_song).ToArray();
            buff_c = Enumerable.Repeat(false, time_song).ToArray();

            charBuffCalcSub(comboBox1);
            charBuffCalcSub(comboBox2);
            charBuffCalcSub(comboBox3);
            charBuffCalcSub(comboBox4);

            for (int i = 0; i < time_song; i++)
            {
                buff_va[i] = buff_v[i] + 100;
                buff_ta[i] = buff_t[i] + 100;
                buff_la[i] = buff_l[i] + 100;
                buff_sa[i] = buff_s[i] + 100;
                buff_fa[i] = buff_f[i] + 100;

                if (buff_c[i])
                {
                    buff_va[i] *= 2;
                    buff_ta[i] *= 2;
                    buff_la[i] *= 2;
                    buff_sa[i] *= 2;
                    buff_fa[i] *= 2;
                    cnt++;
                }
                sum_t += buff_ta[i];
                sum_l += buff_la[i];
                sum_s += buff_sa[i];
                sum_f += buff_fa[i];
                sum += buff_va[i];

            }
            label5.Text = string.Format("{0}", sum);
            label6.Text = string.Format("{0:N2}", (double)sum_t / (double)time_song);
            label7.Text = string.Format("{0:N2}", (double)sum_l / (double)time_song);
            label8.Text = string.Format("{0:N2}", (double)sum_s / (double)time_song);
            label9.Text = string.Format("{0:N2}", (double)sum_f / (double)time_song);
            label10.Text = string.Format("{0:N2}", (double)cnt / (double)time_song);

            print();
            print2();
        }

        /// <summary>
        /// 캐릭의 시간 단위마다 버프 값 배열 설정.
        /// 만약 타입별 버프 필요하다면 여기서 수정 필요
        /// </summary>
        /// <param name="c"></param>
        private static void charBuffCalcSub(ComboBox comboBox)
        {
            if (!(comboBox.SelectedIndex >= 0))
                return;

            Chra c = (Chra)comboBox.SelectedItem;

            switch (c.buff)
            {
                case Buff.combo:
                    for (int i = 0; i < time_song; i++)
                    {
                        if (i % (c.on + c.off) > c.off)
                        {
                            buff_c[i] = true;
                        }
                    }
                    break;
                case Buff.buff:
                    buffCul(c, ref buff_v);
                    break;
                case Buff.buff_a:
                    buffCul(c, ref buff_t);
                    buffCul(c, ref buff_l);
                    buffCul(c, ref buff_s);
                    buffCul(c, ref buff_f);
                    buffCul(c, ref buff_v);
                    break;
                case Buff.buff_t:
                    buffCul(c, ref buff_t);
                    buffCul(c, ref buff_v);
                    break;
                case Buff.buff_l:
                    buffCul(c, ref buff_l);
                    buffCul(c, ref buff_v);
                    break;
                case Buff.buff_s:
                    buffCul(c, ref buff_s);
                    buffCul(c, ref buff_v);
                    break;
                case Buff.buff_f:
                    buffCul(c, ref buff_f);
                    buffCul(c, ref buff_v);
                    break;
                case Buff.etc:
                    break;
                default:
                    break;
            }

        }

        private static void buffCul(Chra c, ref int[] b)
        {
            for (int i = 0; i < time_song; i++)
            {
                if (i % (c.on + c.off) > c.off)
                {
                    b[i] += c.buff_v;
                }
            }
        }

        void print2()
        {
            graphics2.Clear(Color.Black);

            //1초마다 세로줄
            for (int i = 0; i < time_song; i += 1)
            {
                graphics2.FillRectangle(Brushes.White, i * (time_px) + time_px - 1, 0, 1, 100);
            }
            //5초마다
            for (int i = 4; i < time_song; i += 5)
            {
                graphics2.FillRectangle(Brushes.Red, i * (time_px) + time_px - 1, 0, 1, 100);
            }

            // 가로줄
            graphics2.FillRectangle(Brushes.White, 0, 0 + 19, time_song * time_px, 1);
            graphics2.FillRectangle(Brushes.White, 0, 20 + 19, time_song * time_px, 1);
            graphics2.FillRectangle(Brushes.White, 0, 40 + 19, time_song * time_px, 1);
            graphics2.FillRectangle(Brushes.White, 0, 60 + 19, time_song * time_px, 1);
            graphics2.FillRectangle(Brushes.White, 0, 80 + 19, time_song * time_px, 1);
            //SolidBrush s=new SolidBrush(Color.FromArgb(0, 0, 0));

            //칸 색칠
            for (int i = 0; i < time_song; i++)
            {
                //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t", buff_ta[i], buff_la[i],  buff_sa[i], buff_fa[i]);
                //Debug.WriteLine("{0}\t{1}\t{2}\t{3}\t", buff_ta[i], buff_la[i],  buff_sa[i], buff_fa[i]);
                graphics2.FillRectangle(getChraBursh(buff_c[i], buff_t[i] > 0), i * (time_px), 0, time_px - 1, 19);
                graphics2.FillRectangle(getChraBursh(buff_c[i], buff_l[i] > 0), i * (time_px), 20, time_px - 1, 19);
                graphics2.FillRectangle(getChraBursh(buff_c[i], buff_s[i] > 0), i * (time_px), 40, time_px - 1, 19);
                graphics2.FillRectangle(getChraBursh(buff_c[i], buff_f[i] > 0), i * (time_px), 60, time_px - 1, 19);
                if (buff_c[i])
                {
                    graphics2.FillRectangle(Brushes.Yellow, i * (time_px), 80, time_px - 1, 19);
                }
            }

            pictureBox2.Image = bitmap2;
        }

        private Brush getChraBursh(bool c, bool b)
        {
            //solidBrush.Color = Color.FromArgb(r, g, b);
            if (c && b) return Brushes.LightBlue;
            if (c && !b) return Brushes.Yellow;
            if (!c && b) return Brushes.GreenYellow;
            return Brushes.Black;
        }

        /// <summary>
        /// 세로 그래프 표시
        /// </summary>
        void print()
        {
            graphics.Clear(BackColor);
            // 100% 마다 가로줄
            for (int i = grp_h - (99 / grp_h_div); i >= 0; i -= (100 / grp_h_div))
            {
                graphics.FillRectangle(Brushes.Black, 0, i, time_song * time_px, 1);
            }

            // 초라인 표시
            for (int i = 4; i < time_song; i += 5)
            {
                graphics.FillRectangle(Brushes.White, i * time_px, 0, time_px, grp_h);
            }
            for (int i = 29; i < time_song; i += 30)
            {
                graphics.FillRectangle(Brushes.Black, i * time_px, 0, time_px, grp_h);
            }

            // 버프값에 따른 그래프
            for (int i = 0; i < time_song; i++)
            {
                graphics.FillRectangle(Brushes.Green, i * time_px, grp_h - (buff_va[i] / grp_h_div), time_px, buff_va[i] / grp_h_div);
            }
            pictureBox1.Image = bitmap;
        }

        /// <summary>
        /// 캐릭목록 선택시 마다 반응
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="c"></param>
        /// <param name="label"></param>
        private void chraSelectedIndexChanged(object sender, List<Chra> c, Label label)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1)
            {
                label.Text = "";
                return;
            }
            if (string.IsNullOrWhiteSpace(comboBox.Text))
            {
                label.Text = "";
                return;
            }
            label.Text = c[comboBox.SelectedIndex].ToString();
            comboBox.BackColor = getColorAtt(((Chra)comboBox.SelectedItem).att);
            //label1.Text = comboBox1.Text; 
            charBuffCalc();
        }

        /// <summary>
        /// 시간 라벨에 출력
        /// </summary>
        private void time_somgSet()
        {
            time_song = trackBar1.Value;
            label0.Text = string.Format("{0:D2}:{1:D2}", time_song / 60, time_song % 60);
        }

        /// <summary>
        /// 콤보 리스트에서 속성에 따라 배경색 설정
        /// </summary>
        /// <typeparam name="T">Song,Chra인지 선택</typeparam>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void boxColor<T>(object sender, DrawItemEventArgs e)
        {
            //Console.WriteLine("comboBox1_DrawItem");
            //System.Diagnostics.Debug.WriteLine("boxColor");

            Att att;

            // 이러면 성능이 별로일거 같은데
            T checkType = default(T);
            if (checkType is Chra) { att = ((Chra)((ComboBox)sender).Items[e.Index]).att; }
            else if (checkType is Song) { att = ((Song)((ComboBox)sender).Items[e.Index]).att; }
            else { return; }

            e.DrawBackground();
            e.Graphics.FillRectangle(getBrushAtt(att), e.Bounds);
            e.Graphics.DrawString(((ComboBox)sender).Items[e.Index].ToString(), ((Control)sender).Font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
        }


        /// <summary>
        /// 속성별 색 얻기
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        private static Brush getBrushAtt(Att att)
        {
            switch (att)
            {
                case Att.dancer:
                    return Brushes.LightPink;

                case Att.session:
                    return Brushes.LightGreen;

                case Att.vocal:
                    return Brushes.LightBlue;

                case Att.none:
                    return Brushes.Yellow;

                default:
                    return Brushes.Brown;

            }
        }
        /// <summary>
        /// 속성별 색 얻기
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        private static Color getColorAtt(Att att)
        {
            switch (att)
            {
                case Att.dancer:
                    return Color.LightPink;

                case Att.session:
                    return Color.LightGreen;

                case Att.vocal:
                    return Color.LightBlue;

                case Att.none:
                    return Color.Yellow;

                default:
                    return Color.Brown;

            }
        }
    }
}
