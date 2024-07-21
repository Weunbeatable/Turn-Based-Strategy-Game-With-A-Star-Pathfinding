using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer MeshRenderer;

    private GridPosition gridPosition;
    private bool isGreen;
    private Action onInteractionComplete;
    private bool isActive;
    private float timer;


    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        SetColorGreen();
    }
    private void Update()
    {
        if (!isActive)
        { return; }


        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            isActive = false;
            onInteractionComplete();
        }
    }
    private void SetColorGreen()
    {
        isGreen = true;
        MeshRenderer.material = greenMaterial;
    }
    private void SetColorRed()
    {
        isGreen = false;
        MeshRenderer.material = redMaterial;
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;


        timer = .5f;


        if (isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
        }
    }
}
