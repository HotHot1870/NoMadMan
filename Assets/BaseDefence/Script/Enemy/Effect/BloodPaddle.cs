using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodPaddle : MonoBehaviour
{

    [SerializeField] private ParticleSystem m_Smoke;

    void Start(){
        m_Smoke.Play();
    }
}
