using System;
using System.IO.Ports;
using System.Threading;
using TMPro;
using UnityEngine;


public class SerialPortManager : MonoBehaviour
{
    [Header("Port Settings")]
    public SerialPort dataStream = new SerialPort("COM4", 9600);

    [SerializeField] TMP_InputField portNumber_InputField;
    public string linedata;
    private Thread readThread;

    [SerializeField] GameObject PORTCanvas;
    [SerializeField] GameObject GameCanvas;

    public void Set_StartPortButton()
    {
        string port = portNumber_InputField.text;

        if (!string.IsNullOrEmpty(port))
        {
            PlayerPrefs.SetString("myport", port);
        }

        dataStream = new SerialPort("COM" + PlayerPrefs.GetString("myport"), 9600);

        try
        {
            if (!dataStream.IsOpen)
                dataStream.Open();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error opening serial port: {e.Message}");
            return;
        }

        readThread = new Thread(Read);
        readThread.Start();

        PORTCanvas.SetActive(false);
        GameCanvas.SetActive(true);
    }

    private void Read()
    {
        while (true)
        {
            try
            {
                linedata = dataStream.ReadLine();
                Debug.Log(linedata);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error reading from the serial port: {e.Message}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (dataStream != null && dataStream.IsOpen)
        {
            dataStream.Close();
        }

        if (readThread != null && readThread.IsAlive)
        {
            readThread.Abort();
        }
    }
}
