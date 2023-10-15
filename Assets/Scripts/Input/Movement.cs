using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    IEnumerator SwipeMove()
    {
        yield return new WaitForSeconds(2f);
    }
}
