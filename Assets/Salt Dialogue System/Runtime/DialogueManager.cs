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
        private Data.Character speakerCharacter;
        private AudioSource audioSource;

        [SerializeField] private bool isTyping;

        [Space]
        //typing speed in character per second
        [SerializeField] [Tooltip("In Character per Second")] private float defaultTypingSpeed = 20;

        //[SerializeField] private Image expressionSprite;
        [SerializeField] private TMP_Text lineText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private CharacterSpriteController speakerSprite;
        [SerializeField] private CharacterSpriteController dialoguePortraitSprite;
        [SerializeField] private DialogueLogManager logManager;
        //[SerializeField] private ChoiceButtonController choiceController;

        private float speed;

        string[] customTagLists = new string[] { "pause", "speed", "expression", "sfx", "bgm", "cameraShake", "charaShake", "changeBg", "hide", "unhide", "charaName" };

        private IEnumerator typingCoroutine;

        [Space]
        [Header("Choice Related")]
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private List<Button> buttons = new List<Button>();
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonContainer;

        [Space]
        [Header("Background")]
        [SerializeField] private Image backgroundPanel;
       

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
            audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            onLineFinish.AddListener(StopTyping);
            currentNode = parser.GetNextNode(entryPoint);
            StartTyping(currentNode);
        }
        private void Update()
        {
            
            

        }

        public void InputHandler()
        {
            if (isTyping) StopTyping();
            else NextLine();
        }

        void StartTyping(NodeData data)
        {
            currentNode = data;
            ChangeBackground(data);
            speakerCharacter = currentNode.Properties.CharacterList[0];
            lineText.text = speakerCharacter.Name;
            string[] subTexts = { };
            if (currentNode.Properties.CharacterList.Count < 2)
            {
                dialoguePortraitSprite.HideCharacter();
            }
            else
            {
                dialoguePortraitSprite.ShowCharacter();
                dialoguePortraitSprite.SetBodySprite(currentNode.Properties.CharacterList[0].Sprites.Body);
                dialoguePortraitSprite.SetExpressionSprite(currentNode.Properties.CharacterList[0].Sprites.Idle);
            }
            ParseCustomTag(currentNode.Properties.Text, ref subTexts, ref lineText);
            typingCoroutine = Typing(subTexts, lineText);
            StartCoroutine(Typing(subTexts, lineText));
            logManager.AddLog(speakerCharacter.Name, lineText.text);

        }

        public void ChangeBackground(NodeData data)
        {
            if (data.Properties.Background == null) return;
            else backgroundPanel.sprite = data.Properties.Background;
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
                else if(currentNode.isChoiceNode)
                {
                    CreateButton(currentNode);
                    Debug.Log("Choice Node");
                }
            }




        }

        public void CreateButton(NodeData data)
        {
            buttons.Clear();
            buttonContainer.DetachChildren();
            choicePanel.SetActive(true);
            foreach (var choice in data.Choices)
            {
                var choiceButton = Instantiate(buttonPrefab, buttonContainer);
                var button = choiceButton.GetComponent<Button>();
                var text = button.GetComponentInChildren<TMP_Text>();
                text.text = choice.Question;
                var next = choice.Next;
                button.onClick.AddListener(
                    delegate
                    {
                        Debug.Log(next + " : " + text.text);

                        OnChoice(next);

                    }
                );
                buttons.Add(button);
            }
        }


        private void OnChoice(string nextGuid)
        {
            
            var nextNode = parser.GetNode(nextGuid);
            Debug.Log("Checked : " + nextNode.Guid);
            StartTyping(nextNode);
            choicePanel.SetActive(false);
            foreach (var button in buttons)
            {
                Destroy(button);
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
                    Sprite sprite = speakerCharacter.Sprites.Idle;
                    switch (emosi)
                    {
                        case "kaget":
                            {
                                sprite = speakerCharacter.Sprites.Surprised;
                                break;
                            }
                        case "malu":
                            {
                                sprite = speakerCharacter.Sprites.Embarrassed;
                                break;
                            }
                        case "marah":
                            {
                                sprite = speakerCharacter.Sprites.Angry;
                                break;
                            }
                        case "sedih":
                            {
                                sprite = speakerCharacter.Sprites.Sad;
                                break;
                            }
                        case "senang":
                            {
                                sprite = speakerCharacter.Sprites.Happy;
                                break;
                            }
                    }
                    speakerSprite.SetExpressionSprite(sprite);
                    return null;
                }
                if (tag.StartsWith("sfx="))
                {
                    int index = int.Parse(tag.Split('=')[1]);
                    AudioClip clip = dialogueData.SoundFx[index];
                    audioSource.PlayOneShot(clip);
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