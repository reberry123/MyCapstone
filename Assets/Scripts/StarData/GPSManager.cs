using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float heightAboveGround = 1.0f;

    private double currentLat;
    private double currentLong;
    public double targetLat = 90.0;
    public double targetLong = 0.0;
    public Vector3 northDirection;

    private const double EARTH_RADIUS = 6371000;
    private const double DEG_TO_RAD = Math.PI / 180.0;

    private float initialCompassHeading;
    private bool isCompassInitialized = false;

    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();
        StartCoroutine(RequestLocationPermission());

        // ��ħ�� Ȱ��ȭ
        Input.compass.enabled = true;
    }

    private void SetupLineRenderer()
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if (!isCompassInitialized && Input.compass.enabled)
        {
            // �ʱ� ��ħ�� ���� ����
            initialCompassHeading = Input.compass.trueHeading;
            isCompassInitialized = true;
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            currentLat = Input.location.lastData.latitude;
            currentLong = Input.location.lastData.longitude;
            UpdateLine();
        }
    }

    private void UpdateLine()
    {
        Vector3 startPoint = Vector3.zero;
        Vector3 endPoint = CalculatePositionWithHaversine(currentLat, currentLong, targetLat, targetLong);

        // ��ħ�� ���� ����
        float currentHeading = Input.compass.trueHeading;
        float headingDifference = initialCompassHeading;// - currentHeading;

        // ȸ�� ��� ����
        Quaternion rotation = Quaternion.Euler(0, headingDifference, 0);
        endPoint = rotation * endPoint;
        northDirection = endPoint;

        // AR ȯ�濡 �°� ���� ����
        startPoint.y = heightAboveGround;
        endPoint.y = heightAboveGround;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        // ����� ����
        Debug.Log($"Compass Heading: {currentHeading}, Initial Heading: {initialCompassHeading}, Difference: {headingDifference}");
    }

    private Vector3 CalculatePositionWithHaversine(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = (lat2 - lat1) * DEG_TO_RAD;
        double dLon = (lon2 - lon1) * DEG_TO_RAD;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1 * DEG_TO_RAD) * Math.Cos(lat2 * DEG_TO_RAD) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EARTH_RADIUS * c;

        // ������ ���
        double y = Math.Sin(dLon) * Math.Cos(lat2 * DEG_TO_RAD);
        double x = Math.Cos(lat1 * DEG_TO_RAD) * Math.Sin(lat2 * DEG_TO_RAD) -
                  Math.Sin(lat1 * DEG_TO_RAD) * Math.Cos(lat2 * DEG_TO_RAD) * Math.Cos(dLon);
        double bearing = Math.Atan2(y, x);

        // Unity ��ǥ��� ��ȯ (���� ����)
        float unityX = (float)(distance * Math.Sin(bearing));
        float unityZ = (float)(distance * Math.Cos(bearing));

        return new Vector3(unityX, 0, unityZ);
    }

    // ������ ��� (���� ����, �ð� �������� 0-360��)
    private float CalculateBearing(Vector2 from, Vector2 to)
    {
        // �ܼ� ������ ��� (���� -> ���� ��ȯ)
        float dLon = Mathf.Deg2Rad * (to.y - from.y);
        float lat1 = Mathf.Deg2Rad * from.x;
        float lat2 = Mathf.Deg2Rad * to.x;

        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2);
        float x = Mathf.Cos(lat1) * Mathf.Sin(lat2) -
                  Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(dLon);

        float bearing = Mathf.Atan2(y, x);
        return (Mathf.Rad2Deg * bearing + 360) % 360; // 0 ~ 360���� ��ȯ
    }

    // GPS ���� ��û �ڵ�� ������ ����
    private IEnumerator RequestLocationPermission()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS not enabled");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            Debug.Log("GPS initialization timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
    }
}