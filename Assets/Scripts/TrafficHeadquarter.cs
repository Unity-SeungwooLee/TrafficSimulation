using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficHeadquarter : MonoBehaviour
{
    //세그먼트와 세그먼트 사이의 검출 간격
    public float segDetectThresh = 0.1f;
    //웨이포인트의 크기
    public float waypointSize = 0.5f;
    //충돌 레이어들
    public string[] collisionLayers;

    public List<TrafficSegment> segments = new List<TrafficSegment>();
    public TrafficSegment curSegment;

    public const string VehicleTagLayer = "AutonomousVehicle";
    //교차로 어디있는지
    public List<TrafficIntersection> intersections = new List<TrafficIntersection>();

    //에디터용 기즈모 속성들. HQ에서 조절
    public enum ArrowDraw
    {
        FixedCount,
        ByLength,
        Off
    }
    //기즈모에 그릴 화살표 속성
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
    //데이터 로딩 속성들
    public class EmergencyData
    {
        public int ID = -1;
        public bool IsEmergency = false;
        public EmergencyData(string id, string emergency)
        {
            ID = int.Parse(id);
            IsEmergency = emergency.Contains("1"); //1이라는 글자만 있으면 emergency로
        }
    }
    public class TrafficData
    {
        public List<EmergencyData> datas = new List<EmergencyData>();
    }
    //data 출력한 UI라벨
    public TMPro.TextMeshProUGUI stateLabel;
    //구글 스프레드 시트 읽어올 로더
    public SpreadSheetLoader dataLoader;
    //읽어온 데이터 클래스
    private TrafficData trafficData;

    private void Start()
    {
        dataLoader = GetComponentInChildren<SpreadSheetLoader>();
        stateLabel = GameObject.FindWithTag("TrafficLabel").GetComponent<TMPro.TextMeshProUGUI>();
        //일정 주기로 데이터 로딩을 시킬예정, 텀이 짧으면 URL이 막힌다.
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
        //data를 class에 담는다.
        trafficData = new TrafficData();
        string[] AllRow = loadedData.Split('\n'); //줄바꿈
        foreach(string onerow in AllRow)
        {
            string[] datas = onerow.Split("\t"); //탭으로 쪼개기
            EmergencyData data = new EmergencyData(datas[0], datas[1]);
            trafficData.datas.Add(data);
        }
        //TrafficData.datas = dataLoader.GetDatas<EmergencyData>(loadedData);
        //data 검사. 응급상황 발생시 세팅
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