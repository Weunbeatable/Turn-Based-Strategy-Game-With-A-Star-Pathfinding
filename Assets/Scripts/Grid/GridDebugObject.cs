using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro gridText;
    private object gridObject; // generic object
    
    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }

    protected virtual void Update()
    {
      gridText.text = gridObject.ToString();
    }
}
