using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject OtherProjectileType;
    [SerializeField] private Transform projectileHitVFXPrefab;

    private void Awake()
    {
        
    }
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        Vector3 moveDir = (targetPosition - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if(distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPosition;
            if (TryGetComponent<TrailRenderer>(out TrailRenderer trailRenderer))
            {
                trailRenderer.transform.parent = null;
            }
            else if(TryGetComponent<GameObject>(out GameObject otherProjectileType))
            {
                OtherProjectileType.transform.parent = null;
            }
            Destroy(gameObject);
            Instantiate(projectileHitVFXPrefab, targetPosition, Quaternion.identity);
        }
    }
}
