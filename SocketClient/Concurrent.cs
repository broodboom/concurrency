using System;
using System.Threading;
using System.Threading.Tasks; // 👈
using Sequential;

namespace Concurrent
{
    public class ConcurrentClient : SimpleClient
    {
        public Thread workerThread;

        public ConcurrentClient(int id, Setting settings) : base(id, settings)
        {
            // todo [Assignment]: implement required code
        }
        public void run()
        {
            // todo [Assignment]: implement required code
        }
    }
    public class ConcurrentClientsSimulator : SequentialClientsSimulator
    {
        private ConcurrentClient[] clients;

        public ConcurrentClientsSimulator() : base()
        {
            Console.Out.WriteLine("\n[ClientSimulator] Concurrent simulator is going to start with {0}", settings.experimentNumberOfClients);
            clients = new ConcurrentClient[settings.experimentNumberOfClients];
            // 👇
            configure();
            for (int i = 0; i < settings.experimentNumberOfClients; i++)
            {
                clients[i] = new ConcurrentClient(i + 1, settings); // id>0 means this is not a terminating client
            }
            // 👆
        }

        public void ConcurrentSimulation()
        {
            try
            {
                // todo [Assignment]: implement required code
                // 👇
                var options = new ParallelOptions();
                Parallel.For(0, settings.experimentNumberOfClients, options, (i) =>
                {
                    clients[i].prepareClient();
                    clients[i].communicate();
                });
                // 👆
            }
            catch (Exception e)
            { Console.Out.WriteLine("[Concurrent Simulator] {0}", e.Message); }

            // 👇
            Console.Out.WriteLine("\n[Concurrentimulator] All clients finished with their communications ... ");
            Thread.Sleep(settings.delayForTermination);
            SimpleClient endClient = new SimpleClient(-1, settings);
            endClient.prepareClient();
            endClient.communicate();
            // 👆
        }
    }
}