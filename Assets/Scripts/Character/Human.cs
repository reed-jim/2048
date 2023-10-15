using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum HumanState
{
   Inactive,
   Idle,
   Walk
}

public class Human : MonoBehaviour, IMovable
{
   [SerializeField] private string id;
   [SerializeField] private HumanState state;
   
   public string Id { get; set; }
   
   public void Move(Vector3 destination)
   {
      
   }

   public void Jump()
   {
      
   }
}
