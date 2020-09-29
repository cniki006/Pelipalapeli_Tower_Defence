using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWD_EnemyAnimation : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = transform.localRotation * Quaternion.Inverse(transform.rotation);

    }
    private void LateUpdate()
    {
        
    }
}
