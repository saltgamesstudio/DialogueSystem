using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Salt.DialogueSystem.Data;

namespace Salt.DialogueSystem.Editor
{
    public class DialogueGraph : GraphView
    {
        public readonly Vector2 nodeSize = new Vector2(150,200);
        public delegate void Logger(string a);
        private Logger LogToConsole;
        private ContextMenu menu;
        
        public DialogueGraph()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("EditorStyle"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
            AddElement(GenerateEntryPoint());
            var grid = new GridBackground();
            Insert(0, grid);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Create Dialogue Node", delegate (DropdownMenuAction a)
                {
                        
                    var node = CreateLineNode("Dialogue Node", string.Empty);
                    var nodePos = this.contentViewContainer.WorldToLocal(a.eventInfo.mousePosition);
                    node.SetPosition(new Rect(nodePos, nodeSize));
                    AddElement(node);
                    Debug.Log(nodePos);
                    
                    
                });
            }
            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Create Choice Node", delegate (DropdownMenuAction a)
                {
                    var node = CreateChoiceNode("Choice Node");
                    var nodePos = this.contentViewContainer.WorldToLocal(a.eventInfo.mousePosition);
                    node.SetPosition(new Rect(nodePos, nodeSize));
                    AddElement(node);
                    Debug.Log(nodePos);
                });
            }
            base.BuildContextualMenu(evt);
        }

        private Port CreatePort(DialogueNode node, Direction direction, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal,direction, capacity, typeof(string)) ;
        } 
        private Port CreatePort(ChoiceNode node, Direction direction, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal,direction, capacity, typeof(string)) ;
        }

       
        private DialogueNode GenerateEntryPoint()
        {
            
            var node = new DialogueNode()
            {
                title = "Entry Point",
                Guid = Guid.NewGuid().ToString(),
                Text = "Entry Point",
                isEntryPoint = true
            };
            var port = CreatePort(node, Direction.Output);
            
            port.portName = "Next";

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;


            node.outputContainer.Add(port);
            node.SetPosition(new Rect(100,100,0,0));
            node.RefreshPorts();
            node.RefreshExpandedState();
            return node;
        }



        public DialogueNode CreateLineNode(string nodeName, string lineValue)
        {
            var node = new DialogueNode
            {
                title = nodeName,
                Guid = Guid.NewGuid().ToString()

            };

            var inputPort = CreatePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Prev";
            node.inputContainer.Add(inputPort);

            var outputPort = CreatePort(node, Direction.Output, Port.Capacity.Single);
            outputPort.portName = "Next";
            node.outputContainer.Add(outputPort);

            
            ///Begin Log Button, if its Delete Later
           var LogButton = new Button(() =>
            {
                //node.Questions.ForEach(e => Debug.Log($"<color=red>{e}</color>"));
                var util = DataUtilities.GetInstance(this);
            })
            {
                text = "Log Data"
            };
            node.titleButtonContainer.Add(LogButton);
            ///End Log Button


            var sentenceText = new TextField
            {
                value = lineValue,
                multiline = true,
                
            };
            node.Text = sentenceText.value;
            sentenceText.RegisterValueChangedCallback(e => {
                node.Text = e.newValue;
            });
            node.mainContainer.Add(sentenceText);
            node.RefreshPorts();
            node.RefreshExpandedState();
            node.SetPosition(new Rect(Vector2.zero, nodeSize));

            
            return node;
            
        }

        public ChoiceNode CreateChoiceNode(string nodeName)
        {
            var node = new ChoiceNode
            {
                title = nodeName,
                Guid = Guid.NewGuid().ToString(),
                isChoiceNode = true,

            };
            
            
            var inputPort = CreatePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Prev";
            node.inputContainer.Add(inputPort);

            var choiceButton = new Button(() => {
                AddChoicePort(node, string.Empty);
            });
            choiceButton.text = "Add Branch";
            node.titleButtonContainer.Add(choiceButton);

           

            ///Begin Log Button, if its Delete Later
            var LogButton = new Button(() =>
            {
                //node.Questions.ForEach(e => Debug.Log($"<color=red>{e}</color>"));
                foreach (var i in node.QuestionDict)
                {
                    Debug.Log($"Choice {i.Key} : {i.Value}");
                }
            })
            {
                text = "Log Data"
            };
            node.titleButtonContainer.Add(LogButton);
            ///End Log Button

            return node;

        }


        public void AddChoicePort(ChoiceNode node, string questionValue)
        {
            var port = CreatePort(node, Direction.Output);
            int questIndex = node.choiceCount;
            node.QuestionDict.Add(questIndex, string.Empty);
            port.portName = $"Choice {node.choiceCount}";
           
            var lineText = new TextField
            {
                name = string.Empty,
                multiline = true
            };
            lineText.value = questionValue;
            lineText.RegisterValueChangedCallback( e =>{
                node.QuestionDict[questIndex] = e.newValue;
            });
            node.QuestionDict[questIndex] = lineText.value;
            node.choiceCount++;
            
            node.outputContainer.Add(port);
            node.outputContainer.Add(lineText);
            
            node.RefreshPorts();
            node.RefreshExpandedState();

        }

        public void AddDialogueNode(string nodeName)
        {
            AddElement(CreateLineNode(nodeName, string.Empty));
        } 
        public void AddChoiceNode(string nodeName)
        {
            AddElement(CreateChoiceNode(nodeName));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port) => {
                if (startPort != port && startPort.node != port.node) compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        
    }

}
