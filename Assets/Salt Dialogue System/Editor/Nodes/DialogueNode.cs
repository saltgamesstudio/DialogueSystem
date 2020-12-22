using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int characterCount=0;
        public static DialogueNode Create(string title, DialogueProperties properties, string next = "")
        {
            var node = new DialogueNode
            {
                title = title,
                Guid = System.Guid.NewGuid().ToString(),
                name = "CustomNode",
                Properties = properties,
            };


            node.characterCount = 0;



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
                value = properties.Background,
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


            node.mainContainer.Add(propertiesFoldout);

            propertiesFoldout.contentContainer.Add(bgField);

            foreach (var chara in node.Properties.CharacterList)
            {
                propertiesFoldout.contentContainer.Add(CreateCharacterFieldFromData(node, chara));
            }

            var addCharaButton = new Button(() => {
                propertiesFoldout.contentContainer.Add(CreateCharacterField(node, null));
            });
            addCharaButton.text = "Add Character";
            node.titleButtonContainer.Add(addCharaButton);

            
            node.mainContainer.Add(sentenceTextfield);

            node.inputContainer.Add(inputPort);
            
            node.outputContainer.Add(outputPort);
            node.RefreshPorts();
            node.RefreshExpandedState();
            node.SetPosition(new Rect(Vector2.zero, nodeSize));
            



            return node;
        }
        
        public static ObjectField CreateCharacterField(DialogueNode node, Character chara)
        {
            
            //Create Conversant2
            node.Properties.CharacterList.Add(chara);
            int charaIndex = node.Properties.CharacterList.Count - 1;
            var field = new ObjectField
            {
                objectType = typeof(Character),
                value = chara,
                label = $"Character {node.characterCount + 1}"
            };
            field.RegisterValueChangedCallback(e =>
            {
                node.Properties.CharacterList[charaIndex] = e.newValue as Character;
            });
            node.Properties.CharacterList[charaIndex] = field.value as Character;
            node.characterCount++;
            return field;
        }

        private static ObjectField CreateCharacterFieldFromData (DialogueNode node, Character chara)
        {

            //Create Conversant2
            //node.Properties.CharacterList.Add(chara);
            int charaIndex = node.Properties.CharacterList.Count - 1;
            var field = new ObjectField
            {
                objectType = typeof(Character),
                value = chara,
                label = $"Character {node.characterCount + 1}"
            };
            field.RegisterValueChangedCallback(e =>
            {
                node.Properties.CharacterList[charaIndex] = e.newValue as Character;
            });
            //node.Properties.CharacterList[charaIndex] = field.value as Character;
            node.characterCount++;
            return field;
        }


    }

    

}
