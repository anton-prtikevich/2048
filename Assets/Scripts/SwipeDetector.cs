using UnityEngine;

public enum SwipeDirection
{
    None,
    Left,
    Right,
    Up,
    Down
}

public class SwipeDetector : MonoBehaviour
{
    public static SwipeDirection SwipeDirection { get; private set; }

    private Vector2 touchStart;
    private float minSwipeDistance = 50f;

    private void Update()
    {
        SwipeDirection = SwipeDirection.None;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    break;

                case TouchPhase.Ended:
                    Vector2 swipeDelta = touch.position - touchStart;

                    if (swipeDelta.magnitude < minSwipeDistance)
                        return;

                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        SwipeDirection = swipeDelta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                    }
                    else
                    {
                        SwipeDirection = swipeDelta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                    }
                    break;
            }
        }
    }
}
