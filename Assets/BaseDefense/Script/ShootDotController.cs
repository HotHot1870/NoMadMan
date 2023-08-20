using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootDotController : MonoBehaviour
{
    [SerializeField] private Image m_Image;

    [SerializeField] private Color m_MissColor = Color.grey;
    [SerializeField] private Color m_HitColor = Color.yellow;
    [SerializeField] private Color m_CritColor = Color.red;

    public void OnMiss(){
        m_Image.color = m_MissColor;
    }

    public void OnHit(){
        m_Image.color = m_HitColor;
    }

    public void OnCrit(){
        m_Image.color = m_CritColor;
    }
}
