using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Salt.DialogueSystem.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.EditorTools;
using UnityEditor.ProjectWindowCallback;
using System.Runtime.CompilerServices;

namespace Salt.DialogueSystem.Editor
{
    public class DialogueEditor : GraphViewEditorWindow
    {
        private static string path = "Assets/Salt Dialogue/New Dialogues";
        private static DialogueContainer container;
        private static DialogueGraph graph;

        

        [OnOpenAssetAttribute(1)]
        public static bool OpenAssetCallback1(int instanceID, int line)
        {
          
            path = AssetDatabase.GetAssetPath(instanceID);
            container = AssetDatabase.LoadAssetAtPath<DialogueContainer>(path);
            if (container != null)
            {
                OpenWindow();
                if (container is DialogueContainer)
                {
                    DataUtilities.GetInstance(graph).LoadGraph(container);
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
                graph.AddDialogueNode("Dialogue Node");
            })
            {
                text = "New Dialogue Node"
            };

            var createChoiceNodeButton = new Button(() =>
            {
                graph.AddChoiceNode("Choice Node");
            })
            {
                text = "New Choice Node"
            };

            var saveButton = new Button(() =>
            {
                var saveUtility = DataUtilities.GetInstance(graph);
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

       

        private void GenerateBlackBoard()
        {
            var blackboard = new Blackboard(graph);
            blackboard.Add(new BlackboardSection { title = "Exposed Variables" });
            blackboard.addItemRequested = board =>
            {
                
            };
            
            blackboard.SetPosition(new Rect(10, 30, 200, 300));
            graph.Add(blackboard);
            //graph.Blackboard = blackboard;

        }
        private void OnDisable()
        {
            rootVisualElement.Remove(graph);
        }

        
    }

}
