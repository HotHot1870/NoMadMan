using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XinSpawnBallController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private MeshRenderer m_Renderer;
    private float m_Speed = 15f;
    private XinController m_Xin;

    private bool m_ShouldStop = false;


    public void Init(XinController xin){
        m_Xin = xin;
    }


    // Update is called once per frame
    void Update()
    {
        if(m_ShouldStop){
            float previousNormalized = m_Renderer.material.GetFloat("_Normalized");
            m_Renderer.material.SetFloat("_Normalized",previousNormalized-Time.deltaTime);
            m_Renderer.material.SetFloat("_Seed", Random.Range(0f,1f));
            return;
        }

        transform.position += Vector3.down * Time.deltaTime * m_Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(m_ShouldStop)
            return;

        var ground = other.GetComponent<GroundController>();
        if( ground != null){
            // hit ground
            m_Xin.StartWave();
            m_ShouldStop = true;
            Destroy(m_Self, 3f);
            // explode effect
            m_Renderer.material.SetFloat("_Normalized",0.75f);
            m_Self.transform.localScale = Vector3.one * 5;
        }
    }
}
