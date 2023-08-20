using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnController : MonoBehaviour
{
    [SerializeField][Range(0f,1f)] private float m_BurnNormalized = 0;

    [SerializeField]private SpriteRenderer m_Self;

    private Material m_Mat;

    private void Start() {
        m_Mat = m_Self.material;
    }

    private void Update() {
        if(m_Mat){
            m_Mat.SetFloat("_Normalized",m_BurnNormalized);
        }
    }
}
