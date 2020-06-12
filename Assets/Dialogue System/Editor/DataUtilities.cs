using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Salt.DialogueSystem.Runtime;
using Salt.DialogueSystem.Data;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;

namespace Salt.DialogueSystem.Editor
{
    public class DataUtilities
    {
        private DialogueGraph graph;
        private DialogueContainer container;

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

        private static DataUtilities instance;


        public static DataUtilities GetInstance(DialogueGraph graph, DialogueContainer dialogueContainer)
        {
            return new DataUtilities
            {
                graph = graph,
                container = dialogueContainer
            };
        }
        public void GetGraphData(DialogueContainer dialogueContainer)
        {
            //if (Edges.Count == 0)
            //{
            //    EditorUtility.DisplayDialog("No Dialogue Connection", "Make Sure You Have at Least 1 Connection Between Dialogues", "OK");
            //    return;
            //}
            foreach (var edge in Edges)
            {
                if (edge.input != null)
                {
                    dialogueContainer.Edges.Add(new EdgeData
                    {
                        Prev = (edge.output.node as CustomNode).Guid,
                        Next = (edge.input.node as CustomNode).Guid,
                    });
                }
            }
            foreach (var node in Nodes)
            {
                if (!node.isEntryPoint)
                {
                    var data = new NodeData
                    {
                        Guid = node.Guid,
                        Position = node.GetPosition().position
                    };
                    if (node is DialogueNode)
                    {
                        data.TextDatas.Add((node as DialogueNode).Text);
                    }
                    else
                    {
                        List<string> temp = new List<string>();
                        foreach (var question in (node as ChoiceNode).QuestionDict)
                        {
                            temp.Add(question.Value);
                        }
                        data.TextDatas = temp;
                        data.isChoiceNode = true;
                    }
                    dialogueContainer.Nodes.Add(data);
                }
            }
        }
        public void SaveGraph(string path)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            container = dialogueContainerObject;
            GetGraphData(container);
            if (!AssetDatabase.IsValidFolder("Assets/Salt Dialogues"))
            {
                AssetDatabase.CreateFolder("Assets", "Salt Dialogues");
            }
            AssetDatabase.CreateAsset(container, path);
            AssetDatabase.Refresh();
        }
        public void LoadGraph()
        {
           // ClearGraph();
            GenerateNodeFromContainer();
            GenerateEdgeFromContainer();
        }
        private void GenerateNodeFromContainer()
        {
            foreach (var node in container.Nodes)
            {
                if (node.isChoiceNode)
                {
                    ChoiceNode temp = graph.CreateChoiceNode("Choice Node");
                    temp.Guid = node.Guid;
                    foreach (var question in node.TextDatas)
                    {
                        graph.AddChoicePort(temp as ChoiceNode, question);
                    }
                    temp.SetPosition(new Rect(node.Position, graph.nodeSize));
                    graph.AddElement(temp);
                }
                else
                {
                    DialogueNode temp = graph.CreateDialogueNode("Dialogue Node", node.TextDatas[0], node.Character); ;
                    temp.Guid = node.Guid;
                    temp.SetPosition(new Rect(node.Position, graph.nodeSize));
                    graph.AddElement(temp);
                }
            }
        }
     
        private void GenerateEdgeFromContainer()
        {
            var entryNode = Nodes.Find(x => x.isEntryPoint);
            entryNode.Guid = container.Edges[0].Prev;
            for (var i = 0; i < Nodes.Count; i++)
            {
                var connections = container.Edges.Where(x => x.Prev == Nodes[i].Guid).ToList();
                foreach (var c in connections)
                {
                    List<Port> choicePorts = new List<Port>();
                    var prevNode = Nodes.Find(n => n.Guid == c.Prev);
                    var nextNode = Nodes.First(n => n.Guid == c.Next);
                    var outputPort = prevNode.outputContainer.Q<Port>();
                    var inputPort = nextNode.inputContainer.Q<Port>();
                    //if (prevNode.isChoiceNode)
                    //{
                    //    choicePorts = prevNode.outputContainer.Query<Port>().ToList();
                    //    Debug.Log(outputPort.portName);
                    //    foreach (var p in choicePorts)
                    //    {
                    //        ConnectNodes(p, inputPort);
                            
                    //    }
                    //    continue;
                    //}
                    ConnectNodes(outputPort, inputPort);
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
