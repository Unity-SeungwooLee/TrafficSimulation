using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TrafficHQInspectorEditor
{
    public static void DrawInspector(TrafficHeadquarter trafficHeadquarter, SerializedObject serializedObject, out bool restructureSystem)
    {
        //����� ����
        InspectorHelper.Header("����� ����");
        InspectorHelper.Toggle("����� ������?", ref trafficHeadquarter.hideGizmos);
        //ȭ��ǥ ����
        InspectorHelper.DrawArrowTypeSelection(trafficHeadquarter);
        InspectorHelper.FloatField("��������Ʈ ũ��", ref trafficHeadquarter.waypointSize);
        EditorGUILayout.Space();
        //�ý��� ����
        InspectorHelper.Header("�ý��� ����");
        InspectorHelper.FloatField("���� ���� �ּ� �Ÿ�", ref trafficHeadquarter.segDetectThresh);
        InspectorHelper.PropertyField("�浹 ���̾��, ", "collisionLayers", serializedObject);
        EditorGUILayout.Space();
        //����
        InspectorHelper.HelpBox("Ctrl + ���콺 ���� : ���׸�Ʈ ���� \n " + "Shift + ���콺 ���� : ��������Ʈ ���� \n" + "Alt + ���콺 ���� : ������ ����");
        InspectorHelper.HelpBox("������ �߰��Ѵ�� ��������Ʈ�� ���� �̵��ϰ� �˴ϴ�.");

        EditorGUILayout.Space();
        restructureSystem = InspectorHelper.Button("���� �ùķ��̼� �ý��� �籸��");
    }
}
