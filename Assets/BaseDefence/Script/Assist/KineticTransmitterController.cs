using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class KineticTransmitterController : MonoBehaviour
{
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    [SerializeField] private Transform m_Sphere;
    [SerializeField] private AnimationCurve m_SphereHeighCurve;
    void Start(){
        m_Sphere.localScale = Vector3.zero;
    }
    public void init(){
        m_Sphere.localScale = Vector3.zero;
        var allEnemy = m_EnemySpawnController.GetAllEnemyTrans().ToList();
        foreach (var item in allEnemy)
        {
            if(item != null)
                item.GetComponent<EnemyControllerBase>().OnNet();
        }
        StartCoroutine(SphereGrow());
    }

    private IEnumerator SphereGrow(){
        float passTime= 0f ;
        float duration = 0.5f;
        float maxSize = 85f;
        var mat = m_Sphere.GetComponent<MeshRenderer>().material;
        while (passTime<duration)
        {
            passTime += Time.deltaTime;
            mat.SetFloat("_Heigh",Mathf.Lerp(0.1f,0.5f, m_SphereHeighCurve.Evaluate(passTime / duration) ) );
            m_Sphere.localScale = Vector3.one * Mathf.Lerp(0f,maxSize, passTime / duration);
            yield return null;
        }
        
        m_Sphere.localScale = Vector3.zero;
    }
}
