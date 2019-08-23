using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    private Transform targetLook;
    
    private Vector3 startOffset;
    private Vector3 moveVector;

    // Start is called before the first frame update
    void Start()
    {
        targetLook = GameObject.Find("/Player").transform;
        startOffset = transform.position - targetLook.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetLook.transform.position.y == 1)
            Start();
        moveVector = targetLook.position + startOffset;

        // X
        moveVector.x = 0;

        // Y
        moveVector.y = 0.5f;
        
        // Cube Follow User
        transform.position = moveVector;
    }
    
}
