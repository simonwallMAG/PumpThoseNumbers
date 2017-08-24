using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

public class NumberTile : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private Color m_unpressedColour = Color.white;
    [SerializeField] private Color m_pressedColour = Color.black;
    [SerializeField] private Color m_textUnpressedColour = Color.black;
    [SerializeField] private Color m_textPressedColour = Color.white;

    private PlayableDirector m_tileDirector;

    private Vector2 m_gridPos = new Vector2();

    private Text m_myNumberText;
    private int m_myNumber;

    private Button m_myButton;
    private Image m_buttonImage;

    private bool m_down;

    private bool m_disabled = false;

    private BoardManager m_boardManager;

    public bool Down
    {
        get { return m_down; }
        set
        {
            m_down = value;
            SetDown();
        }
    }

    public bool Disabled
    {
        get { return m_disabled; }
        set { m_disabled = value; }
    }

    public int MyNumber
    {
        get { return m_myNumber; }
    }

    public Vector2 GridPos
    {
        get { return m_gridPos; }
    }

    public PlayableDirector TileDirector
    {
        get { return m_tileDirector; }
    }

    public void Init(Vector2 gridPos)
    {
        m_tileDirector = GetComponent<PlayableDirector>();
        m_boardManager = GetComponentInParent<BoardManager>();
        m_buttonImage = GetComponent<Image>();
        m_myNumberText = GetComponentInChildren<Text>();
        m_myNumber = UnityEngine.Random.Range(1, 9);
        m_myNumberText.text = MyNumber.ToString();

        m_buttonImage.color = m_unpressedColour;
        m_myNumberText.color = m_textUnpressedColour;
        m_gridPos = gridPos;
    }

    public void GenerateNewNumber()
    {
        m_myNumber = UnityEngine.Random.Range(1, 9);
        m_myNumberText.text = MyNumber.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_disabled)
        {
            if (!Down)
            {
                if (m_boardManager.CanBePressed(GridPos))
                { 
                    if (Input.GetMouseButton(0))
                    {
                        Down = true;
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!m_disabled)
        {
            if (!Down)
            {
                if (m_boardManager.CanBePressed(GridPos))
                {
                    Down = true;
                }
            }
        }
    }

    private void Update()
    {
        if (!m_disabled)
        {
            if (Down)
            {
                if (!Input.GetMouseButton(0))
                {
                    Down = false;

                    m_boardManager.UpdateTargetNumber();
                }
            }
        }
    }

    private void SetDown()
    {
        if (Down)
        {
            m_boardManager.ModifySwipeTotal(MyNumber);
            m_buttonImage.color = m_pressedColour;
            m_myNumberText.color = m_textPressedColour;
        }
        else
        {
            m_buttonImage.color = m_unpressedColour;
            m_myNumberText.color = m_textUnpressedColour;
        }
    }
}
