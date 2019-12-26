using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;     //Dispatcher함수 쓰기 위해

namespace Digital_Analog_Clock
{
    public partial class MainWindow : Window
    {
        private Point Center;
        private int hourHand;
        private int minHand;
        private double radius;
        private int secHand;
        private DateTime currentTime;
        private bool analog = true;
        private bool sec_flag = true;
        private double radHr;
        private double radMin;
        private double radSec;

        public MainWindow()
        {
            InitializeComponent();
            TimerSetting();            //시간설정
            aClockSetting();        //아날로그 시계판 설정
        }

        private void aClockSetting()    //아날로그 시계판 설정
        {
            Center = new Point(aClock.Width / 2, aClock.Height / 2);//중심점 설정
            radius = aClock.Width / 2;//반지름 설정

            hourHand = (int)(radius * 0.6);//시침의 길이 설정
            minHand = (int)(radius * 0.8);//분침의 길이 설정
            secHand = (int)(radius * 0.9);//초침의 길이 설정


        }

        private void TimerSetting()     //시간 설정
        {
            DispatcherTimer timer = new DispatcherTimer();  //내가 정의하는 시간 설정을 저장
            if (sec_flag == true)                   //sec_flag가 참이면 일초에 한번씩 흐르게
                timer.Interval = new TimeSpan(0, 0, 1); //일초에 한번씩
            else                    //그렇지않으면 1밀리초에 한번으로 시간이 흐르게 설정
                timer.Interval = new TimeSpan(0, 0, 0, 0, 100);//밀리초에 한번씩
            timer.Start();  //timer의설정대로시간이흐르게시작
            timer.Tick += new EventHandler(dispatcherTimer_Tick);   //1초 혹은 1밀리초 지날 때마다 시간을 더함
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)//실제로 시간이 흐르는 것을 나타냄
        {
            Canvas1.Children.Clear();//화면지우기

            if (analog == false)        //디지털
            {
                currentTime = DateTime.Now;//현재시간을 지금으로 설정
                DTimer.Text = DateTime.Today.ToString("D");//오늘날짜를 지정한 서식에 맞게 표시
                string x = null;//x를 null로 초기화

                if (sec_flag == true)  //sec_flag가 참이면 x의 포맷을 시분초에 따라 나타냄
                    x = string.Format("{0:D2}시{1:D2}분{2:D2}초", currentTime.Hour % 12, currentTime.Minute, currentTime.Second);
                else            //그렇지 않으면 시분초밀리초로 나타냄
                    x = string.Format("{0:D2}시{1:D2}분{2:D2}초{3:D3}milsec", currentTime.Hour % 12, currentTime.Minute,
                        currentTime.Second, currentTime.Millisecond);
                dClock.Text = x;  //조건에 따라 설정된 포맷으로 x를 출력->디지털시계가 나타난다.

            }
            else //아날로그
            {
                currentTime = DateTime.Now;//현재시간을 지금으로 설정
                DTimer.Text = DateTime.Today.ToString("D");//오늘 날짜를 지정한 서식에 맞게 표시
                DrawClockFace();//아날로그시계를 그린다.
                dClock.Text = "";//디지털시계는 공백으로.

                radHr = (currentTime.Hour % 12 + currentTime.Minute / 60) * 30 * Math.PI / 180;//시침의각도
                radMin = (currentTime.Minute) * 6 * Math.PI / 180;//분침의각도
                if (sec_flag == true)//sec_flag가 참이면 1초마다 움직이게.
                    radSec = ((currentTime.Second) * 6) * Math.PI / 180;
                else        //그렇지 않으면 0.001초마다 움직이게.
                    radSec = ((currentTime.Second) * 6 + (currentTime.Millisecond * 6 / 1000)) * Math.PI / 180;

                DrawHands(radHr, radMin, radSec);//각도에 맞추어 시침, 분침, 초침을 그린다.
            }
        }

        private void DrawClockFace()//시계판그리기
        {
            //시계판 바깥원
            Ellipse outer = new Ellipse(); //원을 설정
            outer.Stroke = Brushes.Black;//테두리는 검정
            outer.StrokeThickness = 3;//두께는 3
            outer.Width = 300;//넓이는 300
            outer.Height = 300;//높이는 300
            //outer.Fill = Brushes.White;
            Canvas1.Children.Add(outer);//

            //눈금
            int L;
            int strokeThickness;
            SolidColorBrush strokeColor;
            for (int deg = 0; deg < 360; deg += 6)
            {
                double rad = deg * Math.PI / 180;
                if (deg % 30 == 0)
                {
                    L = (int)(radius * 0.95);
                    strokeThickness = 8;
                    strokeColor = Brushes.Black;
                }
                else
                {
                    L = (int)(radius * 0.95);
                    strokeThickness = 3;
                    strokeColor = Brushes.Black;
                }
                DrawLine(L * Math.Sin(rad), -L * Math.Cos(rad), (radius - 2) * Math.Sin(rad), -(radius - 2) * Math.Cos(rad),
                    strokeColor, strokeThickness, new Thickness(Center.X, Center.Y, 0, 0));
                // DTimer.Text = DateTime.Today.ToString("D");

            }
        }

        private void DrawHands(double radHr, double radMin, double radSec)
        {
            //hour
            DrawLine(hourHand * Math.Sin(radHr), -hourHand * Math.Cos(radHr), 0, 0, Brushes.Black, 6, new Thickness(Center.X, Center.Y, 0, 0));

            //minute
            DrawLine(minHand * Math.Sin(radMin), -minHand * Math.Cos(radMin), 0, 0, Brushes.Black, 5, new Thickness(Center.X, Center.Y, 0, 0));

            //second
            DrawLine(secHand * Math.Sin(radSec), -secHand * Math.Cos(radSec), 0, 0, Brushes.Black, 3, new Thickness(Center.X, Center.Y, 0, 0));

            //center 중심점
            Ellipse center = new Ellipse();
            center.Margin = new Thickness(140, 140, 0, 0);
            center.StrokeThickness = 2;
            center.Fill = Brushes.Black;
            center.Width = 20;
            center.Height = 20;
            Canvas1.Children.Add(center);

        }

        private void DrawLine(double x1, double y1, double x2, double y2, SolidColorBrush color, int thickness, Thickness margin)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.Stroke = color;
            line.StrokeThickness = thickness;
            line.Margin = margin;
            line.StrokeEndLineCap = PenLineCap.Round;
            Canvas1.Children.Add(line);
        }
        //메뉴처리:아날로그시계
        private void analog_Click(object sender, RoutedEventArgs e)
        {
            analog = true;
        }

        //메뉴처리:디지털시계
        private void digital_Click(object sender, RoutedEventArgs e)
        {
            analog = false;
        }
        //초단위
        private void Second_Click(object sender, RoutedEventArgs e)
        {
            sec_flag = true;
            TimerSetting();
        }
        //밀리초단위
        private void Milli_Click(object sender, RoutedEventArgs e)
        {
            sec_flag = false;
            TimerSetting();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
