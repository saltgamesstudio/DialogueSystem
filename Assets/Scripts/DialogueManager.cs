using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Create temporary text to hold formatted text and send them to real one
/// </summary>


public enum Emotion {HAPPY, SAD, ANGRY }
[Serializable] public class ExpressionEvent : UnityEvent<Emotion> { };
[Serializable] public class ActionEvent : UnityEvent<string> { };
[Serializable] public class DialogueEvent : UnityEvent<string> { };

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    [SerializeField] private Conversation conversation;
    [SerializeField] private LogManager log;

    [Space]
    //typing speed in character per second
    [SerializeField][Tooltip("In Character per Second")] private float defaultTypingSpeed = 20;

    [SerializeField] private Sprite charaSprite;
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private TMP_Text nameText;

    
    string[] customTagLists = new string[] {"speed","pause","expression"};

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        ReadText(conversation.lines[0].text);
    }

    private void ReadText(string line)
    {
        string displayText = string.Empty;
        lineText.text = string.Empty;

        //parse tag and normal text
        //tag will be odd number index, normal text or empty string will be even number 
        string[] subTexts = line.Split('<','>');
        for(int i = 0; i < subTexts.Length; i++)
        {
            if(i % 2 == 0)
            {
                displayText += subTexts[i];
            }
            //if it is built in tag
            else if(!isCustomTag(subTexts[i].Replace(" ", "")))
            {
                displayText += $"<{subTexts[i]}>";
            }
        }
        lineText.text = displayText;
        lineText.maxVisibleCharacters = 0;
        StartCoroutine(Typing(line));

    }

    private IEnumerator Typing(string line)
    {
        int counter = 0;
        float speed = defaultTypingSpeed;
        while (counter < lineText.text.Length)
        {
            lineText.maxVisibleCharacters += 1;
            yield return new WaitForSeconds(1/speed);
        }
        yield return null;
    }


    private bool isCustomTag(string tag)
    {
        foreach(string customTag in customTagLists)
        {
            return tag.StartsWith(customTag);
        }
        return false;
    }

    private void EvaluateTag()
    {

    }
    

}
