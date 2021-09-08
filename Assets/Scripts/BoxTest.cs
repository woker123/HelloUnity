using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var children = GetComponentsInChildren<Transform>();
        foreach(var child in children)
        {
            Debug.Log(child.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
