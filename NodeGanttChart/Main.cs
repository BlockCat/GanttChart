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

		Graph graph;
		public Main()
		{
			graph = CreateGraph();

			InitializeComponent();

			this.Size = new Size(800, 600);
			this.Paint += PaintBoxesEvent;
		}

		private void PaintBoxesEvent(object sender, PaintEventArgs e)
		{
			List<Node> nodes = graph.Sort();
			int drawCounter = 0;
			for (int i = 0; i < nodes.Count; i++)
			{
				var n = nodes[i].CreateBar();
				if (n != null)
				{
					n.Draw(e.Graphics, X_OFFSET, 10 + 20 * drawCounter, 400, 20, 120 * 60);
					drawCounter++;
				}
			}
		}

		private Graph CreateGraph()
		{
			var graph = new Graph();


			var ar1 = new ArrivalNode("(1,2)", 0);
			var ar2 = new ArrivalNode("(3)", 30 * 60);

			var m1 = new MovementNode("(1) to T3", 150);
			var m2 = new MovementNode("(2) to T2", 170);
			var m3 = new MovementNode("(3) to T5", 200);


			var dep1 = new DepartureNode("(1, 2)", 45 * 60);
			var dep2 = new DepartureNode("(3)", 120 * 60);

			ar1.AddSuccessor(m1);
			ar1.AddSuccessor(m2);

			ar2.AddSuccessor(m3);

			m1.AddSuccessor(m2);

			m1.AddSuccessor(dep1);
			m2.AddSuccessor(dep1);

			m3.AddSuccessor(dep2);

			ar1.AdjustStartTimes(0);
			ar2.AdjustStartTimes(0);

			dep1.AdjustEndTimes(0);
			dep2.AdjustEndTimes(0);
			

			graph.ArrivalNodes.Add(ar1);
			graph.ArrivalNodes.Add(ar2);

			return graph;
		}
	}
}
