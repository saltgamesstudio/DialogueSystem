using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace Salt.DialogueSystem.Editor
{
    public class BlackboardPrivider
    {
        private Blackboard blackboard;

        public BlackboardPrivider(Blackboard board)
        {
            blackboard = board;
        }

        private void CreateBlackBoard(GraphView graph)
        {
            var blackboard = new Blackboard(graph);
            blackboard.Add(new BlackboardSection { title = "Dialogue Properties" });
            blackboard.addItemRequested = BuildBlackboardMenu;

            blackboard.SetPosition(new Rect(10, 30, 200, 300));
            graph.Add(blackboard);

        }

        private void BuildBlackboardMenu(Blackboard board)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Background"), false, null, true);
            menu.ShowAsContext();
        }

        
    }
}
