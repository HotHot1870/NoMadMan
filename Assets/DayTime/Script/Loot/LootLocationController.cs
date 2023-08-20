using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using LootLocationControllerNameSpace;

namespace LootLocationControllerNameSpace
{
    public class LootLocationControllerConfig
    {
        public LootLocationScriptable Scriptable ;
        public Action OnClick;
        public int LootBotCount = 0;
    }
}


// the location icon prefab controller 
public class LootLocationController : MonoBehaviour
{
    [SerializeField] private RectTransform m_Self; 
    [SerializeField] private Image m_Icon; 
    [SerializeField] private Button m_Btn; 
    [SerializeField] private GameObject m_LootBotPanret;
    [SerializeField] private TMP_Text m_LootBotCountText;
    private int m_LootBotCount = 0;
    private LootLocationScriptable m_LootLocationScriptable ;

    public void Init(LootLocationControllerConfig config){
        m_LootLocationScriptable = config.Scriptable;
        m_Self.localPosition = new Vector3(
                            config.Scriptable.Position.x, 
                            config.Scriptable.Position.y,
                            0 );

        m_Icon.GetComponent<RectTransform>().sizeDelta = config.Scriptable.Size;
        m_Icon.sprite = config.Scriptable.Icon;
        m_Btn.onClick.AddListener(()=>{config.OnClick?.Invoke();});

        SetLootBotCount(config.LootBotCount);
    }

    public void SetLootBotCount(int lootBotCount = 0){
        m_LootBotPanret.SetActive(lootBotCount > 0);
        m_LootBotCount = lootBotCount;
        if(lootBotCount > 0){
            m_LootBotCountText.text = $"x {lootBotCount}";
        }
        DayTimeManager.GetInstance().SetUsedBotCountText();
    }

    public LootLocationScriptable GetScriptable(){
        return m_LootLocationScriptable;
    }


    public int GetLootBotCount(){
        return m_LootBotCount;
    }


}
