using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] class TwoThings { public LayerMask Layer1; public LayerMask Layer2; }

public class CollisionManager : MonoBehaviour
{
    [SerializeField]
    private List<TwoThings> layersToIgnore = new List<TwoThings>();
    [SerializeField]
    public LayerMask RaycastIgnoreLayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpIngoreLayer();
    }

    void SetUpIngoreLayer()
    {
        foreach (var pair in layersToIgnore)
        {
            foreach (int layer1 in GetLayersFromMask(pair.Layer1))
            {
                foreach (int layer2 in GetLayersFromMask(pair.Layer2))
                {
                    Physics2D.IgnoreLayerCollision(layer1, layer2, true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerable<int> GetLayersFromMask(LayerMask mask)
    {
        int maskValue = mask.value;

        for (int i = 0; i < 32; i++)
        {
            if ((maskValue & (1 << i)) != 0)
                yield return i;
        }
    }
}
