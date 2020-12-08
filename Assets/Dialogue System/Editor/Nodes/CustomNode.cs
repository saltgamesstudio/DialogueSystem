using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Salt.DialogueSystem.Editor
{
    [Serializable]
    public class CustomNode : Node
    {
        public string Guid;
        public bool isChoiceNode = false;
        public bool isEntryPoint = false;
        public static readonly Vector2 nodeSize = new Vector2(150, 200);

    }

}

