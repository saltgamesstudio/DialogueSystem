using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Salt.DialogueSystem.Runtime
{
    public class DialogueLog : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text lineText;

        public void setText(string name, string line)
        {
            nameText.text = name;
            lineText.text = line;
        }
    }
}
