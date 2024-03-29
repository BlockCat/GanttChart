﻿using System;
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

        public static Graph THINGS()
        {
            /*
     Sizes
    - EC3: 80
    - EC4: 110
    - SNG-3: 70
    - SNG-4: 90
    - SNG-5: 110
    In:
    - 0:00:00, EC3(C), EC3(C)
    - 0:05:00,  SNG-4
    - 0:12:00,  EC3, EC4
    - 0:30:00,  EC4
    - 1:00:00,  SNG-3, SNG-3
    - 1:10:00,  SNG-5
    Out:
    - 0:20:00,  EC3
    - 0:25:00,  SNG-4
    - 0:29:00,  EC4, EC3
    - 0:45:00,  EC3, EC4
    - 1:55:00,  SNG-5, SNG-3
    - 2:10:00,  SNG-3

    Cleaning (C):
    - EC3: 32
    - EC4: 47
    - SNG-3: 27
    - SNG-4: 35
    - SNG-5: 42

    Insepction (I):
    - EC3: 32
    - EC4: 47
    - SNG-3: 27
    - SNG-4: 35
    - SNG-5: 42
 */
            var graph = new Graph();

            Func<int, int> hours = (int h) => h * 3600;
            Func<int, int> minutes = (int m) => m * 60;
            Func<int, int> seconds = (int s) => s;

            Func<int, int, int> track2secs = (int t, int s) => t * 60 + s * 30;
            Func<int, int> turnaround2secs = (int c) => c * 30;

            /*
             *  - 0:00:00, EC3(C), EC3(C)
                - 0:05:00,  SNG4
                - 0:12:00,  EC3, EC4
                - 0:30:00,  EC4
                - 1:00:00,  SNG3, SNG3
                - 1:10:00,  SNG5
            */
            var ar1 = new ArrivalNode("(1,2)", 0);
            var ar2 = new ArrivalNode("(3)", minutes(5));
            var ar3 = new ArrivalNode("(4, 5)", minutes(12));
            var ar4 = new ArrivalNode("(6)", minutes(30));
            var ar5 = new ArrivalNode("(7, 8)", hours(1));
            var ar6 = new ArrivalNode("(9)", hours(1) + minutes(10));

            /*
             * Out:
                - 0:50:00,  EC3
                - 0:25:00,  SNG-4
                - 0:29:00,  EC4, EC3
                - 0:45:00,  EC3, EC4
                - 1:55:00,  SNG-5, SNG-3
                - 2:10:00,  SNG-3
            */
            var dep1 = new DepartureNode("(1)", minutes(60));
            var dep2 = new DepartureNode("(3)", minutes(25));
            var dep3 = new DepartureNode("(5,2)", minutes(69));
            var dep4 = new DepartureNode("(4, 6)", minutes(45));
            var dep5 = new DepartureNode("(9, 7)", hours(1) + minutes(55));
            var dep6 = new DepartureNode("(8)", hours(2) + minutes(10));


            {
                Node m12P1 = new MovementNode("(1, 2) to P1", track2secs(2, 1));
                Node s12 = new SplitNode("(1, 2) -> (1), (2)", minutes(2) + 30);
                Node m1p7 = new MovementNode("(1) to P7", track2secs(3, 2));
                Node service1p7 = new ServiceNode("(1) insepction", minutes(32));
                Node service2p7 = new ServiceNode("(1) insepction", minutes(32));
                Node m2p7 = new MovementNode("(2) to P7", track2secs(3, 2));
                ar1.AddSuccessor(m12P1);
                
                m12P1 += s12;
                s12 += m1p7;
                s12 += m2p7;
                m1p7 += m2p7;

                m1p7 += service1p7;
                m2p7 += service2p7;

                Node m1p4 = new MovementNode("(1) to P4", track2secs(5, 4));
                Node m2p4 = new MovementNode("(2) to P4", track2secs(5, 4));

                service1p7 += m1p4;
                service2p7 += m2p4;

                m1p4.AddSuccessor(dep1);
                m2p4.AddSuccessor(dep6);
            }

            graph.ArrivalNodes.Add(ar1);
            graph.ArrivalNodes.Add(ar2);
            graph.ArrivalNodes.Add(ar3);
            graph.ArrivalNodes.Add(ar4);
            graph.ArrivalNodes.Add(ar5);
            graph.ArrivalNodes.Add(ar6);

            graph.DepartureNodes.Add(dep1);
            graph.DepartureNodes.Add(dep2);
            graph.DepartureNodes.Add(dep3);
            graph.DepartureNodes.Add(dep4);
            graph.DepartureNodes.Add(dep5);
            graph.DepartureNodes.Add(dep6);

            graph.Propagate();

            return graph;

        }
        public static Graph Create()
        {

            var graph = new Graph();

            Func<int, int> hours = (int h) => h * 3600;
            Func<int, int> minutes = (int m) => m * 60;
            Func<int, int> seconds = (int s) => s;

            Func<int, int, int> track2secs = (int t, int s) => t * 60 + s * 30;
            Func<int, int> turnaround2secs = (int c) => c * 30;

            Debug.Assert(track2secs(2, 1) == minutes(2) + seconds(30));
            Debug.Assert(turnaround2secs(4) == minutes(2));

            var ar1 = new ArrivalNode("(1,2)", 0);
            var ar2 = new ArrivalNode("(3)", minutes(2) + 30);
            var arm1 = new MovementNode("(1, 2) to T3", track2secs(2, 1));


            var split12 = new SplitNode("(1, 2) to (1), (2)", minutes(2));

            var m2 = new MovementNode("(2) to T2", track2secs(2, 1));
            var s1 = new ServiceNode("(2) on T2", minutes(34));


            var m3 = new MovementNode("(1) to T4", track2secs(2, 1));



            var p3a = new ParkingNode("(1) P3");
            var p3b = new ParkingNode("(2) P3");

            var arm2a = new MovementNode("(3) to T3", track2secs(2, 1));
            var arm2b = new TurnAroundNode(turnaround2secs(4));
            var arm2c = new MovementNode("(3) to T1", track2secs(2, 1));

            var m4 = new MovementNode("(2) to G", track2secs(3, 2));
            



            var m5a = new MovementNode("(1) to T3", track2secs(2, 1));
            var m5b = new TurnAroundNode(turnaround2secs(3));
            var m5c = new MovementNode("(1) to T2", track2secs(2, 1));


            var s2 = new ServiceNode("(1) on T2", minutes(34));

            var m6 = new MovementNode("(1) to T1", track2secs(3, 2));

            var m7 = new SplitNode("(3) (1) to (3, 1)", minutes(3));


            var m8a = new MovementNode("(3, 1) to T4", track2secs(3, 2));
            var m8b = new TurnAroundNode(turnaround2secs(7));
            var m8c = new MovementNode("(3, 1) to G", track2secs(3, 2));

            var dep1 = new DepartureNode("(2)", minutes(90));
            var dep2 = new DepartureNode("(3, 1)", hours(2));

            
            ar1.AddSuccessor(arm1);
            ar2.AddSuccessor(arm2a);
            split12.AddSuccessor(m2);
            split12.AddSuccessor(m3);

            split12.AddSuccessor(p3a);
            split12.AddSuccessor(p3b);


            p3a.AddSuccessor(m2);
            p3b.AddSuccessor(m3);
            m2.AddSuccessor(s1);
            m2.AddSuccessor(m3);
            arm1.AddSuccessor(split12);
            arm2a.AddSuccessor(arm2b);
            arm2b.AddSuccessor(arm2c);
            s1.AddSuccessor(m4);
            m4.AddSuccessor(dep1);
            m3.AddSuccessor(m5a);
            m5a.AddSuccessor(m5b);
            m5b.AddSuccessor(m5c);
            m5c.AddSuccessor(s2);
            s2.AddSuccessor(m6);
            m6.AddSuccessor(m7);
            arm2c.AddSuccessor(m7);
            m7.AddSuccessor(m8a);
            m8a.AddSuccessor(m8b);
            m8b.AddSuccessor(m8c);
            m8c.AddSuccessor(dep2);
            



            // Other
            graph.ArrivalNodes.Add(ar1);
            graph.ArrivalNodes.Add(ar2);
            graph.DepartureNodes.Add(dep1);
            graph.DepartureNodes.Add(dep2);

            graph.Propagate();
            return graph;

        }

        public void Propagate()
        {
            this.ArrivalNodes.ForEach(x => x.AdjustStartTimes(0));
            this.DepartureNodes.ForEach(x => x.AdjustEndTimes(0));
        }

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

        private float _start = 0;
        private float _end = float.MaxValue;
        protected float Start
        {
            get => _start;
            set => _start = Math.Max(_start, value);
        }
        protected float End
        {
            get => _end;
            set => _end = Math.Min(_end, value);
        }

        protected Node(int maxIn, int maxOut, string name)
        {
            this.MaxIn = maxIn;
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

        public static Node operator +(Node a, Node b) {
            a.AddSuccessor(b);
            return a;
        }
		

		public abstract void AdjustStartTimes(float start, bool stick = false);
		public abstract void AdjustEndTimes(float end, bool stick = false);
		public virtual BarBox CreateBar() => null;

		public int MaxOut { get; }
		public int MaxIn { get; }
		public string Name { get; }

	}

	class ArrivalNode : Node
	{
		
		public ArrivalNode(String tag, int start) : base(0, 1, $"ArrivalNode: {tag}")
		{
			this.Start = start;
		}

		public override void AdjustStartTimes(float _, bool stick = false) => Successors.ForEach(s => s.AdjustStartTimes(this.Start, true));

		public override BarBox CreateBar() => new BarBox.Builder(this.Name)
				.SetStart(Start)
				.SetEnd(Start)
				.SetDuration(0)
                .SetLineColor(Color.Blue)
				.Build();
		

		public override void AdjustEndTimes(float end, bool stick = false) {}
	}

	class DepartureNode : Node
	{

		public DepartureNode(String tag, float end) : base(1, 0, $"DepartureNode: {tag}")
		{
			this.End = end;
		}

		public override void AdjustStartTimes(float _, bool stick = false) {}
		public override void AdjustEndTimes(float end, bool stick = false) => Predecessors.ForEach(p => p.AdjustEndTimes(this.End, true));

		public override BarBox CreateBar() => new BarBox.Builder(this.Name)
			.SetStart(End)
			.SetEnd(End)
			.SetLineColor(Color.Red)
			.SetDuration(0)
			.Build();
		
	}

	class MovementNode : Node
    { 
		public readonly float Duration;

		public MovementNode(string tag, float duration) : base(3,3, $"MovementNode: {tag}")
		{
			this.Duration = duration;
		}

		public override void AdjustStartTimes(float start, bool stick = false)
		{            
			this.Start = Math.Max(Start, start);

            if (stick)
            {
                this.End = this.Start + Duration;
            }

			this.Successors.ForEach(s => s.AdjustStartTimes(Start + Duration, stick));
		}

		public override void AdjustEndTimes(float end, bool stick = false)
		{
			this.End = Math.Min(End, end);

            if (stick)
            {
                this.Start = this.End - this.Duration;
            }

			this.Predecessors.ForEach(s => s.AdjustEndTimes(End - Duration, stick));
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

        public override void AdjustEndTimes(float end, bool stick = false)
        {
            this.End = Math.Min(end, End);
            this.Predecessors.ForEach(p => p.AdjustEndTimes(end - Duration, false));
        }
        public override void AdjustStartTimes(float start, bool stick = false)
        {
            this.Start = Math.Max(start, Start);
            this.Successors.ForEach(s => s.AdjustStartTimes(start + Duration));
        }

        public override BarBox CreateBar() => new BarBox.Builder(this.Name)
            .SetStart(Start)
            .SetEnd(End)
            .SetDuration(Duration)
            .SetBoxColor(Color.BlueViolet)
            .Build();

    }

    class SplitNode : Node
    {
        public readonly float Duration;
        

        public SplitNode(string tag, float duration) : base(1, 1, $"SplitNode: {tag}")
        {
            this.Duration = duration;
        }

        public override void AdjustEndTimes(float end, bool stick = false)
        {
            this.End = Math.Min(end, End);
            this.Predecessors.ForEach(p => p.AdjustEndTimes(end - Duration));
        }
        public override void AdjustStartTimes(float start, bool stick = false)
        {
            this.Start = Math.Max(this.Start, start);            
            
            this.Successors.ForEach(s => s.AdjustStartTimes(start + Duration));
        }

        public override BarBox CreateBar() => new BarBox.Builder(Name)
            .SetStart(Start)
            .SetEnd(End)
            .SetDuration(Duration)
            .SetBoxColor(Color.Aqua)
            .Build();
        
    }

    class ServiceNode : Node
	{
		public readonly float Duration;

		public ServiceNode(string tag, float duration) : base(1, 1, $"ServiceNode: {tag}")
		{
			this.Duration = duration;
		}

		public override void AdjustStartTimes(float start, bool stick = false)
		{
			this.Start = Math.Max(start, Start);

            if (stick)
            {
                this.End = this.Start + Duration;
            }

			this.Successors.ForEach(s => s.AdjustStartTimes(Start + Duration, stick));
		}

		public override void AdjustEndTimes(float end, bool stick)
		{
			this.End = Math.Min(end, End);
            if (stick)
            {
                this.Start = this.End - Duration;
            }
			this.Predecessors.ForEach(s => s.AdjustEndTimes(End - Duration, stick));
		}

		public override BarBox CreateBar() => new BarBox.Builder(this.Name)
		.SetStart(Start)
		.SetEnd(End)
		.SetDuration(Duration)
		.SetBoxColor(Color.DarkOliveGreen)
		.Build();
	}

    class ParkingNode : Node
    {
        
        public ParkingNode(string tag) : base(1, 1, $"P: {tag}")
        {            
        }

        public override void AdjustEndTimes(float end, bool stick = false)
        {
            this.End = Math.Min(end, End);
            this.Predecessors.ForEach(p => p.AdjustEndTimes(end, false));
        }
        public override void AdjustStartTimes(float start, bool stick = false)
        {
            this.Start = Math.Max(start, Start);
            this.Successors.ForEach(s => s.AdjustStartTimes(start, false));
        }

        /*public override BarBox CreateBar() => new BarBox.Builder(this.Name)
            .SetStart(Start)
            .SetEnd(End)            
            .SetBoxColor(null)
            .Build();
        */

        }
    }
