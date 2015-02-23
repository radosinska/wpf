using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace NewtonsCradle
{
    class Element
    {
        public Ellipse _ellipse{get;set;}
        public Line _line { get; set; }
        //public int _nr { get; set; } // numer elementu na liście

        public double _angle = 0.0;
        public double _velocity = 0.0;
        public double _acceleration = 0.0;

        public Element(Ellipse ellipse, Line line)
        {
            _ellipse = ellipse;
            _line = line;
            //_nr = nr;
        }
        //private Element(Ellipse ellipse);

        public void Draw(double mouseX)
        {
            double y = returnY(_line.X1,_line.Y1,175,mouseX);
            _ellipse.SetValue(Canvas.LeftProperty, mouseX - (_ellipse.ActualWidth  / 2)); //set x
            _ellipse.SetValue(Canvas.TopProperty,  y      - (_ellipse.ActualHeight / 2)); //set y

            _line.X2 = mouseX;
            try
            {
                _line.Y2 = y;
            }
            catch
            {
                _line.Y2 = (_ellipse.ActualWidth / 2);
            }
        } 

        public double returnY(double a, double b,  double r, double x) 
        {
            // (a,b)    -   środek okręgu 
            // r        -   promień 
            // x        -   jedna ze współrzędnych na okręgu
            double y;
            y = Math.Sqrt(Math.Pow(r, 2) - Math.Pow((x - a), 2)) + b;

            return y;
        }

       public void Verlet(double dt)
       {
           double angle_next;
           double velocity_halfnext;
           double velocity_next;
           double acceleration_next;

           angle_next = _angle + _velocity * dt + 0.5 * (-9.81 / 175) * Math.Sin(_angle) * Math.Pow(dt, 2);
           velocity_halfnext = _velocity + 0.5 * (-9.81 / 175) * Math.Sin(_angle) * dt;
           acceleration_next = (-9.81 / 175) * Math.Sin(angle_next);
           velocity_next = velocity_halfnext + 0.5 * acceleration_next * dt;

           System.Diagnostics.Debug.WriteLine("angle_next: " + angle_next);
           System.Diagnostics.Debug.WriteLine("velocity_next: " + velocity_next);
           System.Diagnostics.Debug.WriteLine("acceleration_next: " + acceleration_next);

           _angle = angle_next;
           _velocity = velocity_next;
           _acceleration = acceleration_next;

       }
       
       public Ellipse Shadow()
       {
           Ellipse ellipse=new Ellipse();

           double x = _line.X1 - (175 * Math.Sin(_angle));
           double y = returnY(_line.X1, _line.Y1, 175, x);

           ellipse.SetValue(Canvas.LeftProperty, x - (_ellipse.ActualWidth / 2)); //set x
           ellipse.SetValue(Canvas.TopProperty, y - (_ellipse.ActualHeight / 2)); //set y
           ellipse.Width = 50;
           ellipse.Height = 50;

           return ellipse;
       }


    }
}
