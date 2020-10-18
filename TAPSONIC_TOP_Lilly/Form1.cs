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
        static int grp_h_div = 1;//그래픽 배율 감소
        static int grp_h = 600;// 그래픽 크기
        static int[] buff_vall = Enumerable.Repeat(100, time_song).ToArray();
        static int[] buff_v = Enumerable.Repeat(100, time_song).ToArray();
        static bool[] buff_c = Enumerable.Repeat(false, time_song).ToArray();
        static int sum = 0;// 그래픽 크기

        static List<Chra> ca;
        static List<Chra> ct;
        static List<Chra> cl;
        static List<Chra> cs;
        static List<Chra> cp;

        static Font fnt = new Font("맑은 고딕", 16);
        //var fontf = new Font("맑은 고딕", 16, FontStyle.Regular, GraphicsUnit.Pixel);
        static Bitmap bitmap = new Bitmap(grp_w, grp_h);
        static Graphics graphics = Graphics.FromImage(bitmap);

        enum Att    // 속성
        {
            dancer,
            session,
            vocal
        }

        enum Type    // 타입
        {
            t,
            l,
            s,
            p,
            n
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

            public override string ToString() => $"{name},\t {off},\t {on},\t {buff_v}";
        }



        public Form1()
        {
            InitializeComponent();

            ca = new List<Chra>();

            // 캐릭 세팅            
            ca.Add(new Chra("엘리10", Att.vocal, Type.t, Buff.combo, 100, 18, 14));
            ca.Add(new Chra("제시10", Att.vocal, Type.t, Buff.combo, 100, 12, 13));
            ca.Add(new Chra("케르밀라10", Att.vocal, Type.t, Buff.buff, 73, 11, 19));
            ca.Add(new Chra("혁10", Att.vocal, Type.t, Buff.buff, 101, 15, 20));
            ca.Add(new Chra("이브10", Att.vocal, Type.t, Buff.buff, 105, 10, 12));
            ca.Add(new Chra("혀니10", Att.vocal, Type.t, Buff.buff, 85, 7, 6));
            ca.Add(new Chra("지크10", Att.vocal, Type.t, Buff.etc, 9, 10, 25));
            ca.Add(new Chra("엘 클리어10", Att.vocal, Type.t, Buff.combo, 100, 18, 10));
            ca.Add(new Chra("베아트리스10", Att.vocal, Type.l, Buff.buff, 44, 11, 20));
            ca.Add(new Chra("아리아10", Att.vocal, Type.l, Buff.combo, 100, 16, 10));
            ca.Add(new Chra("다이나10", Att.vocal, Type.l, Buff.etc, 0, 9, 1));
            ca.Add(new Chra("하이틴 유리10", Att.vocal, Type.l, Buff.buff, 100, 15, 14));
            ca.Add(new Chra("미로10", Att.vocal, Type.l, Buff.buff, 90, 16, 11));
            ca.Add(new Chra("니콜10", Att.vocal, Type.s, Buff.combo, 100, 15, 13));
            ca.Add(new Chra("파르티나10", Att.vocal, Type.s, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("오키드10", Att.vocal, Type.s, Buff.buff, 83, 10, 20));
            ca.Add(new Chra("스이10", Att.vocal, Type.s, Buff.buff, 97, 10, 12));
            ca.Add(new Chra("슬래셔10", Att.vocal, Type.p, Buff.buff, 68, 13, 22));
            ca.Add(new Chra("걸윙10", Att.vocal, Type.p, Buff.combo, 100, 14, 10));
            ca.Add(new Chra("캐롤10", Att.dancer, Type.t, Buff.buff, 72, 12, 20));
            ca.Add(new Chra("셜리10", Att.dancer, Type.t, Buff.combo, 100, 17, 10));
            ca.Add(new Chra("데몬10", Att.dancer, Type.t, Buff.buff, 94, 17, 15));
            ca.Add(new Chra("로아10", Att.dancer, Type.t, Buff.buff, 94, 16, 16));
            ca.Add(new Chra("시테10", Att.dancer, Type.t, Buff.etc, 0, 11, 1));
            ca.Add(new Chra("하이틴 제이10", Att.dancer, Type.t, Buff.etc, 0, 14, 1));
            ca.Add(new Chra("발렌타인10", Att.dancer, Type.l, Buff.buff, 99, 12, 20));
            ca.Add(new Chra("장고10", Att.dancer, Type.l, Buff.buff, 40, 10, 20));
            ca.Add(new Chra("라파엘10", Att.dancer, Type.l, Buff.combo, 100, 16, 18));
            ca.Add(new Chra("도로시10", Att.dancer, Type.l, Buff.buff, 88, 12, 15));
            ca.Add(new Chra("웨니10", Att.dancer, Type.l, Buff.buff, 80, 18, 17));
            ca.Add(new Chra("크라켄10", Att.dancer, Type.l, Buff.combo, 100, 16, 10));
            ca.Add(new Chra("스페이시10", Att.dancer, Type.l, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("아수라10", Att.dancer, Type.s, Buff.buff, 112, 12, 15));
            ca.Add(new Chra("호련10", Att.dancer, Type.s, Buff.combo, 100, 15, 15));
            ca.Add(new Chra("스테파니10", Att.dancer, Type.s, Buff.etc, 7, 10, 24));
            ca.Add(new Chra("토미 슐츠10", Att.dancer, Type.s, Buff.buff, 89, 14, 14));
            ca.Add(new Chra("엘 페일10", Att.dancer, Type.s, Buff.buff, 93, 14, 15));
            ca.Add(new Chra("벨10", Att.dancer, Type.s, Buff.combo, 100, 21, 10));
            ca.Add(new Chra("아폴로10", Att.dancer, Type.s, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("로제10", Att.dancer, Type.p, Buff.buff, 71, 13, 22));
            ca.Add(new Chra("카론10", Att.dancer, Type.p, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("아레스무스10", Att.dancer, Type.p, Buff.buff, 68, 17, 24));
            ca.Add(new Chra("하이틴 세나10", Att.dancer, Type.p, Buff.buff, 68, 15, 15));
            ca.Add(new Chra("엔젤라10", Att.dancer, Type.p, Buff.etc, 0, 14, 1));
            ca.Add(new Chra("메두사10", Att.session, Type.t, Buff.etc, 20, 10, 30));
            ca.Add(new Chra("아르웬10", Att.session, Type.t, Buff.buff, 72, 12, 18));
            ca.Add(new Chra("루시퍼10", Att.session, Type.t, Buff.combo, 100, 15, 14));
            ca.Add(new Chra("조크 에리스갓10", Att.session, Type.t, Buff.buff, 90, 15, 17));
            ca.Add(new Chra("조이10", Att.session, Type.t, Buff.buff, 35, 15, 15));
            ca.Add(new Chra("베로니카10", Att.session, Type.t, Buff.buff, 99, 16, 16));
            ca.Add(new Chra("촙스10", Att.session, Type.t, Buff.etc, 0, 9, 1));
            ca.Add(new Chra("트래셔 박사10", Att.session, Type.l, Buff.combo, 100, 19, 15));
            ca.Add(new Chra("프랑켄10", Att.session, Type.l, Buff.etc, 0, 14, 1));
            ca.Add(new Chra("타냐10", Att.session, Type.l, Buff.buff, 89, 10, 18));
            ca.Add(new Chra("액슬10", Att.session, Type.l, Buff.etc, 7, 15, 27));
            ca.Add(new Chra("재규어10", Att.session, Type.l, Buff.combo, 100, 15, 11));
            ca.Add(new Chra("포에트10", Att.session, Type.l, Buff.buff, 80, 15, 20));
            ca.Add(new Chra("우리엘10", Att.session, Type.l, Buff.etc, 0, 0, 300));
            ca.Add(new Chra("티타나10", Att.session, Type.s, Buff.buff, 108, 11, 14));
            ca.Add(new Chra("패리스 업튼10", Att.session, Type.s, Buff.combo, 100, 18, 14));
            ca.Add(new Chra("신시아10", Att.session, Type.s, Buff.buff, 79, 15, 20));
            ca.Add(new Chra("오하나10", Att.session, Type.s, Buff.buff, 52, 10, 20));
            ca.Add(new Chra("에이바10", Att.session, Type.s, Buff.buff, 97, 10, 12));
            ca.Add(new Chra("하이틴 체르니10", Att.session, Type.s, Buff.combo, 100, 17, 10));
            ca.Add(new Chra("볼프강10", Att.session, Type.p, Buff.combo, 100, 10, 9));
            ca.Add(new Chra("루핑10", Att.session, Type.p, Buff.buff, 400, 4, 4));
            ca.Add(new Chra("지니10", Att.session, Type.p, Buff.etc, 0, 12, 1));
            ca.Add(new Chra("에이미10", Att.session, Type.p, Buff.etc, 0, 11, 1));
            ca.Add(new Chra("탈리아10", Att.session, Type.p, Buff.etc, 0, 0, 300));




            ct = ca.Where(Chra => Chra.type == Type.t).ToList();
            cl = ca.Where(Chra => Chra.type == Type.l).ToList();
            cs = ca.Where(Chra => Chra.type == Type.s).ToList();
            cp = ca.Where(Chra => Chra.type == Type.p).ToList();

            //comboBox2.Items.AddRange(cd2.Where(Chra => Chra.type == Type.t).Select(Chra=> Chra.name).ToArray());
            comboBox1.Items.AddRange(ct.Select(Chra=> Chra.name).ToArray());
            comboBox2.Items.AddRange(cl.Select(Chra=> Chra.name).ToArray());
            comboBox3.Items.AddRange(cs.Select(Chra=> Chra.name).ToArray());
            comboBox4.Items.AddRange(cp.Select(Chra=> Chra.name).ToArray());

            trackBar1.Maximum = 180;
            trackBar1.Value = 120;
            trackBar1.LargeChange = 1;

            pictureBox1.BackColor = Color.Black;

            time_somgSet();
            print();            
        }

        void charBuffCalc()
        {
            sum = 0;
            buff_vall = Enumerable.Repeat(0, time_song).ToArray();
            buff_v = Enumerable.Repeat(0, time_song).ToArray();
            buff_c = Enumerable.Repeat(false, time_song).ToArray();

            if (comboBox1.SelectedIndex >= 0)
                charBuffCalcSub(ct[comboBox1.SelectedIndex]);            
            if(comboBox2.SelectedIndex>=0)
                charBuffCalcSub(cl[comboBox2.SelectedIndex]);            
            if(comboBox3.SelectedIndex>=0)
                charBuffCalcSub(cs[comboBox3.SelectedIndex]);            
            if(comboBox4.SelectedIndex>=0)
                charBuffCalcSub(cp[comboBox4.SelectedIndex]);

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

        void print()
        {
            graphics.Clear(BackColor);
            for (int i = grp_h-99; i >=0 ; i-=100)
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
            chgChk(comboBox1,ct,label1);

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            chgChk(comboBox2,cl,label2);
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            chgChk(comboBox3,cs,label3);
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            chgChk(comboBox4,cp,label4);
        }

        private void chgChk(ComboBox comboBox, List<Chra> c, Label label)
        {
            if (comboBox.SelectedIndex == -1) return;
            if (string.IsNullOrWhiteSpace(comboBox.Text)) return;
            label.Text = c[comboBox.SelectedIndex].ToString();
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
    }
}
