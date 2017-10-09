using System;
using System.Windows.Forms;
using KarraPath;

namespace Interface
{
    public partial class Form1 : Form
    {
        public MainUc MainUc { get; set; }

        public Form1()
        {
            InitializeComponent();
            MainUc = new MainUc
            {
                Dock = DockStyle.Fill
            };
            panel1.Controls.Add(MainUc);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var useDiag = diagTxtBox.Text == "o";

            if (!int.TryParse(startPos.Text, out int start)) return;
            if (!int.TryParse(targetPos.Text, out int end)) return;

            MainUc.Clear();

            if (!MainUc.FindPath(start, end, useDiag))
                MessageBox.Show("Aucun trajet trouvé");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(mapIdTxtBox.Text, out int mapId))
                MainUc.Init(mapId);
        }

        private void toolStripButton3_Click(object sender, EventArgs e) => MainUc.Clear();
    }
}
