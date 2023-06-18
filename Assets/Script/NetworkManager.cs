
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
        public const int ServerPort = 8888;

        public event Action<string, string> MessageReceivedAction;

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
            server = new UdpClient(ServerPort);
            clientEndpoint = new IPEndPoint(IPAddress.Any, ServerPort);

            while (true)
            {
                try
                {
                    byte[] data = server.Receive(ref clientEndpoint);
                    string message = Encoding.UTF8.GetString(data);
                    Debug.Log($"Listener heard: {message} - {clientEndpoint.Address}");

                    OnMessageReceived(message, clientEndpoint.Address.ToString());
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode != 10060)
                        Debug.LogError(ex.ErrorCode + "\n" + ex.StackTrace);
                    else
                        Debug.LogWarning("Expected timeout error");
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnMessageReceived(string message, string ip)
        {
            MessageReceivedAction?.Invoke(message, ip);
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
