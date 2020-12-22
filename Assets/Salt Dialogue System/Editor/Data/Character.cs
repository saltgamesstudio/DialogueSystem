using UnityEngine;
namespace Salt.DialogueSystem.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "New Character", menuName = "Salt Studio/Character")]
    [System.Serializable]
    public class Character : UnityEngine.ScriptableObject
    {
        public string Name;
        public string Nickname;
        public CharacterSprites Sprites;

    }
    [System.Serializable]
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
}

