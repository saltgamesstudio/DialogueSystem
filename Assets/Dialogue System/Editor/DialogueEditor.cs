using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Salt.DialogueSystem.Runtime;
using UnityEditor.VersionControl;

namespace Salt.DialogueSystem.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private static string path = "New Dialogues";
        private static DialogueContainer container;
        private static DialogueGraph graph;


        [OnOpenAssetAttribute(1)]
        public static bool OpenAssetCallback1(int instanceID, int line)
        {
            path = AssetDatabase.GetAssetPath(instanceID);
            container = AssetDatabase.LoadAssetAtPath<DialogueContainer>(path);
            if (container != null)
            {
                Debug.Log("Contol");
                OpenWindow();
                
                // path = AssetDatabase.GetAssetPath(container) ;
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

            var addDilogueNodeButton = new Button(() => {
                graph.AddDialogueNode("Dialogue Node");
            });

            addDilogueNodeButton.text = "New Dialogue Node";

            var createChoiceNodeButton = new Button(() => {
                graph.AddChoiceNode("Choice Node");
            });
            createChoiceNodeButton.text = "New Choice Node";

      

            var saveButton = new Button(() => {


                var saveUtility = DataUtilities.GetInstance(graph);
                saveUtility.SaveGraph(path);
                


            });
            saveButton.text = "Save";


            toolbar.Add(addDilogueNodeButton);
            toolbar.Add(createChoiceNodeButton);
            toolbar.Add(saveButton);

            rootVisualElement.Add(toolbar);
        }
       

        private void OnDisable()
        {
            rootVisualElement.Remove(graph);
        }


    }

}
