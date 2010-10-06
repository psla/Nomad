using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace PipeRunner
{
    public class PipeServer
    {
        public static void Main()
        {
            while (true)
            {
                using (var pipeServer =
                    new NamedPipeServerStream("runnerPipe", PipeDirection.InOut))
                {
                    Console.WriteLine("NamedPipeServerStream object created.");

                    Console.WriteLine("Waiting for client");

                    // Wait for a client to connect
                    pipeServer.WaitForConnection();

                    Console.WriteLine("Client connected.");

                    StreamWriter sw = null;
                    StreamReader sr = null;
                    try
                    {
                        // Read user input and send that to the client process.

                        string commandToExecute = "";
                        string parameters = "";
                        int exitCode = 1;
                        sr = new StreamReader(pipeServer);
                        commandToExecute = sr.ReadLine();
                        parameters = sr.ReadLine();

                        Console.WriteLine("Executing provided command {0} {1}", commandToExecute, parameters);

                        using (Process process = Process.Start(commandToExecute, parameters))
                        {
                            process.WaitForExit();
                            exitCode = process.ExitCode;
                            Console.WriteLine("Exit code returned from application: {0}", exitCode);
                        }

                        Console.WriteLine("Finishing");
                        sw = new StreamWriter(pipeServer);

                        sw.AutoFlush = true;
                        sw.WriteLine(exitCode);
                    }
                        // Catch the IOException that is raised if the pipe is 
                        // broken or disconnected.
                    catch (IOException e)
                    {
                        Console.WriteLine("PIPE ERROR: {0}", e.Message);
                    }
                    finally
                    {
                        if (sw != null)
                        {
                            sw.Close();
                            sw.Dispose();
                        }
                        if(sr!=null)
                        {
                            sr.Close();
                            sr.Dispose();
                        }
                    }
                }
            }
        }
    }
}