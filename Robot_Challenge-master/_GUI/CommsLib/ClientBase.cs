using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Sockets;

namespace CommsLib
{
    //Arguments class for message received event
    public class Client_Message_EventArgs : EventArgs
    {
        public readonly byte[] RawMessage;

        public Client_Message_EventArgs(byte[] RawMessage)
        {
            this.RawMessage = RawMessage;
        }
    }

    public abstract class ClientBase
    {
        /// <summary>
        /// Event virtual function for firing
        /// </summary>
        protected virtual void _ClientMessageReceivedEvent(byte[] MessageData)
        {
            if (OnMessageReceived != null) OnMessageReceived(new Client_Message_EventArgs(MessageData));
        }

        public delegate void ClientMessageReceivedEvent(Client_Message_EventArgs e);
        public event ClientMessageReceivedEvent OnMessageReceived;


        public virtual int Port
        {
            get;
            protected set;
        }

        public string IP
        {
            get;
            protected set;
        }

        public abstract void ConnectToServer(string serverIP, int port);

        public abstract Boolean SendData(CommandID command, Array data);
        public abstract Boolean SendData(CommandID Command);

        public abstract void Disconnect();
    }
}
