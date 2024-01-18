
using System.Collections.Generic;
using BaseDefenceNameSpace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class XinHpController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_HpParent;
    [SerializeField] private GameObject m_SubHpParent;
    [SerializeField] private Image m_HpImage;
    [SerializeField] private Image m_SubHpImage;
    [SerializeField] private List<GameObject> m_Skulls = new List<GameObject>();
    private float m_CurHp = 100;
    private float m_SubHp = 100;
    private float m_MaxSubHp = 100;
    private EnemyScriptable m_XinScriptable;
    private int m_SkullCount =0;
    private XinController m_XinController;

    void Start(){
        m_Self.SetActive(false);

    }

    public void SetHpVisable(bool isVisable){
        m_HpParent.SetActive(isVisable);
        m_SubHpParent.SetActive(isVisable);

    }

    public void Init(EnemyScriptable xin, XinController xincontroller){
        m_Self.SetActive(true);
        m_XinScriptable = xin;
        m_XinController = xincontroller;
        m_CurHp = m_XinScriptable.MaxHp;
        m_SubHp = m_XinScriptable.MaxHp/25f;
        m_MaxSubHp = m_XinScriptable.MaxHp/25f;
        m_SkullCount =0;

        m_HpImage.fillAmount = m_CurHp/m_XinScriptable.MaxHp;
        m_SubHpImage.fillAmount = m_SubHp/m_MaxSubHp;
        for (int i = 0; i < m_Skulls.Count; i++)
        {
            m_Skulls[i].SetActive(i<m_SkullCount);
        }

    }

    public void ResetRound(){
        m_SubHp = m_MaxSubHp;
        m_SubHpImage.fillAmount = m_SubHp/m_MaxSubHp;
        m_SkullCount = 0;
        for (int i = 0; i < m_Skulls.Count; i++)
        {
            m_Skulls[i].SetActive(i<m_SkullCount);
        }
        
    }

    
    public void GainOneStar(){
        if(m_SkullCount >= m_Skulls.Count)
            return;

        m_SkullCount++;
        for (int i = 0; i < m_Skulls.Count; i++)
        {
            m_Skulls[i].SetActive(i<m_SkullCount);
        }
        m_XinController.ResetAllWeakPoint();
    }


    public int GetSkullCount(){
        return m_SkullCount;
    }

    public void ChangeHp(float change){
        m_CurHp += change;
        if(m_CurHp<=0){
            // TODO : close Hp bar
            m_XinController.StartCoroutine( m_XinController.SetResult());
            m_XinController.m_IsDyingEffect = true;
            m_Self.SetActive(false);
            return;
        }

        if(change<0){
            m_SubHp += change;
            if( m_SubHp <= 0 && m_SkullCount < m_Skulls.Count ){
                // sub hp to 0
                m_SubHp = m_MaxSubHp - m_SubHp;
                GainOneStar();

            }

        }
        
        m_HpImage.fillAmount = m_CurHp/m_XinScriptable.MaxHp;
        m_SubHpImage.fillAmount = m_SubHp/m_MaxSubHp;

    }
}
