using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    private Renderer[] rendererArray;
    private int floor; // to know what floor I'm on 
    [SerializeField] private bool dynmaicFloorPosition;
    [SerializeField] private List<Renderer> ignoreRenderList;
    private void Awake()
    {
       rendererArray = GetComponentsInChildren<Renderer>(true); // include inactive by passing true
    }
    private void Start()
    {
        floor = LevelGrid.Instance.GetFloor(transform.position);

        if (floor == 0 && !dynmaicFloorPosition)
        {
            Destroy(this);
        }

    }
    private void Update()
    {
        if(dynmaicFloorPosition)
        {
            floor = LevelGrid.Instance.GetFloor(transform.position); //kinda wasteful calling this so often but 
            // such few units so its not too big a deal. 
        }
        float cameraHeight = CameraController.Instance.GetCameraHeight();

        float floorHeightOffset = 2f;
        bool showObject = cameraHeight > LevelGrid.FLOOR_HEIGHT * floor + floorHeightOffset;

        if (showObject || floor == 0)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    private void Show()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRenderList.Contains(renderer)) continue;
            renderer.enabled = true;
        }
    }

    private void Hide()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRenderList.Contains(renderer)) continue;
            renderer.enabled = false;
        }
    }
}
