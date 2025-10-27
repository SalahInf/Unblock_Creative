using System;
using UnityEngine;
using UnityEngine.UI;


namespace UIParty
{
    public class ButtonUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Button _button;

        public RectTransform Rect => _rect;



        public void Init (Action callback)
        {
            Precondition.CheckNotNull(callback);
            
            _button.onClick.AddListener(() => callback());
            _button.onClick.AddListener(() => { AudioManager.Play("Click"); });
        }
    }
}