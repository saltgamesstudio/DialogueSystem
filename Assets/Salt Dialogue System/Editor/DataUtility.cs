using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Salt.DialogueSystem.Data;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.Analytics;

namespace Salt.DialogueSystem.Editor
{
    public class DataUtility
    {
        private DialogueGraph graph;
        private DialogueData dataContainer = null;

        private List<Edge> Edges => graph.edges.ToList();
        private List<CustomNode> Nodes
        {
            get
            {
                List<CustomNode> nodes = new List<CustomNode>();
                List<Node> list = graph.nodes.ToList();
                list.ForEach((node) =>
                {
                    nodes.Add(node as CustomNode);
                });
                return nodes;
            }
            set
            {

            }
        }


        public static DataUtility GetInstance(DialogueGraph graph, DialogueData data)
        {
            return new DataUtility
            {
                graph = graph,
                dataContainer = data
             
            };
        }

        public void GetGraphData()
        {
            if (Edges.Count == 0)
            {
                EditorUtility.DisplayDialog("No Dialogue Connection", "Make Sure You Have at Least 1 Connection Between Dialogues", "OK");
                return;
            }
            foreach (var node in Nodes)
            {
                var json = JsonUtility.ToJson(node.GetPosition().position);
                if (!node.isEntryPoint)
                {
                    var data = new NodeData
                    {
                        Guid = node.Guid,
                        JsonData = json,
                        Next = (node.outputContainer.Q<Port>() as DialoguePort).Next
                    };
                    if (node is DialogueNode)
                    {

                        //New Dialogue Properties System
                        var properties = new DialogueProperties
                        {
                            Text = (node as DialogueNode).Properties.Text,
                            Background = (node as DialogueNode).Properties.Background,
                            CharacterList = (node as DialogueNode).Properties.CharacterList

                        };
                        data.Properties = properties;

                    }
                    else
                    {
                        var listPort = node.outputContainer.Query<Port>().ToList();
                        foreach (var port in listPort)
                        {
                            var outputPort = port as DialoguePort;
                            ChoiceData tmpChoice = new ChoiceData
                            {
                                Next = outputPort.Next,
                                Question = outputPort.Question
                            };
                            data.Choices.Add(tmpChoice);
                        }
                        
                        data.isChoiceNode = true;
                    }

                    dataContainer.Nodes.Add(data);
                }
                else if(node.isEntryPoint)
                {
                    var tmp = new NodeData
                    {
                        Guid = node.Guid,
                        isEntryPoint = true,
                        Next = (node.outputContainer.Q<Port>() as DialoguePort).Next
                    };
                    dataContainer.Nodes.Add(tmp);
                }
            }

        }
        public void SaveGraph(string path)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueData>();
            dataContainer = dialogueContainerObject;
            GetGraphData();
            if (!AssetDatabase.IsValidFolder("Assets/Salt Dialogues"))
            {
                AssetDatabase.CreateFolder("Assets", "Salt Dialogues");
            }
            AssetDatabase.CreateAsset(dataContainer, path);
            AssetDatabase.Refresh();
            Debug.Log("<color=blue>Dialogue Saved</color>");
        }
        public void LoadGraph()
        {
            ClearGraph();
            GenerateNodeFromContainer(dataContainer);
            GenerateEdgeFromContainer(dataContainer);
        }
        private void GenerateNodeFromContainer(DialogueData dialogueContainer)
        {
            foreach (var node in dialogueContainer.Nodes)
            {
                if (node.isEntryPoint)
                {
                    var entryPoint = Nodes.Find(n => n.isEntryPoint);
                    entryPoint.Guid = node.Guid;
                    (entryPoint.outputContainer.Q<Port>() as DialoguePort).Next = node.Next;
                    continue;
                }
                if (node.isChoiceNode)
                {
                    ChoiceNode temp = ChoiceNode.Create("Choice Node");
                    temp.Guid = node.Guid;
                    foreach (var choice in node.Choices)
                    {
                        ChoiceNode.AddChoicePort(temp, choice.Question, choice.Next);
                    }
                    temp.SetPosition(new Rect(JsonUtility.FromJson<Vector2>(node.JsonData), graph.nodeSize));
                    graph.AddElement(temp);
                }
                else
                {
                    var properties = new DialogueProperties
                    {
                        Text = node.Properties.Text,
                        Background = node.Properties.Background,
                        CharacterList = node.Properties.CharacterList
                    };
                    DialogueNode temp = DialogueNode.Create("Dialogue Node", properties, node.Next);
                    temp.Guid = node.Guid;
                    temp.SetPosition(new Rect(JsonUtility.FromJson<Vector2>(node.JsonData), graph.nodeSize));
                    graph.AddElement(temp);
                   

                   
                }
            }
        }
        private void GenerateEdgeFromContainer(DialogueData dialogueContainer)
        {
            if (dialogueContainer.Nodes.Count < 1) return;
            
            foreach (var node in dialogueContainer.Nodes)
            {
                var currentNode = Nodes.Find(n => n.Guid == node.Guid);
                if(string.IsNullOrEmpty(node.Next) )
                {
                    continue;
                }
                var nextNode = Nodes.Find(n => n.Guid == node.Next);
                var outputPort = currentNode.outputContainer.Q<Port>();
                var inputPort = nextNode.inputContainer.Q<Port>();

                int choiceCounter = 0;
                if (currentNode.isChoiceNode)
                {
                    foreach(var choice in node.Choices)
                    {
                        var listPort = currentNode.outputContainer.Query<Port>().ToList();
                        outputPort = listPort[choiceCounter];
                        inputPort = Nodes.Find(n => n.Guid == choice.Next).inputContainer.Q<Port>();
                        choiceCounter++;
                        ConnectNodes(outputPort, inputPort);
                    }
                }
                else
                {

                    choiceCounter = 0;
                    ConnectNodes(outputPort, inputPort);
                }
            
            }

        }

        private void ClearGraph()
        {
            if (Nodes.Any() && Nodes.Count > 1)
            {
                Nodes.Find(x => x.isEntryPoint).Guid = dataContainer.Nodes.Find(x => x.isEntryPoint).Guid;
                foreach (var n in Nodes)
                {
                    if (n.isEntryPoint) continue;
                    Edges.Where(x => x.input.node == n).ToList().ForEach(edge => graph.RemoveElement(edge));
                    graph.RemoveElement(n);
                }

            }
        }



        private void ConnectNodes(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            graph.Add(tempEdge);
        }




    }

}

