using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using Salt.DialogueSystem.Data;
using Salt.DialogueSystem.Runtime;

public enum Emotion {HAPPY, SAD, ANGRY }
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
    [SerializeField] private Conversation conversation = null;
    private LogManager log;

    [SerializeField] private Conversation  Cococd = null;

    [SerializeField] private bool isTyping;

    [Space]
    //typing speed in character per second
    [SerializeField][Tooltip("In Character per Second")] private float defaultTypingSpeed = 20;

    [SerializeField] private Sprite charaSprite;
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private TMP_Text nameText;

    private float speed;
    private int lineIndex = 0;
    
    string[] customTagLists = new string[] {"pause", "speed","expression","sfx","bgm","cameraShake","charaShake","changeBg","hide","unhide"};

    private IEnumerator typingCoroutine;

    [Space]
    [Header("Events")]
    public CustomTagEvent onCustomTag;
    public DialogueEvent onLineFinish;
    public DialogueEvent onConversationFinish;


    private void Awake()
    {
        instance = this;
        
    }
    private void Start()
    {
        onLineFinish.AddListener(StopTyping);
        StartTyping(0);
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping) StopTyping();
            else NextLine();
        }
        
    }

    private void StartTyping(int Index)
    {
        string[] subTexts = { };
        typingCoroutine = Typing(subTexts, lineText);
        ParseCustomTag(conversation.lines[Index].text, ref subTexts, ref lineText);
        StartCoroutine(Typing(subTexts, lineText));
        
    }
    private void NextLine()
    {
        if(lineIndex == conversation.lines.Length - 1)
        {
            Debug.Log("<color=yellow>Line Skipped</color>");
            onConversationFinish.Invoke();
            return;
        }
        else
        {
            lineIndex++;
            StartTyping(lineIndex);
        }
    }

    private void ParseCustomTag(string line, ref string[] outSubTexts, ref TMP_Text txt)
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
        foreach(string customTag in customTagLists)
        {
            if(tag.StartsWith(customTag)) return true;
        }
        return false;
    }

    WaitForSeconds EvaluateTag(string tag)
    {
        if(tag.Length > 0)
        {
            if (tag.StartsWith("speed="))
            {
                speed = float.Parse(tag.Split('=')[1]);
            }
            if (tag.StartsWith("pause="))
            {
                return new WaitForSeconds(float.Parse(tag.Split('=')[1]));
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
public abstract class CustomTag 
{
    public string name;
    public abstract void Action();
}


public static class TagExecutor
{
    private static Dictionary<string, CustomTag> tagDict = new Dictionary<string, CustomTag>();
    private static string[] customTagLists = new string[] { "pause", "speed", "expression", "sfx", "bgm", "cameraShake", "charaShake", "changeBg", "hide", "unhide" };
    private static bool isInitialized = false;

    private static void InitializeDict()
    {
        tagDict.Clear();
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(CustomTag));
        var allTags = assembly.GetTypes();//.Where(t => typeof(CustomTag).IsAssignableFrom(t) && t.IsAbstract == false);
        List<Type> validTags = new List<Type>();
        foreach(var tag in allTags)
        {
            if (typeof(CustomTag).IsAssignableFrom(tag) && tag.IsAbstract == false) validTags.Add(tag);
        }
        foreach(var tag in validTags)
        {
            CustomTag customTag = Activator.CreateInstance(tag) as CustomTag;
            tagDict.Add(customTag.name, customTag);
        }
        isInitialized = true;
    }

    public static void Execute(string tag)
    {
        if (isInitialized == false) InitializeDict();
       
        var tagToExecute = tagDict[tag];
        tagToExecute.Action();
    }
}