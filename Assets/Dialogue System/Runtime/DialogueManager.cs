using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using Salt.DialogueSystem.Data;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;

namespace Salt.DialogueSystem.Runtime
{
    public enum Emotion { HAPPY, SAD, ANGRY }
    [Serializable] public class ExpressionEvent : UnityEvent<Emotion> { };
    [Serializable] public class CustomTagEvent : UnityEvent<string> { };
    [Serializable] public class DialogueEvent : UnityEvent { };

    /// <summary>
    /// Tag Info
    /// pause = delay line for a moment in second
    /// speed = change typing animation speed
    /// expression = change current speaker expression
    /// sfx = play sound effect once
    /// bgm = play/change background music (in loop)
    /// cameraShake = shake Camera
    /// charaShake = shake speaker sprite
    /// changeBg = change current background to index of background array (default bg is index 0)
    /// hide = hide dialog box
    /// unhide = unhide dialog box
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        private DialogueParser parser;
        [SerializeField] private DialogueData dialogueData;
        private Character speakerCharacter;


        [SerializeField] private bool isTyping;

        [Space]
        //typing speed in character per second
        [SerializeField] [Tooltip("In Character per Second")] private float defaultTypingSpeed = 20;

        //[SerializeField] private Image expressionSprite;
        [SerializeField] private TMP_Text lineText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private CharacterSpriteController leftCharacter;
        [SerializeField] private CharacterSpriteController middleCharacter;
        [SerializeField] private CharacterSpriteController rightCharacter;

        private float speed;

        string[] customTagLists = new string[] { "pause", "speed", "expression", "sfx", "bgm", "cameraShake", "charaShake", "changeBg", "hide", "unhide", "charaName" };

        private IEnumerator typingCoroutine;

        [Space]
        [Header("Events")]
        public CustomTagEvent onCustomTag;
        public DialogueEvent onLineFinish;
        public DialogueEvent onConversationFinish;


        private NodeData entryPoint;
        private NodeData currentNode;

        private void Awake()
        {
            parser = new DialogueParser(dialogueData);
            entryPoint = parser.EntryPoint;

        }
        private void Start()
        {
            onLineFinish.AddListener(StopTyping);
            currentNode = parser.GetNextNode(entryPoint);
            StartTyping(currentNode);
            rightCharacter.DimSprite();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping) StopTyping();
                else NextLine();
            }

        }

        private void StartTyping(NodeData data)
        {
            string[] subTexts = { };
            speakerCharacter = currentNode.Properties.Speaker;
            DimAllConversant(data);
            ParseCustomTag(data.Properties.Text, ref subTexts, ref lineText);
            typingCoroutine = Typing(subTexts, lineText);
            StartCoroutine(Typing(subTexts, lineText));


        }
        private void CheckConversant(NodeData node)
        {
            if (node.Properties.Conversant1 == null)
            {

            }
            if (node.Properties.Conversant2 == null)
            {

            }
        }
        private void DimAllConversant(NodeData node)
        {
            if(node.Properties.Conversant1 != null)
            {
                switch (node.Properties.conversant1Position)
                {
                    case CharacterScreenPosition.Left:
                        {
                            leftCharacter.isSpeaking = false;
                            leftCharacter.onScreen = true;
                            break;
                        }
                    case CharacterScreenPosition.Middle:
                        {
                            middleCharacter.isSpeaking = false;
                            middleCharacter.onScreen = true;
                            break;
                        }
                    case CharacterScreenPosition.Right:
                        {
                            rightCharacter.isSpeaking = false;
                            rightCharacter.onScreen = true;
                            break;
                        }
                }
            }
            else
            {
                switch (node.Properties.conversant1Position)
                {
                    case CharacterScreenPosition.Left:
                        {
                            leftCharacter.onScreen = false;

                            break;
                        }
                    case CharacterScreenPosition.Middle:
                        {
                            middleCharacter.onScreen = false;
                            break;
                        }
                    case CharacterScreenPosition.Right:
                        {
                            rightCharacter.onScreen = false;
                            break;
                        }
                }
            }

            if(node.Properties.Conversant2 != null)
            {
                switch (node.Properties.conversant2Position)
                {
                    case CharacterScreenPosition.Left:
                        {
                            leftCharacter.isSpeaking = false;
                            leftCharacter.onScreen = true;
                            break;
                        }
                    case CharacterScreenPosition.Middle:
                        {
                            middleCharacter.isSpeaking = false;
                            middleCharacter.onScreen = true;
                            break;
                        }
                    case CharacterScreenPosition.Right:
                        {
                            rightCharacter.isSpeaking = false;
                            rightCharacter.onScreen = true;
                            break;
                        }
                }
            }
            else
            {
                switch (node.Properties.conversant2Position)
                {
                    case CharacterScreenPosition.Left:
                        {
                            leftCharacter.onScreen = false;

                            break;
                        }
                    case CharacterScreenPosition.Middle:
                        {
                            middleCharacter.onScreen = false;
                            break;
                        }
                    case CharacterScreenPosition.Right:
                        {
                            rightCharacter.onScreen = false;
                            break;
                        }
                }
            }
            switch (node.Properties.speakerPosition)
            {
                case CharacterScreenPosition.Left:
                    {
                        leftCharacter.isSpeaking = true;
                        leftCharacter.onScreen = true;
                        Debug.Log("Speaker Left");

                        break;
                    }
                case CharacterScreenPosition.Middle:
                    {
                        middleCharacter.isSpeaking = true;
                        middleCharacter.onScreen = true;
                        Debug.Log("Speaker Middle");

                        break;
                    }
                case CharacterScreenPosition.Right:
                    {
                        rightCharacter.isSpeaking = true;
                        middleCharacter.onScreen = true;
                        Debug.Log("Speaker Right");

                        break;
                    }
            }
        }
        private void NextLine()
        {
            if (parser.IsEndNode(currentNode))
            {
                Debug.Log("<color=red>Dialogue End</color>");
                onConversationFinish.Invoke();
                return;
            }
            else
            {
                currentNode = parser.GetNextNode(currentNode);
                if (!currentNode.isChoiceNode) StartTyping(currentNode);
                else
                {
                    NextLine();
                    Debug.Log("Choice Node");
                }
            }




        }

        private void ParseCustomTag(string line, ref string[] outSubTexts, ref TMP_Text txt)
        {
            string displayText = string.Empty;
            lineText.text = string.Empty;

            //parse tag and normal text
            //tag will be odd number index, normal text or empty string will be even number 
            string[] subTexts = line.Split('<', '>');
            for (int i = 0; i < subTexts.Length; i++)
            {
                if (i % 2 == 0)
                {
                    displayText += subTexts[i];
                }
                //if it is built in tag
                else if (!isCustomTag(subTexts[i].Replace(" ", "")))
                {
                    displayText += $"<{subTexts[i]}>";
                }
            }

            txt.text = displayText;
            txt.maxVisibleCharacters = 0;
            outSubTexts = subTexts;


        }

        private IEnumerator Typing(string[] subTexts, TMP_Text txt)
        {
            isTyping = true;
            speed = defaultTypingSpeed;
            for (int i = 0; i < subTexts.Length; i++)
            {

                if (i % 2 == 1)
                {
                    yield return EvaluateTag(subTexts[i]);
                }
                else
                {
                    int counter = 0;

                    while (counter < subTexts[i].Length)
                    {

                        txt.maxVisibleCharacters++;
                        counter++;
                        yield return new WaitForSeconds(1 / speed);
                    }
                    yield return null;

                }

            }
            isTyping = false;
            onLineFinish.Invoke();
        }


        private bool isCustomTag(string tag)
        {
            foreach (string customTag in customTagLists)
            {
                if (tag.StartsWith(customTag)) return true;
            }
            return false;
        }

        WaitForSeconds EvaluateTag(string tag)
        {
            if (tag.Length > 0)
            {
                if (tag.StartsWith("speed="))
                {
                    speed = float.Parse(tag.Split('=')[1]);
                }
                if (tag.StartsWith("pause="))
                {
                    return new WaitForSeconds(float.Parse(tag.Split('=')[1]));
                }
                if (tag.StartsWith("expression="))
                {
                    //var emosi = tag.Split('=')[1].ToLower();
                    //Sprite sprite = speakerCharacter.Sprites.Surprised;
                    //switch (emosi)
                    //{
                    //    case "kaget":
                    //        {
                    //            sprite = speakerCharacter.Sprites.Surprised;
                    //            break;
                    //        }
                    //    case "malu":
                    //        {
                    //            sprite = speakerCharacter.Sprites.Embarrassed;
                    //            break;
                    //        }
                    //    case "marah":
                    //        {
                    //            sprite = speakerCharacter.Sprites.Angry;
                    //            break;
                    //        }
                    //    case "sedih":
                    //        {
                    //            sprite = speakerCharacter.Sprites.Sad;
                    //            break;
                    //        }
                    //    case "senang":
                    //        {
                    //            sprite = speakerCharacter.Sprites.Happy;
                    //            break;
                    //        }
                    //}
                    //speakerCharacter.Sprites..sprite = sprite;
                    //return null;
                }


            }

            //onCustomTag.Invoke(tag);
            return null;
        }

        private void StopTyping()
        {
            Debug.Log("<color=red>Line Finished</color>");
            if (isTyping) SkipLine();
            else StopCoroutine(typingCoroutine);



        }

        private void SkipLine()
        {
            Debug.Log("<color=yellow>Line Skipped</color>");
            lineText.maxVisibleCharacters = lineText.text.Length;
            isTyping = false;
        }
    }


}