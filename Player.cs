using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * 3f, 0, Input.GetAxis("Vertical") * Time.deltaTime * 3f));
        
    }
}
