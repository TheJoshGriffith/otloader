using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace ServerLoader
{
    class Program
    {
        // Configurations
        static string exe_serverFile = "tfs.exe";

        public static List<SVNFile> SVNFiles = new List<SVNFile>();

        // Declarations
        public static Process TFS = new Process();
        public static Socket _serverSocket, _clientSocket;
        public static byte[] _buffer;
        public static bool shutdown = false;

        static void Main(string[] args)
        {
            Console.WriteLine("\nWelcome to XtrmJosh's control centre for TFS Console.\n");

            LoadBatchFiles();

            // Function generate processes
            Init();

            // Function start server
            StartServer();

            // Function begin process
            RunTFS();

            while (!shutdown)
            {
                Thread.Sleep(10);
            }
        }

        public static void LoadBatchFiles()
        {
            // Read BatchFiles.dat file
            string batches = File.ReadAllText("BatchFiles.dat");
            // Split them by line
            string[] indi = batches.Split('\n');
            // Cycle through the results
            foreach (string batfile in indi)
            {
                // Split each result by the comma
                string[] bat = batfile.Split(',');
                // Create a new instance of an SVN File
                SVNFile svnf = new SVNFile();
                // Append the file name to the first part of the last split string
                svnf.fileName = bat[0];
                // Set the TCP command to the second part of the last split string
                svnf.command = bat[1];
                // Generate a new process object under the batFile object
                svnf.batFile = new Process();
                // Set the file name to the correct 
                svnf.batFile.StartInfo.FileName = bat[0];
                // Add it to the globally declared list of batch files
                SVNFiles.Add(svnf);
                // Check for the files existance else disable it
                if (!File.Exists(svnf.fileName))
                {
                    Console.WriteLine("Could not find file: " + svnf.fileName + ". Using command: " + svnf.command);
                    svnf.fileFound = false;
                }
                else
                {
                    Console.WriteLine("Loaded batch file: " + svnf.fileName + ". Using command: " + svnf.command);
                    svnf.fileFound = true;
                }
            }
        }


        public static void StartServer()
        {
            try
            {
                Console.Write("Configuring sockets for TCP Listener...");
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 3333));
                _serverSocket.Listen(0);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                Console.Write(" Done\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initialising TCP Server: " + ex.Message);
                Console.Read();
            }
        }

        public static void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                _clientSocket = _serverSocket.EndAccept(AR);
                _buffer = new byte[_clientSocket.ReceiveBufferSize];
                _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

                Console.WriteLine("Client has connected.\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error accepting client: " + ex.Message);
                Console.Read();
            }
        }

        public static void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                int received = _clientSocket.EndReceive(AR);
                if (received == 0)
                {
                    return;
                }
                string text = Encoding.ASCII.GetString(_buffer, 0, received);

                // TODO: HANDLE ISSUED COMMANDS HERE

                string[] message = text.Split(' ');

                if (message[1] == "restart")
                {
                    RestartServer();
                }
                else
                {
                    foreach (SVNFile svnf in SVNFiles)
                    {
                        Console.Write(message[1].Split('\0')[0] + " : ");
                        Console.Write(svnf.command + " : ");
                        if (message[1].Split('\0')[0] == svnf.command)
                        {
                            Console.WriteLine("Found a match: " + message[1]);
                            RunBat(svnf);
                        }
                    }
                }



                Console.WriteLine("Command issued: " + text);
                _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing message: " + ex.Message);
                Console.Read();
            }
        }


        private static void RunBat(SVNFile svnf)
        {
            if (!svnf.fileFound)
            {
                Console.WriteLine("User issued command could not be processed as the batch file could not be found.");
            }
            else
            {
                try
                {
                    Console.Write("Starting SVN Update...");
                    svnf.batFile.Start();
                    Console.Write(" Done.\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SVN Update failed: " + ex.Message + ". Trying: " + svnf.fileName);
                    Console.Read();
                }
            }
        }



        static void Init()
        {
            try
            {
                //Initialise the TFS process
                Console.Write("Initialising Process...");
                TFS.StartInfo.FileName = exe_serverFile;
                Console.Write(" Done\n");
            }
            catch (Exception ex)
            {
                Console.Write(" Failed \nTFS Restart Failed: " + ex.Message + "\n");
            }
        }

        static void RunTFS()
        {
            Process[] procs = Process.GetProcessesByName("tfs");
            if (procs.Length > 0)
            {
                Console.Write("Found and attached to running TFS...");
                TFS = procs[0];
                Console.Write(". Done\n");
            }
            else
            {
                try
                {
                    Console.Write("Running Server...");
                    TFS.Start();
                    Console.Write(" Done\n");
                }
                catch (Exception ex)
                {
                    Console.Write(" Failed \nTFS Start Failed: " + ex.Message + "\n");
                    Console.Read();
                }
            }
        }

        static void RestartServer()
        {
            TFS.Kill();
            TFS.Start();
        }

        static void StopServer()
        {
            try
            {
                Console.Write("Closing Process...");
                TFS.Close();
                Console.Write(" Done\n");
                Console.Write("Releasing Resources...");
                TFS.Dispose();
                Console.Write(" Done\n");
            }
            catch (Exception ex)
            {
                Console.Write(" Failed \nTFS Stop Failed: " + ex.Message + "\n");
                Console.Read();
            }
        }
    }
}
