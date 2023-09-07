using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Animator unitAnimator;
    private Vector3 targetPosition;

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

        if (Input.GetMouseButtonDown(0))
        {
            Move(MouseWorld.GetPosition());
        }
    }
    private void Move(Vector3 targetPosition)
    {
       
        this.targetPosition = targetPosition;
    }
}
