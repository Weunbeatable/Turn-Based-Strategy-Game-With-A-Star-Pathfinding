using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVFXPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve; // simulate arc on grenade

    private Action onGrenadeBehaviourComplete;
    private Vector3 targetPosition;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized; // direction for nade to move in

        float moveSpeed = 15f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime; // grenade velocity

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance; // normalized will have the value start at 0 and slowly get closer to 1 by using the divided distance
        // and subtracting it from 1 it will give the inverted value which is whats needed for the correct curve  animation of the grenade.
        // 

        

        float maxHeight = totalDistance / 4; // MAX HEIGHT will be dependent on total distance.

        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z); // the Y value will now be from the animation curve
        // giving the look of an arc 

        float reachedTargetDistance = .2f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)  // contact radius 

        { 
            // query for objects around grenade to deal damage to before destroying itself. 
            // grid positions are in world units, so 2 units are 1 grid position
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach(Collider collider in colliderArray)
            {
              if(collider.TryGetComponent<Unit>(out Unit targetUnit)) // if in range 
                {
                    targetUnit.Damage(30); //damage unit
                }
                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate crate)) // if in range 
                {
                    crate.Damage(); //damage unit
                }
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;
            Instantiate(grenadeExplodeVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);

            Destroy(gameObject); // one shot nade 

            onGrenadeBehaviourComplete();
        }
    }
    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete) // have a callback action for when grenade does damage to end busy action
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0f;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
