using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public Conversation conversation;
    public int lineCounter;

    public Sprite charaSprite;
    public Text lineText;
    public Text nameText;

    public float typingSpeed = .01f;
    public bool isTyping = false;

    Coroutine speaking;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        lineCounter = 0;
        nameText.text = conversation.lines[lineCounter].character.charaName;
        lineText.text = "";

        StartTyping( conversation.lines[lineCounter].character.charaName, conversation.lines[lineCounter].text);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(isTyping)
                StopTyping();
            else {
                lineCounter++;
                StartTyping(conversation.lines[lineCounter].character.charaName, conversation.lines[lineCounter].text);
            }
        }
    }

    public void StartTyping(string name, string sentence)
    {
        StopAllCoroutines();
        speaking = StartCoroutine(Typing(sentence));
        isTyping = true;
    }

    IEnumerator Typing(string sentence)
    {
        lineText.text = "";
        isTyping = true;
        foreach (char letter in sentence)
        {
            lineText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        if(lineText.text == sentence)
        {
            StopTyping();
        }
        
    }

    public void NextLine()
    {

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
