using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExtendedButtons;
using UnityEditor;

public class WeaponUpgradeRowController : MonoBehaviour
{
    [SerializeField] private Button2D m_UpgradeBtn;
    [SerializeField] private TMP_Text m_StatName;  
    [SerializeField] private TMP_Text m_Cost;  
    [SerializeField] private GameObject m_UpgradeStatSmallBoxPrefab;
    [SerializeField] private Transform m_BlockParent;
    private GunScriptable m_GunScriptable;
    private WeaponUpgradeDetail m_UpgradeDetail;
    private List<WeaponUpgradeBoxController> m_AllBlock = new List<WeaponUpgradeBoxController>();
    private int m_UpgradeCount = 0;


    public void Init( WeaponUpgradeDetail upgradeDetail, GunScriptable gunScriptable){
        m_UpgradeDetail = upgradeDetail;
        m_GunScriptable = gunScriptable;

        string upgradeSaveKey = m_GunScriptable.DisplayName+m_UpgradeDetail.UpgradeStat.ToString();
        m_StatName.text = m_UpgradeDetail.UpgradeStat+" : "+ gunScriptable.GetStatValue(m_UpgradeDetail.UpgradeStat).ToString();

        m_UpgradeCount = (int)MainGameManager.GetInstance().GetData<int>(upgradeSaveKey);

        for (int i = 0; i < m_UpgradeDetail.CostAndValue.Count; i++)
        {
            var block = Instantiate(m_UpgradeStatSmallBoxPrefab,m_BlockParent);
            float upgradeValue =0;
            var blockController = block.GetComponent<WeaponUpgradeBoxController>();
            m_AllBlock.Add(blockController);
            blockController.m_Text.text = m_UpgradeDetail.CostAndValue[i].UpgradeValue;
            blockController.m_BG.color = m_UpgradeCount>i?Color.green:Color.white;

#if UNITY_EDITOR
            EditorUtility.SetDirty(block);
#endif
        }
        

        m_UpgradeBtn.onClick.AddListener(()=>{
            // TODO : On Hold , not on click
            m_UpgradeCount++;
            m_AllBlock[m_UpgradeCount-1].m_BG.color = Color.green;
            m_StatName.text = m_UpgradeDetail.UpgradeStat+" : "+ m_AllBlock[m_UpgradeCount-1].m_Text.text;
            MainGameManager.GetInstance().SaveData<int>(upgradeSaveKey,m_UpgradeCount);
            Debug.Log(upgradeSaveKey+"     "+m_UpgradeCount);
            // TODO : cost goo
        });
    }
}
