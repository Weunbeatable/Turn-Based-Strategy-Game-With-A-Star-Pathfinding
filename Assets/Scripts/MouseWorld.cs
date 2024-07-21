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
}
