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
        // before
        var resourceData = MainGameManager.GetInstance().GetOwnedResources();
        m_Wall.Before.text = ((int)MainGameManager.GetInstance().GetWallCurHp()).ToString();
        m_Bot.Before.text = resourceData.Bot.ToString();

        // changes
        float wallHpGain = MainGameManager.GetInstance().GetWallMaxHp() * Random.Range(0.1f,0.2f);
        MainGameManager.GetInstance().ChangeWallHp(wallHpGain);
        m_Wall.Changes.text = "+"+((int)wallHpGain).ToString();

        int botGain = Random.Range(1, 4);
        MainGameManager.GetInstance().GetOwnedResources().Change(new ResourcesRecord { Bot = botGain });
        m_Bot.Changes.text = $"+{botGain.ToString()}";

        // after
        m_Wall.After.text = ((int)MainGameManager.GetInstance().GetWallCurHp()).ToString();
        m_Bot.After.text = MainGameManager.GetInstance().GetOwnedResources().Bot.ToString();

    }
}
