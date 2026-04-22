using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollider : MonoBehaviour
{
    [SerializeField]
    private List<string> layersToIgnore = new List<string>();
    [SerializeField]
    private List<string> tagsToIgnore = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return null; // poèká 1 frame
        SetUpIngoreLayer();
        SetUpIgnoreObjects();
    }

    void SetUpIngoreLayer()
    {
        for (int i = 0; i < layersToIgnore.Count; i++)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layersToIgnore[i]));
            Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layersToIgnore[i]));
        }
    }

    void SetUpIgnoreObjects()
    {
        Collider localCollider = this.GetComponent<Collider>();
        Collider2D localCollider2D = this.GetComponent<Collider2D>();

        for(int i = 0; i <  tagsToIgnore.Count; i++)
        {
            GameObject[] objectsToIgnore = GameObject.FindGameObjectsWithTag(tagsToIgnore[i]);

            foreach (GameObject obj in objectsToIgnore)
            {
                Collider objectCollider = obj.GetComponent<Collider>();
                Collider2D objectCollider2D = obj.GetComponent<Collider2D>();

                if (localCollider != null && objectCollider != null)
                    Physics.IgnoreCollision(localCollider, objectCollider);

                if (localCollider2D != null && objectCollider2D != null)
                    Physics2D.IgnoreCollision(localCollider2D, objectCollider2D);
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
