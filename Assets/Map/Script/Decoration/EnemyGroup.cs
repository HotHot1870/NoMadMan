using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> m_AllEnemy = new List<SkinnedMeshRenderer>();

    void Start(){
        StartCoroutine(SpawnEffect());
    }

    private IEnumerator SpawnEffect(){
        float passTime = 0f;
        float duration = 1.25f;
        while (passTime<duration)
        {
            passTime += Time.deltaTime;
            foreach (var skin in m_AllEnemy)
            {
                foreach (var mat in skin.materials)
                {
                    mat.SetFloat("_Normalized",passTime/duration);
                }
            }
            yield return null;
        }
    }
}
