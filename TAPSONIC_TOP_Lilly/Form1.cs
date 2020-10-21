using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TAPSONIC_TOP_Lilly
{
    public partial class Form1 : Form
    {
        static int time_px = 5;// 초당 가로 픽셀
        static int time_song = 180;// 음악 최대 시간
        static int grp_w = time_song* time_px;// 그래픽 크기
        static int grp_h_div = 2;//그래픽 배율 감소
        static int grp_h = 300;// 그래픽 크기
        static int[] buff_vall = Enumerable.Repeat(100, time_song).ToArray();
        static int[] buff_v = Enumerable.Repeat(100, time_song).ToArray();
        static bool[] buff_c = Enumerable.Repeat(false, time_song).ToArray();
        static int sum = 0;// 그래픽 크기

        static List<Chra> ca;
        static List<Chra> ct;
        static List<Chra> cl;
        static List<Chra> cs;
        static List<Chra> cp;

        static List<Song> so;

        static Font fnt = new Font("맑은 고딕", 16);
        //var fontf = new Font("맑은 고딕", 16, FontStyle.Regular, GraphicsUnit.Pixel);
        static Bitmap bitmap = new Bitmap(grp_w, grp_h);
        static Graphics graphics = Graphics.FromImage(bitmap);

        enum Att    // 속성
        {
            dancer,
            session,
            vocal,
            none
        }

        enum Type    // 타입
        {
            tap,
            @long,
            slide,
            plit,
            none
        }

        enum Buff
        {
            combo,
            buff,
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
                this.on = on ;
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



        public Form1()
        {
            InitializeComponent();

            ca = new List<Chra>();

            // 캐릭 세팅            
            ca.Add(new Chra(" ", Att.none, Type.none, Buff.etc, 100, 10, 10));
            ca.Add(new Chra(" ", Att.none, Type.tap, Buff.etc, 100, 10, 10));
            ca.Add(new Chra(" ", Att.none, Type.@long, Buff.etc, 100, 10, 10));
            ca.Add(new Chra(" ", Att.none, Type.slide, Buff.etc, 100, 10, 10));
            ca.Add(new Chra(" ", Att.none, Type.plit, Buff.etc, 100, 10, 10));
            ca.Add(new Chra("파트라10", Att.dancer, Type.tap, Buff.combo, 100, 15, 13));
            ca.Add(new Chra("엘리10", Att.vocal, Type.tap, Buff.combo, 100, 18, 14));
            ca.Add(new Chra("제시10", Att.vocal, Type.tap, Buff.combo, 100, 12, 13));
            ca.Add(new Chra("케르밀라10", Att.vocal, Type.tap, Buff.buff, 73, 11, 19));
            ca.Add(new Chra("혁10", Att.vocal, Type.tap, Buff.buff, 101, 15, 20));
            ca.Add(new Chra("이브10", Att.vocal, Type.tap, Buff.buff, 105, 10, 12));
            ca.Add(new Chra("혀니10", Att.vocal, Type.tap, Buff.buff, 85, 7, 6));
            ca.Add(new Chra("지크10", Att.vocal, Type.tap, Buff.etc, 9, 10, 25));
            ca.Add(new Chra("엘 클리어10", Att.vocal, Type.tap, Buff.combo, 100, 18, 10));
            ca.Add(new Chra("베아트리스10", Att.vocal, Type.@long, Buff.buff, 44, 11, 20));
            ca.Add(new Chra("아리아10", Att.vocal, Type.@long, Buff.combo, 100, 16, 10));
            ca.Add(new Chra("다이나10", Att.vocal, Type.@long, Buff.etc, 0, 9, 1));
            ca.Add(new Chra("하이틴 유리10", Att.vocal, Type.@long, Buff.buff, 100, 15, 14));
            ca.Add(new Chra("미로10", Att.vocal, Type.@long, Buff.buff, 90, 16, 11));
            ca.Add(new Chra("니콜10", Att.vocal, Type.slide, Buff.combo, 100, 15, 13));
            ca.Add(new Chra("파르티나10", Att.vocal, Type.slide, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("오키드10", Att.vocal, Type.slide, Buff.buff, 83, 10, 20));
            ca.Add(new Chra("스이10", Att.vocal, Type.slide, Buff.buff, 97, 10, 12));
            ca.Add(new Chra("슬래셔10", Att.vocal, Type.plit, Buff.buff, 68, 13, 22));
            ca.Add(new Chra("걸윙10", Att.vocal, Type.plit, Buff.combo, 100, 14, 10));
            ca.Add(new Chra("보니10", Att.vocal, Type.slide, Buff.combo, 100, 15, 8));
            ca.Add(new Chra("캐롤10", Att.dancer, Type.tap, Buff.buff, 72, 12, 20));
            ca.Add(new Chra("셜리10", Att.dancer, Type.tap, Buff.combo, 100, 17, 10));
            ca.Add(new Chra("데몬10", Att.dancer, Type.tap, Buff.buff, 94, 17, 15));
            ca.Add(new Chra("로아10", Att.dancer, Type.tap, Buff.buff, 94, 16, 16));
            ca.Add(new Chra("시테10", Att.dancer, Type.tap, Buff.etc, 0, 11, 1));
            ca.Add(new Chra("하이틴 제이10", Att.dancer, Type.tap, Buff.etc, 0, 14, 1));
            ca.Add(new Chra("발렌타인10", Att.dancer, Type.@long, Buff.buff, 99, 12, 20));
            ca.Add(new Chra("장고10", Att.dancer, Type.@long, Buff.buff, 40, 10, 20));
            ca.Add(new Chra("라파엘10", Att.dancer, Type.@long, Buff.combo, 100, 16, 18));
            ca.Add(new Chra("도로시10", Att.dancer, Type.@long, Buff.buff, 88, 12, 15));
            ca.Add(new Chra("웨니10", Att.dancer, Type.@long, Buff.buff, 80, 18, 17));
            ca.Add(new Chra("크라켄10", Att.dancer, Type.@long, Buff.combo, 100, 16, 10));
            ca.Add(new Chra("스페이시10", Att.dancer, Type.@long, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("아수라10", Att.dancer, Type.slide, Buff.buff, 112, 12, 15));
            ca.Add(new Chra("호련(교)10", Att.dancer, Type.slide, Buff.combo, 100, 15, 15));
            ca.Add(new Chra("호련(교)9", Att.dancer, Type.slide, Buff.combo, 100, 15, 13));
            ca.Add(new Chra("호련10", Att.dancer, Type.slide, Buff.combo, 100, 15, 12));
            ca.Add(new Chra("호련9", Att.dancer, Type.slide, Buff.combo, 100, 15, 10));
            ca.Add(new Chra("스테파니10", Att.dancer, Type.slide, Buff.etc, 7, 10, 24));
            ca.Add(new Chra("토미 슐츠10", Att.dancer, Type.slide, Buff.buff, 89, 14, 14));
            ca.Add(new Chra("엘 페일10", Att.dancer, Type.slide, Buff.buff, 93, 14, 15));
            ca.Add(new Chra("벨10", Att.dancer, Type.slide, Buff.combo, 100, 21, 10));
            ca.Add(new Chra("아폴로10", Att.dancer, Type.slide, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("로제10", Att.dancer, Type.plit, Buff.buff, 71, 13, 22));
            ca.Add(new Chra("카론10", Att.dancer, Type.plit, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("아레스무스10", Att.dancer, Type.plit, Buff.buff, 68, 17, 24));
            ca.Add(new Chra("하이틴 세나10", Att.dancer, Type.plit, Buff.buff, 68, 15, 15));
            ca.Add(new Chra("엔젤라10", Att.dancer, Type.plit, Buff.etc, 0, 14, 1));
            ca.Add(new Chra("메두사10", Att.session, Type.tap, Buff.etc, 20, 10, 30));
            ca.Add(new Chra("아르웬10", Att.session, Type.tap, Buff.buff, 72, 12, 18));
            ca.Add(new Chra("루시퍼10", Att.session, Type.tap, Buff.combo, 100, 15, 14));
            ca.Add(new Chra("조크 에리스갓10", Att.session, Type.tap, Buff.buff, 90, 15, 17));
            ca.Add(new Chra("조이10", Att.session, Type.tap, Buff.buff, 35, 15, 15));
            ca.Add(new Chra("베로니카10", Att.session, Type.tap, Buff.buff, 99, 16, 16));
            ca.Add(new Chra("촙스10", Att.session, Type.tap, Buff.etc, 0, 9, 1));
            ca.Add(new Chra("트래셔 박사10", Att.session, Type.@long, Buff.combo, 100, 19, 15));
            ca.Add(new Chra("프랑켄10", Att.session, Type.@long, Buff.etc, 0, 14, 1));
            ca.Add(new Chra("타냐10", Att.session, Type.@long, Buff.buff, 89, 10, 18));
            ca.Add(new Chra("액슬10", Att.session, Type.@long, Buff.etc, 7, 15, 27));
            ca.Add(new Chra("재규어10", Att.session, Type.@long, Buff.combo, 100, 15, 11));
            ca.Add(new Chra("포에트10", Att.session, Type.@long, Buff.buff, 80, 15, 20));
            ca.Add(new Chra("우리엘10", Att.session, Type.@long, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("티타나10", Att.session, Type.slide, Buff.buff, 108, 11, 14));
            ca.Add(new Chra("패리스 업튼10", Att.session, Type.slide, Buff.combo, 100, 18, 14));
            ca.Add(new Chra("신시아10", Att.session, Type.slide, Buff.buff, 79, 15, 20));
            ca.Add(new Chra("오하나10", Att.session, Type.slide, Buff.buff, 52, 10, 20));
            ca.Add(new Chra("에이바10", Att.session, Type.slide, Buff.buff, 97, 10, 12));
            ca.Add(new Chra("하이틴 체르니10", Att.session, Type.slide, Buff.combo, 100, 17, 10));
            ca.Add(new Chra("볼프강10", Att.session, Type.plit, Buff.combo, 100, 10, 9));
            ca.Add(new Chra("루핑10", Att.session, Type.plit, Buff.buff, 400, 4, 4));
            ca.Add(new Chra("지니10", Att.session, Type.plit, Buff.etc, 0, 12, 1));
            ca.Add(new Chra("에이미10", Att.session, Type.plit, Buff.etc, 0, 11, 1));
            ca.Add(new Chra("탈리아10", Att.session, Type.plit, Buff.etc, 0, 0, 300));



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
            ca.Sort(delegate (Chra x, Chra y)
            {
                return x.name.CompareTo(y.name);
            });

            // 정렬
            so.Sort(delegate (Song x, Song y)
            {
                return x.name.CompareTo(y.name);
            });

            ct = ca.Where(Chra => Chra.type == Type.tap).ToList();
            cl = ca.Where(Chra => Chra.type == Type.@long).ToList();
            cs = ca.Where(Chra => Chra.type == Type.slide).ToList();
            cp = ca.Where(Chra => Chra.type == Type.plit).ToList();

            //comboBox2.Items.AddRange(cd2.Where(Chra => Chra.type == Type.t).Select(Chra=> Chra.name).ToArray());
            //comboBox1.Items.AddRange(ct.Select(Chra=> Chra.name).ToArray());
            //comboBox2.Items.AddRange(cl.Select(Chra => Chra.name).ToArray());
            //comboBox3.Items.AddRange(cs.Select(Chra => Chra.name).ToArray());
            //comboBox4.Items.AddRange(cp.Select(Chra => Chra.name).ToArray());

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

            trackBar1.Maximum = 180;
            trackBar1.Value = 120;
            trackBar1.LargeChange = 1;

            //comboBox5.Items.AddRange(so.Select(Song=> Song.name).ToArray());
            comboBox5.DataSource = so;
            comboBox5.DisplayMember = "name";
            comboBox5.SelectedIndex = 0;



            pictureBox1.BackColor = Color.Black;

            time_somgSet();
            print();            
        }

        /// <summary>
        /// 캐릭 및 곡을 종합해서 합산 계산
        /// </summary>
        void charBuffCalc()
        {
            sum = 0;
            buff_vall = Enumerable.Repeat(0, time_song).ToArray();
            buff_v = Enumerable.Repeat(0, time_song).ToArray();
            buff_c = Enumerable.Repeat(false, time_song).ToArray();

            if (comboBox1.SelectedIndex >= 0)
            {
                //charBuffCalcSub(ct[comboBox1.SelectedIndex]);            
                charBuffCalcSub((Chra)comboBox1.SelectedItem);            
            }
            if(comboBox2.SelectedIndex>=0)
                charBuffCalcSub((Chra)comboBox2.SelectedItem);
            if (comboBox3.SelectedIndex>=0)
                charBuffCalcSub((Chra)comboBox3.SelectedItem);
            if (comboBox4.SelectedIndex>=0)
                charBuffCalcSub((Chra)comboBox4.SelectedItem);

            for (int i = 0; i < time_song; i++)
            {
                if (buff_c[i])
                {
                    buff_vall[i] = (buff_v[i]+100) * 2;
                }
                else
                {
                    buff_vall[i] = buff_v[i] + 100;
                }
                //System.Diagnostics.Debug.WriteLine(buff_vall[i]);
                sum += buff_vall[i];
            }
            label5.Text= string.Format("{0}", sum);
            print();
        }

        /// <summary>
        /// 캐릭의 시간 단위마다 버프 값 배열 설정.
        /// 만약 타입별 버프 필요하다면 여기서 수정 필요
        /// </summary>
        /// <param name="c"></param>
        private static void charBuffCalcSub( Chra c)
        {
            if (c.buff == Buff.buff)
            {
                for (int i = 0; i < time_song; i++)
                {
                    if (i % (c.on+c.off) > c.off)
                    {
                        buff_v[i] += c.buff_v;
                    }
                }
            }

            if (c.buff == Buff.combo)
            {
                for (int i = 0; i < time_song; i++)
                {
                    if (i % (c.on + c.off) > c.off)
                    {
                        buff_c[i] = true;
                    }
                }
            }
        }

        /// <summary>
        /// 그래프 표시
        /// </summary>
        void print()
        {
            graphics.Clear(BackColor);
            for (int i = grp_h-(99/ grp_h_div); i >=0 ; i-=(100/ grp_h_div))
            {
                graphics.FillRectangle(Brushes.Black,0,i, time_song* time_px,1);
            }
            for (int i = 0; i < time_song; i++)
            {
                graphics.FillRectangle(Brushes.Green,i* time_px, grp_h-(buff_vall[i]/ grp_h_div),time_px, buff_vall[i] / grp_h_div);
            }
            pictureBox1.Image = bitmap;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.Write(comboBox1.SelectedIndex);
            //Console.WriteLine(comboBox1.Text);
            chraSelectedIndexChanged(sender, ct,label1);

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            chraSelectedIndexChanged(sender, cl,label2);
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            chraSelectedIndexChanged(sender, cs,label3);
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            chraSelectedIndexChanged(sender, cp,label4);
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

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            time_somgSet();
            charBuffCalc();
        }

        private void time_somgSet()
        {
            int t = trackBar1.Value;
            time_song = trackBar1.Value;
            label0.Text = string.Format("{0:D2}:{1:D2}", t / 60, t % 60);
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
            Console.WriteLine("comboBox1_MeasureItem");
            //System.Diagnostics.Debug.WriteLine("comboBox1_MeasureItem");
        }
    }
}
