using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Undo,
    ChangeColor,
    Swap
}

public class Skill : MonoBehaviour
{
    [Header("REFERENCE")][SerializeField] private GameManager gameManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private NextBlockGenerator nextBlockGenerator;

    public void UndoSkill()
    {
        AudioManager.Instance.PlayPopupSound();

        if (dataManager.IsEnoughGem(SkillType.Undo))
        {
            uiManager.PlaySkillButtonPressEffect(SkillType.Undo);

            gameManager.RevertMove();
        }
        else
        {
            uiManager.ShowNotEnoughGemEffect(SkillType.Undo);
        }
    }

    public void ChangeColorSkill()
    {
        AudioManager.Instance.PlayPopupSound();

        if (dataManager.IsEnoughGem(SkillType.ChangeColor))
        {
            uiManager.PlaySkillButtonPressEffect(SkillType.ChangeColor);

            nextBlockGenerator.GenerateNewBlock(nextBlockGenerator.NextColorIndex);
            dataManager.SpendGem(150);
        }
        else
        {
            uiManager.ShowNotEnoughGemEffect(SkillType.ChangeColor);
        }
    }

    public void SwapSkill()
    {
        AudioManager.Instance.PlayPopupSound();

        if (dataManager.IsEnoughGem(SkillType.Swap))
        {
            uiManager.PlaySkillButtonPressEffect(SkillType.Swap);

            uiManager.swapModePopup.ShowPopup();
            gameManager.EnterSwapMode();
        }
        else
        {
            uiManager.ShowNotEnoughGemEffect(SkillType.Swap);
        }
    }

    public void DestroySkill()
    {
    }
}