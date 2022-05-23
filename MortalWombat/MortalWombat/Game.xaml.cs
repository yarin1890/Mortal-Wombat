using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Media.Imaging;

namespace MortalWombat
{
    /// <summary>
    /// Server response-action dictionary:
    /// 
    /// Mzl = Move Subzero Left 
    /// Mzr = Move Subzero Right
    /// Mzu = Jump Subzero
    /// Mzb = Shoot Subzero's ball
    /// Msl = Move Scorpion Left
    /// Msr = Move Scorpion Right
    /// Msu = Jump Scorpion
    /// Msb = Shoot Scorpion's ball
    /// St1 = Start the game for player 1
    /// St2 = Start the game for player 2
    /// 
    /// </summary>
    public partial class Game : Window
    {

        // Variables
        ImageBrush imageBrush = new ImageBrush();
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
        DispatcherTimer timerfireball = new DispatcherTimer();
        DispatcherTimer jumpscorp5sec = new DispatcherTimer();
        Random rnd = new Random();
        Thread checkmessages;
        byte[] toBytes;
        public int moveScorpAI = 1;
        public int leftAIscorpion = 1, rightAIscorpion = 0;
        public int countdown = 3;
        public int jumpcount = 20, jumpcountscorp = 20;
        public int hpsub = 100, hpscorp = 100;
        public int goneIceball = 0, goneFireball = 0;
        public int onlineOrAI = 0;
        public int scorpShoot = 0;
        public int randomAI = 0;
        public int player = 0;
        public TcpClient clientSocket = new TcpClient();
        NetworkStream stream;
        int randomBackground = 0;
        // To check if there's server connection, if not the game will be AI based.

        public int ifServerConnected = 1;

        public Game()
        {

            // Random number to make the AI a bit randomized
            randomAI = rnd.Next(1, 11);
            randomBackground = rnd.Next(1, 4);
            InitializeComponent();

            // Try to connect to the server or catch the exception to make the game AI based
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

            // Setting colors, background music and making stuff hidden / shown

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

            //stream = clientSocket.GetStream();
            //bytesRead = stream.Read(buffer, 0, buffer.Length);
            //response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            //if (response == "Bg1")
            //    movingbackground.Source = new BitmapImage(new Uri("Images/background.png", UriKind.Relative));
            //if (response == "Bg2")
            //    movingbackground.Source = new BitmapImage(new Uri("Images/background2.png", UriKind.Relative));
            //if (response == "Bg3")
            //    movingbackground.Source = new BitmapImage(new Uri("Images/background3.png", UriKind.Relative));

            movingbackground.Source = new BitmapImage(new Uri("Images/background" + randomBackground.ToString() + ".png", UriKind.Relative));

        }

        // Check the messages from the server in a while true loop to make actions according to the messages.
        private void CheckMessages()
        {

            while (true)
            {
                stream = clientSocket.GetStream();

                if (ifServerConnected == 1)
                {

                    if (countdown == 0 || countdown == 3)
                    {

                        try
                        {
                            // Try to receive the message from the server

                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                            Application.Current.Dispatcher.BeginInvoke(
         DispatcherPriority.Background,
         new Action(() =>
         {

             // Check the messages and do actions according to them.

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

                 if (Canvas.GetLeft(subzero) != 6)
                     Canvas.SetLeft(subzero, (Canvas.GetLeft(subzero)) - 10);

                 if (Canvas.GetLeft(movingbackground) != -10 && (Canvas.GetLeft(subzero) == 6))
                     Canvas.SetLeft(movingbackground, (Canvas.GetLeft(movingbackground)) + 10);

                 if (timericeball.IsEnabled == false)
                 {

                     Canvas.SetLeft(iceball, Canvas.GetLeft(iceball) - 10);

                 }

             }

             else if (response == "Mzr")
             {

                 if (Canvas.GetLeft(subzero) != 726)
                     Canvas.SetLeft(subzero, (Canvas.GetLeft(subzero)) + 10);

                 if (Canvas.GetLeft(movingbackground) != -760 && (Canvas.GetLeft(subzero) == 726))
                     Canvas.SetLeft(movingbackground, (Canvas.GetLeft(movingbackground)) - 10);

                 if (timericeball.IsEnabled == false)
                 {
                     Canvas.SetLeft(iceball, (Canvas.GetLeft(iceball)) + 10);
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
             else if (response == "Msu")
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

             else if (response == "Msl")
             {
                 if (Canvas.GetLeft(scorpion) != 5)
                     Canvas.SetLeft(scorpion, (Canvas.GetLeft(scorpion)) - 10);

                 if (Canvas.GetLeft(movingbackground) != -10 && (Canvas.GetLeft(scorpion) == 5))
                     Canvas.SetLeft(movingbackground, (Canvas.GetLeft(movingbackground)) + 10);

                 if (timerfireball.IsEnabled == false)
                 {

                     Canvas.SetLeft(fireball, Canvas.GetLeft(fireball) - 10);

                 }

             }

             else if (response == "Msr")
             {
                 if (Canvas.GetLeft(scorpion) != 725)
                     Canvas.SetLeft(scorpion, (Canvas.GetLeft(scorpion)) + 10);

                 if (Canvas.GetLeft(movingbackground) != -760 && (Canvas.GetLeft(scorpion) == 725))
                     Canvas.SetLeft(movingbackground, (Canvas.GetLeft(movingbackground)) - 10);

                 if (timerfireball.IsEnabled == false)
                 {
                     Canvas.SetLeft(fireball, (Canvas.GetLeft(fireball)) + 10);
                 }


             }

             else if (response == "Msb")
             {

                 if (timerfireball.IsEnabled == false)
                 {
                     Sound1.Open(new Uri("Resources/shootingsubball.wav", UriKind.Relative));
                     Sound1.Play();
                     fireball.Visibility = System.Windows.Visibility.Visible;
                     timerfireball = new DispatcherTimer();
                     timerfireball.Interval = TimeSpan.FromMilliseconds(70);
                     timerfireball.Tick += timerfireball_Tick;
                     timerfireball.Start();
                 }

             }

             else if (response == "St1")
             {

                 Sound1.Open(new Uri("Resources/button.wav", UriKind.Relative));
                 Sound1.Play();
                 l_countdown.Content = countdown;

                 timercountdown.Interval = TimeSpan.FromSeconds(1);
                 timercountdown.Tick += timercountdown_Tick;
                 timercountdown.Start();
                 MessageBox.Show("Game Starting in 3 seconds");
             }

             else if (response == "St2")
             {
                 player = 1;
                 Sound1.Open(new Uri("Resources/button.wav", UriKind.Relative));
                 Sound1.Play();
                 l_countdown.Content = countdown;

                 timercountdown.Interval = TimeSpan.FromSeconds(1);
                 timercountdown.Tick += timercountdown_Tick;
                 timercountdown.Start();
                 MessageBox.Show("Game Starting in 3 seconds");
             }

             else if (response == "Bg1")
                 movingbackground.Source = new BitmapImage(new Uri("Images/background.png", UriKind.Relative));
             else if (response == "Bg2")
                 movingbackground.Source = new BitmapImage(new Uri("Images/background2.png", UriKind.Relative));
             else if (response == "Bg3")
                 movingbackground.Source = new BitmapImage(new Uri("Images/background3.png", UriKind.Relative));

         }));
                        }

                        catch (Exception ex)
                        {

                            // If something's wrong with the server or receiving messages from it - to make the AI work and show a message

                            MessageBox.Show(ex.Message + "(Client sided)");
                            ifServerConnected = 0;
                        }
                    }


                }
            }
        }

        // Check collision between the iceball and scorpion to reduce 5 hp off scorpion after the collision
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

        // Check collision between the fireball and subzero to reduce 5 hp off subzero after the collision
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

        // Start game and countdown if it's with the AI
        private void startbtn_Click(object sender, RoutedEventArgs e)
        {

            Sound1.Open(new Uri("Resources/button.wav", UriKind.Relative));
            Sound1.Play();
            startbtn.Visibility = System.Windows.Visibility.Hidden;
            l_countdown.Content = countdown;

            timercountdown.Interval = TimeSpan.FromSeconds(1);
            timercountdown.Tick += timercountdown_Tick;
            timercountdown.Start();

        }

        // Loop the background music since it ends after about 47 seconds.
        void timerbackgroundmusic_Tick(object sender, EventArgs e)
        {
            SoundBackground.Open(new Uri("Resources/background.wav", UriKind.Relative));
            SoundBackground.Play();
        }


        // Countdown from 3 to 0 and only then start the game.
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

                if (ifServerConnected == 0)
                {

                    ScorpionAI();

                }

            }

        }

        // Check key presses and do actions according to them.

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {

                {
                    if (countdown == 0)
                    {
                        switch (e.Key)
                        {

                            case Key.Left:
                                if (Canvas.GetLeft(subzero) != 6)
                                {
                                    if (ifServerConnected == 0)
                                    {
                                        Canvas.SetLeft(subzero, (Canvas.GetLeft(subzero)) - 10);

                                        if (timericeball.IsEnabled == false)
                                        {
                                            Canvas.SetLeft(iceball, (Canvas.GetLeft(iceball)) - 10);

                                        }
                                    }
                                    if (ifServerConnected == 1)
                                    {
                                        if (player == 0)
                                        {
                                            toBytes = Encoding.ASCII.GetBytes("Mzl");
                                            stream = clientSocket.GetStream();
                                            stream.Write(toBytes, 0, 3);
                                        }
                                    }
                                }
                                else
                                    if (ifServerConnected == 0 && Canvas.GetLeft(movingbackground) != -10)
                                    Canvas.SetLeft(movingbackground, (Canvas.GetLeft(movingbackground)) + 10);
                                break;

                            case Key.Right:

                                if (Canvas.GetLeft(subzero) != 726)
                                {
                                    if (ifServerConnected == 0)
                                    {
                                        Canvas.SetLeft(subzero, (Canvas.GetLeft(subzero)) + 10);

                                        if (timericeball.IsEnabled == false)
                                        {
                                            Canvas.SetLeft(iceball, (Canvas.GetLeft(iceball)) + 10);
                                        }
                                    }
                                    if (ifServerConnected == 1)
                                    {
                                        if (player == 0)
                                        {
                                            toBytes = Encoding.ASCII.GetBytes("Mzr");
                                            stream = clientSocket.GetStream();
                                            stream.Write(toBytes, 0, 3);
                                        }
                                    }
                                }
                                else
                                    if (ifServerConnected == 0 && Canvas.GetLeft(movingbackground) != -760)
                                    Canvas.SetLeft(movingbackground, (Canvas.GetLeft(movingbackground)) - 10);
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
                                    if (player == 0)
                                    {
                                        toBytes = Encoding.ASCII.GetBytes("Mzu");
                                        stream = clientSocket.GetStream();
                                        stream.Write(toBytes, 0, 3);
                                    }
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
                                    if (player == 0)
                                    {
                                        toBytes = Encoding.ASCII.GetBytes("Mzb");
                                        stream = clientSocket.GetStream();
                                        stream.Write(toBytes, 0, 3);
                                    }
                                }
                                break;

                            case Key.A:
                                //if (ifServerConnected == 0)
                                //{
                                //    Canvas.SetLeft(scorpion, (Canvas.GetLeft(scorpion)) - 10);

                                //    if (timerfireball.IsEnabled == false)
                                //    {
                                //        Canvas.SetLeft(fireball, (Canvas.GetLeft(fireball)) - 10);

                                //    }
                                //}

                                if (Canvas.GetLeft(scorpion) != 5)
                                {
                                    if (ifServerConnected == 1)
                                    {
                                        if (player == 1)
                                        {
                                            toBytes = Encoding.ASCII.GetBytes("Msl");
                                            stream = clientSocket.GetStream();
                                            stream.Write(toBytes, 0, 3);
                                        }
                                    }
                                }
                                else
                                    if (ifServerConnected == 0 && Canvas.GetLeft(movingbackground) != -10)
                                    Canvas.SetLeft(movingbackground, (Canvas.GetLeft(movingbackground)) + 10);
                                break;

                            case Key.D:
                                if (Canvas.GetLeft(scorpion) != 725)
                                {
                                    //if (ifServerConnected == 0)
                                    //{
                                    //    Canvas.SetLeft(scorpion, (Canvas.GetLeft(scorpion)) + 10);

                                    //    if (timerfireball.IsEnabled == false)
                                    //    {
                                    //        Canvas.SetLeft(fireball, (Canvas.GetLeft(fireball)) + 10);
                                    //    }
                                    //}
                                    if (ifServerConnected == 1)
                                    {
                                        if (player == 1)
                                        {
                                            toBytes = Encoding.ASCII.GetBytes("Msr");
                                            stream = clientSocket.GetStream();
                                            stream.Write(toBytes, 0, 3);
                                        }
                                    }
                                }
                                else
                                    if (ifServerConnected == 0 && Canvas.GetLeft(movingbackground) != -760)
                                    Canvas.SetLeft(movingbackground, (Canvas.GetLeft(movingbackground)) - 10);
                                break;

                            case Key.W:
                                //if (ifServerConnected == 0)
                                //{
                                //    if (jumpcountscorp == 0 || jumpcountscorp == 20)
                                //    {
                                //        Sound1.Open(new Uri("Resources/jump.wav", UriKind.Relative));
                                //        Sound1.Play();
                                //        jumpcountscorp = 20;
                                //        timerjumpscorp = new DispatcherTimer();
                                //        timerjumpscorp.Interval = TimeSpan.FromMilliseconds(70);
                                //        timerjumpscorp.Tick += timerjumpscorp_Tick;
                                //        timerjumpscorp.Start();
                                //    }
                                //}
                                if (ifServerConnected == 1)
                                {
                                    if (player == 1)
                                    {
                                        toBytes = Encoding.ASCII.GetBytes("Msu");
                                        stream = clientSocket.GetStream();
                                        stream.Write(toBytes, 0, 3);
                                    }
                                }
                                break;

                            case Key.S:
                                //if (ifServerConnected == 0)
                                //{
                                //    if (timerfireball.IsEnabled == false)
                                //    {
                                //        Sound1.Open(new Uri("Resources/shootingsubball.wav", UriKind.Relative));
                                //        Sound1.Play();
                                //        fireball.Visibility = System.Windows.Visibility.Visible;
                                //        timerfireball = new DispatcherTimer();
                                //        timerfireball.Interval = TimeSpan.FromMilliseconds(70);
                                //        timerfireball.Tick += timerfireball_Tick;
                                //        timerfireball.Start();
                                //    }
                                //}
                                if (ifServerConnected == 1)
                                {
                                    if (player == 1)
                                    {
                                        toBytes = Encoding.ASCII.GetBytes("Msb");
                                        stream = clientSocket.GetStream();
                                        stream.Write(toBytes, 0, 3);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }


            catch (Exception)
            {
                MessageBox.Show("Server not working");
            }
        }
    

        // Shoot the iceball and return him to subzero after it's finished.
        void timericeball_Tick(object sender, EventArgs e)
        {
            double subbackdown = Canvas.GetTop(subzero);
            goneIceball++;
            Canvas.SetLeft(iceball, (Canvas.GetLeft(iceball)) + 10);

            if (CollisionCheckScorpIce() == true && iceball.Visibility == System.Windows.Visibility.Visible)
            {
                hpscorp -= 5;
                lblhpscorp.Content = "HP: " + hpscorp.ToString();
                iceball.Visibility = System.Windows.Visibility.Hidden;
                Canvas.SetLeft(iceball, (Canvas.GetLeft(subzero)) + 91);
                Canvas.SetTop(iceball, (Canvas.GetTop(subzero)) + 73);
                goneIceball = 0;

                if (hpscorp == 0)
                {

                    MessageBox.Show("Subzero Wins!");
                    this.Close();
                    timerleftScorpionAI.Stop();
                    jumpscorp5sec.Stop();
                    timerfireball.Stop();
                }

                timericeball.Stop();
            }
            if (goneIceball > 90 && subbackdown == 391)
            {

                iceball.Visibility = System.Windows.Visibility.Hidden;
                Canvas.SetLeft(iceball, (Canvas.GetLeft(subzero)) + 91);
                Canvas.SetTop(iceball, (Canvas.GetTop(subzero)) + 73);
                goneIceball = 0;

                timericeball.Stop();

            }

        }

        // Scorpion jump
        void timerjump_Tick(object sender, EventArgs e)
        {

            jumpcount--;
            if (jumpcount >= 10)
            {
                Canvas.SetTop(subzero, (Canvas.GetTop(subzero)) - 25);
                if (timericeball.IsEnabled == false)
                {
                    Canvas.SetTop(iceball, (Canvas.GetTop(iceball)) - 25);
                }
            }
            if (jumpcount < 10 && jumpcount >= 0)
            {
                Canvas.SetTop(subzero, (Canvas.GetTop(subzero)) + 25);
                if (timericeball.IsEnabled == false)
                {
                    Canvas.SetTop(iceball, (Canvas.GetTop(iceball)) + 25);
                }
            }
            if (jumpcount == 0)
            {
                timerjump.Stop();
            }
        }

        // Turn on the 3 timers to make the AI work
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

        // Make the scorpion move left and then right after it reaches the middle of the screen
        void timerleftScorpionAI_Tick(object sender, EventArgs e)
        {
            // scorpShoot++;
            timerfireball.Start();

            double left = Canvas.GetLeft(scorpion);

            if (moveScorpAI % 2 == 1 && leftAIscorpion % 21 != 0)
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
            Canvas.SetLeft(scorpion, left);


            // if (scorpShoot % randomAI+5 == 1 )
            // timerfireball.Start();

        }

        // Make scorpion jump every 5 seconds
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
         
        // Scorpion jump
        void timerjumpscorp_Tick(object sender, EventArgs e)
        {
            jumpcountscorp--;
            if (jumpcountscorp >= 10)
            {
                Canvas.SetTop(scorpion, (Canvas.GetTop(scorpion)) - 25);
                if (timerfireball.IsEnabled == false)
                {
                    Canvas.SetTop(fireball, (Canvas.GetTop(fireball)) - 25);
                }
            }
            if (jumpcountscorp < 10 && jumpcountscorp >= 0)
            {
                Canvas.SetTop(scorpion, (Canvas.GetTop(scorpion)) + 25);
                if (timerfireball.IsEnabled == false)
                {
                    Canvas.SetTop(fireball, (Canvas.GetTop(fireball)) + 25);

                }
            }
            if (jumpcountscorp == 0)
            {
                timerjumpscorp.Stop();
            }
        }

        // Shoot the fireball and return it to scorpion after it finishes
        void timerfireball_Tick(object sender, EventArgs e)
        {
            double scorpbackdown = Canvas.GetTop(scorpion);
            goneFireball++;

            if (goneFireball == 1)
            {
                Sound1.Open(new Uri("Resources/shootingsubball.wav", UriKind.Relative));
                Sound1.Play();
            }

            Canvas.SetLeft(fireball, (Canvas.GetLeft(fireball)) - 10);

            fireball.Visibility = System.Windows.Visibility.Visible;

            if (CollisionCheckSubFire() == true && fireball.Visibility == System.Windows.Visibility.Visible)
            {
                hpsub -= 5;
                lblhpsub.Content = "HP: " + hpsub.ToString();
                fireball.Visibility = System.Windows.Visibility.Hidden;
                Canvas.SetLeft(fireball, (Canvas.GetLeft(scorpion)) - 91);
                goneFireball = 0;
                timerfireball.Stop();

                if (hpsub == 0)
                {

                    MessageBox.Show("Scorpion Wins!");
                    this.Close();
                    timerleftScorpionAI.Stop();
                    jumpscorp5sec.Stop();
                    timerfireball.Stop();

                }
            }

            if (goneFireball > 100 && scorpbackdown == 391)
            {

                fireball.Visibility = System.Windows.Visibility.Hidden;
                Canvas.SetLeft(fireball, (Canvas.GetLeft(scorpion)) - 91);
                Canvas.SetTop(fireball, (Canvas.GetTop(scorpion)) + 73);
                goneFireball = 0;
                timerfireball.Stop();

            }

        }

    }
}
