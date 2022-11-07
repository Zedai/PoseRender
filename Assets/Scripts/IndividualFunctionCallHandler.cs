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


public class IndividualFunctionCallHandler : MonoBehaviour
{
    public Camera cam;
    Thread thread;
    public RenderTexture render;
    byte[] bytefield;
    bool threadSentinel;
    String currentMessage;
    bool messageReady, updateParams, postRender, paramsUpdated;
    Vector3 pos;
    Quaternion rot;
    float fov;

    // Start is called before the first frame update
    void Start()
    {

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

        //print(Time.deltaTime);
        // print(updateParams);
        if (updateParams)
        {
            cam.transform.position = pos;
            cam.transform.rotation = rot;
            cam.fieldOfView = fov;
            print("params updated");
            updateParams = false;
            paramsUpdated = true;
        }
    }

    void LateUpdate()
    {
        bytefield = GetRenderBytes();

        if (paramsUpdated)
        {
            paramsUpdated = false;
            postRender = true;
        }
    }

    void RunServer()
    {
        //int camWidth = 256;
        //int camHeight = 256;

        TcpListener server = null;
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

            print("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            socket = server.AcceptSocket();
            print("Connection accepted.");
            socket.ReceiveTimeout = 100;

            //#int count = 1;        
            while (threadSentinel)
            {
                if (postRender)
                {
                    print("sending image to python");
                    socket.Send(bytefield);
                    socket.Send(Encoding.ASCII.GetBytes("done"));
                    postRender = false;

                    socket = server.AcceptSocket();
                    print("Connection accepted.");
                    socket.ReceiveTimeout = 1000;
                }

                //Texture2D t2D = new Texture2D(camWidth, camHeight, TextureFormat.RGB24, false);

                byte[] recv_bytes = new byte[1024];
                // foreach(byte by in recv_bytes){
                //     by = 1;
                // }

                // for(int i = 0; i<1024;i++){
                // recv_bytes[i]=1;
                // }
                //   print(messageReady);
                //  print(currentMessage);
                if (messageReady)
                {
                    int XposI = currentMessage.IndexOf("Xpos:");
                    int YposI = currentMessage.IndexOf("Ypos:");
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
                    print(currentMessage);
                    messageReady = false;
                    currentMessage = null;
                    updateParams = true;
                    //     print(updateParams);
                    // print("hi");
                }
                else
                {

                    try
                    {
                        socket.Receive(recv_bytes);
                    }
                    catch (SocketException e)
                    {
                        //print("nodata");
                    }

                    //print(string.Join(", ", recv_bytes));
                    String message_snippet = Encoding.ASCII.GetString(recv_bytes);
                    //print(message_snippet);

                    if (currentMessage is null && message_snippet.Contains("start"))
                    {
                        if (message_snippet.Contains("end"))
                        {
                            // print(message_snippet);
                            if (message_snippet.IndexOf("start") > message_snippet.IndexOf("end"))
                            {
                                message_snippet = message_snippet.Substring(message_snippet.IndexOf("end") + 3);
                            }
                            currentMessage = message_snippet.Substring(message_snippet.IndexOf("start") + 5, message_snippet.IndexOf("end") - message_snippet.IndexOf("start") - 5).Trim();
                            messageReady = true;
                            // print(messageReady);
                        }
                        else
                        {
                            currentMessage = message_snippet.Substring(message_snippet.IndexOf("start") + 5).Trim();
                        }
                    }
                    else if (currentMessage is null) { }
                    else if (!message_snippet.Contains("end"))
                    {
                        currentMessage = String.Concat(currentMessage, message_snippet.Trim());
                    }
                    else
                    {
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
        catch (SocketException e)
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

    byte[] GetRenderBytes()
    {
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

    void OnDisable()
    {
        // print("dis");
        // thread.Close();
        threadSentinel = false;
        thread.Abort();
    }
}

