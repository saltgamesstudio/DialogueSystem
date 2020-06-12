using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Salt.DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Salt Studio/Character")]
    public class Character : ScriptableObject
    {
        public string charaName;

        //bunch of sprite for additional expression according to enum Expression in Line class
        public Sprite idle;


        public string description;

    }
}
