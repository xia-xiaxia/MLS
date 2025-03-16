using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagControl : MonoBehaviour
{

    public bag mbag;
    public GameObject slotGrid;
    public slot slotPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Item items in mbag.ingredient)
        {



            Destroy(items);
        }
    }
}
