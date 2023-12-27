using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEmitter : MonoBehaviour
{
    [SerializeField] private Transform m_Self;
    [SerializeField] private GameObject m_FireBall;
    [SerializeField] private float m_MaxDelay = 5f;

    public void ShootFireBall(float damage, float radius, int ballCount){
        for (int i = 0; i < ballCount; i++)
        {
            StartCoroutine(SpawnFireball( damage, radius, Random.Range(0,m_MaxDelay)));
        }
    }

    private IEnumerator SpawnFireball(float damage, float radius, float delay){
        yield return new WaitForSeconds(delay);
        var fireball = Instantiate(m_FireBall, m_Self);
        fireball.GetComponent<FireballController>().Init(damage, radius);
        // TODO : random look angel
        fireball.transform.eulerAngles += new Vector3(
            Random.Range(-10f,10f),
            Random.Range(-10f,10f),
            Random.Range(-10f,10f)
        );

    }
}
