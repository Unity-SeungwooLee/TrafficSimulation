using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficHeadquarter : MonoBehaviour
{
    //���׸�Ʈ�� ���׸�Ʈ ������ ���� ����
    public float segDetectThresh = 0.1f;
    //��������Ʈ�� ũ��
    public float waypointSize = 0.5f;
    //�浹 ���̾��
    public string[] collisionLayers;

    public List<TrafficSegment> segments = new List<TrafficSegment>();
    public TrafficSegment curSegment;

    public const string VehicleTagLayer = "AutonomousVehicle";
    //������ ����ִ���
    public List<TrafficIntersection> intersections = new List<TrafficIntersection>();

    //�����Ϳ� ����� �Ӽ���. HQ���� ����
    public enum ArrowDraw
    {
        FixedCount,
        ByLength,
        Off
    }
    //����� �׸� ȭ��ǥ �Ӽ�
    public bool hideGizmos = false;
    public ArrowDraw arrowDrawType = ArrowDraw.ByLength;
    public int arrowCount = 1;
    public float arrowDistance = 5f;
    public float arrowSizeWaypoint = 1;
    public float arrowSizeIntersection = 0.5f;

    public List<TrafficWaypoint> GetAllWaypoints()
    {
        List<TrafficWaypoint> Waypoints = new List<TrafficWaypoint>();
        foreach (var segment in segments)
        {
            Waypoints.AddRange(segment.Waypoints);
        }
        return Waypoints;
    }
    [Serializable]
    //������ �ε� �Ӽ���
    public class EmergencyData
    {
        public int ID = -1;
        public bool IsEmergency = false;
        public EmergencyData(string id, string emergency)
        {
            ID = int.Parse(id);
            IsEmergency = emergency.Contains("1"); //1�̶�� ���ڸ� ������ emergency��
        }
    }
    public class TrafficData
    {
        public List<EmergencyData> datas = new List<EmergencyData>();
    }
    //data ����� UI��
    public TMPro.TextMeshProUGUI stateLabel;
    //���� �������� ��Ʈ �о�� �δ�
    public SpreadSheetLoader dataLoader;
    //�о�� ������ Ŭ����
    private TrafficData trafficData;

    private void Start()
    {
        dataLoader = GetComponentInChildren<SpreadSheetLoader>();
        stateLabel = GameObject.FindWithTag("TrafficLabel").GetComponent<TMPro.TextMeshProUGUI>();
        //���� �ֱ�� ������ �ε��� ��ų����, ���� ª���� URL�� ������.
        InvokeRepeating("CallLoaderAndCheck", 5f, 5f);
    }
    private void CallLoaderAndCheck()
    {
        string loadedData = dataLoader.StartLoader();
        stateLabel.text = "Traffic Status \n " + loadedData;
        if (string.IsNullOrEmpty(loadedData))
        {
            return;
        }
        //data�� class�� ��´�.
        trafficData = new TrafficData();
        string[] AllRow = loadedData.Split('\n'); //�ٹٲ�
        foreach(string onerow in AllRow)
        {
            string[] datas = onerow.Split("\t"); //������ �ɰ���
            EmergencyData data = new EmergencyData(datas[0], datas[1]);
            trafficData.datas.Add(data);
        }
        //TrafficData.datas = dataLoader.GetDatas<EmergencyData>(loadedData);
        //data �˻�. ���޻�Ȳ �߻��� ����
        CheckData();
    }

    private void CheckData()
    {
        for(int i = 0; i < trafficData.datas.Count; i++)
        {
            EmergencyData data = trafficData.datas[i];
            if(intersections.Count <= 1 || intersections[i] == null)
            {
                return;
            }
            if(data.IsEmergency)
            {
                intersections[data.ID].IntersectionType = IntersectionType.EMERGENCY;
            }
            else
            {
                intersections[data.ID].IntersectionType = IntersectionType.TRAFFIC_LIGHT;
            }
        }
    }
}