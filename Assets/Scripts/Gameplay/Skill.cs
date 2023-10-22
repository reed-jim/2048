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
    [Header("REFERENCE")] [SerializeField] private GameManager gameManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private NextBlockGenerator nextBlockGenerator;

    public void UndoSkill()
    {
        if (dataManager.IsEnoughGem(SkillType.Undo))
        {
            gameManager.RevertMove();
        }
        else
        {
            uiManager.ShowNotEnoughGemEffect(SkillType.Undo);
        }
    }

    public void ChangeColorSkill()
    {
        // if (dataManager.IsEnoughGem(SkillType.ChangeColor))
        // {
        //     nextBlockGenerator.GenerateNewBlock(nextBlockGenerator.NextColorIndex);
        // }
        // else
        // {
        //     uiManager.ShowNotEnoughGemEffect(SkillType.ChangeColor);
        // }
        nextBlockGenerator.GenerateNewBlock(nextBlockGenerator.NextColorIndex);
    }

    public void SwapSkill()
    {
        // if (dataManager.IsEnoughGem(SkillType.Swap))
        // {
        //     uiManager.swapModePopup.ShowPopup();
        //     gameManager.EnterSwapMode();
        // }
        // else
        // {
        //     uiManager.ShowNotEnoughGemEffect(SkillType.Swap);
        // }
        uiManager.swapModePopup.ShowPopup();
        gameManager.EnterSwapMode();
    }

    public void DestroySkill()
    {
    }
}