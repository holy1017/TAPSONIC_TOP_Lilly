using ClosedXML.Excel;
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



// 닷넷 코어 3.0 런타임이 필요합니다
// https://dotnet.microsoft.com/download/dotnet-core/current/runtime

// 소스 코드
// https://github.com/holy1017/TAPSONIC_TOP_Lilly﻿

namespace TAPSONIC_TOP_Lilly
{
    public partial class Form1 : Form
    {
        static Encoding euckr = Encoding.GetEncoding(51949);
        static char[] split={ ',','\t',';'};
        
        static int time_px = 5;// 초당 가로 픽셀
        static int time_song_max = 180;// 음악 최대 시간
        static int time_song = 180;// 음악 최대 시간

        static int[] buff_v = Enumerable.Repeat(0, time_song_max).ToArray();//버프값. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_t = Enumerable.Repeat(0, time_song_max).ToArray();//버프값 탭. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_l = Enumerable.Repeat(0, time_song_max).ToArray();//버프값 롱. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_s = Enumerable.Repeat(0, time_song_max).ToArray();//버프값 슬라. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_f = Enumerable.Repeat(0, time_song_max).ToArray();//버프값 플릭. 만약 타입별로 한다면 이걸 분할해야함.

        static int[] buff_va = Enumerable.Repeat(0, time_song_max).ToArray();//버프 기본값 및 콤보 적용
        static int[] buff_ta = Enumerable.Repeat(0, time_song_max).ToArray();//버프값 탭. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_la = Enumerable.Repeat(0, time_song_max).ToArray();//버프값 롱. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_sa = Enumerable.Repeat(0, time_song_max).ToArray();//버프값 슬라. 만약 타입별로 한다면 이걸 분할해야함.
        static int[] buff_fa = Enumerable.Repeat(0, time_song_max).ToArray();//버프값 플릭. 만약 타입별로 한다면 이걸 분할해야함.
        
        static bool[] buff_c = Enumerable.Repeat(false, time_song_max).ToArray();//콤보 유무
        static int[] combo_cum = Enumerable.Repeat(0, time_song_max).ToArray();//각 시간별 콤보 갯수 누적

        static int song_avg = 0;// 
        static int sum = 0;// 
        static int sum_t = 0;// 
        static int sum_l = 0;// 
        static int sum_s = 0;// 
        static int sum_f = 0;// 
        static int combo_cnt = 0;// 임시 변수

        static List<Chra> chra_a= new List<Chra>();//모든캐릭
        static List<Chra> chra_t;//탭캐릭. 분류 코드로 자동 처리
        static List<Chra> chra_l;//탭캐릭. 분류 코드로 자동 처리
        static List<Chra> chra_s;//탭캐릭. 분류 코드로 자동 처리
        static List<Chra> chra_f;//탭캐릭. 분류 코드로 자동 처리

        static List<Song> song=new List<Song>();//음악 목록
        //Dictionary<string, List<Note>> openWith = new Dictionary<string, List<Note>>();

        static Font fnt = new Font("맑은 고딕", 16);
        //var fontf = new Font("맑은 고딕", 16, FontStyle.Regular, GraphicsUnit.Pixel);

        static int grp_w = time_song_max * time_px;// 그래픽 가로 크기
        static int grp_h_div = 4;//그래픽 배율 감소
        static int grp_h = 150;// 그래픽 세로 크기

        // 세로 그래프용
        static Bitmap bitmap = new Bitmap(grp_w, grp_h);
        static Graphics graphics = Graphics.FromImage(bitmap);

        // 가로 그래프용
        static Bitmap bitmap2 = new Bitmap(grp_w, grp_h);
        static Graphics graphics2 = Graphics.FromImage(bitmap2);
        //static SolidBrush solidBrush = new SolidBrush(Color.FromArgb(0, 0, 0));

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

        void ReadSongCsv()
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@"CSV\Note");
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".csv") == 0)
                {
                    Debug.WriteLine("D:{0}\t{1}", File.Name, File.FullName);
                    ReadNoteCsv(File.Name);
                }
            }
        }

        List<Note> ReadNoteCsv(string s)
        {
            List<Note> lnt = new List<Note>();
            using (StreamReader sr = new StreamReader(@"CSV\Note\" + s))
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

            ReadSongCsv();

            SetChra();
            SetSong();

            trackBar1.Maximum = 180;
            trackBar1.Value = song_avg;
            trackBar1.LargeChange = 1;

            // 콤보박스 세팅

            comboBox1.DataSource = chra_t;
            comboBox1.DisplayMember = "name";
            comboBox1.SelectedIndex = 0;

            comboBox2.DataSource = chra_l;
            comboBox2.DisplayMember = "name";
            comboBox2.SelectedIndex = 0;

            comboBox3.DataSource = chra_s;
            comboBox3.DisplayMember = "name";
            comboBox3.SelectedIndex = 0;

            comboBox4.DataSource = chra_f;
            comboBox4.DisplayMember = "name";
            comboBox4.SelectedIndex = 0;

            comboBox5.DataSource = song;
            comboBox5.DisplayMember = "name";
            comboBox5.SelectedIndex = 0;

            //pictureBox1.BackColor = Color.Black;

            Time_somgSet();

            print();
            Print2();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChraSelectedIndexChanged(sender, chra_t, label1);
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChraSelectedIndexChanged(sender, chra_l, label2);
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChraSelectedIndexChanged(sender, chra_s, label3);
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChraSelectedIndexChanged(sender, chra_f, label4);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1) return;
            if (string.IsNullOrWhiteSpace(((ComboBox)sender).Text)) return;
            time_song = ((Song)((ComboBox)sender).SelectedItem).time;
            label0.Text = string.Format("{0:D2}:{1:D2}", time_song / 60, time_song % 60);
            trackBar1.Value = time_song;
            ((ComboBox)sender).BackColor = GetColorAtt(((Song)((ComboBox)sender).SelectedItem).att);
            CharBuffCalc();

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Time_somgSet();
            CharBuffCalc();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

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
        /// 캐릭목록 세팅
        /// </summary>
        private static void SetChra()
        {
            using (StreamReader sr = new StreamReader(@"CSV\chra.csv", euckr))//EUC-KR
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        string[] s = sr.ReadLine().Split(split);        // Split() 메서드를 이용하여 ',' 구분하여 잘라냄

                        Debug.Write(s.Length, ";\t");
                        for (int i = 0; i < s.Length; i++)
                        {
                            Debug.Write(s[i], "\t");
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

                        GetAtt(s[cl], out Att att);

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

                        chra_a.Add(new Chra(s[0], att, type, buff, int.Parse(s[4]), int.Parse(s[5]), int.Parse(s[6])));

                        Debug.WriteLine("ca.Last:{0}", chra_a.Last());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    Debug.WriteLine("ca.Count:{0}", chra_a.Count);
                }

            }

            // 정렬
            chra_a.Sort(delegate (Chra x, Chra y)
            {
                return x.name.CompareTo(y.name);
            });

            chra_t = chra_a.Where(Chra => Chra.type == Type.tap).ToList();
            chra_l = chra_a.Where(Chra => Chra.type == Type.@long).ToList();
            chra_s = chra_a.Where(Chra => Chra.type == Type.slide).ToList();
            chra_f = chra_a.Where(Chra => Chra.type == Type.Flick).ToList();
        }

        /// <summary>
        /// 음악 목록 세팅
        /// </summary>
        private static void SetSong()
        {
            using (StreamReader sr = new StreamReader(@"CSV\song.csv", euckr))//EUC-KR
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        string[] s = sr.ReadLine().Split(split);        // Split() 메서드를 이용하여 ',' 구분하여 잘라냄

                        Debug.Write(s.Length, ";\t");
                        for (int i = 0; i < s.Length; i++)
                        {
                            Debug.Write(s[i], "\t");
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

                        GetAtt(s[cl], out Att att);

                        cl++;

                        song.Add(new Song(s[0], att, int.Parse(s[2])));
                        song_avg += int.Parse(s[2]);
                        Debug.WriteLine("so.Last:{0}", song.Last());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    Debug.WriteLine("so.Count:{0}", song.Count);
                    song_avg /= song.Count;
                }

            }

            // 정렬
            song.Sort(delegate (Song x, Song y)
            {
                return x.name.CompareTo(y.name);
            });
        }

        /// <summary>
        /// 텍스트 앞자리 한글자만 따서 속성 설정
        /// </summary>
        /// <param name="s"></param>
        /// <param name="att"></param>
        private static void GetAtt(string s, out Att att)
        {
            switch (s.Substring(0, 1))
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
                    att = Att.none;
                    break;
            }
        }

        /// <summary>
        /// 캐릭 및 곡을 종합해서 합산 계산
        /// </summary>
        void CharBuffCalc()
        {
            // 값 초기화
            sum = 0;
            sum_t = 0;
            sum_l = 0;
            sum_s = 0;
            sum_f = 0;

            combo_cnt = 0;

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
            combo_cum = Enumerable.Repeat(0, time_song).ToArray();//각 시간별 콤보 갯수 누적

            CharBuffCalcSub(comboBox1);
            CharBuffCalcSub(comboBox2);
            CharBuffCalcSub(comboBox3);
            CharBuffCalcSub(comboBox4);

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
                    combo_cnt++;
                }
                sum_t += buff_ta[i];
                sum_l += buff_la[i];
                sum_s += buff_sa[i];
                sum_f += buff_fa[i];
                sum += buff_va[i];
                combo_cum[i] = combo_cnt;
            }
            label5.Text = string.Format("{0}", sum);

            label6.Text = string.Format("{0:N2}", (double)sum_t / (double)time_song);
            label7.Text = string.Format("{0:N2}", (double)sum_l / (double)time_song);
            label8.Text = string.Format("{0:N2}", (double)sum_s / (double)time_song);
            label9.Text = string.Format("{0:N2}", (double)sum_f / (double)time_song);

            label10.Text = string.Format("{0:N2}", (double)combo_cnt / (double)time_song);

            print();
            Print2();
        }

        /// <summary>
        /// 캐릭의 시간 단위마다 버프 값 배열 설정.
        /// 만약 타입별 버프 필요하다면 여기서 수정 필요
        /// </summary>
        /// <param name="c"></param>
        private static void CharBuffCalcSub(ComboBox comboBox)
        {
            if (!(comboBox.SelectedIndex >= 0))
                return;

            Chra c = (Chra)comboBox.SelectedItem;

            switch (c.buff)
            {
                case Buff.combo:
                    SetComboArr(buff_c, c);
                    //for (int i = 0; i < time_song; i++)
                    //{
                    //    if (i % (c.on + c.off) > c.off)
                    //    {
                    //        buff_c[i] = true;
                    //    }
                    //}
                    break;
                case Buff.buff:
                    BuffCul(c, ref buff_v);
                    break;
                case Buff.buff_a:
                    BuffCul(c, ref buff_t);
                    BuffCul(c, ref buff_l);
                    BuffCul(c, ref buff_s);
                    BuffCul(c, ref buff_f);
                    BuffCul(c, ref buff_v);
                    break;
                case Buff.buff_t:
                    BuffCul(c, ref buff_t);
                    BuffCul(c, ref buff_v);
                    break;
                case Buff.buff_l:
                    BuffCul(c, ref buff_l);
                    BuffCul(c, ref buff_v);
                    break;
                case Buff.buff_s:
                    BuffCul(c, ref buff_s);
                    BuffCul(c, ref buff_v);
                    break;
                case Buff.buff_f:
                    BuffCul(c, ref buff_f);
                    BuffCul(c, ref buff_v);
                    break;
                case Buff.etc:
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// charBuffCalcSub 함수용 계산
        /// </summary>
        /// <param name="c"></param>
        /// <param name="b"></param>
        private static void BuffCul(Chra c, ref int[] b)
        {
            for (int i = 0; i < time_song; i++)
            {
                if (i % (c.on + c.off) > c.off)
                {
                    b[i] += c.buff_v;
                }
            }
        }

        /// <summary>
        /// 가로 그래프 그리기
        /// </summary>
        void Print2()
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
                graphics2.FillRectangle(GetChraBursh(buff_c[i], buff_t[i] > 0), i * (time_px), 0, time_px - 1, 19);
                graphics2.FillRectangle(GetChraBursh(buff_c[i], buff_l[i] > 0), i * (time_px), 20, time_px - 1, 19);
                graphics2.FillRectangle(GetChraBursh(buff_c[i], buff_s[i] > 0), i * (time_px), 40, time_px - 1, 19);
                graphics2.FillRectangle(GetChraBursh(buff_c[i], buff_f[i] > 0), i * (time_px), 60, time_px - 1, 19);
                if (buff_c[i])
                {
                    graphics2.FillRectangle(Brushes.Yellow, i * (time_px), 80, time_px - 1, 19);
                }
            }

            pictureBox2.Image = bitmap2;
        }

        /// <summary>
        /// 가로 그래프용. 콤보/버프 조합에 따라 브러시 얻기
        /// </summary>
        /// <param name="c">콤보 여부</param>
        /// <param name="b">버프 여부</param>
        /// <returns></returns>
        private Brush GetChraBursh(bool c, bool b)
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
        private void ChraSelectedIndexChanged(object sender, List<Chra> c, Label label)
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
            comboBox.BackColor = GetColorAtt(((Chra)comboBox.SelectedItem).att);
            //label1.Text = comboBox1.Text; 
            CharBuffCalc();
        }

        /// <summary>
        /// 시간 라벨에 출력
        /// </summary>
        private void Time_somgSet()
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
            e.Graphics.FillRectangle(GetBrushAtt(att), e.Bounds);
            e.Graphics.DrawString(((ComboBox)sender).Items[e.Index].ToString(), ((Control)sender).Font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
        }


        /// <summary>
        /// 속성별 색 얻기
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        private static Brush GetBrushAtt(Att att)
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
        private static Color GetColorAtt(Att att)
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, bool[]> dchra = GetDicChar();

            // 테이블 처리
            DataTable dt = new DataTable("Table1");//DataTable 정의

            // 칼럼 정의
            dt.Columns.Add(new DataColumn("곡이름", typeof(string)));
            dt.Columns.Add(new DataColumn("속성", typeof(string)));
            dt.Columns.Add(new DataColumn("시간", typeof(int)));
            foreach (var c in dchra)
            {
                //col = new DataColumn(c.Key, typeof(string));
                dt.Columns.Add(new DataColumn(c.Key, typeof(double)));
            }

            int cnt;
            DataRow row;

            // 곡별로 캐릭 효율
            foreach (var s in song)
            {
                row = dt.NewRow();
                row["곡이름"] = s.name;
                row["속성"] = s.att;
                row["시간"] = s.time;

                foreach (var c in dchra)
                {
                    cnt = 0;
                    for (int i = 0; i < s.time; i++)
                    {
                        if (c.Value[i])
                        {
                            cnt++;
                        }
                    }
                    //row[c.Key] = string.Format("{0:N2}", (double)cnt / (double)s.time);
                    row[c.Key] = (double)cnt / (double)s.time;
                }
                dt.Rows.Add(row);
            }

            // 엑셀로 출력
            //excel = new Microsoft.Office.Interop.Excel.Application();
            //excelworkBook = excel.Workbooks.Add();
            //excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;

            XLWorkbook wb = new XLWorkbook();
            IXLWorksheet ws = wb.Worksheets.Add(dt, "ByLilly");
            ws.Range(ws.Cell(2, 4), ws.Cell(song.Count + 1, dchra.Count + 3)).Style.NumberFormat.Format = "0.0%";//퍼센트로 서식 지정
            ws.Row(1).Style.Alignment.WrapText = true;
            ws.Row(1).AdjustToContents();
            ws.Row(1).Height = 90;

            string fs = "곡별콤보캐릭조합" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            try
            {
                wb.SaveAs(fs);
                MessageBox.Show(fs + " 파일 출력 완료");
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show(fs + " 파일 접근불가. 사용중 아니에요?");
            }



        }

        private static Dictionary<string, bool[]> GetDicChar()
        {
            List<Chra> chra_c_t = chra_t.Where(Chra => Chra.buff == Buff.combo).ToList();
            List<Chra> chra_c_l = chra_l.Where(Chra => Chra.buff == Buff.combo).ToList();
            List<Chra> chra_c_s = chra_s.Where(Chra => Chra.buff == Buff.combo).ToList();
            List<Chra> chra_c_f = chra_f.Where(Chra => Chra.buff == Buff.combo).ToList();

            Dictionary<string, bool[]> dchra = new Dictionary<string, bool[]>();

            bool[] b1;// = Enumerable.Repeat(false, time_song).ToArray();//콤보 유무

            // 콤보 효율 계산
            foreach (var c1 in chra_c_t)
            {
                b1 = Enumerable.Repeat(false, time_song_max).ToArray();//콤보 
                SetComboArr(b1, c1);

                SetComboArrToDic(c1, chra_c_l, b1, dchra);
                SetComboArrToDic(c1, chra_c_s, b1, dchra);
                SetComboArrToDic(c1, chra_c_f, b1, dchra);
            }
            foreach (var c1 in chra_c_l)
            {
                b1 = Enumerable.Repeat(false, time_song_max).ToArray();//콤보 
                SetComboArr(b1, c1);

                SetComboArrToDic(c1, chra_c_s, b1, dchra);
                SetComboArrToDic(c1, chra_c_f, b1, dchra);
            }
            foreach (var c1 in chra_c_s)
            {
                b1 = Enumerable.Repeat(false, time_song_max).ToArray();//콤보 
                SetComboArr(b1, c1);

                SetComboArrToDic(c1, chra_c_f, b1, dchra);
            }

            return dchra;
        }

        /// <summary>
        /// 엑셀 출력용 속성 계산
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="chra_c_l"></param>
        /// <param name="b1"></param>
        /// <param name="d"></param>
        private static void SetComboArrToDic(Chra c1, List<Chra> chra_c_l, bool[] b1, Dictionary<string, bool[]> d)
        {
            bool[] b2;// = Enumerable.Repeat(false, time_song).ToArray();//콤보 유무
            foreach (var c2 in chra_c_l)
            {
                b2 = (bool[])b1.Clone();
                SetComboArr(b2, c2);
                d.Add(c1.name + "\n"+c1.att + "\n" + c2.name+ "\n" + c2.att, b2);
            }
        }

        /// <summary>
        /// 받은 배열에 캐릭의 콤보 추가
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="c1"></param>
        private static void SetComboArr(bool[] b1, Chra c1)
        {
            for (int i = 0; i < b1.Length; i++)
            {
                if (i % (c1.on + c1.off) > c1.off)
                {
                    b1[i] = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<BindChra, bool[]> dchra = GetDicChar2();

            // 테이블 처리
            DataTable dt = new DataTable("Table1");//DataTable 정의

            // 칼럼 정의
            dt.Columns.Add(new DataColumn("곡이름", typeof(string)));
            dt.Columns.Add(new DataColumn("곡속성", typeof(string)));
            dt.Columns.Add(new DataColumn("곡시간", typeof(int)));
            dt.Columns.Add(new DataColumn("캐릭1", typeof(string)));
            dt.Columns.Add(new DataColumn("속성1", typeof(string)));
            dt.Columns.Add(new DataColumn("캐릭2", typeof(string)));
            dt.Columns.Add(new DataColumn("속성2", typeof(string)));
            dt.Columns.Add(new DataColumn("효율", typeof(double)));

            int cnt;
            DataRow row;

            // 곡별로 캐릭 효율
            foreach (var s in song)
            {
                foreach (var c in dchra)
                {
                    row = dt.NewRow();

                    row["곡이름"] = s.name;
                    row["곡속성"] = s.att;
                    row["곡시간"] = s.time;
                    row["캐릭1"] = c.Key.c1.name;
                    row["속성1"] = c.Key.c1.att;
                    row["캐릭2"] = c.Key.c2.name;
                    row["속성2"] = c.Key.c2.att;

                    cnt = 0;
                    for (int i = 0; i < s.time; i++)
                    {
                        if (c.Value[i])
                        {
                            cnt++;
                        }
                    }
                    //row[c.Key] = string.Format("{0:N2}", (double)cnt / (double)s.time);
                    row["효율"] = (double)cnt / (double)s.time;

                    dt.Rows.Add(row);
                }
            }

            XLWorkbook wb = new XLWorkbook();
            IXLWorksheet ws = wb.Worksheets.Add(dt, "ByLilly");
            ws.Column(8).Style.NumberFormat.Format = "0.0%";//퍼센트로 서식 지정
            //ws.Column(8).Style.NumberFormat.Format = "0.0%";//퍼센트로 서식 지정
            //ws.Row(1).Style.Alignment.WrapText = true;
            //ws.Row(1).AdjustToContents();
            //ws.Row(1).Height = 90;

            string fs = "곡별콤보캐릭조합" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            try
            {
                wb.SaveAs(fs);
                MessageBox.Show(fs + " 파일 출력 완료");
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show(fs + " 파일 접근불가. 사용중 아니에요?");
            }
        }

        struct BindChra
        {
            public Chra c1;
            public Chra c2;

            public BindChra(Chra c1, Chra c2)
            {
                this.c1 = c1;
                this.c2 = c2;
            }
        }

        private static void SetComboArrToDic(Chra c1, List<Chra> chra_c_l, bool[] b1, Dictionary<BindChra, bool[]> d)
        {
            bool[] b2;// = Enumerable.Repeat(false, time_song).ToArray();//콤보 유무
            foreach (var c2 in chra_c_l)
            {
                b2 = (bool[])b1.Clone();
                SetComboArr(b2, c2);
                d.Add(new BindChra(c1, c2), b1);
                //d.Add(c1.name + "\n" + c1.att + "\n" + c2.name + "\n" + c2.att, b2);
            }
        }

        private static Dictionary<BindChra, bool[]> GetDicChar2()
        {
            List<Chra> chra_c_t = chra_t.Where(Chra => Chra.buff == Buff.combo).ToList();
            List<Chra> chra_c_l = chra_l.Where(Chra => Chra.buff == Buff.combo).ToList();
            List<Chra> chra_c_s = chra_s.Where(Chra => Chra.buff == Buff.combo).ToList();
            List<Chra> chra_c_f = chra_f.Where(Chra => Chra.buff == Buff.combo).ToList();

            Dictionary<BindChra, bool[]> dchra = new Dictionary<BindChra, bool[]>();

            bool[] b1;// = Enumerable.Repeat(false, time_song).ToArray();//콤보 유무

            // 콤보 효율 계산
            foreach (var c1 in chra_c_t)
            {
                b1 = Enumerable.Repeat(false, time_song_max).ToArray();//콤보 
                SetComboArr(b1, c1);

                SetComboArrToDic(c1, chra_c_l, b1, dchra);
                SetComboArrToDic(c1, chra_c_s, b1, dchra);
                SetComboArrToDic(c1, chra_c_f, b1, dchra);
            }
            foreach (var c1 in chra_c_l)
            {
                b1 = Enumerable.Repeat(false, time_song_max).ToArray();//콤보 
                SetComboArr(b1, c1);

                SetComboArrToDic(c1, chra_c_s, b1, dchra);
                SetComboArrToDic(c1, chra_c_f, b1, dchra);
            }
            foreach (var c1 in chra_c_s)
            {
                b1 = Enumerable.Repeat(false, time_song_max).ToArray();//콤보 
                SetComboArr(b1, c1);

                SetComboArrToDic(c1, chra_c_f, b1, dchra);
            }

            return dchra;
        }

    }
}
