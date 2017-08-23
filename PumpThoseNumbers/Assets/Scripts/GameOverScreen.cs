using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField]
    private Text m_boardsCompleteText;

    public void UpdateBoardsCompleteText(int amountOfBoardsComplete)
    {
        m_boardsCompleteText.text = amountOfBoardsComplete.ToString();
    }
}
