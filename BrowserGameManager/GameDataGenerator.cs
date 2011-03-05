using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Game.Game;
using System.IO;

namespace Game
{
    public partial class GameDataGenerator : Form
    {
        GameData game;


        public GameDataGenerator(GameData game)
        {

            InitializeComponent();
            this.game = game;
        }

        private void GameDataGenerator_Load(object sender, EventArgs e)
        {
            #if DEBUG        
            textBox1.Text = @"C:\Users\MRH\Documents\Visual Studio 2010\Projects\Game\Server\Server\bin\Debug\GameData.dat";
            #endif
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = saveFileDialog1.FileName;
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                game.writeIntoFile(textBox1.Text);
                MessageBox.Show("GameData Datei erfolgreich erstellt");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler bei der erstellung: " + ex.Message);
            }
        }
    }
}
