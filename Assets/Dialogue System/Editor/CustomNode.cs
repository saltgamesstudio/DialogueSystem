using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;


namespace Salt.DialogueSystem.Editor
{
    [Serializable]
    public class CustomNode : Node
    {
        public string Guid;
        public bool isChoiceNode = false;
        public bool isEntryPoint = false;
    }

}

