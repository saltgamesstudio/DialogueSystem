using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Salt.DialogueSystem.Data;
using UnityEditor.UIElements;

namespace Salt.DialogueSystem.Editor
{
    public class DialogueGraph : GraphView
    {
        public readonly Vector2 nodeSize = new Vector2(150,200);
        public DialogueGraph()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("EditorStyle"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
            var grid = new GridBackground();
            
            Insert(0, grid);
            AddElement(GenerateEntryPoint());
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Create Dialogue Node", delegate (DropdownMenuAction a)
                {
                    var node = CreateDialogueNode("Dialogue Node", string.Empty);
                    var nodePos = this.contentViewContainer.WorldToLocal(a.eventInfo.mousePosition);
                    node.SetPosition(new Rect(nodePos, nodeSize));
                    AddElement(node);
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
                });
            }
            base.BuildContextualMenu(evt);
        }

       

        private Port CreatePort(Direction direction, Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = DialoguePort.Create(direction, capacity);
            return port;
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
            var port = CreatePort(Direction.Output);
            port.portName = "Next";
            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.outputContainer.Add(port);
            node.SetPosition(new Rect(100,100,0,0));
            node.RefreshPorts();
            node.RefreshExpandedState();
            return node;
        }



        public DialogueNode CreateDialogueNode(string title, string sentence, Character character = null, string next = "")
        {
            var node = new DialogueNode
            {
                title = title,
                Guid = Guid.NewGuid().ToString()

            };


            var inputPort = CreatePort(Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Prev";
            inputPort.name = "Port";

            var outputPort = CreatePort(Direction.Output, Port.Capacity.Single);
            outputPort.portName = "Next";
            outputPort.name = "Port";
            (outputPort as DialoguePort).Next = next;


            
            ///Begin Log Button, if its Delete Later
           var LogButton = new Button(() =>
            {
                //node.Questions.ForEach(e => Debug.Log($"<color=red>{e}</color>"));
            })
            {
                text = "Log Data"
            };
            node.titleButtonContainer.Add(LogButton);
            ///End Log Button


            var sentenceText = new TextField
            {
                value = sentence,
                multiline = true,
                
            };
            node.Text = sentenceText.value;
            sentenceText.RegisterValueChangedCallback(e => {
                node.Text = e.newValue;
            });
            var charaField = new ObjectField
            {
                objectType = typeof(Character),
                value = character,
                
            };
            charaField.RegisterValueChangedCallback(e=> {
                node.Character = e.newValue as Character;
            });
            node.Character = charaField.value as Character;
            node.mainContainer.Add(charaField);
            node.mainContainer.Add(sentenceText);

            node.inputContainer.Add(inputPort);
            node.outputContainer.Add(outputPort);
            node.RefreshPorts();
            node.RefreshExpandedState();
            node.SetPosition(new Rect(Vector2.zero, nodeSize));

            
            return node;
        }

        public ChoiceNode CreateChoiceNode(string title)
        {
            var node = new ChoiceNode
            {
                title = title,
                Guid = Guid.NewGuid().ToString(),
                isChoiceNode = true,
            };
            
            var inputPort = CreatePort(Direction.Input, Port.Capacity.Multi);
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
                
            })
            {
                text = "Log Data"
            };
            node.titleButtonContainer.Add(LogButton);
            ///End Log Button
            
            return node;
        }


        public void AddChoicePort(ChoiceNode node, string questionValue, string next = "")
        {
            var port = CreatePort(Direction.Output);
            int questIndex = node.choiceCount;
            port.portName = $"Choice {node.choiceCount}";
            
           
            var lineText = new TextField
            {
                name = string.Empty,
                multiline = true
            };
            lineText.value = questionValue;
            (port as DialoguePort).Question = questionValue;
            lineText.RegisterValueChangedCallback( e =>{
                (port as DialoguePort).Question = e.newValue;
            });
            (port as DialoguePort).Next = next;
            node.choiceCount++;
            node.outputContainer.Add(port);
            node.outputContainer.Add(lineText);
            
            node.RefreshPorts();
            node.RefreshExpandedState();

        }

        public void AddDialogueNode(string nodeName)
        {
            AddElement(CreateDialogueNode(nodeName, string.Empty));
        } 
        public void AddChoiceNode(string nodeName)
        {
            AddElement(CreateChoiceNode(nodeName));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port) => {
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction) compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        
    }

}
