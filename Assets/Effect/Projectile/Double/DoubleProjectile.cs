using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleProjectile : MonoBehaviour
{
    [SerializeField] private Transform m_Red;
    [SerializeField] private Transform m_Blue;
    [SerializeField][Range(0.1f, 0.5f)] private float m_Distance = 0.25f;
    [SerializeField][Range(0.1f, 30f)] private float m_Speed = 10f;
    private float m_TimePass = 0;


    private void Update()
    {
        m_TimePass += Time.deltaTime;
        m_Red.localPosition = new Vector3( 
            m_Distance * Mathf.Sin(m_Speed *m_TimePass), 
            m_Distance * Mathf.Cos(m_Speed *m_TimePass),
            0);
        m_Blue.localPosition = new Vector3(
            -1 * m_Distance * Mathf.Sin(m_Speed *m_TimePass),
            -1 * m_Distance * Mathf.Cos(m_Speed *m_TimePass),
            0);
    }

}
