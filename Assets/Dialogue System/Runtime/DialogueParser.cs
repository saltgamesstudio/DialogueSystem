using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Salt.DialogueSystem.Data;
namespace Salt.DialogueSystem.Runtime
{
    public class DialogueParser
    {
        [SerializeField] private DialogueData dialogue;
        public DialogueParser(DialogueData data)
        {
            dialogue = data;
        }
        /// <summary>
        /// Get entry point node
        /// </summary>
        public NodeData EntryPoint => dialogue.Nodes.Find(x => x.isEntryPoint);

        /// <summary>
        /// Get node by Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public NodeData GetNode(string guid) => dialogue.Nodes.Find(x => x.Guid == guid);

        /// <summary>
        /// Get Next node
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public NodeData GetNextNode(string current) => GetNode(GetNode(current).Next);

        public NodeData GetNextNode(NodeData current) => GetNode(current.Next);

        /// <summary>
        /// Return true if node is a choice node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsChoiceNode(NodeData node) => node.isChoiceNode;

        public bool IsEndNode(NodeData node)
        {
            if (IsChoiceNode(node) || !string.IsNullOrEmpty(node.Next)) return false;
            return true;

        }



    }

}
