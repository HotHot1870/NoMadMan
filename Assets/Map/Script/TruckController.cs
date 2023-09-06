using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    [SerializeField][Range(0f,50f)] private float m_BounceSpeed = 5f;
    [SerializeField][Range(0f,0.9f)]  private float m_BounceAmount = 0.15f;
    [SerializeField] private Transform m_BounceTarget;
    private float m_BounceNormalized = 0;
    private float m_PassedTime = 0;

    private void Update() {
            m_PassedTime += (Time.deltaTime *m_BounceSpeed * Mathf.PI);
            m_BounceTarget.localScale = new Vector3(1, 1- m_BounceAmount*m_BounceNormalized ,1);
            m_BounceNormalized = (Mathf.Sin(m_PassedTime)+1f)/2f;
        
    }

}
