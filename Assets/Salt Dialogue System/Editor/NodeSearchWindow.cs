using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;


namespace Salt.DialogueSystem.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private GraphView graphView;
        public void CreateSearchWindow(GraphView graphView)
        {
            this.graphView = graphView;
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry> {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeEntry(new GUIContent("Dialogue Node"))
                {
                    userData = new DialogueNode(), level = 1
                },
                new SearchTreeEntry(new GUIContent("Choice Node"))
                {
                    userData = new ChoiceNode(), level = 1
                }
            };
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            return true;
        }
    }
}
