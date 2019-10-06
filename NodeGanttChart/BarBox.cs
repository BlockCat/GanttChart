using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
	class BarBox
	{
		public BarBox(String name, float start, float end, float duration, Color boxColor, Color lineColor)
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
		public float Duration { get; }
		public Color BoxColor { get; }
		public Color LineColor { get; }

		public void Draw(Graphics g, int x, int y, int width, int height, float maxTime)
		{
			float secondsPerUnit = width / maxTime;
			float lx = Start * secondsPerUnit + x;
			float rx = End * secondsPerUnit + x;

			Font font = new Font(FontFamily.GenericSansSerif, 8);
			Brush brush = new SolidBrush(BoxColor);
			Pen pen = new Pen(LineColor);

			var size = g.MeasureString(Name, font);

			g.DrawString(Name, font, brush, x - size.Width, y);
			g.FillRectangle(brush, lx, y, Duration * secondsPerUnit, height);
			g.DrawLine(pen, lx, y, lx, y + height);
			g.DrawLine(pen, rx, y, rx, y + height);
			g.DrawLine(pen, lx, y + height / 2, rx, y + height / 2);
		}

		public class Builder
		{
			string Name;
			float Start;
			float End;
			float Duration;
			string Tag;
			Color BoxColor = Color.MediumVioletRed;
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

			public Builder SetBoxColor(Color color)
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
				Trace.Assert(Start + Duration <= End, "A Barbox yielded an infeasible result");

				return new BarBox(Name, Start, End, Duration, BoxColor, LineColor);
			}
		}

	}
}
