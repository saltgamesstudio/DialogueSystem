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
        public static DialogueManager instance;
        private DialogueParser parser;
        [SerializeField] private DialogueData dialogueData;
        private LogManager log;
        private Character characterData;


        [SerializeField] private bool isTyping;

        [Space]
        //typing speed in character per second
        [SerializeField] [Tooltip("In Character per Second")] private float defaultTypingSpeed = 20;

        [SerializeField] private Image expressionSprite;
        [SerializeField] private TMP_Text lineText;
        [SerializeField] private TMP_Text nameText;

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
            instance = this;
            parser = new DialogueParser(dialogueData);
            entryPoint = parser.EntryPoint;

        }
        private void Start()
        {
            onLineFinish.AddListener(StopTyping);
            currentNode = parser.GetNextNode(entryPoint);
            StartTyping(currentNode.Text);

        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping) StopTyping();
                else NextLine();
            }

        }

        private void StartTyping(string text)
        {
            string[] subTexts = { };
            characterData = currentNode.Character;
            ParseCustomTag(text, ref subTexts, ref lineText);
            typingCoroutine = Typing(subTexts, lineText);
            StartCoroutine(Typing(subTexts, lineText));


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
                if (!currentNode.isChoiceNode) StartTyping(currentNode.Text);
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
                    var emosi = tag.Split('=')[1].ToLower();
                    Sprite sprite = characterData.Sprites.Surprised;
                    switch (emosi)
                    {
                        case "kaget":
                            {
                                sprite = characterData.Sprites.Surprised;
                                break;
                            }
                        case "malu":
                            {
                                sprite = characterData.Sprites.Embarrassed;
                                break;
                            }
                        case "marah":
                            {
                                sprite = characterData.Sprites.Angry;
                                break;
                            }
                        case "sedih":
                            {
                                sprite = characterData.Sprites.Sad;
                                break;
                            }
                        case "senang":
                            {
                                sprite = characterData.Sprites.Happy;
                                break;
                            }
                    }
                    expressionSprite.sprite = sprite;
                    return null;
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
