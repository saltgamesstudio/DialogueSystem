using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Conversation", menuName = "Assets/Character")]
[Serializable]
public class Characters : ScriptableObject
{
    public string charaName;
    public CharacterSprites Sprites;

}
[Serializable]
public class CharacterSprites
{
    public Sprite Body;
    public Sprite Idle;
    public Sprite Surprised;
    public Sprite Embarrassed;
    public Sprite Angry;
    public Sprite Sad;
    public Sprite Happy;
}