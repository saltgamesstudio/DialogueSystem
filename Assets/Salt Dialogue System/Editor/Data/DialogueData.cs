using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Salt.DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "Better Dialogue", menuName = "Salt Studio/New Dialogue")]
    [System.Serializable]
    public class DialogueData : ScriptableObject
    {
        public List<NodeData> Nodes = new List<NodeData>();
        public List<AudioClip> SoundFx = new List<AudioClip>();
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
        public DialogueProperties Properties;
    }
    [System.Serializable]
    public class ChoiceData
    {
        public string Next;
        public string Question;
    }

    [Serializable]
    public class DialogueProperties
    {
        public Sprite Background;
        public List<Character> CharacterList = new List<Character>();
        public string Text;
    }
    public enum CharacterScreenPosition
    {
        Left, Middle, Right
    }
    public enum NodeType
    {
        Choice,
        DialogueLine,
        Start,
        End
    }
}
