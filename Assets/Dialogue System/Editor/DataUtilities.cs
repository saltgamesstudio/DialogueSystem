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
        private DialogueContainer container = null;

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
            set { }
        }

        private static DataUtilities instance;

       
        public static DataUtilities GetInstance(DialogueGraph graph)
        {
            return new DataUtilities
            {
                graph = graph
            };
        }
        public void GetGraphData(DialogueContainer dialogueContainer)
        {
            if (Edges.Count == 0)
            {
                EditorUtility.DisplayDialog("No Dialogue Connection", "Make Sure You Have at Least 1 Connection Between Dialogues", "OK");
                return;
            }
            foreach (var edge in Edges)
            {
                if(edge.input != null)
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
                        foreach(var question in (node as ChoiceNode).QuestionDict)
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

        public void SaveGraph(string fileName)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            container = dialogueContainerObject;
            GetGraphData(container);
            AssetDatabase.CreateAsset(container, fileName);
            AssetDatabase.Refresh();
        }

        public void LoadGraph(DialogueContainer dialogueContainer)
        {
            GenerateNodeFromContainer(dialogueContainer);
            GenerateEdgeFromContainer(dialogueContainer);
        }
        private void GenerateNodeFromContainer(DialogueContainer dialogueContainer)
        {
            foreach (var node in dialogueContainer.Nodes)
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
                    DialogueNode temp = graph.CreateLineNode("Dialogue Node", node.TextDatas[0]); ;
                    temp.Guid = node.Guid;
                    temp.SetPosition(new Rect(node.Position, graph.nodeSize));
                    graph.AddElement(temp);
                }
            }
        }
        private void GenerateEdgeFromContainer(DialogueContainer dialogueContainer)
        {
            if (dialogueContainer.Edges.Count < 1) return;
            var entryNode = Nodes.Find(x => x.isEntryPoint);
            entryNode.Guid = dialogueContainer.Edges[0].Prev;
            LinkNodesTogether(entryNode.outputContainer.Q<Port>(), Nodes.Find(x => x.Guid == dialogueContainer.Edges[0].Next).Q<Port>());
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = dialogueContainer.Edges.Where(x => x.Prev == Nodes[k].Guid).ToList();
                Debug.Log("Connection : " + connections.Count);

                foreach (var c in connections)
                {
                    var prevNode = Nodes.Find(n => n.Guid == c.Prev);
                    List<Port> choicePorts = new List<Port>();
                    var nextNode = Nodes.Find(n => n.Guid == c.Next);
                    var outputPort = prevNode.outputContainer.Q<Port>();
                    var inputPort = nextNode.inputContainer.Q<Port>();
                    if (prevNode.isChoiceNode)
                    {
                        choicePorts = prevNode.outputContainer.Query<Port>().ToList();
                        foreach (var z in choicePorts)
                        {
                            Debug.Log(choicePorts[0].portName);
                            LinkNodesTogether(z, inputPort);
                        }
                        continue;
                    }
                    LinkNodesTogether(outputPort, inputPort);
                    k++;
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
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

