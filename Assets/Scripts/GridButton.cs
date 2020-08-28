using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GameController controller;
    private Image front;
    private Image back;
    private bool isPressed;
    private bool move;
    private bool setStartingPosition = true;
    private Vector2 startingPosition;
    private Vector2 pushedPosition;
    public GridColor Color { get; private set; } = GridColor.BLUE;

    void Awake()
    {
        controller = GameObject.Find("Controller").GetComponent<GameController>();
        front = transform.Find("Front").GetComponent<Image>();
        back = transform.Find("Back").GetComponent<Image>();
    }

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

    public void ChangeColor()
    {
        switch (Color)
        {
            case GridColor.BLUE:
                Color = GridColor.YELLOW;
                break;
            case GridColor.YELLOW:
                Color = GridColor.RED;
                break;
            case GridColor.RED:
                Color = GridColor.BLUE;
                break;
        }
    }

    public void RenderNewColor()
    {
        switch (Color)
        {
            case GridColor.BLUE:
                front.color = new Color32(92, 190, 229, 255);
                back.color = new Color32(92, 190, 229, 255);
                break;
            case GridColor.YELLOW:
                front.color = new Color32(255, 198, 62, 255);
                back.color = new Color32(255, 198, 62, 255);
                break;
            case GridColor.RED:
                front.color = new Color32(221, 101, 85, 255);
                back.color = new Color32(221, 101, 85, 255);
                break;
        }
    }

    public void PressChangeColor()
    {
        if (setStartingPosition)
        {
            setStartingPosition = false;
            startingPosition = front.transform.position;
            pushedPosition = new Vector2(startingPosition.x, startingPosition.y - 0.07f);
        }
        isPressed = true;
        move = true;
        ChangeColor();
        RenderNewColor();
    }

    public void Release()
    {
        isPressed = false;
        move = true;
    }

    public void SimulatePress()
    {

        ChangeColor();
        string[] coordinates = name.Split('_');
        int row = int.Parse(coordinates[0]);
        int column = int.Parse(coordinates[1]);
        if (column < 6)
        {
            GameObject.Find(row + "_" + (column + 1)).GetComponent<GridButton>().ChangeColor();
        }
        if (column > 1)
        {
            GameObject.Find(row + "_" + (column - 1)).GetComponent<GridButton>().ChangeColor();
        }
        if (row < 6)
        {
            GameObject.Find((row + 1) + "_" + column).GetComponent<GridButton>().ChangeColor();
        }
        if (row > 1)
        {
            GameObject.Find((row - 1) + "_" + column).GetComponent<GridButton>().ChangeColor();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        PressChangeColor();
        string[] coordinates = name.Split('_');
        int row = int.Parse(coordinates[0]);
        int column = int.Parse(coordinates[1]);
        if (column < 6)
        {
            GameObject.Find(row + "_" + (column + 1)).GetComponent<GridButton>().PressChangeColor();
        }
        if (column > 1)
        {
            GameObject.Find(row + "_" + (column - 1)).GetComponent<GridButton>().PressChangeColor();
        }
        if (row < 6)
        {
            GameObject.Find((row + 1) + "_" + column).GetComponent<GridButton>().PressChangeColor();
        }
        if (row > 1)
        {
            GameObject.Find((row - 1) + "_" + column).GetComponent<GridButton>().PressChangeColor();
        }
        controller.CheckVictory();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        string[] coordinates = name.Split('_');
        int row = int.Parse(coordinates[0]);
        int column = int.Parse(coordinates[1]);
        Release();
        if (column < 6)
        {
            GameObject.Find(row + "_" + (column + 1)).GetComponent<GridButton>().Release();
        }
        if (column > 1)
        {
            GameObject.Find(row + "_" + (column - 1)).GetComponent<GridButton>().Release();
        }
        if (row < 6)
        {
            GameObject.Find((row + 1) + "_" + column).GetComponent<GridButton>().Release();
        }
        if (row > 1)
        {
            GameObject.Find((row - 1) + "_" + column).GetComponent<GridButton>().Release();
        }
    }

    public void Reset()
    {
        if (!setStartingPosition)
        {
            front.transform.position = startingPosition;
        }
    }

    public enum GridColor
    {
        BLUE,
        YELLOW,
        RED
    }
}
