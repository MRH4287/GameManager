using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Game
{
    public partial class TimeCalc : Form
    {

        TextBox souce;

        public TimeCalc()
        {
            InitializeComponent();
        }

        public TimeCalc(TextBox input)
            : this()
        {
            try
            {
                this.souce = input;
                textBox5.Text = input.Text;

                Game.GameData.TimeData data = Game.GameData.Timestamp2Text(int.Parse(input.Text));
                textBox1.Text = data.days.ToString();
                textBox2.Text = data.hours.ToString();
                textBox3.Text = data.minutes.ToString();
                textBox4.Text = data.seconds.ToString();
                button2.Enabled = true;
            }
            catch
            {
                MessageBox.Show("ungültige Werte");
            }

        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.souce.Text = textBox5.Text;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int tmp = 0;
            tmp += (int.Parse(textBox1.Text)) * 24;
            tmp = (int.Parse(textBox2.Text) + tmp) *60;
            tmp = (int.Parse(textBox3.Text) + tmp) * 60;
            tmp = (int.Parse(textBox4.Text) + tmp);

            textBox5.Text = tmp.ToString();



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "") { textBox1.Text = "0"; }
            if (textBox2.Text == "") { textBox2.Text = "0"; }
            if (textBox3.Text == "") { textBox3.Text = "0"; }
            if (textBox4.Text == "") { textBox4.Text = "0"; }
            if (textBox5.Text == "") { textBox5.Text = "0"; }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Game.GameData.TimeData data = Game.GameData.Timestamp2Text(int.Parse(textBox5.Text));
            textBox1.Text = data.days.ToString();
            textBox2.Text = data.hours.ToString();
            textBox3.Text = data.minutes.ToString();
            textBox4.Text = data.seconds.ToString();
        }
    }
}
