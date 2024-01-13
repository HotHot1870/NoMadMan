using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBlockController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Image m_Image;
    [SerializeField] private TMP_Text m_CountText;
    [SerializeField] private Button m_OnClickBtn;

    public void Init(EnemyScriptable enemyScriptable){
        // TODO : enemy block 
        //m_OnClickBtn.onClick.AddListener();
        m_Image.sprite = enemyScriptable.DisplayImage;

        
    }

}
