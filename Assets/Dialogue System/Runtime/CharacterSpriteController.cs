using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Salt.DialogueSystem.Runtime
{
    public class CharacterSpriteController : MonoBehaviour
    {
        [SerializeField] private Image body;
        [SerializeField] private Image expression;
        [SerializeField] public bool isSpeaking;
        [SerializeField] public bool onScreen;
        public void SetBodySprite(Sprite sprite)
        {
            body.sprite = sprite;
        }
        public void SetExpressionSprite(Sprite sprite)
        {
            expression.sprite = sprite;
        }

        public void DimSprite()
        {
            body.color = new Color(.5f, .5f, .5f, 1);
            expression.color = new Color(.5f, .5f, .5f, 1);
        }
        public void SpeakingSprite()
        {
            body.color = new Color(1, 1, 1, 1);
            expression.color = new Color(1, 1, 1, 1);
        }

        public void HideCharacter()
        {
            body.color = new Color(0,0,0,0);
            expression.color = new Color(0,0,0,0);
        }
        public void ShowCharacter()
        {
            body.color = new Color(1, 1, 1, 1);
            expression.color = new Color(1, 1, 1, 1);
        }
        private void Update()
        {

            if (onScreen)
            {
                ShowCharacter();
                if (isSpeaking) SpeakingSprite();
                else DimSprite();
            }
            else HideCharacter();

        }
    }
}
