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
        int licznik;
        int[] iterator = { 0, 1, 2, 3, 4, 3, 2, 1 };
        
        
        public MainWindow()
        {

            InitializeComponent();
            _elements[0] = new Element(MyEllipse1, MyLine1);
            _elements[1] = new Element(MyEllipse2, MyLine2);
            _elements[2] = new Element(MyEllipse3, MyLine3);
            _elements[3] = new Element(MyEllipse4, MyLine4);
            _elements[4] = new Element(MyEllipse5, MyLine5);

           

            //MyEllipse1.MouseLeftButtonDown += MyEllipse1_MouseLeftButtonDown;
            

        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse = e.GetPosition(MyCanvas);
            double mouseX = mouse.X;
            double mouseY = mouse.Y;

            if (_elements[0]._ellipse.IsMouseOver)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _elements[0].Draw(mouseX);

                }
                
            }
            
            //if (MyEllipse2.IsMouseOver)
            //{
            //    if (e.LeftButton == MouseButtonState.Pressed)
            //    {

            //        _elements[0].Draw(mouseX - 50);
            //        _elements[1].Draw(mouseX);


            //    }
            //}
        }



        private void MyButtonStart_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("x: "+ MyLine1.X2+" y: "+MyLine1.Y2);
            //h=175 ; x1-x2; sin(a)=(x1-x2)/175 -> a=arcsin[ (x1/x2)/175 ]
            System.Diagnostics.Debug.WriteLine("x1-x2: " + (MyLine1.X1 - MyLine1.X2) + " sin(a): " + (MyLine1.X1 - MyLine1.X2)/175);
            System.Diagnostics.Debug.WriteLine("xarcsin->kat: " + Math.Asin((MyLine1.X1 - MyLine1.X2) / 175) + " w stopniach: " + (Math.Asin((MyLine1.X1 - MyLine1.X2) / 175))*(180/Math.PI));

            for (int j = 0; j < 5;j++ )
            {
                _elements[j]._angle = Math.Asin((_elements[j]._line.X1 - _elements[j]._line.X2) / 175); //radiany
                System.Diagnostics.Debug.WriteLine(j + " " + _elements[j]._angle * (180 / Math.PI)); //wyswietla w stopniach
            }



            licznik = 0;            
            Thread thread = new Thread(new ThreadStart(symulation));
            thread.Start();
            

        }

        private void symulation()
        {
            double dt = 0.05;

            int i,j;

            try
            {
                i = iterator[licznik];
            }
            catch
            {
                licznik = 0;
                i = iterator[licznik];
            }
 
            try
            {
                j = iterator[licznik + 1];
            }
            catch
            {
                j = iterator[0];
            }

            System.Diagnostics.Debug.WriteLine("i: " + i);
            
            _elements[i].Verlet(dt); //policzy atrybuty w następnej chwili i podstawia za obecne


            bool intersect = false;

            Dispatcher.Invoke((Action)(() =>
            {
                Ellipse ellipse = new Ellipse(); //tymczasowa elipsa w położeniu następnym

                ellipse = _elements[iterator[i]].Shadow();

                //ellipse.StrokeThickness = 1;
                //ellipse.Stroke = new SolidColorBrush(Colors.Black);
                //MyCanvas.Children.Add(ellipse);

                //obramowanie dla położenia następnego elipsy
                Rect rect1 = new Rect((double)ellipse.GetValue(Canvas.LeftProperty), (double)ellipse.GetValue(Canvas.TopProperty), ellipse.Width, ellipse.Height);

                Ellipse ellipse2 = new Ellipse(); //tymczasowa elipsa w położeniu następnym

                
                ellipse2 = _elements[j].Shadow();

                //obramowanie dla elipsy nr2
                Rect rect2 = new Rect((double)ellipse2.GetValue(Canvas.LeftProperty), (double)ellipse2.GetValue(Canvas.TopProperty), ellipse2.Width, ellipse2.Height);

                //czy obiekty nachodzą na siebie
                if (rect1.IntersectsWith(rect2))
                {
                     intersect = true;
                     Dispatcher.Invoke((Action)(() =>
                     {
                         _elements[i]._angle = 0.0;
                         _elements[i].Draw(_elements[i]._line.X1 - (175 * Math.Sin(_elements[i]._angle)));
                     }));
       
                }
            }));

                    if (!intersect)
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            _elements[i].Draw(_elements[i]._line.X1 - (175 * Math.Sin(_elements[i]._angle)));
                        }));


                    }
                    else 
                    {
                        //zderzenie
                        System.Diagnostics.Debug.WriteLine("ZDERZENIE");

                        double e = 1;
                        double velocity12 = 0.0;
                        double velocity22 = 0.0;


                        
                        velocity12 = _elements[i]._velocity * ((1 - e) / 2) + _elements[j]._velocity * ((e + 1) / 2);
                        velocity22 = _elements[i]._velocity * ((1 + e) / 2) + _elements[j]._velocity * ((-e + 1) / 2);

                        _elements[i]._velocity = velocity12;
                        _elements[j]._velocity = velocity22;

                        System.Diagnostics.Debug.WriteLine("u1: " + velocity12 + " u2: " + velocity22);

                        //_elements[j].Verlet(dt);

                        //Dispatcher.Invoke((Action)(() =>
                        //{
                        //    _elements[j].Draw(_elements[j]._line.X1 - (175 * Math.Sin(_elements[j]._angle)));
                        //}));

                        licznik++;
                        intersect = false;

                    }
                    System.Threading.Thread.Sleep(10);
                    symulation();
                }
            }
}
