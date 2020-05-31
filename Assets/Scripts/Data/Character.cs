using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Character", menuName = "Assets/Character")]
public class Character : ScriptableObject
{
    public string charaName;

    //bunch of sprite for additional expression according to enum Expression in Line class
    public Sprite idle;


    public string description;

}
