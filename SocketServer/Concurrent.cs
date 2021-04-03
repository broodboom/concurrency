//0966972 Maurice Pakvis
//0958779 Thomas Visser﻿

using Sequential;
using System;
//todo [Assignment]: add required namespaces
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Concurrent
{
    public class ConcurrentServer : SequentialServer
    {
        // todo [Assignment]: implement required attributes specific for concurrent server
        List<string> votes = new List<string>();

        List<Thread> threads = new List<Thread>();
        private readonly object lockObject = new object();

        public ConcurrentServer(Setting settings) : base(settings)
        {
            // todo [Assignment]: implement required code
            this.settings = settings;
            this.ipAddress = IPAddress.Parse(settings.serverIPAddress);
        }
        public override void prepareServer()
        {
            Console.WriteLine(this.ipAddress.ToString() + settings.serverPortNumber);

            localEndPoint = new IPEndPoint(this.ipAddress, settings.serverPortNumber);
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(settings.serverListeningQueue);

            // todo 4: analyse details of this listening loop. Everything in the server starts here.
            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for incoming connections ... ");
                    Socket connection = listener.Accept();
                    this.numOfClients++;
                    Console.WriteLine("Connected!");              

                    ThreadStart threadStart = () => this.handleConcurrentClient(connection);
                    Thread thread = new Thread(threadStart) { IsBackground = true };

                    lock (lockObject)
                    {
                        threads.Add(thread);
                    }

                    thread.Start();
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }
            }

            // todo [Assignment]: implement required code
        }

        public void handleConcurrentClient(Socket con)
        {
            string data = "", reply = "";
            byte[] bytes = new Byte[bufferSize];

            this.sendMessage(con, Message.ready);
            int numByte = con.Receive(bytes);
            data = Encoding.UTF8.GetString(bytes, 0, numByte);
            reply = processMessage(data);
            this.sendMessage(con, reply);
        }

        public override string processMessage(String msg)
        {
            //todo 6: check how received messages are processed and handled. 
            Thread.Sleep(settings.serverProcessingTime);
            string replyMsg = Message.confirmed;            
            try
            {
                switch (msg)
                {
                    case Message.terminate:
                        //todo 7: when this case is executed?
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("[Server] received from the client -> {0} ", msg);
                        Console.ResetColor();
                        Console.WriteLine("[Server] END : number of clients communicated -> {0} ", this.numOfClients);
                        lock (lockObject)
                        {
                            foreach (var thread in threads)
                            {
                                if (thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                                {
                                    thread.Join();
                                }
                            }
                        }
                        foreach (string i in votes)
                        {
                            Console.WriteLine(i);
                        }
                        break;
                    default:
                        //todo 8: which part of the protocol is implemented here?
                        replyMsg = Message.confirmed;
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("[Server] received from the client -> {0} ", msg);
                        Console.ResetColor();
                        votes.Add(msg);
                        break;
                }
            }
            catch (Exception e) { Console.Out.WriteLine("[Server] Process Message {0}", e.Message); }

            return replyMsg;
        }
    }
}
