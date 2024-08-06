using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] private LayerMask mousePlaneLayerMAsk;

    private static MouseWorld instance; // we'll only have once instance in the game anyways

    private void Awake()
    {
        instance = this;
    }
    public static Vector3 GetPosition() // we only want it on the class itself and not other instances hence static. 
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMAsk); // 1 << 6 way to change mask with bit shifting without using layermask
        return raycastHit.point;
    }
    public static Vector3 GetPositionOnlyHitVisible()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        // items not gauranteed to be ordered in raycast hit array
        RaycastHit[] rayCastHitArray = Physics.RaycastAll(ray, float.MaxValue, instance.mousePlaneLayerMAsk); // 1 << 6 way to change mask with bit shifting without using layermask
        // sorti using built in function that uses Icompare and  a delegate
        System.Array.Sort(rayCastHitArray, (RaycastHit rayCastHitA, RaycastHit rayCastHitB) =>
        {
        return Mathf.RoundToInt(rayCastHitA.distance - rayCastHitB.distance); // returned ints from comparison makes it easier to sort
        });
        foreach (RaycastHit raycastHit in rayCastHitArray)
        {
           if(raycastHit.transform.TryGetComponent(out Renderer renderer))
            {
                if (renderer.enabled)
                {
                    return raycastHit.point;
                }
                // if disabled like if zoomed in on, we'll just ignore it
            }
        }
        return Vector3.zero;
    }
}
