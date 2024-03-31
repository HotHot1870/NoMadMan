using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XinSpawnBallController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_XinSpawnWaveEffectPrefab;
    private float m_Speed = 15f;
    private XinController m_Xin;



    public void Init(XinController xin){
        m_Xin = xin;
    }


    // Update is called once per frame
    void Update()
    {/*
        if(m_ShouldStop){
            float previousNormalized = m_Renderer.material.GetFloat("_Normalized");
            m_Renderer.material.SetFloat("_Normalized",previousNormalized-Time.deltaTime);
            m_Renderer.material.SetFloat("_Seed", Random.Range(0f,1f));
            return;
        }*/

        transform.position += Vector3.down * Time.deltaTime * m_Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if(m_ShouldStop)
            return;*/


        var ground = other.GetComponent<GroundController>();
        if( ground != null){
            // hit ground
            Instantiate(m_XinSpawnWaveEffectPrefab,m_Self.transform.position, m_Self.transform.rotation );
            m_Xin.StartWave();
            Destroy(m_Self);
            // explode effect
        }
    }
}
