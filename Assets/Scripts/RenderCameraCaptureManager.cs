using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class RenderCameraCaptureManager : MonoBehaviour
{
    public Camera cam;
    Thread thread;
    public string saveDirectory = "/home/sai/PythonRenderImages/";
    public Button button;
    public RenderTexture render;
    byte[] bytefield;
    bool threadSentinel;
    String currentMessage;
    bool messageReady, updateParams, postRender;
    Vector3 pos;
    Quaternion rot;
    float fov;

    // Start is called before the first frame update
    void Start()
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = "Save Render";
        button.GetComponent<Button>().onClick.AddListener(SaveRender);
 //       thread = new Thread(() => RunServer(image));
        
        thread = new Thread(() => RunServer());
        thread.Start();
        
        bytefield = GetRenderBytes();
        threadSentinel = true;
        messageReady = false;
        currentMessage = null;
        // Xpos = ;
        // Ypos = ;
        // Zpos = ;
        // Xrot = ;
        // Yrot = ;
        // Zrot = ;
        // fov = ;
        pos = cam.transform.position;
        rot = cam.transform.rotation;
        fov = cam.fieldOfView;
        updateParams = false;
        postRender = false;
    }

    // Update is called once per frame
    void Update()
    {
        // int i = 30;
        // cam.fov = 30;
        // print(cam.fov.GetType());
        // cam.transform.position = new Vector3(0, 20, 0);
        // print(cam.transform.rotation.eulerAngles);
        // print(new Quaternion(1, 2, 3, 1).eulerAngles);
        // print("running");
        if(updateParams){
            cam.transform.position = pos;
            cam.transform.rotation = rot;
            cam.fieldOfView = fov;
        }
    }

    void LateUpdate(){
        bytefield = GetRenderBytes();

        if(updateParams){
            updateParams = false;
            postRender = true;
        }
    }

    void RunServer()
    {
        //int camWidth = 256;
        //int camHeight = 256;
        
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

        //#int count = 1;        
        while(threadSentinel)
        {
        //     if(postRender){
        //         print("sending image to python");
        //         socket.Send(bytefield);
        //         socket.Send(Encoding.ASCII.GetBytes("done"));
        //         postRender = false;
        //     }

            //Texture2D t2D = new Texture2D(camWidth, camHeight, TextureFormat.RGB24, false);

            byte[] recv_bytes = new byte[1024];
            // foreach(byte by in recv_bytes){
            //     by = 1;
            // }

            // for(int i = 0; i<1024;i++){
                // recv_bytes[i]=1;
            // }
            socket.Receive(recv_bytes);
            //print(string.Join(", ", recv_bytes));
            String message_snippet = Encoding.ASCII.GetString(recv_bytes);

            if(messageReady){
                int XposI = currentMessage.IndexOf("Xpos:");
                int YposI= currentMessage.IndexOf("Ypos:");
                int ZposI = currentMessage.IndexOf("Zpos:");
                int XrotI = currentMessage.IndexOf("Xrot:");
                int YrotI = currentMessage.IndexOf("Yrot:");
                int ZrotI = currentMessage.IndexOf("Zrot:");
                int WrotI = currentMessage.IndexOf("Wrot:");
                int fovI = currentMessage.IndexOf("fov:");
                pos = new Vector3(Int32.Parse(currentMessage.Substring(XposI + 5, YposI - XposI - 5).Trim()),
                                Int32.Parse(currentMessage.Substring(YposI + 5, ZposI - YposI - 5).Trim()),
                                Int32.Parse(currentMessage.Substring(ZposI + 5, XrotI - ZposI - 5).Trim()));
                rot = new Quaternion(float.Parse(currentMessage.Substring(XrotI + 5, YrotI - XrotI - 5).Trim()),
                                float.Parse(currentMessage.Substring(YrotI + 5, ZrotI - YrotI - 5).Trim()),
                                float.Parse(currentMessage.Substring(ZrotI + 5, WrotI - ZrotI - 5).Trim()),
                                float.Parse(currentMessage.Substring(WrotI + 5, fovI - WrotI - 5).Trim()));
                fov = float.Parse(currentMessage.Substring(fovI + 4).Trim());
                // print(currentMessage.Substring(currentMessage.IndexOf("Xpos:") + 5, currentMessage.IndexOf("Ypos:")-5).Trim());
                // print(currentMessage);
                messageReady = false;
                currentMessage = null;
                updateParams = true;
                // print("hi");
            }else{
                if(currentMessage is null && message_snippet.Contains("start")){
                    if(message_snippet.Contains("end")){
                        // print(message_snippet);
                        if(message_snippet.IndexOf("start") > message_snippet.IndexOf("end")){
                            message_snippet = message_snippet.Substring(message_snippet.IndexOf("end") + 3);
                        }
                        currentMessage = message_snippet.Substring(message_snippet.IndexOf("start") + 5, message_snippet.IndexOf("end") - message_snippet.IndexOf("start") - 5).Trim();
                        messageReady = true;
                    }
                    else{
                        currentMessage = message_snippet.Substring(message_snippet.IndexOf("start") + 5).Trim();
                    }
                }
                else if(currentMessage is null){}
                else if(!message_snippet.Contains("end")){
                    currentMessage = String.Concat(currentMessage, message_snippet.Trim());
                }else{
                    currentMessage = String.Concat(currentMessage, message_snippet.Substring(0, message_snippet.IndexOf("end")));
                    messageReady = true;
                }

            }
            
            string responseString = "You have successfully connected to me";


            //Forms and sends a response string to the connected client.
          
     //        Byte[] sendBytes = Encoding.ASCII.GetBytes(responseString);
    //         int i = socket.Send(sendBytes);
            // socket.Send(bytefield);
         //#   socket.Send(Encoding.ASCII.GetBytes("done number " + count));
            // socket.Send(Encoding.ASCII.GetBytes("done"));
            //  Thread.Sleep(5000);
         //#   count++;
        }
        socket.Send(Encoding.ASCII.GetBytes("close"));
        }
        catch(SocketException e)
        {
        print(String.Format("SocketException: {0}", e));
        }
        finally
        {
        // Stop listening for new clients.
        server.Stop();
        }
        

        // Console.WriteLine("\nHit enter to continue...");
        // Console.Read();
    }

    void SaveRender(){
        //yield return new WaitForEndOfFrame();

        byte[] bytes = GetRenderBytes();
        string now = System.DateTime.Now.ToString().Replace('/', '_');
        print("Render saved to " + saveDirectory+"render-"+now+".jpg");
        File.WriteAllBytes(saveDirectory+"render-"+now+".jpg", bytes);
    }

    byte[] GetRenderBytes(){
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = render;
        Texture2D tex = new Texture2D(render.width, render.width, TextureFormat.RGB24, false);  //can use Screen.height and Screen.width to get current screen size
        tex.ReadPixels(new Rect(0, 0, render.width, render.height), 0, 0);
        tex.Apply();
        byte[] bytes = tex.EncodeToJPG();
        UnityEngine.Object.Destroy(tex); //Not sure if necessary
        RenderTexture.active = currentActiveRT;

        return bytes;
    }
    
    void OnDisable(){
        // print("dis");
        // thread.Close();
        threadSentinel = false;
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