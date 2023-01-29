using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBucket : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("UsingBucket", false);
    }
}
