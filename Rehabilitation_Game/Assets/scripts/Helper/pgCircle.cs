﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Steps;

public class pgCircle : MonoBehaviour
{
    float angle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float current_angle = GetComponent<RectTransform>().rotation.z * Mathf.Rad2Deg;
        Debug.Log(current_angle);
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            angle);
    }

    public void setAngle(float angle)
    {
        this.angle = angle;
    }
}