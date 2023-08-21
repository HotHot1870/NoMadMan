using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MainGameNameSpace;
using UnityEngine.SceneManagement;

public class BaseDefenseTimmyPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;

    [SerializeField] private Button m_NextBtn;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Wall;
    [SerializeField] private BaseDefenseResultPanelRowContent m_Bot;


    private void Start()
    {
        m_NextBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("DayTime");
        });

        m_Self.SetActive(false);
    }

    public void Init()
    {
        m_Self.SetActive(true);

    }
}
