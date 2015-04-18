using System;

namespace Server
{
    class Program
    {
        static void Main()
        {
            var manager = ServerManager.ServerManagerInstance;
            manager.Start();

            
            if (manager.Running)
            {
                Console.WriteLine("Server started! IP: {0}, port: {1}", manager.ServerIp, manager.ServerPort);
                Console.WriteLine("Send key for stopping server!");
                Console.ReadKey();
            }


            manager.Stop();
        }
    }
}
