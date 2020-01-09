using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Conversation",menuName ="Assets/Conversation")]
public class Conversation : ScriptableObject
{
    public Line[] lines;
}
