using UnityEngine;
using DG.Tweening;

namespace UIParty
{
    public class UILevel : UIView
    {

        DataLevelValue _level;

        public void Init(DataLevelValue level)
        {
            Precondition.CheckNotNull(level);

            base.Init();
            _level = level;
            
        }

        protected override void ShowView()
        {
            DOTween.Kill(gameObject);
        }

        protected override void CloseView()
        {
            DOTween.Kill(gameObject);
        }
    }
}
