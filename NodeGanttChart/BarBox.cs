using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    class BarSet
    {
        IList<BarBox> Barboxes;
        private readonly int X;
        private readonly int Y;
        private readonly int Width;
        private readonly int MaxSeconds;

        public BarSet(int x, int y, int width, int maxSeconds, IList<BarBox> barboxes)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.MaxSeconds = maxSeconds;
            this.Barboxes = barboxes;
        }

        public void Draw(Graphics g)
        {
            float secondsPerUnit = 60 * Width / (float)MaxSeconds;
            int totalMinutes = MaxSeconds / 60;

            Pen major = new Pen(Color.Gray, 0.2f);
            Pen minor = new Pen(Color.LightGray, 0.2f);

            for (int i = 0; i <= totalMinutes; i++)
            {
                var x = X + i * secondsPerUnit;
                if ( i % 5 == 0)
                {
                    g.DrawLine(major, x, Y, x, Y + 20 * Barboxes.Count);
                } else
                {
                    g.DrawLine(minor, x, Y, x, Y + 20 * Barboxes.Count);
                }
                
            }

            for (int i = 0; i < Barboxes.Count; i++)
            {
                Barboxes[i].Draw(g, X, Y + 20 * i, Width, 20, MaxSeconds);
            }
        }
    }
	class BarBox
	{
		public BarBox(String name, float start, float end, float? duration, Color? boxColor, Color lineColor)
		{
			this.Start = start;
			this.End = end;
			this.Duration = duration;
			this.BoxColor = boxColor;
			this.LineColor = lineColor;
			this.Name = name;
		}

		public String Name { get; }
		public float Start { get; }
		public float End { get; }
		public float? Duration { get; }
		public Color? BoxColor { get; }
		public Color LineColor { get; }

		public void Draw(Graphics g, int x, int y, int width, int height, float maxTime)
		{
			float secondsPerUnit = width / maxTime;
			float lx = Start * secondsPerUnit + x;
			float rx = End * secondsPerUnit + x;

			Font font = new Font(FontFamily.GenericSansSerif, 8);
			
			Pen pen = new Pen(LineColor);

			var size = g.MeasureString(Name, font);

			g.DrawString(Name, font, new SolidBrush(LineColor), x - size.Width, y);

            if (BoxColor.HasValue && Duration.HasValue)
            {
                Brush brush = new SolidBrush(BoxColor.Value);
                g.FillRectangle(brush, lx, y, Duration.Value * secondsPerUnit, height);
            }

			
			g.DrawLine(pen, lx, y, lx, y + height);
			g.DrawLine(pen, rx, y, rx, y + height);
			g.DrawLine(pen, lx, y + height / 2, rx, y + height / 2);
		}

		public class Builder
		{
			string Name;
			float Start;
			float End;
			float? Duration;
			string Tag;
			Color? BoxColor = Color.MediumVioletRed;
			Color LineColor = Color.Black;

			public Builder(string name)
			{
				this.Name = name;
			}

			public Builder SetTag(string tag)
			{
				this.Tag = tag;
				return this;
			}

			public Builder SetStart(float start)
			{
				this.Start = start;
				return this;
			}

			public Builder SetEnd(float end)
			{
				this.End = end;
				return this;
			}

			public Builder SetDuration(float seconds)
			{
				this.Duration = seconds;
				return this;
			}

			public Builder SetBoxColor(Color? color)
			{
				this.BoxColor = color;
				return this;
			}

			public Builder SetLineColor(Color color)
			{
				this.LineColor = color;
				return this;
			}

			public BarBox Build()
			{
				Trace.Assert(Start + Duration.GetValueOrDefault(0) <= End, $"A Barbox: {Name} yielded an infeasible result");

				return new BarBox(Name, Start, End, Duration, BoxColor, LineColor);
			}
		}

	}
}
