using System;
using System.Text;
using System.Windows;
using System.Net.Sockets;
using System.Threading;

namespace MWServer
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
   /// </summary>
    public class Match
    {
        Thread matchloop,matchloop2;
        string r1 = "", r2 = "";
        byte[] buffer = new byte[1024];
        TcpClient player1 = null;
        NetworkStream clientStream;
        TcpClient player2 = null;
        int randomBackground = 0;
        Random rnd = new Random();
        public void AddPlayer(TcpClient player)
        {

            if (MatchFull() == true)
            {
                MessageBox.Show("Error match full");
            }

            if (player1 == null)
            {
                player1= new TcpClient();
                MessageBox.Show("Adding Player1");
                player1 = player;


            }
            else if (player2 == null)
            {
                player2 = new TcpClient();
                MessageBox.Show("Adding Player2");
                player2 = player;

            }

            if (MatchFull() == true)
            {
                //Start the game
               
                matchloop = new Thread(MatchLoop);
                matchloop.Start();
                matchloop2 = new Thread(MatchLoop2);
                matchloop2.Start();
            }

        }

        private bool MatchFull()
        {

            if (player1 != null && player2 != null)
                return true;
            return false;

        }

        private void MatchLoop()
        {
            //randomBackground = rnd.Next(1, 4);

            //if (randomBackground == 1)
            //{
            //    SendMessage("Bg1", player1);
            //    SendMessage("Bg1", player2);
            //}

            //else if (randomBackground == 2)
            //{
            //    SendMessage("Bg2", player1);
            //    SendMessage("Bg2", player2);
            //}

            //else if (randomBackground == 3)
            //{
            //    SendMessage("Bg3", player1);
            //    SendMessage("Bg3", player2);
            //}

            SendMessage("St1", player1);
            SendMessage("St2", player2);
            Thread.Sleep(10);

            while (true)
            {

                r1 = ReceiveMessage(player1);

                SendMessage(r1, player1);
                SendMessage(r1, player2);

            }
        }
        private void MatchLoop2()
        {

            while (true)
            {

                r2 = ReceiveMessage(player2);

                SendMessage(r2, player1);
                SendMessage(r2, player2);

            }
        }

        private string ReceiveMessage(TcpClient client)
        {

            int bytesRead = 0;

            try
            {
                clientStream = client.GetStream();
                bytesRead = clientStream.Read(buffer, 0, buffer.Length);             
            }
            catch (System.Exception)
            {

                MessageBox.Show("No client found, can't receive a message");
                
            }
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            return response;
        }

        private void SendMessage(string message, TcpClient client)
        {

            try
            {
                byte[] toBytes = Encoding.ASCII.GetBytes(message);
                clientStream = client.GetStream();
                clientStream.Write(toBytes, 0, 3);
            }
            catch (System.Exception)
            {
                MessageBox.Show("No client found, can't send a messsage");
            }        
        }
        
    }
}