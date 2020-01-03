using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMove : MonoBehaviour
{

    public Transform camTransform;

    public float coeffX = 0.1f;
    public float coeffY = 0.1f;

    Transform tr;
    Vector3 prevposition;
    Vector3 parallaxOffset = Vector3.zero;

    void Start()
    {
        prevposition = camTransform.position;
        tr = GetComponent<Transform>();
    }


    void LateUpdate()
    {
        parallaxOffset += new Vector3((-camTransform.position + prevposition).x * coeffX, 0);
        tr.localPosition = new Vector3(parallaxOffset.x, tr.localPosition.y);
        prevposition = camTransform.position;
    }
}
