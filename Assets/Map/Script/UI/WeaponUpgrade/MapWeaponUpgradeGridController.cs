using System;
using System.Collections;
using System.Collections.Generic;
using MainGameNameSpace;
using UnityEngine;
using UnityEngine.UI;
public class MapWeaponUpgradeGridController : MonoBehaviour
{
    [SerializeField] private Image m_Border;
    [SerializeField] private Image m_WeaponImage;
    [SerializeField] private Button m_Btn;
    [SerializeField] private GameObject m_Lock;
    private UnityEngine.Events.UnityAction<WeaponOwnership> m_OnClickAction;
    private WeaponOwnership m_GunOwnership;

    public void Init(WeaponUpgradeGridConfig config){
        m_GunOwnership = config.gunOwnership;
        m_WeaponImage.sprite = config.gunOwnership.Gun.DisplayImage;
        m_OnClickAction = config.onClickAction;
        m_Btn.onClick.AddListener(()=>m_OnClickAction(m_GunOwnership));
        m_Lock.SetActive(config.isLock);
    }

}
public class WeaponUpgradeGridConfig
{
    public bool isLock;
    public WeaponOwnership gunOwnership;
    public UnityEngine.Events.UnityAction<WeaponOwnership> onClickAction;
}
