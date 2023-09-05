using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    [SerializeField] private float m_BounceTimeGap = 0.5f;
    [SerializeField] private float m_BounceAmount = 0.15f;
    [SerializeField] private Transform m_BounceTarget;
    private float m_PassedTime = 0;

    private void Update() {
        
            m_BounceTarget.localScale = new Vector3(1, 1- m_BounceAmount*m_PassedTime/ (m_BounceTimeGap/2f) ,1);
            m_PassedTime += Time.deltaTime;

        
    }

}
