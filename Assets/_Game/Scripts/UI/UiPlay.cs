using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UIParty;
using UnityEngine;

public class UiPlay : UIView
{
    [SerializeField] private ButtonUI _buttonReset;
    public override void Init()
    {
        base.Init();
        _buttonReset.Rect.anchoredPosition = new Vector2(_buttonReset.Rect.anchoredPosition.x, -600);
        _buttonReset.Init(StartGame);
    }

    protected override void ShowView()
    {
        FinShow();
        DOTween.Kill(gameObject);
        _buttonReset.Rect.DOAnchorPosY(0, .85f).SetEase(Ease.OutBack).SetId(gameObject);
    }

    protected override void CloseView()
    {
        DOTween.Kill(gameObject);
        _buttonReset.Rect.DOAnchorPosY(-600, .7f).SetId(gameObject).OnComplete(FinClose);
    }

    void StartGame()
    {
        print("Reset the Spring");
        Root.Controller.RestPosLastSpring();
    }
}

