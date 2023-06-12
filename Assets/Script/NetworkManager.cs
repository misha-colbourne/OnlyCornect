
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using System.Text;
using System.Threading;

namespace OnlyCornect
{
    public class NetworkManager : MonoBehaviour
    {
        // --------------------------------------------------------------------------------------------------------------------------------------
        private const int serverPort = 8888;

        private Thread listenThread;
        private UdpClient server;
        private IPEndPoint clientEndpoint;

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            listenThread = new Thread(new ThreadStart(Listen));
            listenThread.Start();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void Listen()
        {
            server = new UdpClient(serverPort);
            clientEndpoint = new IPEndPoint(IPAddress.Any, serverPort);

            while (true)
            {
                try
                {
                    byte[] data = server.Receive(ref clientEndpoint);
                    string message = Encoding.UTF8.GetString(data);
                    Debug.Log("Listener heard: " + message);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode != 10060)
                        Debug.Log("a more serious error " + ex.ErrorCode);
                    else
                        Debug.Log("expected timeout error");
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnDisable() { CleanUp(); }
        private void OnDestroy() { CleanUp(); }
        private void OnApplicationQuit() { CleanUp(); }

        void CleanUp()
        {
            server?.Close();
            listenThread?.Abort();
            listenThread?.Join(5000);
            listenThread = null;
        }
    }
}
