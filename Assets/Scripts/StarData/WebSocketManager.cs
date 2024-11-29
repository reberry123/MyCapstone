using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketManager : MonoBehaviour
{
    private WebSocket ws;
    public event Action<string> OnReceive;

    void Start()
    {
        // WebSocket �ʱ�ȭ
        ws = new WebSocket("wss://port-0-capstoneserver-m2qhwewx334fe436.sel4.cloudtype.app/ws");
        //ws = new WebSocket("ws://localhost:8000/ws");

        // �������� �����͸� ���� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received Data: " + e.Data);

            OnReceive?.Invoke(e.Data);
        };

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("SUCCESS");
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log(e.Message);
        };

        // WebSocket ������ ����
        try
        {
            ws.Connect();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        

        // �ʱ� �޽����� ������ ����
        //SendDataToServer();
    }

    // ������ �����͸� ���� �� ���
    void SendDataToServer()
    {
        if (ws != null && ws.IsAlive)
        {
            // ���� ������
            string data = "{\"location\":{\"lat\":37.5665, \"lon\":126.9780}}";
            Debug.Log("Sending data to server: " + data);
            ws.Send(data);
        }
    }

    // WebSocket ���� ����
    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}
