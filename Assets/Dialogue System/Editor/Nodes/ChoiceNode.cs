using Salt.DialogueSystem.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Salt.DialogueSystem.Editor
{
    [Serializable]
    public class ChoiceNode : CustomNode
    {
        public int choiceCount = 0;
        public static ChoiceNode Create(string title)
        {
            var node = new ChoiceNode
            {
                title = title,
                Guid = System.Guid.NewGuid().ToString(),
                isChoiceNode = true,
                name = "CustomNode",
            };

            var inputPort = DialoguePort.Create(Direction.Input, Port.Capacity.Multi);
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


        public static void AddChoicePort(ChoiceNode node, string questionValue, string next = "")
        {
            var port = DialoguePort.Create(Direction.Output, Port.Capacity.Single);
            int questIndex = node.choiceCount;
            port.portName = $"Choice {node.choiceCount}";


            var lineText = new TextField
            {
                name = string.Empty,
                multiline = true
            };
            lineText.value = questionValue;
            (port as DialoguePort).Question = questionValue;
            lineText.RegisterValueChangedCallback(e => {
                (port as DialoguePort).Question = e.newValue;
            });
            (port as DialoguePort).Next = next;
            node.choiceCount++;
            node.outputContainer.Add(port);
            node.outputContainer.Add(lineText);

            node.RefreshPorts();
            node.RefreshExpandedState();

        }
    }

}
