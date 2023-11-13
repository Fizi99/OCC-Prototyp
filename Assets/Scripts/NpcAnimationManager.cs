using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAnimationManager : MonoBehaviour
{

    private Animator animator;

    /// <summary>
    /// initilize NpcAnimationManager.
    /// </summary>
    private void Start(){
        animator = transform.GetComponent<Animator>();
    }

    /// <summary>
    /// Change current Animation by specification. Transitions into new animation.
    /// </summary>
    /// <param name="input">Name for Animator Trigger, that triggers animation. Print error if trigger is not found.</param>
    public void ChangeAnimation(string input){

        foreach(var param in animator.parameters){
            animator.ResetTrigger(param.name);
        }
        animator.SetTrigger(input);

    }

    //TODO: später löschen///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// prints name of currently playing animation for debugging.
    /// </summary>
    /// <returns></returns>
    IEnumerator PrintCurrAnimation(){
        yield return new WaitForSecondsRealtime(0.5f);
        Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip);
    }

}
