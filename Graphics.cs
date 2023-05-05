using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WinForms16_lab_304_kurdekb
{
    public class Blueprint
    {
        public Bitmap Bitmap { get; private set; }
        public Color backgroundColor = Color.White;
        public List<(string, int, int)> data_list;
        public List<BarChart> barCharts;
        public List<LogBarChart> LogBarCharts;

        public bool isReady = false;
        int width;
        int height;

        public Blueprint(int weight, int height, List<(string,int,int)> data_list)
        {
            width = weight;
            this.height = height;
            Bitmap = new Bitmap(weight, height);
            this.data_list = data_list;
            barCharts = new List<BarChart>();
            LogBarCharts = new List<LogBarChart>(); 
            CreateCharts();

            //Draw_BarCharts();
        }

        public void Draw_BarCharts()
        {
            using (Graphics g = Graphics.FromImage(Bitmap))
            {
                g.Clear(backgroundColor);

                foreach(BarChart chart in barCharts)
                {
                    chart.DrawChart(g);
                }
            }
        }

        public void Draw_BarLogCharts()
        {
            using (Graphics g = Graphics.FromImage(Bitmap))
            {
                g.Clear(backgroundColor);

                foreach (LogBarChart chart in LogBarCharts)
                {
                    chart.DrawChart(g);
                }
            }
        }

        public void CreateCharts()
        {
            //count chart
            List<int> counts = new List<int>();
            List<string> names = new List<string>();
            List<int> sizes = new List<int>();
            foreach (var s in data_list)
            {
                names.Add(s.Item1);
                counts.Add(s.Item2);
                sizes.Add(s.Item3);
            }
            Point p1 = new Point(0, 0);
            Point p2 = new Point(width/2 + 10, 0);
            BarChart countBarChart = new BarChart(counts.ToArray(), names.ToArray(), height, width / 2, p1,false);
            BarChart sizeBarChart = new BarChart(sizes.ToArray(), names.ToArray(), height, width / 2, p2,true);

            LogBarChart countLogBarChart = new LogBarChart(counts.ToArray(), names.ToArray(), height, width / 2, p1, false);
            LogBarChart sizeLogBarChart = new LogBarChart(sizes.ToArray(), names.ToArray(), height, width / 2, p2, true);

            barCharts.Add(countBarChart);
            barCharts.Add(sizeBarChart);

            LogBarCharts.Add(countLogBarChart);
            LogBarCharts.Add(sizeLogBarChart);
        }
        
    }
    public class BarChart
    {
        public int[] values;
        public string[] names;
        public int height;
        public int width;
        public Point position;
        public Font font;
        public Pen pen = new Pen(Brushes.Black,1);
        public bool Size;

        

        public BarChart(int[] values, string[] names, int height, int width, Point position,bool Size)
        {
            this.values = values;
            this.names = names;
            this.height = height;
            this.width = width;
            font = new Font("Arial", 6, FontStyle.Regular);
            this.Size = Size;
            
            this.position = position;
        }

        public void DrawChart(Graphics g)
        {
            if(Size)
            {
                for(int i =0; i<values.Length; i++)
                {
                    values[i] = values[i] / (int)(Math.Pow(1024, 2));
                }
            }
            int max_value = values.Max();

            string[] labels= new string[11];
            int max_len = 0;
            int str_pix = 0;
            for(int i =0; i<11; i++)
            {
                int height = max_value * ((10 - i)) / 10;
                if (Size)
                {
                    labels[i] = $"{height} MB";
                }
                else
                {
                    labels[i] = $"{height}  ";
                }
                if (labels[i].Length > max_len)
                {
                    //FontMetrics metrics = graphics.getFontMetrics(font);
                    str_pix = (int)g.MeasureString(labels[i], font).Width + 10;
                }
            }
            if (Size == false) str_pix += 14;


            g.FillRectangle(Brushes.Gray,position.X + str_pix, position.Y, width - str_pix, height - 25);

            int chart_height = height - 35;
            int step = (chart_height) / 10;
            for(int i =0; i<11; i++)
            {
                int height = max_value * ((10 - i) )/ 10;
                g.DrawRectangle(pen,position.X +str_pix  , position.Y + 10+ (i) * step, width - str_pix - 1, 1);

                g.DrawString(labels[i], font, Brushes.Black, new Point(position.X, position.Y + 5 + (i) * step));

            }

            int current_width = width - 1 * str_pix;

            int bar_width = (current_width / values.Length);

            for(int i =0; i<values.Length; i++)
            {
                Color c = ColorFromHSV(i * 30 + 12, 0.90, 1);
                Brush b = new SolidBrush(c);
                int curr_height = (int) ( chart_height * (double) ((double)values[i]) / (double)max_value); 
                g.FillRectangle(b, position.X + str_pix + 10 + i * bar_width,position.Y + 10 + chart_height - curr_height, bar_width - 10, curr_height);
                g.DrawString(names[i], font, Brushes.Black, new Point(position.X  + 4+str_pix+ i * (bar_width ) ,position.Y +  +chart_height + 20));
            }
            return;
        }
        //https://stackoverflow.com/questions/1335426/is-there-a-built-in-c-net-system-api-for-hsv-to-rgb
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }


    }

    public class LogBarChart : BarChart
    {
        public LogBarChart(int[] values, string[] names, int height, int width, Point position, bool Size) : base(values, names, height, width, position, Size)
        {}

        public void DrawChart(Graphics g)
        {
            double[] vals = new double[values.Length];
            
            for (int i = 0; i < values.Length; i++)
            {
                vals[i] = Math.Log(values[i],10);
            }
            
            double max_value = vals.Max();

            string[] labels = new string[11];
            int max_len = 0;
            int str_pix = 0;
            for (int i = 0; i < 11; i++)
            {
                double height =  max_value * ((10 - i)) / 10;
                if (Size)
                {
                    labels[i] = $"{Math.Pow(10, height):E2} B";
                }
                else
                {
                    labels[i] = $"{Math.Pow(10, height):F2}  ";
                }
                if (labels[i].Length > max_len)
                {
                    //FontMetrics metrics = graphics.getFontMetrics(font);
                    str_pix = (int)g.MeasureString(labels[i], font).Width + 10;
                }
            }
            if (Size == false) str_pix += 14;


            g.FillRectangle(Brushes.Gray, position.X + str_pix, position.Y, width - str_pix, height - 25);

            int chart_height = height - 35;
            int step = (chart_height) / 10;
            for (int i = 0; i < 11; i++)
            {
                int height =(int)  max_value * ((10 - i)) / 10;
                g.DrawRectangle(pen, position.X + str_pix, position.Y + 10 + (i) * step, width - str_pix - 1, 1);

                g.DrawString(labels[i], font, Brushes.Black, new Point(position.X, position.Y + 5 + (i) * step));

            }

            int current_width = width - 1 * str_pix;

            int bar_width = (current_width / values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                Color c = ColorFromHSV(i * 30 + 12, 0.90, 1);
                Brush b = new SolidBrush(c);
                int curr_height = (int)(chart_height * (vals[i] / max_value));
                g.FillRectangle(b, position.X + str_pix + 10 + i * bar_width, position.Y + 10 + chart_height - curr_height, bar_width - 10, curr_height);
                g.DrawString(names[i], font, Brushes.Black, new Point(position.X + 4 + str_pix + i * (bar_width), position.Y + +chart_height + 20));
            }
            return;
        }


    }


}
