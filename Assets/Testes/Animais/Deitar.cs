using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deitar : StateMachineBehaviour
{
    private float aux = 30;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Deitado",true);
    }
}
