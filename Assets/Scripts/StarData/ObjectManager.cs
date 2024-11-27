using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ObjectManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI headAcc;

    public Button Reset;
    public GameObject starPrefab;
    public GameObject moonPrefab;
    public GameObject sunPrefab;
    public GameObject otherPrefab;
    public GameObject arrow;
    public Transform arCamera;
    public GameObject originObject;
    public Vector3 origin = Vector3.zero;
    public float radius = 1000.0f;
    public float lineWidth = 0.5f;

    private DataManager dataManager;
    private CompassManager compassManager;

    private GameObject sun, mercury, venus, mars, jupiter, saturn, uranus, neptune, pluto, moon;
    private List<List<GameObject>> constellationObjects = new List<List<GameObject>>();
    private List<GameObject> tmp = new List<GameObject>();
    private List<GameObject> ursaMinor = new List<GameObject>();

    private float trueNorth = 0.0f;
    private bool isInitialized = false;
    private bool isConstellationInitialized = false;

    void Start()
    {
        dataManager = FindAnyObjectByType<DataManager>();
        compassManager = FindAnyObjectByType<CompassManager>();

        InitializeObjects();
    }

    public void UpdateObjects()
    {
        if (!isConstellationInitialized)
        {
            InitializeConstellations();
            isConstellationInitialized = true;
        }
        /*
        if (isInitialized)
        {
            UpdateSolarObjects();
            UpdateConstellations();
        }
        */
    }

    void Update()
    {
        if (!isInitialized)
        {
            trueNorth = compassManager.GetTrueNorth();
            if (trueNorth == -1.0f) return;

            
            isInitialized = true;
        }

        if (isInitialized)
        {
            UpdateSolarObjects();
            UpdateConstellations();
        }
    }

    
    // �¾�� õü ��ġ ������Ʈ
    void UpdateSolarObjects()
    {
        CelestialObject obj;

        obj = dataManager.celestialObjects["sun"];
        UpdateStar(sun, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["mercury"];
        UpdateStar(mercury, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["venus"];
        UpdateStar(venus, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["mars"];
        UpdateStar(mars, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["jupiter"];
        UpdateStar(jupiter, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["saturn"];
        UpdateStar(saturn, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["uranus"];
        UpdateStar(uranus, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["neptune"];
        UpdateStar(neptune, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["pluto"];
        UpdateStar(pluto, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["moon"];
        UpdateStar(moon, obj.alt, obj.az, radius);
    }

    // ���ڸ� ��ġ ������Ʈ
    private void UpdateConstellations()
    {
        int i = 0;

        foreach (KeyValuePair<string, CelestialObject> kvp in dataManager.celestialObjects)
        {
            if (kvp.Value.type != "constellation")
            {
                continue;
            }

            Constellation obj = kvp.Value.ConvertTo<Constellation>();

            for (int j = 0; j < obj.stars.Count; j++)
            {
                Star star = obj.stars[j];
                UpdateStar(constellationObjects[i][j], star.alt, star.az, radius);
            }
            UpdateConstellationLine(constellationObjects[i], obj.lines);

            i++;
        }
    }

    // õü ��ġ ������Ʈ
    public void UpdateStar(GameObject star, float altitude, float azimuth, float distance)
    {
        float adjustedAzimuth = azimuth - trueNorth;
        Vector3 starPosition = SphericalToCartesian(altitude, adjustedAzimuth, distance);

        star.transform.position = starPosition;
    }

    // �¾�� õü ������Ʈ �ʱ�ȭ
    private void InitializeObjects()
    {
        sun = Instantiate(sunPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        sun.name = "Sun";
        mercury = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mercury.GetComponent<TextScript>().SetText("Mercury");
        mercury.name = "Mercury";
        venus = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        venus.GetComponent<TextScript>().SetText("Venus");
        venus.name = "Venus";
        mars = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mars.GetComponent<TextScript>().SetText("Mars");
        mars.name = "Mars";
        jupiter = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        jupiter.GetComponent<TextScript>().SetText("Jupiter");
        jupiter.name = "Jupiter";
        saturn = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        saturn.GetComponent<TextScript>().SetText("Saturn");
        saturn.name = "Saturn";
        uranus = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        uranus.GetComponent<TextScript>().SetText("Uranus");
        uranus.name = "Uranus";
        neptune = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        neptune.GetComponent<TextScript>().SetText("Neptune");
        neptune.name = "Neptune";
        pluto = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        pluto.GetComponent<TextScript>().SetText("Pluto");
        pluto.name = "Pluto";
        moon = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        moon.GetComponent<TextScript>().SetText("Moon");
        moon.name = "Moon";
    }

    // ���ڸ� ������Ʈ �ʱ�ȭ
    private void InitializeConstellations()
    {
        int i = 0;

        foreach (KeyValuePair<string, CelestialObject> kvp in dataManager.celestialObjects)
        {
            if (kvp.Value.type != "constellation")
            {
                continue;
            }

            Constellation obj = kvp.Value.ConvertTo<Constellation>();
            constellationObjects.Add(new List<GameObject>());

            foreach (Star star in obj.stars)
            {
                GameObject newStar = Instantiate(starPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                newStar.name = star.name;
                
                constellationObjects[i].Add(newStar);
            }
            InitializeConstellationLine(constellationObjects[i], obj.lines);

            i++;
        }
    }

    // ���ڸ� �� ������Ʈ
    private void UpdateConstellationLine(List<GameObject> starObjects, List<List<int>> lines)
    {
        foreach (List<int> newLine in lines)
        {
            List<int> tmp = newLine.ToList();

            int startIndex = tmp[0];
            LineRenderer line = starObjects[startIndex].GetComponent<LineRenderer>();
            if (line == null) continue;
            line.positionCount = newLine.Count;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            int i = 0;
            tmp.RemoveAt(0);

            foreach (int endIndex in tmp)
            {
                line.SetPosition(i, starObjects[startIndex].transform.position);
                line.SetPosition(i + 1, starObjects[endIndex].transform.position);

                i++;
                startIndex = endIndex;
            }
        }
    }

    // ���ڸ� �� ǥ���ϱ� ���� LineRenderer �ʱ�ȭ
    void InitializeConstellationLine(List<GameObject> starObjects, List<List<int>> lines)
    {
        foreach (List<int> newLine in lines)
        {
            int startIndex = newLine[0];
            LineRenderer line = starObjects[startIndex].AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.yellow;
            line.endColor = Color.yellow;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
        }
    }

    // ���� �������� ���� ��ǥ��� ��ȯ
    Vector3 SphericalToCartesian(float altitude, float azimuth, float radius)
    {
        float alt_radian = altitude * Mathf.Deg2Rad;
        float az_radian = azimuth * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(alt_radian) * Mathf.Sin(az_radian);
        float y = radius * Mathf.Sin(alt_radian);
        float z = radius * Mathf.Cos(alt_radian) * Mathf.Cos(az_radian);

        return new Vector3(x, y, z) + originObject.transform.position;
    }
}