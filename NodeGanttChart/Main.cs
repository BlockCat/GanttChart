using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class Main : Form
	{

		const int X_OFFSET = 200;

		private readonly Graph graph;
        private readonly Yard tracks;
        private readonly BarSet barset;

        public Main()
		{
			InitializeComponent();

			this.Size = new Size(800, 600);
			this.Paint += PaintBoxesEvent;

            this.graph = Graph.THINGS();
            this.tracks = Yard.CreateTracks();
            this.barset = new BarSet(200, 10, 800, 2 * 3600 + 10 * 60, graph.Sort().Select(x => x.CreateBar()).Where(x => x != null).ToList());
        }

		private void PaintBoxesEvent(object sender, PaintEventArgs e)
		{
            //this.barset.Draw(e.Graphics);

            this.tracks.Draw(e.Graphics, 10, 20, 700, 300);
		}

	}
}
