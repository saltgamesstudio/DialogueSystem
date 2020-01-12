using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Create temporary text to hold formatted text and send them to real one
/// </summary>

public class DialogueManager : MonoBehaviour
{
    public Conversation conversation;
    public LogManager log;

    public Sprite charaSprite;
    public Text lineText;
    public Text nameText;
    public Color colorFormat;


    private readonly float typingSpeed = .01f;


    private int lineCounter = 0;
    private bool isTyping = false;

    private Coroutine speaking;
    private string formatHelper = "";

    public bool isColoredText = false;
    private void Start()
    {
        lineCounter = 0;
        nameText.text = conversation.lines[lineCounter].character.charaName;
        lineText.text = "";

        StartTyping(conversation.lines[lineCounter].character.charaName, conversation.lines[lineCounter].text);
    }

    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           
        }

     
    }
    public void UpdateLog()
    {
        
    }

    public void ButtonControl()
    {
        if (isTyping)
        {
            
            StopTyping();
        }
        else
        {
            NextLine();
        }
    }

    public void StartTyping(string name, string sentence)
    {
        if (speaking != null) StopAllCoroutines();
        
        speaking = StartCoroutine(Typing(sentence));
        isTyping = true;
    }

    IEnumerator Typing(string sentence)
    {
        lineText.text = "";
        string helper = "";
        
        foreach (char letter in sentence)
        {

            helper += letter;
            
            if (letter == '<')
            {
                isColoredText = true;
                continue;
            }
            
            if (isColoredText)
            {
                if(letter == '>')
                {
                    isColoredText = false;
                    continue;
                }
                //string formattedChar = "<color=red>" + letter + "</color>";
                string formattedChar = "<color=" + ColorToHexString(colorFormat) + ">" + letter + "</color>";
                lineText.text += formattedChar;
                formatHelper += formattedChar;
                yield return new WaitForSeconds(typingSpeed);
            }
            else
            {
                lineText.text += letter;
                formatHelper += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

        }
        if (helper == sentence)
        {
            
            StopTyping();
        }

    }

    public void NextLine()
    {
        if (lineCounter == conversation.lines.Length - 1)
        {
            Debug.Log("Selesai");
            return;
        }
        else
        {
            lineCounter++;
            StartTyping(conversation.lines[lineCounter].character.charaName, conversation.lines[lineCounter].text);
        }
    }


    public void SkipTyping()
    {
        string skippedLine = "";
        foreach (char letter in conversation.lines[lineCounter].text)
        {
            if (letter == '<')
            {
                isColoredText = true;
                continue;
            }

            if (isColoredText)
            {
                if (letter == '>')
                {
                    isColoredText = false;
                    continue;
                }
                //string formattedChar = "<color=red>" + letter + "</color>";
                string formattedChar = "<color=" + ColorToHexString(colorFormat) + ">" + letter + "</color>";
                skippedLine += formattedChar;
            }
            else
            {
                skippedLine += letter;
            }
        }
        lineText.text = skippedLine;
    }
    public void StopTyping()
    {
        if (isTyping) SkipTyping();

        StopCoroutine(speaking);
        isTyping = false;
        formatHelper = lineText.text;
        log.AddLog(conversation.lines[lineCounter].character.charaName, lineText.text);
    }
    string ColorToHexString(Color color)
    {
        Color32 color32 = color;
        return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color32.r, color32.g, color32.b, color32.a);
    }


}
