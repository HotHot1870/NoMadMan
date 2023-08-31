using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    [SerializeField] private EnemyController m_EnemyController;
    [SerializeField][Range(0f,3f)] private float m_DamageMod = 1;

    public void OnHit(float damage){
        m_EnemyController.ChangeHp(damage*m_DamageMod *-1);
    }

}
