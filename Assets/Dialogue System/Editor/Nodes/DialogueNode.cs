using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Salt.DialogueSystem.Data;

namespace Salt.DialogueSystem.Editor
{
    [Serializable]
    public class DialogueNode : CustomNode
    {
        public DialogueProperties Properties;
        public string Text;
        public Character Character;
        public static DialogueNode Create(string title, string sentence, Character properties = null, string next = "") { return null; }
        public static DialogueNode Create(string title, DialogueProperties properties, string next = "")
        {
            var node = new DialogueNode
            {
                title = title,
                Guid = System.Guid.NewGuid().ToString(),
                name = "CustomNode",
                Properties = properties,
            };


            var inputPort = DialoguePort.Create(Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Prev";
            inputPort.name = "Port";

            var outputPort = DialoguePort.Create(Direction.Output, Port.Capacity.Single);
            outputPort.portName = "Next";
            outputPort.name = "Port";
            (outputPort as DialoguePort).Next = next;

            var propertiesFoldout = new Foldout
            {
                text = "Dialogue Properties"
            };

            //Dialogue Background
            var bgField = new ObjectField
            {
                objectType = typeof(Sprite),
                value = properties.Speaker,
                label = "Background"
            };
            bgField.RegisterValueChangedCallback(e => {
                node.Properties.Background = e.newValue as Sprite;
            });
            node.Properties.Background = bgField.value as Sprite;

            //Create Dialogue Properties
            var sentenceTextfield = new TextField
            {
                value = properties.Text,
                multiline = true,

            };

            node.Properties.Text = sentenceTextfield.value;
            sentenceTextfield.RegisterValueChangedCallback(e => {
                node.Properties.Text = e.newValue;
            });

            //Create Speaker
            var speakerField = new ObjectField
            {
                objectType = typeof(Character),
                value = properties.Speaker,
                label = "Speaker"
            };
            speakerField.RegisterValueChangedCallback(e => {
                node.Properties.Speaker = e.newValue as Character;
            });
            node.Properties.Speaker = speakerField.value as Character;
            var speakerPositionEnum = new EnumField();
            speakerPositionEnum.label = "Speaker On-Screen Position";
            speakerPositionEnum.Init(CharacterScreenPosition.Middle);
            speakerPositionEnum.value = CharacterScreenPosition.Middle;
            speakerPositionEnum.RegisterValueChangedCallback( e => {
                node.Properties.speakerPosition = (CharacterScreenPosition)e.newValue;
            });

            
            //Create Conversant1
            var conversant1Field = new ObjectField
            {
                objectType = typeof(Character),
                value = properties.Conversant1,
                label = "Conversant1"
            };
            conversant1Field.RegisterValueChangedCallback(e => {
                node.Properties.Conversant1 = e.newValue as Character;
            });
            node.Properties.Conversant1 = conversant1Field.value as Character;
            var conversant1PositionEnum = new EnumField();
            conversant1PositionEnum.label = "Conversant1 On-Screen Position";
            conversant1PositionEnum.Init(properties.conversant1Position);
            conversant1PositionEnum.value = properties.conversant1Position;
            conversant1PositionEnum.RegisterValueChangedCallback(e => {
                node.Properties.conversant1Position = (CharacterScreenPosition)e.newValue;
            });
            //Create Conversant2
            var conversant2Field = new ObjectField
            {
                objectType = typeof(Character),
                value = properties.Conversant2,
                label = "Conversant2"
            };
            conversant2Field.RegisterValueChangedCallback(e => {
                node.Properties.Conversant2 = e.newValue as Character;
            });
            node.Properties.Conversant2 = conversant2Field.value as Character;
            var conversant2PositionEnum = new EnumField();
            conversant2PositionEnum.label = "Conversant2 On-Screen Position";
            conversant2PositionEnum.Init(properties.conversant2Position);
            conversant2PositionEnum.value = properties.conversant2Position;
            conversant2PositionEnum.RegisterValueChangedCallback(e => {
                node.Properties.conversant2Position = (CharacterScreenPosition)e.newValue;
            });



            node.mainContainer.Add(propertiesFoldout);

            propertiesFoldout.contentContainer.Add(bgField);
            propertiesFoldout.contentContainer.Add(speakerField);
            propertiesFoldout.contentContainer.Add(speakerPositionEnum);
            propertiesFoldout.contentContainer.Add(conversant1Field);
            propertiesFoldout.contentContainer.Add(conversant1PositionEnum);
            propertiesFoldout.contentContainer.Add(conversant2Field);
            propertiesFoldout.contentContainer.Add(conversant2PositionEnum);



            node.mainContainer.Add(sentenceTextfield);

            node.inputContainer.Add(inputPort);
            
            node.outputContainer.Add(outputPort);
            node.RefreshPorts();
            node.RefreshExpandedState();
            node.SetPosition(new Rect(Vector2.zero, nodeSize));
            

            return node;
        }

    }

    

}
