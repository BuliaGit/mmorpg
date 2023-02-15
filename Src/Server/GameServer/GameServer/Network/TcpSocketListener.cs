// RayMix Libs - RayMix's .Net Libs
// Copyright 2018 Ray@raymix.net.  All rights reserved.
// https://www.raymix.net
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of RayMix.net. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// Listens for socket connection on a given address and port.
    /// </summary>
    public class TcpSocketListener : IDisposable
    {
        #region Fields
        private Int32 connectionBacklog;
        private IPEndPoint endPoint;

        private Socket listenerSocket;
        private SocketAsyncEventArgs args;  //异步操作
        #endregion

        #region Properties
        /// <summary>
        /// Length of the connection backlog.
        /// </summary>
        public Int32 ConnectionBacklog
        {
            get { return connectionBacklog; }
            set
            {
                lock (this)
                {
                    if (IsRunning)
                        throw new InvalidOperationException("Property cannot be changed while server running.");
                    else
                        connectionBacklog = value;
                }
            }
        }
        /// <summary>
        /// The IPEndPoint to bind the listening socket to.
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
            set
            {
                lock (this)
                {
                    if (IsRunning)
                        throw new InvalidOperationException("Property cannot be changed while server running.");
                    else
                        endPoint = value;
                }
            }
        }
        /// <summary>
        /// Is the class currently listening.
        /// </summary>
        public Boolean IsRunning
        {
            get { return listenerSocket != null; }
        }
        #endregion

        #region Constructors 构造函数
        /// <summary>
        /// Listens for socket connection on a given address and port.
        /// </summary>
        /// <param name="address">The address to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        /// <param name="connectionBacklog">The connection backlog.</param>
        public TcpSocketListener(String address, Int32 port)
            : this(IPAddress.Parse(address), port)
        { }

        /// <summary>
        /// Listens for socket connection on a given address and port.
        /// </summary>
        /// <param name="address">The address to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        /// <param name="connectionBacklog">The connection backlog.</param>
        public TcpSocketListener(IPAddress address, Int32 port)
            : this(new IPEndPoint(address, port))
        { }

        /// <summary>
        /// Listens for socket connection on a given address and port.
        /// </summary>
        /// <param name="endPoint">The endpoint to listen on.</param>
        /// <param name="connectionBacklog">The connection backlog.</param>
        public TcpSocketListener(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;

            args = new SocketAsyncEventArgs();
            args.Completed += OnSocketAccepted;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start listening for socket connections.
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                if (!IsRunning)
                {
                    listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    listenerSocket.Bind(endPoint);
                    listenerSocket.Listen(connectionBacklog);   //排队数+多线程数（至少为1）
                    BeginAccept(args);
                }
                else
                    throw new InvalidOperationException("The Server is already running.");
            }
        }

        /// <summary>
        /// Stop listening for socket connections.
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                if (listenerSocket == null)
                    return;
                listenerSocket.Close();
                listenerSocket = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Asynchronously listens for new connections.
        /// </summary>
        /// <param name="args"></param>
        private void BeginAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            listenerSocket.AcceptAsync(args);
            /*listenerSocket.InvokeAsyncMethod(new SocketAsyncMethod(listenerSocket.AcceptAsync)
                , OnSocketAccepted, args);*/
        }
        /// <summary>
        /// Invoked when an asynchrounous accept completes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The SocketAsyncEventArgs for the operation.</param>
        private void OnSocketAccepted(object sender, SocketAsyncEventArgs e)
        {
            SocketError error = e.SocketError;
            if (e.SocketError == SocketError.OperationAborted)
                return; //Server was stopped

            if (e.SocketError == SocketError.Success)
            {
                Socket handler = e.AcceptSocket;
                OnSocketConnected(handler);
            }

            lock (this)
            {
                BeginAccept(e);     //循环建立连接，不加它，由于服务器一直开，而客户端不能重复登录上线
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Fired when a new connection is received. 当接收到新连接时触发
        /// </summary>
        public event EventHandler<Socket> SocketConnected;
        /// <summary>
        /// Fires the SocketConnected event.
        /// </summary>
        /// <param name="client">The new client socket.</param>
        private void OnSocketConnected(Socket client)
        {
            if (SocketConnected != null)
                SocketConnected(this, client);
        }
        #endregion

        #region IDisposable Members
        private Boolean disposed = false;

        ~TcpSocketListener()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Stop();
                    if (args != null)
                        args.Dispose();
                }

                disposed = true;
            }
        }
        #endregion
    }
}
