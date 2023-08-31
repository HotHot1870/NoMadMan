using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseDefenseNameSpace;
using Unity.Collections;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private Transform m_EnemyParent;
    [SerializeField] private WavesScriptable m_WavesData;
    [SerializeField] private Transform m_GroundSpawnerCenter;
    [SerializeField] private float m_GroundSpawnerWidth = 40;
    [SerializeField] private Transform m_WallCenter;
    [SerializeField] private float m_WallWidth = 10;


    private int m_WaveCount = 0;
    private float m_TimePassed = 0;
    private bool m_IsFinalWaveStarted = false;
    private float m_MaxSpawnDelay = 10f;
    private List<GameObject> m_AllSpawnedEnemy = new List<GameObject>();

    private void Start()
    {
        StartNextNormalWave();
        BaseDefenseManager.GetInstance().m_ShootUpdateAction += EnemySpawnUpdate;
    }

    private void EnemySpawnUpdate(){
        if(m_IsFinalWaveStarted){
            if(m_EnemyParent.childCount<=0){
                // all dead
                Debug.Log("win");
            }
            return;
        }

        // check time pass to prevent all wave start at begin
        if(m_TimePassed>=m_MaxSpawnDelay){
            // is all enemy dead
            if(m_EnemyParent.childCount<=0){
                // all dead
                m_TimePassed = 0;
                StartNextNormalWave();
            }
        }else{
            m_TimePassed += Time.deltaTime;
        }
    }

    private void StartNextNormalWave()
    {
        if(m_WaveCount>=m_WavesData.NormalWavesCount){
            m_IsFinalWaveStarted = true;
        }
        float dangerValue = m_IsFinalWaveStarted?m_WavesData.FinalWaveDangerValue:m_WavesData.NormalWavesDangerValue;
        List<EnemyScriptable> taregtEnemyTypes = m_IsFinalWaveStarted?m_WavesData.FinalWaveEnemy:m_WavesData.NormalWaveEnemy;
        while (dangerValue > 0)
        {
            var targetEnemy = taregtEnemyTypes[Random.Range(0, taregtEnemyTypes.Count)];
            StartCoroutine(SpawnEnemy(Random.Range(0f, m_MaxSpawnDelay), targetEnemy));
            dangerValue -= targetEnemy.DangerValue;
        }
        m_WaveCount++;

    }


    private IEnumerator SpawnEnemy(float delay, EnemyScriptable enemyData)
    {
        Debug.Log("");
        float passedTime = 0;
        while (passedTime < delay)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }

        var newEnemy = Instantiate(enemyData.Prefab, m_EnemyParent);

        var targetPos = new Vector3( m_WallCenter.position.x, m_GroundSpawnerCenter.position.y ,m_WallCenter.position.z) 
            + Vector3.right * Random.Range(m_WallWidth * -1f, m_WallWidth) +
            Vector3.forward * Random.Range(-1f, 1f);
        newEnemy.GetComponent<FlatEnemyController>().Init(enemyData, targetPos);

        newEnemy.transform.position = m_GroundSpawnerCenter.position +
            Random.Range(m_GroundSpawnerWidth * -1f, m_GroundSpawnerWidth) * Vector3.right;


    }



}
