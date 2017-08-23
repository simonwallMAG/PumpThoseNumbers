using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private int m_boardWidth = 4;

    [SerializeField]
    private int m_boardHeight = 4;

    [SerializeField]
    private GameObject m_tilePrefab;

    [SerializeField]
    private Text m_swipeTotal;
    private int m_currentSwipeTotal;

    [SerializeField]
    private Text m_targetNumber;
    private int m_currentTargetNumber = 100;

    [SerializeField] private Text m_timerText;
    [SerializeField] private float m_turnTime = 30;
    [SerializeField] private float m_timeReward = 5;
    private float m_timer = 30;

    [SerializeField] private GameOverScreen m_gameOverScreen;

    private bool m_gameOver = false;
    private int m_boardsComplete = 0;
    private int m_swipeAmount = 0;

    private NumberTile[,] m_boardGrid;

    public NumberTile[,] BoardGrid
    {
        get { return m_boardGrid; }
    }

    // Use this for initialization
	void Awake ()
	{
	    m_timer = m_turnTime;
	    m_timerText.text = Mathf.CeilToInt(m_timer).ToString();

		m_boardGrid = new NumberTile[m_boardWidth,m_boardHeight];
	    int boardTotal = 0;
        for (int y = 0; y < BoardGrid.GetLength(1); y++)
        {
            for (int x = 0; x < BoardGrid.GetLength(0); x++)
            {
                GameObject tileObject = Instantiate(m_tilePrefab);
                tileObject.transform.SetParent(transform, false);
                NumberTile tile = tileObject.GetComponent<NumberTile>();
                BoardGrid[x, y] = tile;
                PositionTile(tileObject, x, y);
                tile.Init();
                boardTotal += tile.MyNumber;
            }
        }

	    m_swipeAmount = 0;
	    m_currentSwipeTotal = 0;
	    m_currentTargetNumber = UnityEngine.Random.Range(boardTotal / 4, boardTotal - 1);

	    m_swipeTotal.text = m_currentSwipeTotal.ToString();
	    m_targetNumber.text = m_currentTargetNumber.ToString();
    }

    private void Update()
    {
        if (!m_gameOver)
        {
            if (m_timer >= 0)
            {
                m_timer -= Time.deltaTime;
                m_timerText.text = Mathf.CeilToInt(m_timer).ToString();
            }
            else
            {
                m_gameOver = true;
                GameOver();
            }
        }
    }

    private void PositionTile(GameObject tile, int xPos, int yPos)
    {
        RectTransform rectTransform = tile.GetComponent<RectTransform>();
        float width = 1 / (float)m_boardWidth;
        float height = 1 / (float)m_boardHeight;
        rectTransform.anchorMin = new Vector2(0.01f + width * xPos, 0.01f + height * yPos);
        rectTransform.anchorMax = new Vector2(-0.01f + width * (xPos + 1), -0.01f + height * (yPos + 1));
    }

    

    public void ModifySwipeTotal(int amount)
    {
        m_swipeAmount++;
        m_currentSwipeTotal += amount;
        m_swipeTotal.text = m_currentSwipeTotal.ToString();
    }

    public void UpdateTargetNumber()
    {
        if (m_swipeAmount > 1)
        {
            m_currentTargetNumber -= m_currentSwipeTotal;
            m_targetNumber.text = m_currentTargetNumber.ToString();
            m_currentSwipeTotal = 0;
            m_swipeAmount = 0;
            m_swipeTotal.text = m_currentSwipeTotal.ToString();
            if (m_currentTargetNumber == 0)
            {
                // WIN!
                GameWon();

            }
            if (m_currentTargetNumber < 0)
            {
                GameOver();
            }
        }
        else
        {
            m_swipeAmount = 0;
            m_currentSwipeTotal = 0;
        }
    }

    private void GameWon()
    {
        BoardComplete();
    }

    private void GameOver()
    {
        m_gameOver = true;
        DisableAllTiles();
        m_gameOverScreen.UpdateBoardsCompleteText(m_boardsComplete);
        m_gameOverScreen.gameObject.SetActive(true);

    }

    private void DisableAllTiles()
    {
        foreach (var numberTile in m_boardGrid)
        {
            numberTile.Disabled = true;
        }
    }

    private void BoardComplete()
    {
        RestartGame(false);
        m_boardsComplete++;
        m_timer += m_timeReward;
        m_timer = Mathf.CeilToInt(m_timer);
    }

    public void RestartGame(bool resetTimer = true)
    {
        int total = 0;
        foreach (var numberTile in m_boardGrid)
        {
            numberTile.GenerateNewNumber();
            numberTile.Down = false;
            numberTile.Disabled = false;
            total += numberTile.MyNumber;
        }

        m_swipeAmount = 0;
        m_currentSwipeTotal = 0;
        m_currentTargetNumber = UnityEngine.Random.Range(total/4, total - 1);

        m_swipeTotal.text = m_currentSwipeTotal.ToString();
        m_targetNumber.text = m_currentTargetNumber.ToString();
        
        m_gameOverScreen.gameObject.SetActive(false);

        m_gameOver = false;

        if (resetTimer)
        {
            m_timer = m_turnTime;
            m_timerText.text = Mathf.CeilToInt(m_timer).ToString();
            m_boardsComplete = 0;
        }
    }
}
