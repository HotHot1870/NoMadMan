using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameXinSpawner : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer m_XinRenderer;
    private Material m_XinMat;

    void Start(){
        m_XinMat = m_XinRenderer.material;
        StartCoroutine(SpawnXin());
    }

    // TODO : remove battle music , members only
    // TODO : remove map music , members only

    private IEnumerator SpawnXin(){
        float passTime = 0f;
        float duration = 15f;
        while (passTime<duration)
        {
            passTime+=Time.deltaTime;
            yield return null;
            m_XinMat.SetFloat("_Normalized",Mathf.Clamp01(passTime/duration) );
        }
    }
}
