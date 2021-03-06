﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] float screenWidthInUnit = 30f;
    [SerializeField] float screenHeightInUnit = 1.55f;
    [SerializeField] float movementRange;
    [SerializeField] float rate = 1f;
    [SerializeField] UPersian.Components.RtlText angleText;
    [SerializeField] PlatformHeight platform;
    [SerializeField] float heightonScaleOne = 2.53f;
    Vector3 mainPos;
    float arm = 0.78322239f;
    float initialY;

    float velocity;
    float g;
    float angle;
    float platformHeight;

    int direction = 1;
    // Start is called before the first frame update
   void Start()
    {
        initialY = FindObjectOfType<HandMove>().transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        swing();
    }

    void swing()
    {
        if (movementRange == 0)
            return;
        Vector3 pos = transform.position;
        if (pos.y - mainPos.y > movementRange ||
            mainPos.y - pos.y > movementRange)
        {
            direction *= -1;
        }
        transform.Translate(direction * Vector3.up * rate * Time.deltaTime);
    }

    public void setAngle(int angle)
    {
        angle += 90;
        if(angle < 90)
        {
            platformHeight = -(3.5f / 90) * (angle) + 5.5f;
        }
        else
        {
            platformHeight = 0;
        }
        angle -= 90;

        Vector2 initialPos;
        initialPos.x = FindObjectOfType<HandMove>().transform.position.x + arm * Mathf.Cos(angle * Mathf.Deg2Rad);
        initialPos.y = platformHeight + initialY + arm * Mathf.Sin(angle * Mathf.Deg2Rad);
        this.angle = angle;
        platform.setHeight(platformHeight);
        Vector3 pos = transform.position;
        float x = (pos.x - initialPos.x);
        float theta = angle * Mathf.Deg2Rad;
        pos.y = calculateHeight(x, theta, initialPos);
        transform.position = pos;
        mainPos = pos;


        angleText.text = (angle + 90).ToString() + " درجه";
    }

    public void setLocation(int fromLeft)
    {
        mainPos = transform.position;
        mainPos.x = 1920 * fromLeft / 100 / screenWidthInUnit + 1;
        transform.position = mainPos;
    }

    public void setMoveRange(int movement)
    {
        movementRange = Screen.width * movement / 200 / screenWidthInUnit;
    }

    public void setScale(int angle)
    {
        Vector2 initialPos;
        float x;
        float max = (this.angle + angle / 2f) * Mathf.Deg2Rad;
        float min = (this.angle - angle / 2f) * Mathf.Deg2Rad;
        initialPos.x = FindObjectOfType<HandMove>().transform.position.x + arm * Mathf.Cos(max);
        initialPos.y = platformHeight + initialY + arm * Mathf.Sin(max);
        x = transform.position.x - initialPos.x;
        float maxH = calculateHeight(x, max, initialPos);
        initialPos.x = FindObjectOfType<HandMove>().transform.position.x + arm * Mathf.Cos(min);
        initialPos.y = platformHeight + initialY + arm * Mathf.Sin(min);
        x = transform.position.x - initialPos.x;
        float minH = calculateHeight(x, min, initialPos);
        float scale = Mathf.Clamp(Mathf.Abs(maxH - minH) / heightonScaleOne, 0.3f, 3);
        if (scale > 2)
            scale = 2f;
        transform.localScale = new Vector3(scale, scale, 1);
    }
    private float calculateHeight(float x, float theta, Vector2 initialPos)
    {
        g = -FindObjectOfType<Ground>().getGravityAcceleration();
        velocity = FindObjectOfType<HandMove>().getVelocity();
        float initialY = initialPos.y;
        float y;
        if (GameHandler.arrowIsDynamics)
        {
            y = g / 2 * Mathf.Pow(x / Mathf.Cos(theta) / velocity, 2);
            y += Mathf.Tan(theta) * x;
            y += initialY;
        } 
        else
        {
            y = Mathf.Tan(theta) * x;
            y += initialY;
        }
        return y;
    }
}
