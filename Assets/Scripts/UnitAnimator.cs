using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private float waitTime = .1f;


    private void Awake()
    {
      if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

      if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", true);
    }


    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");
       StartCoroutine( LaunchProjectilePostTimer(e));
    }

    private IEnumerator LaunchProjectilePostTimer(ShootAction.OnShootEventArgs e)
    {
        yield return new WaitForSeconds(waitTime);
        Transform bulletProjectileTransform =
             Instantiate(projectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletPojectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
        if(e.targetUnit != null)
        {
            Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();

            targetUnitShootAtPosition.y = shootPointTransform.position.y;

            bulletPojectile.Setup(targetUnitShootAtPosition);
        }
        
    }
}

