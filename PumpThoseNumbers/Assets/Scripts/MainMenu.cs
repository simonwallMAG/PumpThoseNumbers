using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_gameCanvas;

    public void Play()
    {
        m_gameCanvas.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
