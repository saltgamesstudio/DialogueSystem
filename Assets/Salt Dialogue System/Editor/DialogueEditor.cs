using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.EditorTools;
using UnityEditor.ProjectWindowCallback;
using System.Runtime.CompilerServices;
using Salt.DialogueSystem.Data;

namespace Salt.DialogueSystem.Editor
{
    public class DialogueEditor : GraphViewEditorWindow
    {
        //private static string path = "Assets/Salt Dialogue/New Dialogues";
        private static string path = "";
        private static DialogueData container;
        private static DialogueGraph graph;

        

        [OnOpenAssetAttribute(1)]
        public static bool OpenAssetCallback1(int instanceID, int line)
        {
          
            path = AssetDatabase.GetAssetPath(instanceID);
            container = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
            if (container != null)
            {
                OpenWindow();
                if (container is DialogueData)
                {
                    DataUtility.GetInstance(graph, container).LoadGraph();
                }
                return true;
            }
            return false;
        }
     
        [MenuItem("Salt Studio/Tools/Dialogue Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Salt Dialogue Editor");
        }

        private void OnEnable()
        {
            CreateGraph();
            CreateBlackBoard();
        }

        private void CreateGraph()
        {
            graph = new DialogueGraph()
            {
                name = "Dialogue Graph"
            };
            graph.StretchToParentSize();
            rootVisualElement.Add(graph);
            
            var toolbar = new Toolbar();
            

            var addDilogueNodeButton = new Button(() =>
            {
                graph.AddElement(DialogueNode.Create("Dialogue Node", new DialogueProperties())) ;
            })
            {
                text = "New Dialogue Node"
            };

            var createChoiceNodeButton = new Button(() =>
            {
                graph.AddElement(ChoiceNode.Create("Choice Node"));
            })
            {
                text = "New Choice Node"
            };

            var saveButton = new Button(() =>
            {
                var saveUtility = DataUtility.GetInstance(graph, container);
                saveUtility.SaveGraph(path);
            })
            {
                text = "Save"
            };

            toolbar.Add(addDilogueNodeButton);
            toolbar.Add(createChoiceNodeButton);
            toolbar.Add(saveButton);

            
            
            rootVisualElement.Add(toolbar);
        }

       

        private void CreateBlackBoard()
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
            menu.AddItem(new GUIContent("Background"), false, null);
            menu.ShowAsContext();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graph);
        }

        
    }

}
