using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            this.gameObject.transform.position -= new Vector3(0, 0.3f, 0);

        }
            

    }

    private void OnCollisionEnter(Collision other) {
        //Debug.Log("Collided!");
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("On trigger enter");
    }
}
