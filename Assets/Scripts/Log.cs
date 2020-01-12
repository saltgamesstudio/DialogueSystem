using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    public Text nameText;
    public Text lineText;

    public void setText(string name, string line)
    {
        nameText.text = name;
        lineText.text = line;
    }
}
