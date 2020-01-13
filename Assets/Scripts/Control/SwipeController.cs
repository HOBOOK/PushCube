using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour
{
    private bool tap, swipeUp, swipeDown, swipeLeft, swipeRight;
    private bool isDragging = false;
    private Vector2 startTouch, swipeDelta;
    int swipeAbleControl = 0;
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool Tap { get { return tap; } set { } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }

    private void Update()
    {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        #region 마우스 인풋
        if (Input.GetMouseButtonDown(0)&&!isTouchOnUI())
        {
            tap = true;
            isDragging = true;
            startTouch = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0) && !isTouchOnUI())
        {
            Reset();
        }
        #endregion

        #region 모바일 인풋
        if (Input.touches.Length > 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began && !isTouchOnUI())
            {
                tap = true;
                isDragging = true;
                startTouch = Input.touches[0].position;
            }
            else if((Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) && !isTouchOnUI())
            {
                Reset();
            }
        }
        #endregion

        // 스와이프 거리
        swipeDelta = Vector2.zero;
        if(isDragging)
        {
            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }

        // 스와이프 방향
        if(swipeDelta.magnitude > 125)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            float a = x / y;
            if(a>=0)//상하
            {
                if (x >= 0)
                    swipeUp = true;
                else
                    swipeDown = true;

            }
            else //좌우
            {
                if (y >= 0)
                    swipeLeft = true;
                else
                    swipeRight = true;
            }
            SwipeRestrictControl();
            Reset();
        }
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }

    bool isTouchOnUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void SwipeRestrictControl()
    {
        switch(swipeAbleControl)
        {
            case 1:
                swipeDown = false;
                swipeLeft = false;
                swipeRight = false;
                break;
            case 2:
                swipeUp = false;
                swipeLeft = false;
                swipeRight = false;
                break;
            case 3:
                swipeUp = false;
                swipeDown= false;
                swipeRight = false;
                break;
            case 4:
                swipeUp = false;
                swipeDown= false;
                swipeLeft = false;
                break;
        }
    }
    public void SwipeRestrict(int dir)
    {
        swipeAbleControl = dir;
    }
    public void SwipeRestrictClear()
    {
        swipeAbleControl = 0;
    }
}
