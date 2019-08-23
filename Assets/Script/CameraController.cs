using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private Transform targetLook;
    
    public GameObject canvasGUI;

    public GameObject boxs;

    private Vector3 startOffset;
    private Vector3 moveVector;

    private float transition = 0.0f;
    private float animationDuration = 3.0f;
    private Vector3 animationOffset = new Vector3(0, 5, 5);

    // Start is called before the first frame update
    void Start()
    {
        targetLook = GameObject.Find("/Player").transform;
        startOffset = transform.position - targetLook.position;
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = targetLook.position + startOffset;

        // X
        moveVector.x = 0;

        // Y
        moveVector.y = Mathf.Clamp(moveVector.y, 3, 5);
        
        if (transition > 1.0f)
        {
            // memunculkan tamplan GUI
            canvasGUI.SetActive(true);
            boxs.SetActive(true);

            moveVector.y = 3;
            transform.position = moveVector;
        }
        else
        {
            // menyembunyikan tamplan GUI
            canvasGUI.SetActive(false);
            boxs.SetActive(false);

            //Animation at Start Game
            transform.position = Vector3.Lerp(moveVector + animationOffset, moveVector, transition);
            transition += Time.deltaTime * 1 / animationDuration;
            transform.LookAt(targetLook.position + Vector3.up);
        }
    }
}
