using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Deitado : StateMachineBehaviour
{
    float aux = 30;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (aux <= 0)
        {
            int random = Random.Range(1, 4);
            if (random == 1)
            {
                animator.SetTrigger("Levantar");
            }
            aux = 30;
        }
        else
        {
            aux = aux - Time.deltaTime;
        }
    }
}
