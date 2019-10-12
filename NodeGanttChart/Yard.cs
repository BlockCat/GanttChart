using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    enum PointState { LEFT, RIGHT, SWITCH }
    class Yard: Panel
    {
        private Dictionary<String, Track> tracks;
        private Dictionary<String, Switch> switches;
        private List<Connection> connections;
        public int Columns;
        public int Rows;

        // The yard is drawn in a grid sorry not sorry
        public Yard(int width, int height)
        {
            this.Columns = width;
            this.Rows= height;            
            this.tracks = new Dictionary<string, Track>();
            this.switches = new Dictionary<string, Switch>();
            this.connections = new List<Connection>();
        }


        public static Yard Parse(String input)
        {
            int width, height;
            

            string[] lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (!lines[0].Contains("x")) {
                throw new FormatException("First line should contain the dimensions of the grid");            
            }

            {
                string[] temp = lines[0].Split('x');
                width = int.Parse(temp[0]);
                height = int.Parse(temp[1]);
            }

            Yard yard = new Yard(width, height);

            {
                Regex tracks = new Regex(@"(?<name>T[\w]+?) (?<length>[0-9]+)m on \((?<x>[0-9]+),\s?(?<y>[0-9]+)\) w(?<size>[0-9]+)");
                var collection = tracks.Matches(input);

                foreach (Match match in collection)
                {
                    String name = match.Groups["name"].Value;
                    int length = int.Parse(match.Groups["length"].Value);
                    int x = int.Parse(match.Groups["x"].Value);
                    int y = int.Parse(match.Groups["y"].Value);
                    int w = int.Parse(match.Groups["size"].Value);

                    yard.tracks[name] = new Track(name, length, new Point(x, y), w);
                }
            }

            {
                Regex switches = new Regex(@"(?<name>W[\w]+?) on \((?<x>[0-9]+),\s?(?<y>[0-9]+)\)");
                var collection = switches.Matches(input);

                foreach (Match match in collection)
                {
                    String name = match.Groups["name"].Value;
                    int x = int.Parse(match.Groups["x"].Value);
                    int y = int.Parse(match.Groups["y"].Value);

                    yard.switches[name] = new Switch(name, x, y);
                }
            }

            {
                Regex lrx = new Regex(@"l\((?<id>\w+?)\)");
                Regex rrx = new Regex(@"r\((?<id>\w+?)\)");
                Regex nrx = new Regex(@"(?<id>\w+)");
                Func<String, Point> extract = s =>
                {
                    var lm = lrx.Match(s);
                    if (lm.Success)
                    {
                        String id = lm.Groups["id"].Value;
                        Track track = yard.tracks[id];
                        return new Point(track.left.X, track.left.Y);
                    }
                    var rm = rrx.Match(s);
                    if (rm.Success)
                    {
                        String id = rm.Groups["id"].Value;
                        Track track = yard.tracks[id];
                        return new Point(track.right.X, track.right.Y);
                    }
                    var nm = nrx.Match(s);
                    if (nm.Success)
                    {
                        String id = nm.Groups["id"].Value;
                        Switch sw = yard.switches[id];
                        return new Point(sw.X, sw.Y);
                    }

                    throw new FormatException($"{s} was invalid format");
                };

                foreach (var line in lines)
                {
                    if (!line.StartsWith("---")) continue;

                    string[] entry = line.Replace("---", "").Split(new[] { " - " }, StringSplitOptions.None);

                    var one = extract(entry[0]);
                    var two = extract(entry[1]);

                    yard.connections.Add(new Connection(one, two));
                }
            }

            return yard;
        }
        public static Yard CreateTracks()
        {
            String input = @"17x6

TA 230m on (0,2) w3
T1 230m on (4,1) w4
T2 130m on (4,2) w4
T3 120m on (10,2) w4
T4 120m on (11,3) w4
T5 240m on (12,4) w4
T6 180m on (12,5) w4
T7 120m on (9,0) w2

WA1 on (3,2)
W17 on (8,1)
W23 on (9,2)
W24 on (10,3)
W25 on (11,4)

--- r(TA) - WA1
--- WA1 - l(T1)
--- WA1 - l(T2)
--- r(T1) - W17
--- W17 - l(T7)
--- W17 - W23
--- r(T2) - W23
--- W23 - l(T3)
--- W23 - W24
--- W24 - l(T4)
--- W24 - W25
--- W25 - l(T5)
--- W25 - l(T6)
";
            return Parse(input);
        }


        public void CheckHover(object o, MouseEventArgs mea)
        {

        }

        public void Draw(object o, PaintEventArgs pea)
        {
            int gridWidth = this.Width / this.Columns;
            int gridHeight = this.Height / this.Rows;
            Graphics g = pea.Graphics;

            for (int i = 0; i < Columns; i += gridWidth)
            {
                g.DrawLine(Pens.LightGray, i, 0, i, Height);
            }
            for (int i = 0; i <Rows; i+= gridHeight)
            {
                g.DrawLine(Pens.LightGray, 0, i, Width, i);
            }

            foreach(var t in tracks.Values)
            {
                int txl = t.left.X * gridWidth + gridWidth / 2;
                int tyl = t.left.Y * gridHeight + gridHeight / 2;
                int txr = t.right.X * gridWidth + gridWidth / 2;
                int tyr = t.right.Y * gridHeight + gridHeight / 2;
                //g.FillRectangle(Brushes.Red, x + t.rect.X + OFFSET, y + t.rect.Y, t.rect.Width - OFFSET * 2, t.rect.Height);
                g.FillRectangle(SystemBrushes.HotTrack, txl + gridWidth / 4, tyl - gridHeight / 3, txr - txl - gridWidth / 2, 2 * gridHeight / 3);
                g.DrawString($"{t.Name}, L: {t.Length}", SystemFonts.DefaultFont, Brushes.Black, txl, tyl);
                g.DrawLine(Pens.Black, txl, tyl, txr, tyr);
            }

            connections.ForEach(t =>
            {
                int shlx = t.left.X * gridWidth + gridWidth / 2;
                int shly = t.left.Y * gridHeight + gridHeight / 2;
                int shrx = t.right.X * gridWidth + gridWidth / 2;
                int shry = t.right.Y * gridHeight + gridHeight / 2;
                g.DrawLine(Pens.Black, shlx, shly, shrx, shry);
            });

            foreach (var sw in switches.Values)
            {
                sw.Draw(g, gridWidth, gridHeight);
            }
        }
    }

    abstract class YardShape {
        public abstract void Draw(Graphics g, int gridWidth, int gridHeight);
        public abstract bool CheckHovering(int mx, int my, int gridWidth, int gridHeight);
    }

    internal class Switch: YardShape
    {
        public readonly string Name;
        public readonly int X;
        public readonly int Y;
        private bool Hovering;

        public Switch(string name, int x, int y)
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
        }

        public override void Draw(Graphics g, int gridWidth, int gridHeight)
        {
            int thicknessX = Hovering ? gridWidth / 2 : gridWidth / 3;
            int thicknessY = Hovering ? gridWidth / 2 : gridWidth / 3;
            int swx = X * gridWidth + gridWidth / 2;
            int swy = Y * gridHeight + gridHeight / 2;
            g.FillEllipse(Brushes.PaleVioletRed, swx - thicknessX / 2, swy - thicknessY / 2, thicknessX, thicknessY);
        }

        public override bool CheckHovering(int mx, int my, int gridWidth, int gridHeight)
        {
            this.Hovering = mx / gridWidth == X && my / gridHeight == Y;
            return this.Hovering;
        }
    }

    struct Connection
    {
        public Point left;
        public Point right;
        public bool sw;

        public Connection(Point l, Point r)
        {
            this.left = l;
            this.right = r;
            this.sw = true;
        }
        public Connection(Point l, Point r, bool dope)
        {
            this.left = l;
            this.right = r;
            this.sw = dope;
        }

        
    }

    class Track
    {        
        public String Name;
        public int Length;

        public Point left;
        public Point right;

        public Track(String name, int length, Point start, int width)
        {
            this.Name = name;
            this.Length = length;

            this.left = start;
            this.right = Point.Add(start, new Size(width, 0));
            
        }
    }


}
