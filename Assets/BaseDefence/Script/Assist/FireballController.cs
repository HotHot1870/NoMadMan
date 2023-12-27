using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private GameObject m_ExplosionPrefab;
    private float m_Damage = 10f;
    private float m_Radius = 5f;


    public void Init(float damage, float radius){
        m_Damage = damage;
        m_Radius = radius;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * m_Speed;
    }

    // TODO : init ExplosionController when touch enemy collider
}
