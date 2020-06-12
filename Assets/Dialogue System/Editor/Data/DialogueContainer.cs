using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Salt.DialogueSystem.Data;
using System;

namespace Salt.DialogueSystem.Runtime
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Salt Studio/Dialogue")]
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeData> Nodes = new List<NodeData>();
        public List<EdgeData> Edges = new List<EdgeData>();
    }

}
