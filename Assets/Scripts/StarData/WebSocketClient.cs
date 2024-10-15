using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        // WebSocket �ʱ�ȭ (Python ������ ����)
        ws = new WebSocket("ws://localhost:8765");

        // �������� �����͸� ���� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received Data: " + e.Data);

            // �������� ���� �����͸� ó���ϴ� ��
        };

        // WebSocket ������ ����
        ws.Connect();

        // �ʱ� �޽����� ������ ����
        SendDataToServer();
    }

    // ������ �����͸� ���� �� ���
    void SendDataToServer()
    {
        if (ws != null && ws.IsAlive)
        {
            // ���� ������
            string data = "{\"location\":[37.5665, 126.9780]}";
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

    // Update is called once per frame
    void Update()
    {

    }
}
