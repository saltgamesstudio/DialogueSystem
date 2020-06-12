using Salt.DialogueSystem.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Salt.DialogueSystem.Editor
{
    [Serializable]
    public class DialogueNode : CustomNode
    {
        public Character Character;
        public string Text;
    }


}
