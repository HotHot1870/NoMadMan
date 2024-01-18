using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XinBodyPart : EnemyBodyPart
{
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private Color m_HealColor;
    [SerializeField] private Color m_Orange;

    public void SetXinBodyPart(bool isHeal){
        if(isHeal){
            foreach (var item in m_MeshRenderer.materials)
            {
                item.SetColor("_MainColor",m_HealColor);
                SetDamageMod(-0.25f);
                ChangeBodyType(EnemyBodyPartEnum.Heal);
            }
        }else{
            foreach (var item in m_MeshRenderer.materials)
            {
                item.SetColor("_MainColor",m_Orange);
                SetDamageMod(1);
                ChangeBodyType(EnemyBodyPartEnum.Crit);
            }
        }
    }
}
