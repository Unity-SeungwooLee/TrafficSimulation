using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficWaypoint : MonoBehaviour
{
    public TrafficSegment segment;

    //웨이포인트 중간에 있을 수 있는 콜라이더와 차량의 충돌 방지
    public void RemoveCollider()
    {
        if (GetComponent<SphereCollider>())
        {
            Debug.Log("Remove Collider");
            DestroyImmediate(gameObject.GetComponent<SphereCollider>());
        }
    }
    public void Refresh(int newID, TrafficSegment newSegment)
    {
        segment = newSegment;
        //WayPoint-1, WayPoint-10
        name = "WayPoint-" + newID.ToString();
        tag = "Waypoint";
        gameObject.layer = LayerMask.NameToLayer("Dafault");
        RemoveCollider();
    }
    public Vector3 GetVisualPos()
    {
        return transform.position + new Vector3(0.0f, 0.5f, 0.0f);
    }
}