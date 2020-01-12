using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public GameObject logPrefab;
    public Transform container;

    public void AddLog(string name, string line)
    {
        GameObject goLog = Instantiate(logPrefab) as GameObject;
        goLog.GetComponent<Log>().setText(name, line);

        goLog.transform.SetParent(container);
    }
}
