using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackHorizontalPosition : MonoBehaviour
{
    public Transform target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, WaterLevelManager.Instance.waterLevel, target.transform.position.z);
    }
}
