using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public List<SpringConntroller> springs = new List<SpringConntroller>();

    public string targetTag;
    List<Collider> triggeredObjects = new List<Collider>();
    public List<SpringConntroller> springController = new List<SpringConntroller>();
    private void OnTriggerEnter(Collider other)
    {    
        if (!triggeredObjects.Contains(other) && other.CompareTag(targetTag))
        {

            springController.Add(other.GetComponentInParent<SpringConntroller>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            springController.Remove(other.GetComponentInParent<SpringConntroller>());
        }
    }
}
