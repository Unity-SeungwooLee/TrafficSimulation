using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSegment : MonoBehaviour
{
    //다음에 이동할 segment들
    public List<TrafficSegment> nextSegments = new List<TrafficSegment>();
    //이 세그먼트의 ID 값
    public int ID = -1;
    //구간이 갖고있는 웨이포인트들 시작 -> 끝점 등 2~3개를 보통 가지고 있다.
    public List<TrafficWaypoint> Waypoints = new List<TrafficWaypoint>();

    public bool IsOnSegment(Vector3 pos)
    {
        TrafficHeadquarter trafficHeadquarter = GetComponentInParent<TrafficHeadquarter>(); //부모것 가져오기
        
        for(int i = 0; i < Waypoints.Count - 1; i++)
        {
            Vector3 pos1 = Waypoints[i].transform.position;
            Vector3 pos2 = Waypoints[i + 1].transform.position;
            //첫 번째 웨이포인트와 차량의 거리
            float d1 = Vector3.Distance(pos1, pos);
            //두 번째 웨이포인트와 차량의 거리
            float d2 = Vector3.Distance(pos2, pos);
            //첫 번째 웨이포인트와 두 번째 웨이포인트의 거리
            float d3 = Vector3.Distance(pos1, pos2);

            float diff = (d1 + d2) - d3;
            //값 사이에 있다면
            if(diff < trafficHeadquarter.segDetectThresh && diff > -trafficHeadquarter.segDetectThresh)
            {
                //자동차가 두 웨이포인트 사이에 가까이 있다.
                return true;
            }
        }
        //자동차가 두 웨이포인트 사이에서 멀리 있다. for문 끝나고 false
        return false;
    }
}
