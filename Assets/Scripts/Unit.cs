using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Animator unitAnimator;
    private Vector3 targetPosition;

    private void Awake()
    {
        targetPosition = transform.position;
    }
    private void Update()
    {
        
        float stoppingDistance = .1f;
        if(Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized; // only care about direction so we normalize
            float moveSpeed = 4;
            transform.position += moveDirection * moveSpeed * Time.deltaTime; // framerate independent movement
                                                                              // using transform. forward/up/right * a vector allows for easy rotation
            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward,moveDirection, rotateSpeed * Time.deltaTime); // we'll lerp rotation to make it smoother
            unitAnimator.SetBool("isWalking", true);
        }
        else
        {
            unitAnimator.SetBool("isWalking", false);
        }

       
    }

    public void ReadyAnim()
    {
        unitAnimator.Play("Intro");
    }
    public void Move(Vector3 targetPosition)
    {
       
        this.targetPosition = targetPosition;
    }
    public float GetNormalizedTime(Animator animator, string tag) // purpose here is to check how far through the animation and if we pass a threshold so if they player is still attacking or holding attack , go to the next animation.
    {
        // want to know the data for current and next state to figure out which one we are in whenever blending
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0); // gettinig animation layer info and storing in variables
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag)) // already know we are in layer 0, so if we are in layer 0 and transitioning to an attack we want to get the data for the next state.

        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag)) // if not in an attack animation case
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f; // just in case none of this is true;s
        }
    }

    public Animator GetAnimator() => unitAnimator;
}
