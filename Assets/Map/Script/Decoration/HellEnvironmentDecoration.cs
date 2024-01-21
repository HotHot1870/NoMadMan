using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HellEnvironmentDecoration : MonoBehaviour
{
    [SerializeField] private List<Transform> m_EnemySpawnPos = new List<Transform>() ;
    [SerializeField] private GameObject m_EnemyPrefab;
    [SerializeField] private GameObject m_RocketPrefab;
    [SerializeField] private GameObject m_ExplosionPrefab;

    void Start(){
        InvokeRepeating("RepeatSpawnEnemy", 0, 4f);
        // do one more time to warm up
        RepeatSpawnEnemy();
    }

    private void RepeatSpawnEnemy(){
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy(){
        yield return new WaitForSeconds(UnityEngine.Random.Range(0,6));
        // spawn enemy
        int randomInt = UnityEngine.Random.Range(0,m_EnemySpawnPos.Count);
        var enemyGroup = Instantiate(m_EnemyPrefab,m_EnemySpawnPos[randomInt].position,quaternion.identity,this.transform);
        enemyGroup.transform.eulerAngles += Vector3.up*UnityEngine.Random.Range(-180,180);
        yield return new WaitForSeconds(UnityEngine.Random.Range(4,8));
        // shoot rocket
        Vector3 rocketPos = m_EnemySpawnPos[randomInt].position+ new Vector3(
                    UnityEngine.Random.Range(-200f,-50f),
                    100,
                    UnityEngine.Random.Range(-50f,50f)
                );
        var rocket = Instantiate(m_RocketPrefab,rocketPos,quaternion.identity);
        rocket.transform.LookAt(enemyGroup.transform);
        // move rocket
        float passTime = 0f;
        float duration = 1.25f;
        while (passTime<duration)
        {
            passTime += Time.deltaTime;
            rocket.transform.position = Vector3.Lerp(rocketPos,m_EnemySpawnPos[randomInt].position,passTime/duration);
            yield return null;
        }
        // kill all
        Destroy(enemyGroup);
        // Explosion
        var explosion = Instantiate(m_ExplosionPrefab,m_EnemySpawnPos[randomInt].position,quaternion.identity,this.transform);
        explosion.GetComponent<ExplosionController>().Init(0 , 20);
        // detach rocket smock from rocket , prevent smock destroy on rocket destroy
        rocket.GetComponent<ProjectileController>().SetParent(this.transform);
        Destroy(rocket);

    }
}
