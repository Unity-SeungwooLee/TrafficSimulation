using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Reflection;

public class SpreadSheetLoader : MonoBehaviour
{
    //구글 스프레드 시트를 TSV 형식으로 읽어올 수 있도록 주소를 만든다.
    public static string GetSheetDataAddress(string address, string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

    public readonly string ADDRESS = "https://docs.google.com/spreadsheets/d/1CeR3NISoN_HY_hbqCdl_q7mHdx2YvoFodcAdZ4xwySc";
    public readonly string RANGE = "A2:B15";
    public readonly long SHEET_ID = 0;
    //읽어온 스트링 데이터를 임시 저장 합니다.
    private string loadString = string.Empty;
    //구글 스프레드 시트의 TSV 얻는 주소를 이용해 데이터를 읽어옵니다.
    private IEnumerator LoadData(Action<string> onMessageReceived)
    {
        //구글 데이터 로딩 시작
        UnityWebRequest www = UnityWebRequest.Get(GetSheetDataAddress(ADDRESS, RANGE, SHEET_ID));
        yield return www.SendWebRequest();
        //데이터 로딩 완료
        Debug.Log(www.downloadHandler.text);
        if (onMessageReceived != null)
        {
            onMessageReceived(www.downloadHandler.text);
        }
        yield return null;
    }
    //
    public string StartLoader()
    {
        StartCoroutine(LoadData(output => loadString = output)); //람다식

        return loadString;
    }

    T GetData<T>(string[] datas, string childType = "")
    {
        object data;
        if (string.IsNullOrEmpty(childType) || Type.GetType(childType) == null)
        {
            data = Activator.CreateInstance(typeof(T));
        }
        else
        {
            data = Activator.CreateInstance(Type.GetType(childType));
        }

        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        for(int i = 0; i< datas.Length; i++)
        {
            try
            {
                Type type = fieldInfos[i].FieldType;

                if (string.IsNullOrEmpty(datas[i]))
                {
                    continue;
                }
                if(type == typeof(int))
                {
                    fieldInfos[i].SetValue(data, int.Parse(datas[i]));
                }
                else if(type == typeof(float))
                {
                    fieldInfos[i].SetValue(data, float.Parse(datas[i]));
                }
                else if(type == typeof(bool))
                {
                    fieldInfos[i].SetValue(data, bool.Parse(datas[i]));
                }
                else if(type == typeof(string)) //스트링은 맨 마지막에
                {
                    fieldInfos[i].SetValue(data, datas[i]);
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"SpreadSheet Load Error : {e.Message}");
            } 
        }
        return (T)data;
    }
    public List<T> GetDatas<T>(string data)
    {
        List<T> returnList = new List<T>();
        string[] splitedData = data.Split('\n');
        foreach(string element in splitedData)
        {
            string[] datas = element.Split('\t');
            returnList.Add(GetData<T>(datas));
        }
        return returnList;
    }
}