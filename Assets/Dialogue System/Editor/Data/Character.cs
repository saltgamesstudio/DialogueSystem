
namespace Salt.DialogueSystem.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "New Character", menuName = "Salt Studio/Character")]
    [System.Serializable]
    public class Character : UnityEngine.ScriptableObject
    {
        public string Name;
        public CharacterSprites Sprites;

    }
    [System.Serializable]
    public class CharacterSprites
    {
        public UnityEngine.Sprite Body;
        public UnityEngine.Sprite Idle;
        public UnityEngine.Sprite Surprised;
        public UnityEngine.Sprite Embarrassed;
        public UnityEngine.Sprite Angry;
        public UnityEngine.Sprite Sad;
        public UnityEngine.Sprite Happy;
    }
}

