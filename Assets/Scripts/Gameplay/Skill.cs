using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [Header("REFERENCE")]
    [SerializeField] private NextBlockGenerator nextBlockGenerator;
    
    public void UndoSkill()
    {
        
    }

    public void ChangeColorSkill()
    {
        nextBlockGenerator.GenerateNewBlock(nextBlockGenerator.NextColorIndex);
    }

    public void SwapSkill()
    {
        
    }
    
    public void DestroySkill()
    {
        
    }
}
