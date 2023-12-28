using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    private float m_Speed = 22f;
    [SerializeField] private GameObject m_ExplosionPrefab;
    private float m_Damage = 10f;
    private float m_Radius = 5f;
    private bool m_ShouldStop = false;


    public void Init(float damage, float radius){
        m_Damage = damage;
        m_Radius = radius;
    }


    // Update is called once per frame
    void Update()
    {
            transform.position += transform.forward * Time.deltaTime * m_Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(m_ShouldStop)
            return;


        var enemyBodyPart = other.GetComponent<EnemyBodyPart>();
        var ground = other.GetComponent<GroundController>();
        if(enemyBodyPart !=null || ground != null){
            // hit enemy or ground , explode
            var explosion = Instantiate(m_ExplosionPrefab);
            explosion.transform.position = m_Self.transform.position;
            explosion.GetComponent<ExplosionController>().Init(m_Damage , m_Radius);
            m_ShouldStop = true;
            Destroy(explosion, 2f);
            Destroy(m_Self, 4f);
        }
    }
}
