using System;
using System.Collections.Generic;
using System.Text;

namespace CommsLib
{
    public abstract class ServerBase
    {
        //Arguments class for message received event
        public class Message_EventArgs : EventArgs
        {
            public readonly byte[] RawMessage;

            public Message_EventArgs(byte[] RawMessage)
            {
                this.RawMessage = RawMessage;
            }
        }

        /// <summary>
        /// Event virtual function for firing
        /// </summary>
        protected virtual void _MessageReceivedEvent(byte[] MessageData)
        {
            if (OnMessageReceived != null) OnMessageReceived(new Message_EventArgs(MessageData));
        }

        public delegate void MessageReceivedEvent(Message_EventArgs e);
        public event MessageReceivedEvent OnMessageReceived;

        /// <summary>
        /// Base class constructor (empty)
        /// </summary>
        public ServerBase()
        {}

        /// <summary>
        /// Start server function
        /// </summary>
        /// <returns></returns>
        public abstract bool StartServer();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool ClientStatus();

        /// <summary>
        /// Method used to ascertain server status (TCP any client connect, UDP is socket open)
        /// </summary>
        /// <returns></returns>
        public abstract bool ServerStatus();
        
        /// <summary>
        /// Method used to send data (TCP to connected client, UDP to end point)
        /// </summary>
        /// <param name="Buffer"></param>
        public abstract void SendPacket(byte[] Buffer);
        
        /// <summary>
        /// Closes any currently open connections and stops threads
        /// </summary>
        public abstract void Dispose();
    }
}
