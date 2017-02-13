using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace CommsLib
{
    public class TCPClient : ClientBase
    {
        //Net.Sockets client class
        TcpClient theClient;
        //Thread that runs the asynchronous client communications
        Thread commThread;

        List<byte> Packet;
        bool isRunning = false;

        public override int Port
        {
            get
            {
                return base.Port;
            }
            protected set
            {
                base.Port = value;
            }
        }

        /// <summary>
        /// Public constructor (not really needed)
        /// </summary>
        public TCPClient()
        {
            Packet = new List<byte>();
            //Instantiates the TCP client variable
            theClient = new TcpClient();
        }

        /// <summary>
        /// Connection function
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="port"></param>
        public override void ConnectToServer(string serverIP, int port)
        {
            theClient = new TcpClient();

            if (theClient.Connected)
            {
                throw new Exception("The client is already connected! Please disconnect first.");
            }

           // theClient.ReceiveTimeout = 10;
           // theClient.SendTimeout = 50;
            
            //Connects the client to the specified IP address and Port
            theClient.Connect(serverIP, port);

            if (!theClient.Connected)
            {
                throw new Exception("Could not establish connection");
            }

            //Instantiate communication thread
            commThread = new Thread(HandleClientComm);
            //Start thread
            commThread.Start();

            isRunning = true;
        }

        /// <summary>
        /// Send data to the comm device
        /// </summary>
        /// <param name="address">The address of the device eg. 000001</param>
        /// <param name="command">The command id of the message</param>
        /// <param name="data">The message payload</param>
        /// <returns>The result of the transmission</returns>
        public override Boolean SendData(CommandID command, Array data)
        {
            //Attempt to send data to the sepecified server
            try
            {
                //Create a new and local network stream for communication
                NetworkStream serverStream = theClient.GetStream();

                //make a new array based on the amount of data to be sent + necessary command bytes
                byte[] message = new byte[data.Length + 3];

                //Set value of necessary bytes
                //0xff packet header
                message.SetValue((byte)255, 0);
                //Number of remaining bytes in packet
                message.SetValue((byte)(data.Length + 1), 1);
                //command byte
                message.SetValue((byte)command, 2);

                //Copy all data into message array previously defined
                for (int idx = 0; idx < data.Length; idx++)
                {
                    message.SetValue(data.GetValue(idx), idx + 3);
                }

                //Write array to server stream
                serverStream.Write(message, 0, message.Length);
                serverStream.Flush();

                //return A-OK
                return true;
            }
            catch
            {
                //If there were any errors return false
                return false;
            }
        }

        /// <summary>
        /// Send data to the comm device
        /// </summary>
        /// <param name="address">The address of the device eg. 000001</param>
        /// <param name="command">The command id of the message</param>
        /// <returns>The result of the transmission</returns>
        public override Boolean SendData(CommandID Command)
        {
            //Send blank array with defined command byte
            return SendData(Command, new byte[] { });
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleClientComm()
        {
            //Create a new and local network stream for communication
            NetworkStream serverStream = theClient.GetStream();
            
            isRunning = true;

            //Create large array to acommodate any data received from the server
            byte[] message = new byte[10025];
            //Local variable to identify how many bytes were read from the server
            int bytesRead;

            //Infinite loop, should not impact the running on the program as it is running in a separate thread
            while (isRunning)
            {
                    //Reset number of bytes read from server stream
                    bytesRead = 0;
			
		if (serverStream.DataAvailable)
		{
                    //Attempt to read data from the server
                    try
                    {
                        //blocks until a client sends a message
                        bytesRead = serverStream.Read(message, 0, 1000);

                        //byte[] rcv = new byte[bytesRead];

                        //Copy received data into apropriately sized buffer
                        for (int cnt = 0; cnt < bytesRead; cnt++)
                        {
                            //rcv[cnt] = message[cnt];
                            Packet.Add(message[cnt]);
                        }

                        //Fire off message received event
                        ProcessPacket();
                    }
                    catch
                    {
                        //a socket error has occured
                        //break;
                    }

                    if (bytesRead == 0)
                    {
                        //the client has disconnected from the server
                        //break;
                    }
		}
            }

        }

        private void ProcessPacket()
        {
            byte _data;
            Queue<byte> _buffer = new Queue<byte>();
            bool _waitingForExpectedBytes = false;
            int _expectedBytes = -2;

            for (int cnt = 0; cnt < Packet.Count; cnt++)
            {
                _data = Packet[cnt];
                //start packet framing, leading byte should be 0xff
                if ((_data == 255) && (!_waitingForExpectedBytes) && (_expectedBytes < 0))
                {
                    //If received leading byte then next should be the number of bytes
                    _waitingForExpectedBytes = true;
                    //Make sure buffer is clear on receiving new packet
                    _buffer.Clear();
                    _buffer.Enqueue(_data);
                }
                else
                    //Second byte present triggered by previous if
                    if (_waitingForExpectedBytes)
                    {
                        //The payload size is equal to the current byte
                        _expectedBytes = _data;
                        //We are no longer waiting for the payload size
                        _waitingForExpectedBytes = false;
                        _buffer.Enqueue(_data);
                    }
                    else
                    {
                        //Now that all initial conditions have been satisfied
                        //fill buffer until expected payload size is reached
                        _buffer.Enqueue(_data);
                    }

                //When received payload == the expected size end packet and fire received event
                if (_buffer.Count == _expectedBytes + 2)
                {
                    //Reset expected bytes state for next packet
                    _expectedBytes = -2;

                    //Fire message recieved event
                    _ClientMessageReceivedEvent(_buffer.ToArray());                    
                    Packet.RemoveRange(0, _buffer.Count);
                    cnt = -1;
                    _buffer.Clear();
                }
                else
                    if ((_buffer.Count > _expectedBytes + 2) && (_expectedBytes > 0))
                    {
                        _buffer.Clear();
                        _waitingForExpectedBytes = true;
                        _expectedBytes = -2;
                        Packet.Clear();
                    }
            }
        }


        public bool isConnected
        {
            get { return theClient.Connected; }
        }
        /// <summary>
        /// Disconnection routine
        /// </summary>
        public override void Disconnect()
        {
            isRunning = false;
            theClient.ReceiveTimeout = 20;

            //commThread.Join(); //wait until thread quits.

            //Now safe to close down socket
            theClient.Close();

            
        }
    }
}
