using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Salt.DialogueSystem.Runtime {
    public class DialogueLogManager : MonoBehaviour
    {
        [SerializeField] private GameObject logPrefab;
        [SerializeField] private Transform container;

        public void AddLog(string name, string line)
        {
            GameObject goLog = Instantiate(logPrefab) as GameObject;
            goLog.GetComponent<DialogueLog>().setText(name, line);

            goLog.transform.SetParent(container);
        }
    }
}
