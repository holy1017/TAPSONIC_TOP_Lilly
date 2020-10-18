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
        static int mt = 4;// 스킬 영역 크기 배율
        static int ch1 = 15; // 스킬 표시 세로 범위
        static int ch2 = 5;
        static int ch = ch1 + ch2;//캐릭별 세로 크기
        static int tw = 80;// 텍스트 영역 가로 크기
        static int sw = 180 * mt;// 스킬 영역 가로 크기

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
            public int off;
            public int on;

            public Chra(string name, Att att, Type type, Buff buff, int off, int on)
            {
                this.name = name;
                this.type = type;
                this.buff = buff;
                this.att = att;
                this.off = off * mt;
                this.on = on * mt;
            }

            public override string ToString() => $"{name},\t {off},\t {on}";
        }

        List<Chra> ca ;
        List<Chra> ct ;
        List<Chra> cl ;
        List<Chra> cs ;
        List<Chra> cp ;

        public Form1()
        {
            InitializeComponent();

            ca = new List<Chra>();

            // 캐릭 세팅            
            ca.Add(new Chra("크라켄10", Att.dancer, Type.l, Buff.combo, 16, 10));
            ca.Add(new Chra("셜리10", Att.dancer, Type.t, Buff.combo, 17, 10));
            ca.Add(new Chra("호련10", Att.dancer, Type.s, Buff.combo, 15, 15));
            ca.Add(new Chra("벨10", Att.dancer, Type.s, Buff.combo, 21, 10));
            ca.Add(new Chra("라파엘10", Att.dancer, Type.n, Buff.combo, 16, 18));
            ca.Add(new Chra("볼프강10", Att.session, Type.n, Buff.combo, 10, 9));
            ca.Add(new Chra("재규어10", Att.session, Type.n, Buff.combo, 15, 11));
            ca.Add(new Chra("틴체르니10", Att.session, Type.n, Buff.combo, 17, 10));
            ca.Add(new Chra("루시퍼10", Att.session, Type.t, Buff.combo, 15, 14));
            ca.Add(new Chra("업튼10", Att.session, Type.s, Buff.combo, 18, 14));
            ca.Add(new Chra("트래셔10", Att.session, Type.n, Buff.combo, 19, 15));
            ca.Add(new Chra("걸윙10", Att.vocal, Type.n, Buff.combo, 14, 10));
            ca.Add(new Chra("제시10", Att.vocal, Type.t, Buff.combo, 12, 13));
            ca.Add(new Chra("아리아10", Att.vocal, Type.n, Buff.combo, 16, 10));
            ca.Add(new Chra("엘클리어10", Att.vocal, Type.n, Buff.combo, 18, 10));
            ca.Add(new Chra("니콜10", Att.vocal, Type.s, Buff.combo, 15, 13));
            ca.Add(new Chra("엘리10", Att.vocal, Type.t, Buff.combo, 18, 14));
            ca.Add(new Chra("베아트리스10", Att.vocal, Type.l, Buff.buff, 11, 20));
            ca.Add(new Chra("다이나10", Att.vocal, Type.l, Buff.etc, 9, 1));
            ca.Add(new Chra("장고10", Att.dancer, Type.l, Buff.buff, 10, 20));

            ct = ca.Where(Chra => Chra.type == Type.t).ToList();
            cl = ca.Where(Chra => Chra.type == Type.l).ToList();
            cs = ca.Where(Chra => Chra.type == Type.s).ToList();
            cp = ca.Where(Chra => Chra.type == Type.p).ToList();

            //comboBox2.Items.AddRange(cd2.Where(Chra => Chra.type == Type.t).Select(Chra=> Chra.name).ToArray());
            comboBox1.Items.AddRange(ct.Select(Chra=> Chra.name).ToArray());
            comboBox2.Items.AddRange(cl.Select(Chra=> Chra.name).ToArray());
            comboBox3.Items.AddRange(cs.Select(Chra=> Chra.name).ToArray());
            comboBox4.Items.AddRange(cp.Select(Chra=> Chra.name).ToArray());
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
        }
    }
}
