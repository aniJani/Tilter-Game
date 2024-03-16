using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject Ground;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Trigger"))
        {
            Instantiate(Ground, new Vector3(0, 0, 49), Quaternion.identity);
        }
    }
}
