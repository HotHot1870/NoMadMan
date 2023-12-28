using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEmitter : MonoBehaviour
{
    [SerializeField] private Transform m_Self;
    [SerializeField] private GameObject m_FireBall;
    [SerializeField] private float m_RanomAngel = 25f;
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    private float m_MaxDelay = 8f;

    public void ShootFireBall(float damage, float radius, int ballCount){
        for (int i = 0; i < ballCount; i++)
        {
            bool shouldAim = false;
            shouldAim = (BaseDefenceManager.GetInstance().GetLocationScriptable().Level+1) * 0.15f > Random.Range(0f,1f);
            Vector3 randomAngel = Vector3.zero;
            Vector3 targetPos = Vector3.zero;
            if(shouldAim){
                // TODO : aim at one random enemy
                targetPos = m_EnemySpawnController.GetOneRandomEnemyPos();

            }else{
                randomAngel = new Vector3(
                    Random.Range(-1f*m_RanomAngel,m_RanomAngel),
                    Random.Range(-1f*m_RanomAngel,m_RanomAngel),
                    Random.Range(-1f*m_RanomAngel,m_RanomAngel)
                );

            }
            StartCoroutine(SpawnFireball( damage, radius, Random.Range(0f,m_MaxDelay),randomAngel,targetPos));
        }
    }

    private IEnumerator SpawnFireball(float damage, float radius, float delay , Vector3 randomAngel, Vector3 targetPos){
        yield return new WaitForSeconds(delay);
        var fireball = Instantiate(m_FireBall, m_Self);
        fireball.GetComponent<FireballController>().Init(damage, radius);
        if(randomAngel != Vector3.zero){
            fireball.transform.eulerAngles += randomAngel;
        }else{
            fireball.transform.LookAt(targetPos);
        }

    }
}
