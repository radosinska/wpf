using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace NewtonsCradle
{

    public partial class MainWindow : Window
    {

        Element[] _elements=new Element[5];     
        
        public MainWindow()
        {
            InitializeComponent();
            _elements[0] = new Element(MyEllipse1, MyLine1);
            _elements[1] = new Element(MyEllipse2, MyLine2);
            _elements[2] = new Element(MyEllipse3, MyLine3);
            _elements[3] = new Element(MyEllipse4, MyLine4);
            _elements[4] = new Element(MyEllipse5, MyLine5);

        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse = e.GetPosition(MyCanvas);
            double mouseX = mouse.X;
            double mouseY = mouse.Y;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (MyEllipse1.IsMouseOver)
                {
                    _elements[0].Draw(mouseX);

                }
                if (MyEllipse2.IsMouseOver)
                {

                    _elements[0].Draw(mouseX - 50);
                    _elements[1].Draw(mouseX);
                }
                if (MyEllipse3.IsMouseOver)
                {
                    _elements[0].Draw(mouseX - 100);
                    _elements[1].Draw(mouseX - 50);
                    _elements[2].Draw(mouseX);
                }

                if (MyEllipse4.IsMouseOver)
                {
                    _elements[0].Draw(mouseX - 150);
                    _elements[1].Draw(mouseX - 100);
                    _elements[2].Draw(mouseX - 50);
                    _elements[3].Draw(mouseX);
                }

                if (MyEllipse5.IsMouseOver)
                {
                    _elements[0].Draw(mouseX - 200);
                    _elements[1].Draw(mouseX - 150);
                    _elements[2].Draw(mouseX - 100);
                    _elements[3].Draw(mouseX - 50);
                    _elements[4].Draw(mouseX);
                }
            }
        }

        private void MyButtonStart_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                _elements[i]._angle = Math.Asin((_elements[i]._line.X1 - _elements[i]._line.X2) / 175);     //radiany
            }

            Thread thread = new Thread(new ThreadStart(symulation));
            thread.Start();
            
        }

        private void symulation()
        {
            double dt = 0.1;
            double e = 1;
            List<TwoInt> intersects= new List<TwoInt>();

            for(int a=0; a<5;a++)
            {
                _elements[a].Verlet(dt);    //policzy atrybuty w następnej chwili i podstawia za obecne
            }

            for(int a=0;a<5;a++)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    int b;
                    
                    if (a==0)
                    {
                        b = 1;
                    }
                    else if (a==4)
                    {
                        b = 3;
                    }
                    else if (_elements[a]._angle>0 )
                    {
                        b = a + 1;
                    }
                    else
                    {
                        b = a - 1;
                    }

                    Ellipse ellipse = new Ellipse();
                    ellipse = _elements[a].Shadow();
                    Rect rect1 = new Rect((double)ellipse.GetValue(Canvas.LeftProperty), (double)ellipse.GetValue(Canvas.TopProperty), ellipse.Width-0.5, ellipse.Height);

                    Ellipse ellipse2 = new Ellipse();
                    ellipse2 = _elements[b].Shadow();
                    Rect rect2 = new Rect((double)ellipse2.GetValue(Canvas.LeftProperty), (double)ellipse2.GetValue(Canvas.TopProperty), ellipse2.Width-0.5, ellipse2.Height);

                    //czy obiekty nachodzą na siebie
                    if (rect1.IntersectsWith(rect2))
                    {
                        if (!intersects.Exists(x => x.a == b && x.b == a))
                        {
                            intersects.Add(new TwoInt(a, b));
                        }

                    }
                }));
            }
            
            foreach(TwoInt pair in intersects)
            {
                //System.Diagnostics.Debug.WriteLine("aa: " +pair.a + " bb: " + pair.b);
                //System.Diagnostics.Debug.WriteLine("bb: " + pair.b);
                
                //zderzenie
                System.Diagnostics.Debug.WriteLine("ZDERZENIE");

                double velocity12 = 0.0;
                double velocity22 = 0.0;

                velocity12 = _elements[pair.a]._velocity * ((1 - e) / 2) + _elements[pair.b]._velocity * ((e + 1) / 2);
                velocity22 = _elements[pair.a]._velocity * ((1 + e) / 2) + _elements[pair.b]._velocity * ((-e + 1) / 2);

                _elements[pair.a]._velocity = velocity12;   //prędkości po zderzeniu
                _elements[pair.b]._velocity = velocity22;
                System.Diagnostics.Debug.WriteLine("u1: " + velocity12 + " u2: " + velocity22);

                //if(Math.Abs(velocity12)<Math.Abs(velocity22))
                //{
                //    _elements[pair.a]._angle = 0.0;
                   
                //    Dispatcher.Invoke((Action)(() =>
                //    {
                //        _elements[pair.a].Draw(_elements[pair.a]._line.X1);
                //    }));
                //}
                //else
                //{
                //    _elements[pair.b]._angle = 0.0;
                //    Dispatcher.Invoke((Action)(() =>
                //    {
                //        _elements[pair.b].Draw(_elements[pair.b]._line.X1);
                //    }));
                    
                //}

            }

            for(int a=0; a<5;a++)
            {
                if (!intersects.Exists(x => x.a == a && x.b == a))
                {
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            _elements[a].Draw(_elements[a]._line.X1 - (175 * Math.Sin(_elements[a]._angle)));
                        }));

                    }
                }
            }
                        
            System.Threading.Thread.Sleep(10);
            symulation();
        }
    }
}
