using System;
using System.Collections.Generic;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Collections;

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
    public partial class MainWindow : Window
    {

        TcpListener serverSocket;
        TcpClient clientSocket;
        Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
        ArrayList ClientList = new ArrayList();
        Match match1 = new Match();
        Match match2 = new Match();
        Match[] matches = new Match[100];
        NetworkStream stream;
        int i = 0;
        //IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }


        private void Connect()
        {

            serverSocket = new TcpListener(IPAddress.Any, 8000);
            clientSocket = default(TcpClient);
            try
            {
                serverSocket.Start();
                MessageBox.Show("Server started");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }         
            //stream = clientSocket.GetStream();

            while (true)
            {

                //i++;

                //if (i <= 2)
                //{
                //    int id = GetFreeId();
                //    clients.Add(id, serverSocket.AcceptTcpClient());
                //    match1.AddPlayer(clients[id]);
                //}
                //if (i > 2)
                //{
                //    int id = GetFreeId();
                //    clients.Add(id, serverSocket.AcceptTcpClient());
                //    match2.AddPlayer(clients[id]);
                //}
                
                clients.Add(i, serverSocket.AcceptTcpClient());
                if (matches[i / 2] == null)
                    matches[i / 2] = new Match();
                matches[i / 2].AddPlayer(clients[i]);
                
                i++;

            }

            //MessageBox.Show("Client connected!");
            //byte[] response = new byte[clientSocket.ReceiveBufferSize];
            //NetworkStream networkStream = clientSocket.GetStream();
            //Console.WriteLine(networkStream.Read(response, 0, clientSocket.ReceiveBufferSize));

            }

        //private int GetFreeId()
        //{

        //    for (int i = 0; i < int.MaxValue; i++)
        //    {

        //        if (!clients.ContainsKey(i))
        //            return i;

        //    }

        //    return -1;

        //}

    }
}

  
