using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireballEmitter : MonoBehaviour
{
    [SerializeField] private Transform m_Self;
    [SerializeField] private GameObject m_FireBall;
    [SerializeField] private float m_RanomAngel = 25f;
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    private float m_MaxDelay = 8f;
    private Coroutine SpawnFireBall = null;

    public void ShootFireBall(float damage, float radius, int ballCount){
        for (int i = 0; i < ballCount; i++)
        {
            SpawnFireBall = StartCoroutine(SpawnFireball( damage, radius, Random.Range(0f,m_MaxDelay)));
        }
    }

    private IEnumerator SpawnFireball(float damage, float radius, float delay ){
        yield return new WaitForSeconds(delay);
        if(this.IsDestroyed()){
            yield break;
        }
        
        bool shouldAim = false;
        shouldAim = 0.65f > Random.Range(0f,1f);
        Vector3 randomAngel = Vector3.zero;
        Vector3 targetPos = Vector3.zero;
        if(shouldAim){
            // aim at one random enemy
            targetPos = m_EnemySpawnController.GetOneRandomEnemyPos();
            randomAngel = new Vector3(
                Random.Range(-0.25f,0.25f),
                Random.Range(-0.25f,0.25f),
                Random.Range(-0.25f,0.25f)
            );

        }else{
            randomAngel = new Vector3(
                Random.Range(-1f*m_RanomAngel,m_RanomAngel),
                Random.Range(-1f*m_RanomAngel,m_RanomAngel),
                Random.Range(-1f*m_RanomAngel,m_RanomAngel)
            );

        }
        var fireball = Instantiate(m_FireBall, m_Self);
        fireball.GetComponent<FireballController>().Init(damage, radius);
        if(shouldAim){
            fireball.transform.LookAt(targetPos);
            fireball.transform.eulerAngles += randomAngel;
        }else{
            fireball.transform.eulerAngles += randomAngel;
        }

    }
}
