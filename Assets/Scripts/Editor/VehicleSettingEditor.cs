using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

public class VehicleSettingEditor : Editor
{
    //휠콜라이더 세부 설정을 툴에서 관리
    private static void SetupWheelCollider(WheelCollider collider)
    {
        collider.mass = 20f;
        collider.radius = 0.175f;
        collider.wheelDampingRate = 0.25f;
        collider.suspensionDistance = 0.05f;
        collider.forceAppPointDistance = 0f;

        JointSpring jointSpring = new JointSpring();
        jointSpring.spring = 70000f;
        jointSpring.damper = 3500f;
        jointSpring.targetPosition = 1f;
        collider.suspensionSpring = jointSpring;

        WheelFrictionCurve frictionCurve = new WheelFrictionCurve();
        frictionCurve.extremumSlip = 1f;
        frictionCurve.extremumValue = 1f;
        frictionCurve.asymptoteSlip = 1f;
        frictionCurve.asymptoteValue = 1f;
        frictionCurve.stiffness = 1f;
        collider.forwardFriction = frictionCurve;
        collider.sidewaysFriction = frictionCurve;
    }
    [MenuItem("Component/TrafficTool/Setup Vehicle")]
    private static void SetupVehicle()
    {
        EditorHelper.SetUndoGroup("Setup Vehicle");
        //현재 차를 선택했다면(아직 세팅되지 않은)
        GameObject selected = Selection.activeGameObject;

        //프리팹 언팩. 오리지널 프리팹을 끊고 내부 수정 가능.(자식 오브젝트 추가/삭제)
        PrefabUtility.UnpackPrefabInstance(selected, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        //레이캐스트 앵커 만들기
        GameObject anchor = EditorHelper.CreateGameObject("Raycast Anchor", selected.transform);
        anchor.transform.localPosition = new Vector3(0, 0.3f, 1);
        anchor.transform.localRotation = Quaternion.identity;
        //스크립트들 설정
        VehicleControl vehicleControl = EditorHelper.AddComponent<VehicleControl>(selected);
        vehicleControl.raycastAnchor = anchor.transform;
        //바퀴 메쉬 찾고
        Transform tireBackLeft = selected.transform.Find("Tire BackLeft");
        Transform tireBackRight = selected.transform.Find("Tire BackRight");
        Transform tireFrontLeft = selected.transform.Find("Tire FrontLeft");
        Transform tireFrontRight = selected.transform.Find("Tire FrontRight");
        //휠 콜라이더 세팅하고 바퀴를 휠 콜라이더의 차일드로 붙입니다.
        GameObject backLeftWheel = EditorHelper.CreateGameObject("TireBackLeft Wheel", selected.transform);
        backLeftWheel.transform.position = tireBackLeft.position;
        GameObject backRightWheel = EditorHelper.CreateGameObject("TireBackRight Wheel", selected.transform);
        backRightWheel.transform.position = tireBackRight.position;
        GameObject frontLeftWheel = EditorHelper.CreateGameObject("TireFrontLeft Wheel", selected.transform);
        frontLeftWheel.transform.position = tireFrontLeft.position;
        GameObject frontRightWheel = EditorHelper.CreateGameObject("TireFrontRight Wheel", selected.transform);
        frontRightWheel.transform.position = tireFrontRight.position;

        WheelCollider wheelCollider1 = EditorHelper.AddComponent<WheelCollider>(backLeftWheel);
        WheelCollider wheelCollider2 = EditorHelper.AddComponent<WheelCollider>(backRightWheel);
        WheelCollider wheelCollider3 = EditorHelper.AddComponent<WheelCollider>(frontLeftWheel);
        WheelCollider wheelCollider4 = EditorHelper.AddComponent<WheelCollider>(frontRightWheel);
        SetupWheelCollider(wheelCollider1);
        SetupWheelCollider(wheelCollider2);
        SetupWheelCollider(wheelCollider3);
        SetupWheelCollider(wheelCollider4);

        tireBackLeft.parent = backLeftWheel.transform;
        tireBackLeft.localPosition = Vector3.zero;
        tireBackRight.parent = backRightWheel.transform;
        tireBackRight.localPosition = Vector3.zero;
        tireFrontLeft.parent = frontLeftWheel.transform;
        tireFrontLeft.localPosition = Vector3.zero;
        tireFrontRight.parent = frontRightWheel.transform;
        tireFrontRight.localPosition = Vector3.zero;

        //WheelDrivecontrol 스크립트 붙이기
        WheelDriveControl wheelDriveControl = EditorHelper.AddComponent<WheelDriveControl>(selected);
        wheelDriveControl.Init();

        //Rigidbody 세팅
        Rigidbody rb = selected.GetComponent<Rigidbody>();
        rb.mass = 900f;
        rb.drag = 0.1f;
        rb.angularDrag = 3f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        //HeadQuarter 연결
        TrafficHeadquarter headquarter = FindObjectOfType<TrafficHeadquarter>();
        if(headquarter != null)
        {
            vehicleControl.trafficHeadquarter = headquarter;
        }

        //바디 콜라이더 붙이기
        BoxCollider boxCollider = EditorHelper.AddComponent<BoxCollider>(selected);
        boxCollider.isTrigger = true;

        GameObject colliders = EditorHelper.CreateGameObject("Colliders", selected.transform);
        colliders.transform.localPosition = Vector3.zero;
        colliders.transform.localRotation = Quaternion.identity;
        colliders.transform.localScale = Vector3.one;
        GameObject body = EditorHelper.CreateGameObject("Body", colliders.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localRotation = Quaternion.identity;
        body.transform.localScale = Vector3.one;
        BoxCollider bodyCollider = EditorHelper.AddComponent<BoxCollider>(body);
        bodyCollider.center = new Vector3(0f, 0.4f, 0f);
        bodyCollider.size = new Vector3(0.95f, 0.54f, 2.0f);

        // 레이어까지 자동 주행 레이어 AutonomousVehicle set
        //만약 레이어가 없다면 엔진에 추가
        EditorHelper.CreateLayer(TrafficHeadquarter.VehicleTagLayer);
        selected.tag = TrafficHeadquarter.VehicleTagLayer;
        EditorHelper.SetLayer(selected, LayerMask.NameToLayer(TrafficHeadquarter.VehicleTagLayer), true);
        //위에 세팅한 값을 한꺼번에 undo하도록
        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }
}