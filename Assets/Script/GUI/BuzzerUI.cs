using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class BuzzerUI : MonoBehaviour
    {
        // --------------------------------------------------------------------------------------------------------------------------------------
        public Button buzzer;
        public TMP_InputField nameEntry;

        private YmlParser.BuzzerConfig config;

        private UdpClient client;
        private IPEndPoint serverEndPoint;

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            buzzer.SetActive(false);
            nameEntry.SetActive(true);
            nameEntry.onEndEdit.AddListener(ConfirmNameEntry);

            config = YmlParser.ParseBuzzerConfig();

            try
            {
                client = new UdpClient();
                serverEndPoint = new(IPAddress.Parse(config.IP), config.Port);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to create UDP client: " + e.Message);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Buzz()
        {
            EventSystem.current.SetSelectedGameObject(null);

            try
            {
                byte[] message = Encoding.UTF8.GetBytes(nameEntry.text);
                client.Send(message, message.Length, serverEndPoint);
                Debug.Log($"Sent {nameEntry.text} to {serverEndPoint}");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to send message to the host application: " + e.Message + "\n" + e.StackTrace);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnDestroy()
        {
            client?.Close();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void ConfirmNameEntry(string value)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                buzzer.SetActive(true);
                nameEntry.SetActive(false);
            }
        }
    }
}