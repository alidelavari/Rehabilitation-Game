﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictManager : MonoBehaviour
{
    [SerializeField] GameObject predictorCircle;
    [SerializeField] Sprite onAimSprite;
    [SerializeField] Sprite offAimSprite;
    [SerializeField] Sprite timerSprite;
    [SerializeField] float timeBeforeThrown;
    [SerializeField] float rangeVibration;
    [SerializeField] float handMovementRange;
    [SerializeField] GameObject aim;
    [SerializeField] TMPro.TextMeshPro timeText;

    States stateCheckedBefore;
    float timeForCheck = 0.1f;
    float timeChecked;
    int numInAim;

    enum States { 
        onAim,
        offAim,
        onThrow,
        Thrown
    };

    float timeAfterClick;
    float angleWhenClicked;
    GameObject arrow;
    States state;

    // Start is called before the first frame update
    void Start()
    {
        aim = FindObjectOfType<Aim>().gameObject;
        state = States.offAim;
    }

    // Update is called once per frame
    void Update()
    {
        checkState();
    }

    void checkState()
    {
        switch (state)
        {
            case States.onAim:
                setToTick();
                state = checkArrow();
                break;
            case States.offAim:
                setToCross();
                state = checkArrow();
                break;
            case States.onThrow:
                if (timeAfterClick > timeBeforeThrown)
                {
                    arrow.GetComponent<Throw>().throwArrow();
                    state = States.Thrown;
                    timeAfterClick = 0;
                } else {
                    timeAfterClick += Time.deltaTime;
                    timeText.SetText((timeBeforeThrown - timeAfterClick).ToString("0.#"));
                    if (isMoved())
                    {
                        Debug.Log("!!!!");
                        state = checkArrow();
                        timeAfterClick = 0;
                    }
                }
                setToTimer();
                break;
            case States.Thrown:
                setToTick();
                break;
        }
    }

    bool isMoved()
    {
        float theta = transform.rotation.z * 2 / 3 * Mathf.PI * Mathf.Rad2Deg;
        if (checkArrow() == States.offAim)
        {
            return true;
        }
        return false;
    }

    States checkArrow()
    {
        //if (Mathf.Abs(FindObjectOfType<GameHandler>().GetCurrentAngle() - FindObjectOfType<GameHandler>().getTargetAngle() - 90) <= handMovementRange)
        if (timeChecked > timeForCheck)
        {
            timeChecked = 0;
            if (AimChecker.numInAim > 0)
                stateCheckedBefore = States.onAim;
            else
                stateCheckedBefore = States.offAim;
            AimChecker.numInAim = 0;
        }
        timeChecked += Time.deltaTime;
        return stateCheckedBefore;
        
    }

    public void setConsistency(float t)
    {
        timeBeforeThrown = t;
    }

    public void Throw()
    {
        if (state == States.onAim)
        {
            Debug.Log("on aim");
            state = States.onThrow;
            angleWhenClicked = transform.rotation.z * 2 / 3 * Mathf.PI * Mathf.Rad2Deg;
        }
    }

    public void setIdle()
    {
        state = checkArrow();
    }

    public void setArrow(GameObject arrow)
    {
        this.arrow = arrow;
    }

    void setToTick()
    {
        predictorCircle.GetComponent<SpriteRenderer>().sprite = onAimSprite;
        timeText.gameObject.SetActive(false);
    }

    void setToCross()
    {
        predictorCircle.GetComponent<SpriteRenderer>().sprite = offAimSprite;
        timeText.gameObject.SetActive(false);
    }

    void setToTimer()
    {
        predictorCircle.GetComponent<SpriteRenderer>().sprite = timerSprite;
        timeText.gameObject.SetActive(true);
    }
}
