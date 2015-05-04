using System;
using Server.Implementation;

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
                Console.WriteLine("Server started with ip address: {0}, port number: {1}", manager.ServerIp, manager.ServerPort);
                Console.WriteLine("Press enter to stop the server!");
                Console.ReadKey();
            }

            Console.WriteLine("\nAttempting to stop, please wait, it will be closed immediately.");
            manager.Stop();
        }
    }
}
