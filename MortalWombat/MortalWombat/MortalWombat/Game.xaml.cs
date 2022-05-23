using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Threading;


namespace MortalWombat
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        int bytesRead = 0;
        byte[] buffer = new byte[1024];
        string response = "";
        MediaPlayer Sound1 = new MediaPlayer();
        MediaPlayer SoundBackground = new MediaPlayer();
        DispatcherTimer timercountdown = new DispatcherTimer();
        DispatcherTimer timerjump = new DispatcherTimer();
        DispatcherTimer timerjumpscorp = new DispatcherTimer();
        DispatcherTimer timercheckcollision = new DispatcherTimer();
        DispatcherTimer timericeball = new DispatcherTimer();
        DispatcherTimer timerbackgroundmusic = new DispatcherTimer();
        DispatcherTimer timerleftScorpionAI = new DispatcherTimer();
        DispatcherTimer timerrightScorpionAI = new DispatcherTimer();
        DispatcherTimer timerleftscorpion = new DispatcherTimer();
        DispatcherTimer timerfireball = new DispatcherTimer();
        DispatcherTimer jumpscorp5sec = new DispatcherTimer();
        Random rnd = new Random();
        public int ifServerConnected = 1;
        Thread checkmessages;
        byte[] toBytes;
        public int moveScorpAI = 1;
        public int leftAIscorpion = 1, rightAIscorpion = 0;
        public int countdown = 3;
        public int jumpcount = 20,jumpcountscorp=20;
        public int hpsub=100, hpscorp = 100;
        public int goneIceball = 0, goneFireball=0;
        public int onlineOrAI = 0;
        public int scorpShoot = 0;
        public int randomAI = 0;

        public TcpClient clientSocket = new TcpClient();
        NetworkStream stream;
        public Game()
        {
            randomAI = rnd.Next(1, 11);
            InitializeComponent();
            
            try
            {
                string ip = "127.0.0.1";
                int port = 8000;
                MessageBox.Show("Started");
                clientSocket.Connect(ip, port);
                MessageBox.Show("Connected");
                checkmessages = new Thread(CheckMessages);
                checkmessages.Start();
                
            }
            catch (Exception)
            {
                MessageBox.Show("Server not available - starting game with AI only");
                ifServerConnected = 0;
            }

            if (ifServerConnected == 1)
                startbtn.Visibility = Visibility.Hidden;
            lblhpsub.Foreground = Brushes.SteelBlue;
            lblhpscorp.Foreground = Brushes.DarkOrange;
            l_countdown.Foreground = Brushes.Tomato;
            lblhpsub.Content += hpsub.ToString();
            lblhpscorp.Content += hpscorp.ToString();
            iceball.Visibility = Visibility.Hidden;
            fireball.Visibility = Visibility.Hidden;
            SoundBackground.Open(new Uri("Resources/background.wav", UriKind.Relative));
            SoundBackground.Play(); 
            timerbackgroundmusic.Interval = TimeSpan.FromSeconds(47);
            timerbackgroundmusic.Tick += timerbackgroundmusic_Tick;
            timerbackgroundmusic.Start();
            
        }



        private void CheckMessages()
        {

            while (true)
            {
                stream = clientSocket.GetStream();

                if (ifServerConnected == 1)
                {
                    
                        
                        //bytesRead = stream.Read(buffer, 0, buffer.Length);
                        //i = 0;
                        //response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        //if (response == "Str")
                        //{                          
                        //    MessageBox.Show("Game Starting in 3 seconds");
                        //    Sound1.Open(new Uri("Resources/button.wav", UriKind.Relative));
                        //    Sound1.Play();
                        //    startbtn.Visibility = System.Windows.Visibility.Hidden;
                        //    l_countdown.Content = countdown;

                        //    timercountdown.Interval = TimeSpan.FromSeconds(1);
                        //    timercountdown.Tick += timercountdown_Tick;
                        //    timercountdown.Start();

                        //}
                        //}

     //                   Application.Current.Dispatcher.BeginInvoke(
     //DispatcherPriority.Background,
     //new Action(() =>
     //{

     //    if (countdown == 3)
     //                   {
             
     //                       bytesRead = stream.Read(buffer, 0, buffer.Length);
     //                       response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

     //                       if (response == "Str")
     //                       {

     //            MessageBox.Show("Game Starting in 3 seconds");
     //            Sound1.Open(new Uri("Resources/button.wav", UriKind.Relative));
     //            Sound1.Play();
     //            l_countdown.Content = countdown;

     //            timercountdown.Interval = TimeSpan.FromSeconds(1);
     //            timercountdown.Tick += timercountdown_Tick;
     //            timercountdown.Start();

     //                   }

     //                   }
     //               }));

                    if (countdown == 0 || countdown==3)
                        {

                            try
                            {
                                //int intValue = 1;
                                //byte[] intBytes = BitConverter.GetBytes(intValue);
                                //Array.Reverse(intBytes);
                                //byte[] result = intBytes;
                                //clientStream = clientSocket.GetStream();
                                //clientStream.Write(result, 1, 1)

                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                                response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                //MessageBox.Show(response);

                                Application.Current.Dispatcher.BeginInvoke(
             DispatcherPriority.Background,
             new Action(() =>
             {

                 if (response == "Mzu")
                 {

                     if (jumpcount == 0 || jumpcount == 20)
                     {
                         Sound1.Open(new Uri("Resources/jump.wav", UriKind.Relative));
                         Sound1.Play();
                         jumpcount = 20;
                         timerjump = new DispatcherTimer();
                         timerjump.Interval = TimeSpan.FromMilliseconds(70);
                         timerjump.Tick += timerjump_Tick;
                         timerjump.Start();
                     }
                 }

                 else if (response == "Mzl")
                 {

                     Canvas.SetLeft(subzero, (Canvas.GetLeft(subzero)) - 10);

                     if (timericeball.IsEnabled == false)
                     {
                         Canvas.SetLeft((UIElement)iceball, Canvas.GetLeft((UIElement)iceball) - 10);

                     }
                 }


                 else if (response == "Mzr")
                 {

                     Canvas.SetLeft((UIElement)subzero, (Canvas.GetLeft((UIElement)subzero)) + 10);

                     if (timericeball.IsEnabled == false)
                     {
                         Canvas.SetLeft((UIElement)iceball, (Canvas.GetLeft((UIElement)iceball)) + 10);
                     }

                 }



                 else if (response == "Mzb")
                 {

                     if (timericeball.IsEnabled == false)
                     {
                         Sound1.Open(new Uri("Resources/shootingsubball.wav", UriKind.Relative));
                         Sound1.Play();
                         iceball.Visibility = System.Windows.Visibility.Visible;
                         timericeball = new DispatcherTimer();
                         timericeball.Interval = TimeSpan.FromMilliseconds(70);
                         timericeball.Tick += timericeball_Tick;
                         timericeball.Start();
                     }
                 }

                 if (response == "Str")
                 {
                   
                     Sound1.Open(new Uri("Resources/button.wav", UriKind.Relative));
                     Sound1.Play();
                     l_countdown.Content = countdown;

                     timercountdown.Interval = TimeSpan.FromSeconds(1);
                     timercountdown.Tick += timercountdown_Tick;
                     timercountdown.Start();
                     MessageBox.Show("Game Starting in 3 seconds");
                 }

             }));
                            }

                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message + "(Client sided)");
                                ifServerConnected = 0;
                            }
                        }


                    }                
            }
        }


        public bool CollisionCheckScorpIce()
        {
            var x1 = Canvas.GetLeft(iceball);
            var y1 = Canvas.GetTop(iceball);
            Rect r1 = new Rect(x1, y1, iceball.ActualWidth, iceball.ActualHeight);


            var x2 = Canvas.GetLeft(scorpion);
            var y2 = Canvas.GetTop(scorpion);
            Rect r2 = new Rect(x2, y2, scorpion.ActualWidth, scorpion.ActualHeight);

            if (r1.IntersectsWith(r2))
                return true;

            
            return false;
        }

        public bool CollisionCheckSubFire()
        {
            
            var x1 = Canvas.GetLeft(fireball);
            var y1 = Canvas.GetTop(fireball);
            Rect r1 = new Rect(x1, y1, fireball.ActualWidth, fireball.ActualHeight);


            var x2 = Canvas.GetLeft(subzero);
            var y2 = Canvas.GetTop(subzero);
            Rect r2 = new Rect(x2, y2, subzero.ActualWidth, subzero.ActualHeight);

            if (r1.IntersectsWith(r2))
                return true;


            return false;

        }


        // Start game and countdown

        private void startbtn_Click(object sender, RoutedEventArgs e)
        {
            
            Sound1.Open(new Uri("Resources/button.wav", UriKind.Relative));
            Sound1.Play(); 
            startbtn.Visibility = System.Windows.Visibility.Hidden;
            l_countdown.Content=countdown;

            timercountdown.Interval = TimeSpan.FromSeconds(1);
            timercountdown.Tick += timercountdown_Tick;
            timercountdown.Start();

        }

        void timerbackgroundmusic_Tick(object sender, EventArgs e)
        {
            SoundBackground.Open(new Uri("Resources/background.wav", UriKind.Relative));
            SoundBackground.Play(); 
        }

        

        void timercountdown_Tick(object sender, EventArgs e)
        {
            countdown--;
            Sound1.Open(new Uri("Resources/button.wav", UriKind.Relative));
            Sound1.Play();

            l_countdown.Content = countdown;

            if (countdown == 0)
            {
                
                l_countdown.Visibility = System.Windows.Visibility.Hidden;
                timercountdown.Stop();                      
                
                if (ifServerConnected==0)
                {

                    ScorpionAI();

                }

            }

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
            {
                if (countdown == 0)
                {
                    switch (e.Key)
                    {
                        case Key.Left:
                            if (ifServerConnected == 0)
                            {
                                Canvas.SetLeft((UIElement)subzero, (Canvas.GetLeft((UIElement)subzero)) - 10);

                                if (timericeball.IsEnabled == false)
                                {
                                    Canvas.SetLeft((UIElement)iceball, (Canvas.GetLeft((UIElement)iceball)) - 10);

                                }
                            }
                                if (ifServerConnected == 1)
                                {
                                    toBytes = Encoding.ASCII.GetBytes("Mzl");
                                    stream = clientSocket.GetStream();
                                    stream.Write(toBytes, 0, 3);
                                }
                            break;

                        case Key.Right:
                            if (ifServerConnected == 0)
                            {
                                Canvas.SetLeft((UIElement)subzero, (Canvas.GetLeft((UIElement)subzero)) + 10);

                                if (timericeball.IsEnabled == false)
                                {
                                    Canvas.SetLeft((UIElement)iceball, (Canvas.GetLeft((UIElement)iceball)) + 10);
                                }
                            }
                            if (ifServerConnected == 1)
                            {
                                toBytes = Encoding.ASCII.GetBytes("Mzr");
                                stream = clientSocket.GetStream();
                                stream.Write(toBytes, 0, 3);
                            }
                            break;

                        case Key.Up:
                            if (ifServerConnected == 0)
                            {
                                if (jumpcount == 0 || jumpcount == 20)
                                {
                                    Sound1.Open(new Uri("Resources/jump.wav", UriKind.Relative));
                                    Sound1.Play();
                                    jumpcount = 20;
                                    timerjump = new DispatcherTimer();
                                    timerjump.Interval = TimeSpan.FromMilliseconds(70);
                                    timerjump.Tick += timerjump_Tick;
                                    timerjump.Start();
                                }
                            }
                            if (ifServerConnected == 1)
                            {
                                toBytes = Encoding.ASCII.GetBytes("Mzu");
                                stream = clientSocket.GetStream();
                                stream.Write(toBytes, 0, 3);
                            }
                            break;
                        case Key.Space:
                            if (ifServerConnected == 0)
                            {
                                if (timericeball.IsEnabled == false)
                                {
                                    Sound1.Open(new Uri("Resources/shootingsubball.wav", UriKind.Relative));
                                    Sound1.Play();
                                    iceball.Visibility = System.Windows.Visibility.Visible;
                                    timericeball = new DispatcherTimer();
                                    timericeball.Interval = TimeSpan.FromMilliseconds(70);
                                    timericeball.Tick += timericeball_Tick;
                                    timericeball.Start();
                                }
                            }
                            if (ifServerConnected == 1)
                            {
                                toBytes = Encoding.ASCII.GetBytes("Mzb");
                                stream = clientSocket.GetStream();
                                stream.Write(toBytes, 0, 3);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        //
        void timericeball_Tick(object sender, EventArgs e)
        {
            double subbackdown = Canvas.GetTop((UIElement)subzero);
            goneIceball++;
            Canvas.SetLeft((UIElement)iceball, (Canvas.GetLeft((UIElement)iceball)) + 10);

            if (CollisionCheckScorpIce()==true && iceball.Visibility == System.Windows.Visibility.Visible)
            {
                hpscorp -= 5;
                lblhpscorp.Content = "HP: " + hpscorp.ToString();
                iceball.Visibility = System.Windows.Visibility.Hidden;               
                Canvas.SetLeft((UIElement)iceball, (Canvas.GetLeft((UIElement)subzero)) + 91);
                Canvas.SetTop((UIElement)iceball, (Canvas.GetTop((UIElement)subzero)) + 73);
                goneIceball = 0;

                if (hpscorp==0)
                {

                    MessageBox.Show("Subzero Wins!");
                    this.Close();

                }

                timericeball.Stop();
            }
            if (goneIceball>90 && subbackdown==391)
            {
               
                iceball.Visibility = System.Windows.Visibility.Hidden;
                Canvas.SetLeft((UIElement)iceball, (Canvas.GetLeft((UIElement)subzero)) + 91);
                Canvas.SetTop((UIElement)iceball, (Canvas.GetTop((UIElement)subzero)) + 73);
                goneIceball = 0;

                if (hpsub==0)
                {

                    MessageBox.Show("Scorpion Wins!");
                    this.Close();

                }

                timericeball.Stop();

            }
            
        }
            //

            void timerjump_Tick(object sender, EventArgs e)
        {

            jumpcount--;
            if (jumpcount >= 10)
            {
                Canvas.SetTop((UIElement)subzero, (Canvas.GetTop((UIElement)subzero)) - 25);
                if (timericeball.IsEnabled == false)
                {
                    Canvas.SetTop((UIElement)iceball, (Canvas.GetTop((UIElement)iceball)) - 25);
                }
            }
            if (jumpcount < 10 && jumpcount>=0)
            {
                Canvas.SetTop((UIElement)subzero, (Canvas.GetTop((UIElement)subzero)) + 25);
                if (timericeball.IsEnabled == false)
                {
                    Canvas.SetTop((UIElement)iceball, (Canvas.GetTop((UIElement)iceball)) + 25);
                }
            }
            if (jumpcount == 0)
            {
                timerjump.Stop();
            }
        }

        void ScorpionAI()
        {           
        
            timerfireball = new DispatcherTimer();
            timerfireball.Interval = TimeSpan.FromMilliseconds(70);
            timerfireball.Tick += timerfireball_Tick;
            timerfireball.Start();
            timerleftScorpionAI.Interval = TimeSpan.FromMilliseconds(900);
            timerleftScorpionAI.Tick += timerleftScorpionAI_Tick;
            timerleftScorpionAI.Start();          
            jumpscorp5sec.Interval = TimeSpan.FromSeconds(randomAI + 3);
            jumpscorp5sec.Tick += jumpscorp5sec_Tick;
            jumpscorp5sec.Start();

        }

   


        

        void timerleftScorpionAI_Tick(object sender, EventArgs e)
        {
            // scorpShoot++;
            timerfireball.Start();
                 
            double left = Canvas.GetLeft((UIElement)scorpion);

            if (moveScorpAI % 2 == 1 && leftAIscorpion%21!=0)
            {
                leftAIscorpion++;
                left -= 15;
            }

            if (leftAIscorpion % 21 == 0)
                moveScorpAI = 2;


            if (moveScorpAI % 2 == 0 && leftAIscorpion >= 21)
            {
                leftAIscorpion++;
                left += 15;
                if (leftAIscorpion % 41 == 0)
                {
                    moveScorpAI = 1;
                    leftAIscorpion = 1;
                }
            }
                Canvas.SetLeft((UIElement)scorpion, left);
            

           // if (scorpShoot % randomAI+5 == 1 )
               // timerfireball.Start();

        }


        void jumpscorp5sec_Tick(object sender, EventArgs e)
        {
            if (jumpcountscorp == 0 || jumpcountscorp == 20)
            {
                Sound1.Open(new Uri("Resources/jump.wav", UriKind.Relative));
                Sound1.Play();
                jumpcountscorp = 20;
                timerjumpscorp = new DispatcherTimer();
                timerjumpscorp.Interval = TimeSpan.FromMilliseconds(70);
                timerjumpscorp.Tick += timerjumpscorp_Tick;
                timerjumpscorp.Start();
            }
        }


        void timerjumpscorp_Tick(object sender, EventArgs e)
        {
            jumpcountscorp--;
            if (jumpcountscorp >= 10)
            {
                Canvas.SetTop((UIElement)scorpion, (Canvas.GetTop((UIElement)scorpion)) - 25);
                if (timerfireball.IsEnabled == false)
                {
                    Canvas.SetTop((UIElement)fireball, (Canvas.GetTop((UIElement)fireball)) - 25);
                }
            }
            if (jumpcountscorp < 10 && jumpcountscorp >= 0)
            {
                Canvas.SetTop((UIElement)scorpion, (Canvas.GetTop((UIElement)scorpion)) + 25);
                if (timerfireball.IsEnabled == false)
                {
                    Canvas.SetTop((UIElement)fireball, (Canvas.GetTop((UIElement)fireball)) + 25);

                }
            }
            if (jumpcountscorp == 0)
            {
                timerjumpscorp.Stop();
            }
        }
        



        void timerleftscorpion_Tick(object sender, EventArgs e)
        {
            scorpShoot++;

            Canvas.SetLeft((UIElement)scorpion, (Canvas.GetLeft((UIElement)scorpion)) - 15);
            if (scorpShoot%5==0)
                timerfireball.Start();
        }

        void timerfireball_Tick(object sender, EventArgs e)
        {
            double scorpbackdown = Canvas.GetTop((UIElement)scorpion);
            goneFireball++;

            if (goneFireball == 1)
            {
                Sound1.Open(new Uri("Resources/shootingsubball.wav", UriKind.Relative));
                Sound1.Play();
            }

            Canvas.SetLeft((UIElement)fireball, (Canvas.GetLeft((UIElement)fireball)) - 10);

            fireball.Visibility = System.Windows.Visibility.Visible;

            if (CollisionCheckSubFire() == true && fireball.Visibility == System.Windows.Visibility.Visible)
            {
                hpsub -= 5;
                lblhpsub.Content = "HP: " + hpsub.ToString();
                fireball.Visibility = System.Windows.Visibility.Hidden;
                Canvas.SetLeft((UIElement)fireball, (Canvas.GetLeft((UIElement)scorpion)) - 91);
                goneFireball = 0;
                timerfireball.Stop();
            }

            if (goneFireball > 100 && scorpbackdown == 391)
            {

                fireball.Visibility = System.Windows.Visibility.Hidden;
                Canvas.SetLeft((UIElement)fireball, (Canvas.GetLeft((UIElement)scorpion)) - 91);
                Canvas.SetTop((UIElement)fireball, (Canvas.GetTop((UIElement)scorpion)) + 73);
                goneFireball = 0;
                timerfireball.Stop();

            }

        }

    }
}
