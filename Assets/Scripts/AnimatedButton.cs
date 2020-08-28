using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected Image front;
    private bool isPressed;
    private bool move;
    private bool setStartingPosition = true;
    private Vector2 startingPosition;
    private Vector2 pushedPosition;

    void Start() => front = transform.Find("Front").GetComponent<Image>();

    void Update()
    {
        if (move)
        {
            if (isPressed)
            {
                if ((front.transform.position = Vector2.MoveTowards(front.transform.position, pushedPosition, Time.deltaTime)).y == pushedPosition.y)
                {
                    move = false;
                }
            }
            else
            {
                if ((front.transform.position = Vector2.MoveTowards(front.transform.position, startingPosition, Time.deltaTime)).y == startingPosition.y)
                {
                    move = false;
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (setStartingPosition)
        {
            setStartingPosition = false;
            startingPosition = front.transform.position;
            pushedPosition = new Vector2(startingPosition.x, startingPosition.y - 0.07f);
        }
        isPressed = true;
        move = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        move = true;
    }

    public void Reset()
    {
        if (!setStartingPosition)
        {
            front.transform.position = startingPosition;
        }
    }
}
