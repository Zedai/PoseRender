using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class TCPPythonServer : MonoBehaviour
{
    public Camera cam;
    public RenderTexture image;
    Thread thread;
    // Start is called before the first frame update
    void Start()
    {
        thread = new Thread(() => RunServer(image));
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


  public static void RunServer(RenderTexture image)
  {
    int camWidth = 256;
    int camHeight = 256;
    TcpListener server=null;
    Socket socket;
    try
    {
      // Set the TcpListener on port 13000.
      Int32 port = 12349;
      IPAddress localAddr = IPAddress.Parse("127.0.0.1");

      // TcpListener server = new TcpListener(port);
      server = new TcpListener(localAddr, port);

      // Start listening for client requests.
      server.Start();

      // Buffer for reading data
      Byte[] bytes = new Byte[256];
      String data = null;

      // Enter the listening loop.

        Console.Write("Waiting for a connection... ");

        // Perform a blocking call to accept requests.
        // You could also use server.AcceptSocket() here.
           socket = server.AcceptSocket();
            Console.WriteLine("Connection accepted.");      

      
      while(true)
      {
          Texture2D t2D = new Texture2D(camWidth, camHeight, TextureFormat.RGB24, false);

           string responseString = "You have successfully connected to me";

           //Forms and sends a response string to the connected client.
           Byte[] sendBytes = Encoding.ASCII.GetBytes(responseString);
           int i = socket.Send(sendBytes);
           Thread.Sleep(1000);
      }
    }
    catch(SocketException e)
    {
      Console.WriteLine("SocketException: {0}", e);
    }
    finally
    {
       // Stop listening for new clients.
       server.Stop();
    }

    Console.WriteLine("\nHit enter to continue...");
    Console.Read();
  }
}	

// /**
// * The following sample is intended to demonstrate how to use a
// * TcpListener for synchronous communcation with a TCP client
// * It creates a TcpListener that listens on the specified port (13000).
// * Any TCP client that wants to use this TcpListener has to explicitly connect
// * to an address obtained by the combination of the server
// * on which this TcpListener is running and the port 13000.
// * This TcpListener simply echoes back the message sent by the client
// * after translating it into uppercase.
// * Refer to the related client in the TcpClient class.
// */
// using System;
// using System.Text;
// using System.IO;
// using System.Net;
// using System.Net.Sockets;
// using System.Threading;

// public class TcpListenerSample
// {

//     static void Main(string[] args)
//     {
//         try
//         {
//             // set the TcpListener on port 13000
//             int port = 13000;
//             TcpListener server = new TcpListener(IPAddress.Any, port);

//             // Start listening for client requests
//             server.Start();

//             // Buffer for reading data
//             byte[] bytes = new byte[1024];
//             string data;

//             //Enter the listening loop
//             while (true)
//             {
//                 Console.Write("Waiting for a connection... ");

//                 // Perform a blocking call to accept requests.
//                 // You could also use server.AcceptSocket() here.
//                 TcpClient client = server.AcceptTcpClient();
//                 Console.WriteLine("Connected!");

//                 // Get a stream object for reading and writing
//                 NetworkStream stream = client.GetStream();

//                 int i;

//                 // Loop to receive all the data sent by the client.
//                 i = stream.Read(bytes, 0, bytes.Length);

//                 while (i != 0)
//                 {
//                     // Translate data bytes to a ASCII string.
//                     data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
//                     Console.WriteLine(String.Format("Received: {0}", data));

//                     // Process the data sent by the client.
//                     data = data.ToUpper();

//                     byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

//                     // Send back a response.
//                     stream.Write(msg, 0, msg.Length);
//                     Console.WriteLine(String.Format("Sent: {0}", data));

//                     i = stream.Read(bytes, 0, bytes.Length);
//                 }

//                 // Shutdown and end connection
//                 client.Close();
//             }
//         }
//         catch (SocketException e)
//         {
//             Console.WriteLine("SocketException: {0}", e);
//         }

//         Console.WriteLine("Hit enter to continue...");
//         Console.Read();
//     }
// }