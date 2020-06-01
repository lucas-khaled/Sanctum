using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levantar : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Deitado",false);
        animator.SetTrigger("Levantar");
    }
}
