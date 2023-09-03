using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    private float m_WallHp = 1000f;
    [SerializeField] private float m_WallMaxHp = 1000f;

    public void Init(float wallHp){
        m_WallHp = wallHp;

    }

    public void ChangeHp(float changeAmount){
        m_WallHp += changeAmount;
        m_WallHp = Mathf.Clamp(m_WallHp,0,m_WallMaxHp);
        if(m_WallHp <= 0){
            Debug.Log("lose");
        }
        BaseDefenseManager.GetInstance().GetBaseDefenseUIController().SetWallHpBar(m_WallHp,m_WallMaxHp);
    }
}
