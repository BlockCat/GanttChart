using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
	class Graph
	{
		public List<ArrivalNode> ArrivalNodes = new List<ArrivalNode>();
		public List<DepartureNode> DepartureNodes = new List<DepartureNode>();
		
		/*public List<Node> Topological()
		{
			List<Node> topology = new List<Node>();
			HashSet<Node> found = new HashSet<Node>();
			Queue<ArrivalNode> Starters = new Queue<ArrivalNode>(ArrivalNodes);			

			while (Starters.Count > 0)
			{
				ArrivalNode n = Starters.Dequeue();
				foreach (var node in n.Children())
				{
					if (!found.Contains(node))
					{
						found.Add(node);
						topology.Add(node);
					}
				}
			}

			return topology;
		}*/


		public List<Node> Sort()
		{
			var sorted = new List<Node>();
			var visited = new Dictionary<Node, bool>();

			ArrivalNodes.ForEach(s => Visit(s, sorted, visited));

			sorted.Reverse();

			return sorted;
		}

		public void Visit(Node item, List<Node> sorted, Dictionary<Node, bool> visited)
		{			
			var alreadyVisited = visited.TryGetValue(item, out bool inProcess);

			if (alreadyVisited)
			{
				if (inProcess)
				{
					throw new ArgumentException("Cyclic dependency found.");
				}
			}
			else
			{
				visited[item] = true;

				var dependencies = item.Successors;
				if (dependencies != null)
				{
					foreach (var dependency in dependencies)
					{
						Visit(dependency, sorted, visited);
					}
				}

				visited[item] = false;
				sorted.Add(item);
			}
		}
	}

	abstract class Node
	{
		public List<Node> Successors = new List<Node>();
		public List<Node> Predecessors = new List<Node>();

		protected Node(int maxIn, int maxOut, string name)
		{
			this.MaxIn= maxIn;
			this.MaxOut = maxOut;
			this.Name = name;
		}

		public void AddSuccessor(Node n)
		{
			//Trace.Assert(this.Successors.Count < MaxOut, $"Node: {Name} already contains max output connection: {MaxOut}");
			//Trace.Assert(n.Predecessors.Count < n.MaxIn, $"Node: {n.Name} already contains max input connection: {n.MaxIn}");
			this.Successors.Add(n);
			n.Predecessors.Add(this);
		}
		

		public abstract void AdjustStartTimes(float start);
		public abstract void AdjustEndTimes(float end);
		public virtual BarBox CreateBar() => null;

		public int MaxOut { get; }
		public int MaxIn { get; }
		public string Name { get; }

	}

	class ArrivalNode : Node
	{
		public readonly float Start;


		public ArrivalNode(String tag, int start) : base(0, 1, $"ArrivalNode: {tag}")
		{
			this.Start = start;
		}

		public override void AdjustStartTimes(float _) => Successors.ForEach(s => s.AdjustStartTimes(this.Start));

		public override BarBox CreateBar() => new BarBox.Builder(this.Name)
				.SetStart(Start)
				.SetEnd(Start)
				.SetDuration(0)				
				.Build();
		

		public override void AdjustEndTimes(float end) {}
	}

	class DepartureNode : Node
	{

		public readonly float End;

		public DepartureNode(String tag, float end) : base(1, 0, $"DepartureNode: {tag}")
		{
			this.End = end;
		}

		public override void AdjustStartTimes(float _) {}
		public override void AdjustEndTimes(float end) => Predecessors.ForEach(p => p.AdjustEndTimes(this.End));

		public override BarBox CreateBar() => new BarBox.Builder(this.Name)
			.SetStart(End)
			.SetEnd(End)
			.SetLineColor(Color.Red)
			.SetDuration(0)
			.Build();
		
	}

	class MovementNode : Node
	{		
		public float Start;
		public float End = float.MaxValue;
		public readonly float Duration;

		public MovementNode(string tag, float duration) : base(3,3, $"MovementNode: {tag}")
		{
			this.Duration = duration;
		}

		public override void AdjustStartTimes(float start)
		{
			this.Start = Math.Max(Start, start);
			this.Successors.ForEach(s => s.AdjustStartTimes(Start + Duration));
		}

		public override void AdjustEndTimes(float end)
		{
			this.End = Math.Min(End, end);
			this.Predecessors.ForEach(s => s.AdjustEndTimes(End - Duration));
		}

		public override BarBox CreateBar() => new BarBox.Builder(this.Name)
			.SetStart(Start)
			.SetEnd(End)
			.SetDuration(Duration)
			.SetBoxColor(Color.PaleVioletRed)
			.Build();
	}

	class TurnAroundNode : Node
	{
		public readonly float Duration;

		public TurnAroundNode(float duration) : base(1, 1, "TurnAroundNode")
		{
			this.Duration = duration;
		}

		public override void AdjustEndTimes(float end) => this.Predecessors.ForEach(p => p.AdjustEndTimes(end - Duration));
		public override void AdjustStartTimes(float start) => this.Successors.ForEach(s => s.AdjustStartTimes(start + Duration));

	}

	class ServiceNode : Node
	{
		public float Start;
		public float End;
		public readonly float Duration;

		public ServiceNode(string tag, float duration) : base(1, 1, $"ServiceNode: {tag}")
		{
			this.Duration = duration;
		}

		public override void AdjustStartTimes(float start)
		{
			this.Start = start;
			this.Successors.ForEach(s => s.AdjustStartTimes(Start + Duration));
		}

		public override void AdjustEndTimes(float end)
		{
			this.End = end;
			this.Predecessors.ForEach(s => s.AdjustEndTimes(End - Duration));
		}

		public override BarBox CreateBar() => new BarBox.Builder(this.Name)
		.SetStart(Start)
		.SetEnd(End)
		.SetDuration(Duration)
		.SetBoxColor(Color.DarkOliveGreen)
		.Build();
	}
}
