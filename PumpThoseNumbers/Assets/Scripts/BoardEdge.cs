using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardEdge : MonoBehaviour
{
    private BoardManager m_boardManager;

    void Awake()
    {
        m_boardManager = GetComponentInParent<BoardManager>();
    }

    private void OnMouseOver()
    {
        bool atleastOneTitleDown = false;
        foreach (var numberTile in m_boardManager.BoardGrid)
        {
            if (numberTile.Down)
            {
                numberTile.Down = false;
                atleastOneTitleDown = true;
            }
        }

        if (atleastOneTitleDown)
        {
            m_boardManager.UpdateTargetNumber();
        }
    }
}
