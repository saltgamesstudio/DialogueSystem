using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Salt.DialogueSystem.Data
{
    [Serializable]
    public class NodeData 
    {
        public string Guid;
        public Vector2 Position;
        public Character Character;
        public bool isChoiceNode = false;
        public List<string> TextDatas = new List<string>();
    }

}