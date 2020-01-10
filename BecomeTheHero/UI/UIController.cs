using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIController : MonoBehaviour
{
    private GraphicRaycaster input;

    [SerializeField]
    private GameObject textBox;

    [SerializeField]
    private GameObject playerMenu;

    [SerializeField]
    private GameObject spellMenu;

    private RectTransform textBoxRect, playerMenuRect, spellMenuRect;
    [SerializeField]
    private float activePosY = 65f;

    private bool spellOpen = false;


    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<GraphicRaycaster>();

        textBoxRect = textBox.GetComponent<RectTransform>();
        playerMenuRect = playerMenu.GetComponent<RectTransform>();
        spellMenuRect = spellMenu.GetComponent<RectTransform>();

        textBoxRect.anchoredPosition = new Vector2(0, -activePosY);
        playerMenuRect.anchoredPosition = new Vector2(0, -activePosY);
        spellMenuRect.anchoredPosition = new Vector2(0, -activePosY);

        VariableManager.Instance.SetBoolVariableValue(VariableConstants.TEXT_BOX_IS_ACTIVE, false);
        EventManager.Instance.GetGameEvent(EventConstants.ON_BATTLE_BEGIN).AddListener(OnBattleBegin);
        EventManager.Instance.GetGameEvent(EventConstants.ON_TURN_BEGIN).AddListener(OnTurnBegin);
        EventManager.Instance.GetGameEvent(EventConstants.ON_MOVE_SELECTED).AddListener(OnMoveSelected);
    }


    public void OnBattleBegin()
    {
        spellOpen = false;
        textBoxRect.DOAnchorPosY(activePosY, 1.0f).
            OnComplete(OnTextBoxAppear);
    }


    public void OnTextBoxAppear()
    {
        SetTextBoxActiveState(true);
    }


    private void SetTextBoxActiveState(bool val)
    {
        VariableManager.Instance.SetBoolVariableValue(VariableConstants.TEXT_BOX_IS_ACTIVE, val);
    }


    public void OnTurnBegin()
    {
        spellOpen = false;
        SetInputState(false);
        SetTextBoxActiveState(false);
        textBoxRect.DOAnchorPosY(-activePosY, 0.5f).
            OnComplete(OpenPlayerMenu).SetEase(Ease.InSine);
    }


    public void OnMoveSelected()
    {
        if (spellOpen)
            spellMenuRect.DOAnchorPosY(-activePosY, 0.5f).
                OnComplete(OpenTextBox).SetEase(Ease.InSine);
        else
            playerMenuRect.DOAnchorPosY(-activePosY, 0.5f).
                OnComplete(OpenTextBox).SetEase(Ease.InSine);
    }


    public void OpenTextBox()
    {
        textBoxRect.DOAnchorPosY(activePosY, 0.5f).SetEase(Ease.OutSine)
            .OnComplete(EnableInput).OnComplete(OnTextBoxAppear);
    }

    public void OpenPlayerMenu() { playerMenuRect.DOAnchorPosY(activePosY, 0.5f).SetEase(Ease.OutSine).OnComplete(EnableInput); }
    public void OpenSpellMenu() { spellMenuRect.DOAnchorPosY(activePosY, 0.5f).SetEase(Ease.OutSine).OnComplete(EnableInput); }


    #region Button Functions

    public void SpellButtonClick()
    {
        spellOpen = true;
        SetInputState(false);
        playerMenuRect.DOAnchorPosY(-activePosY, 0.5f).
            OnComplete(OpenSpellMenu).SetEase(Ease.InSine);
    }

    public void BackButtonClick()
    {
        spellOpen = false;
        SetInputState(false);
        spellMenuRect.DOAnchorPosY(-activePosY, 0.5f).
            OnComplete(OpenPlayerMenu).SetEase(Ease.InSine);
    }

    #endregion


    #region Input

    public void EnableInput() { SetInputState(true); } 

    private void SetInputState(bool val)
    {
        input.enabled = val;
    }

    #endregion


    void OnDestroy()
    {
        EventManager.Instance.GetGameEvent(EventConstants.ON_BATTLE_BEGIN).RemoveListener(OnBattleBegin);
        EventManager.Instance.GetGameEvent(EventConstants.ON_TURN_BEGIN).RemoveListener(OnTurnBegin);
        EventManager.Instance.GetGameEvent(EventConstants.ON_MOVE_SELECTED).RemoveListener(OnMoveSelected);
    }
}
