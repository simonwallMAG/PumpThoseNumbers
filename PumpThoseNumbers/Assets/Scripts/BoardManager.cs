using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Playables;

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

    [SerializeField] private GameObject m_pauseGameCanvas;

    private int m_currentTargetNumber = 100;

    //[SerializeField] private Text m_timerText;
    [SerializeField] private Image m_timerImage;
    [SerializeField] private Text m_timerText;
    [SerializeField] private float m_turnTime = 30;
    [SerializeField] private float m_timeReward = 5;
    private float m_timer = 30;

    private bool m_pauseTimer = false;

    [SerializeField] private GameOverScreen m_gameOverScreen;

    [SerializeField] private Text m_streakText;
    private PlayableDirector m_streakDirector;
    private PlayableDirector m_boardDirector;
    private PlayableDirector m_timerDirector;

    private bool m_gameOver = false;
    private int m_boardsComplete = 0;
    private int m_swipeAmount = 0;

    private float m_refreshBoardTimer = 0;

    private PlayState m_previousPlayState = PlayState.Playing;

    private NumberTile[,] m_boardGrid;

    private bool m_pauseGame = false;

    public NumberTile[,] BoardGrid
    {
        get { return m_boardGrid; }
    }
    
    // Use this for initialization
	void Awake ()
	{
	    Application.targetFrameRate = -1;
	    m_timerDirector = m_timerText.gameObject.GetComponent<PlayableDirector>();
	    m_streakText.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0);
	    m_streakDirector = m_streakText.gameObject.GetComponent<PlayableDirector>();
	    m_boardDirector = GetComponent<PlayableDirector>();
	    m_timer = m_turnTime;
	    m_timerImage.fillAmount = m_timer / m_turnTime;
	    //m_timerText.text = Mathf.CeilToInt(m_timer).ToString();

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
                tile.Init(new Vector2(x,y));
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
                if (!m_pauseTimer)
                {
                    m_timer -= Time.deltaTime;
                    m_timerImage.fillAmount = m_timer / m_turnTime;
                    //m_timerText.text = Mathf.CeilToInt(m_timer).ToString();
                }
            }
            else
            {
                m_gameOver = true;
                GameOver();
            }
        }

        
        if (m_pauseTimer)
        {
            m_refreshBoardTimer += Time.deltaTime;
            if (m_refreshBoardTimer > (m_boardGrid[0, 0].TileDirector.duration / 3) * 2)
            {
                RestartGame(false);
                m_pauseTimer = false;
            }
        }
        if (m_pauseGame)
        {
            m_refreshBoardTimer = 0;
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
        StartCoroutine(BoardComplete());
    }

    private void GameOver()
    {
        m_gameOver = true;
        DisableAllTiles(true);
        m_gameOverScreen.UpdateBoardsCompleteText(m_boardsComplete);
        m_gameOverScreen.gameObject.SetActive(true);

    }

    private void DisableAllTiles(bool disable)
    {
        foreach (var numberTile in m_boardGrid)
        {
            numberTile.Disabled = disable;
        }
    }

    private IEnumerator BoardComplete()
    {
        m_boardsComplete++;
        //m_boardDirector.Play();
        m_pauseTimer = true;
        yield return null;
        m_refreshBoardTimer = 0;
        foreach (var numberTile in m_boardGrid)
        {
            numberTile.TileDirector.Play();
        }
        yield return null;
        m_streakText.text = m_boardsComplete.ToString();
        m_streakDirector.Play();
        yield return null;
        m_timerText.text = "+" + m_timeReward;
        m_timerDirector.Play();
        yield return null;
        m_timer += m_timeReward;
        m_timer = Mathf.CeilToInt(m_timer);
        m_timerImage.fillAmount = m_timer / m_turnTime;
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
            m_timerImage.fillAmount = m_timer / m_turnTime;
            //m_timerText.text = Mathf.CeilToInt(m_timer).ToString();
            m_boardsComplete = 0;
        }

        if (m_pauseGameCanvas.activeSelf)
        {
            m_pauseGameCanvas.SetActive(false);
        }
    }

    public bool CanBePressed(Vector2 gridPos)
    {
        bool atleastOneTileDown = false;

        foreach (var numberTile in m_boardGrid)
        {
            if (numberTile.Down)
            {
                atleastOneTileDown = true;
                break;
            }
        }

        if (!atleastOneTileDown)
        {
            return true;
        }

        IEnumerable<NumberTile> flattenedTiles = Flatten(m_boardGrid);
        List<NumberTile> pressedTiles = flattenedTiles.Select(x => x).Where(x => x.Down).ToList();
        List<Vector2> possiblePositions = new List<Vector2>();
        possiblePositions.Add(new Vector2(gridPos.x, gridPos.y + 1));
        possiblePositions.Add(new Vector2(gridPos.x, gridPos.y - 1));
        possiblePositions.Add(new Vector2(gridPos.x + 1, gridPos.y));
        possiblePositions.Add(new Vector2(gridPos.x - 1, gridPos.y));

        possiblePositions.Add(new Vector2(gridPos.x + 1, gridPos.y + 1));
        possiblePositions.Add(new Vector2(gridPos.x - 1, gridPos.y - 1));
        possiblePositions.Add(new Vector2(gridPos.x + 1, gridPos.y - 1));
        possiblePositions.Add(new Vector2(gridPos.x - 1, gridPos.y + 1));

        bool nextToAValidTile = false;
        foreach (var tile in pressedTiles)
        {
            if (possiblePositions.Contains(tile.GridPos))
            {
                nextToAValidTile = true;
                break;
            }
        }

        return nextToAValidTile;
    }

    public IEnumerable<T> Flatten<T>(T[,] map)
    {
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                yield return map[row, col];
            }
        }
    }

    public void PauseGame()
    {
        transform.parent.gameObject.SetActive(false);
        m_pauseTimer = true;
        DisableAllTiles(true);
        m_pauseGameCanvas.SetActive(true);
        m_pauseGame = true;
        
    }

    public void ResumeGame()
    {
        transform.parent.gameObject.SetActive(true);
        m_pauseGame = false;
        m_pauseTimer = false;
        DisableAllTiles(false);
        m_pauseGameCanvas.SetActive(false);
    }
}
