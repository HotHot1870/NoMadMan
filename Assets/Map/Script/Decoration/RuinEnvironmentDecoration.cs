using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RuinEnvironmentDecoration : MonoBehaviour
{
    [SerializeField] private Transform m_EnemySpawnPos;
    [SerializeField] private GameObject m_EnemyPrefab;
    [SerializeField] private Transform m_BulletSpawnPos;
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private GameObject m_ExplodeEffect;
    [SerializeField] private AudioClip m_DistanceShot;
    [SerializeField] private AudioSource m_AudioPlayer;
    // Start is called before the first frame update
        void Start(){
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioPlayer);
        InvokeRepeating("RepeatSpawnEnemy", 2, 7f);
        // do one more time to warm up
        RepeatSpawnEnemy();
    }

    private void RepeatSpawnEnemy(){
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy(){
        yield return new WaitForSeconds(UnityEngine.Random.Range(0,6));

        Vector3 startPos = m_EnemySpawnPos.position + new Vector3(UnityEngine.Random.Range(-4f,4f),UnityEngine.Random.Range(-1f,1f),UnityEngine.Random.Range(-4f,4f));
        // Spawn effect
        var explosion = Instantiate(m_ExplodeEffect,startPos,quaternion.identity,this.transform);
        Destroy(explosion,4);

        // spawn enemy
        var newGhost = Instantiate(m_EnemyPrefab,startPos,quaternion.identity,this.transform);
        newGhost.transform.localScale = Vector3.one*UnityEngine.Random.Range(0.5f,2f);
        newGhost.transform.LookAt(m_BulletSpawnPos.position);
        StartCoroutine(GhostMove(newGhost.transform,startPos));
        yield return new WaitForSeconds(UnityEngine.Random.Range(2,8));
        // shoot bullet
        var targetPosForBullet = newGhost.transform.position+Vector3.up*0.5f;
        var newBullet = Instantiate(m_BulletPrefab,m_BulletSpawnPos.position,quaternion.identity,this.transform);
        newBullet.transform.LookAt(targetPosForBullet);
        newBullet.transform.localScale = Vector3.one + new Vector3(1.5f,1.5f,0);
        // shoot sound
        m_AudioPlayer.PlayOneShot(m_DistanceShot);
        float passTime = 0f;
        float duration = 0.125f;
        while (passTime<duration)
        {
            passTime += Time.deltaTime;
            newBullet.transform.position = Vector3.Lerp(m_BulletSpawnPos.position,targetPosForBullet,passTime/duration);
            yield return null;
        }
        // Explode ghost effect
        var explosionKill = Instantiate(m_ExplodeEffect,targetPosForBullet,quaternion.identity,this.transform);
        Destroy(explosionKill,4);
        // Destory All
        Destroy(newGhost);
        Destroy(newBullet);
    }
    private IEnumerator GhostMove(Transform ghost, Vector3 startPos){
        float passTime = 0f;
        float duration = 25f;
        while (passTime<duration && ghost != null)
        {
            passTime += Time.deltaTime;
            ghost.transform.position = Vector3.Lerp(startPos, m_BulletSpawnPos.position,passTime/duration);
            yield return null;
        }
    }
}
