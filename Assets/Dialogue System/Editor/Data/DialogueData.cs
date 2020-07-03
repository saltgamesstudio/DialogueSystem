using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Salt.DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "Better Dialogue", menuName = "Salt Studio/New Dialogue")]
    [System.Serializable]
    public class DialogueData : ScriptableObject
    {
        public List<NodeData> Nodes = new List<NodeData>();

    }
    [System.Serializable]
    public class NodeData
    {
        public string Guid;
        public string JsonData;
        public Character Character;
        public string Text;
        public string Next;
        public bool isChoiceNode;
        public bool isEntryPoint;
        public List<ChoiceData> Choices = new List<ChoiceData>();
    }
    [System.Serializable]
    public class ChoiceData
    {
        public string Next;
        public string Question;
    }

}
