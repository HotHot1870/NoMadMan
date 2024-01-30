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
    [SerializeField] private Animator m_Animator;

    public void Init(EnemyScriptable enemyScriptable){
        // enemy block 
        m_Image.sprite = enemyScriptable.DisplayImage;

        m_CountText.text = "";
        
    }

    public void PlayGrowAnimation(){
        m_Animator.Play("Grow");
    }

    public void SetText(string text){
        m_CountText.text = text;

    }

}
