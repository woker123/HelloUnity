using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeColor : MonoBehaviour
{
    // Start is called before the first frame update
    Material material;
    Button button;
    void Start()
    {
        material = this.gameObject.GetComponent<Renderer>().material;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeMaterialColor()
    {
        Vector3 randVec = Random.onUnitSphere;
        if(material)
        {
            material.color = new Color(randVec.x, randVec.y, randVec.z, 1f);
        }

    }

}
