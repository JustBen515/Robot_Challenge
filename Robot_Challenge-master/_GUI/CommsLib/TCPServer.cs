using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace CommsLib
{
    public class TCPServer : ServerBase
    {
        private TcpListener tcpListener;
        private Thread listenThread;

        private TcpClient TheClient;

        bool Listening = true;

        /// <summary>
        /// Overriden
        /// </summary>
        /// <param name="DestinationPort"></param>
        /// <param name="DestinationIP"></param>
        public TCPServer(int DestinationPort, string DestinationIP) : base()
        {
            this.tcpListener = new TcpListener(IPAddress.Parse(DestinationIP), DestinationPort);
        }

        /// <summary>
        /// Overridden start server function
        /// Instantiates new listen thread and with ListenForClients function, if everything starts ok
        /// return true, otherwise return false
        /// </summary>
        /// <returns></returns>
        public override bool StartServer()
        {
            try
            {
                this.listenThread = new Thread(new ThreadStart(ListenForClients));
                this.listenThread.Start();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (Listening)
            {
                //blocks until a client has connected to the server
                TheClient = this.tcpListener.AcceptTcpClient();

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(TheClient);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client"></param>
        private void HandleClientComm(object Client)
        {
            TheClient = (Client as TcpClient);
            NetworkStream clientStream = TheClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);

                    _MessageReceivedEvent(message);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
            }
            TheClient.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
        public override void SendPacket(byte[] Buffer)
        {
            if ((TheClient != null)&&(TheClient.Connected))
            {
                NetworkStream clientStream = TheClient.GetStream();

                clientStream.Write(Buffer, 0, Buffer.Length);
                clientStream.Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool ClientStatus()
        {
            if (TheClient != null)
                return TheClient.Connected;
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool ServerStatus()
        {
            return Listening;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.listenThread.Abort();

            this.tcpListener.Stop();
            Listening = false;
        }
    }
}