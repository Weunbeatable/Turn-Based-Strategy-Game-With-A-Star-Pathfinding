using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform swordTransform;
    [SerializeField] private bool hasSwappableWeapons; 
    [SerializeField] private float waitTime = .1f;


    private void Awake()
    {
      if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
            moveAction.onChangedFloorStarted += MoveAction_onChangedFloorStarted;
        }

      if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
    }

    private void MoveAction_onChangedFloorStarted(object sender, MoveAction.OnChangeFloorsStartedEventArgs e)
    {
        if(e.targetGridPosition.floor > e.unitGridPosition.floor)
        {
            // Jump
            animator.SetTrigger("JumpUp");
        }
        else
        {
            // Drop 
            animator.SetTrigger("JumpDown");
        }
        
    }

    private void Start()
    {
        EquipGun();
    }
    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipGun();
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        animator.SetTrigger("MeleeSlash");
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

    private void EquipSword()
    {
        if(hasSwappableWeapons == true)
        {
            swordTransform.gameObject.SetActive(true);
            gunTransform.gameObject.SetActive(false);
        }
        else { return; }
    }

    private void EquipGun()
    {
        if (hasSwappableWeapons == true)
        {
            swordTransform.gameObject.SetActive(false);
            gunTransform.gameObject.SetActive(true);
        }
        else { return; }
    }
}

