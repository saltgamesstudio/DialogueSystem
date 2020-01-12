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


    [SerializeField] private float typingSpeed = .01f;


    private int lineCounter = 0;
    private bool isTyping = false;

    private Coroutine speaking;

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
            StopTyping();
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

        log.AddLog(name, sentence);
    }

    IEnumerator Typing(string sentence)
    {
        lineText.text = "";
        foreach (char letter in sentence)
        {
            lineText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        if (lineText.text == sentence)
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
        lineText.text = conversation.lines[lineCounter].text;
    }
    public void StopTyping()
    {
        if (isTyping) SkipTyping();

        StopCoroutine(speaking);
        isTyping = false;


    }


}
