using UnityEngine;

public class RotateLeft : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var transf = animator.transform.parent;
        transf.localEulerAngles = new Vector3(0,transf.localEulerAngles.y-90,0);
        animator.SetFloat("Forward",0);
    }

}
