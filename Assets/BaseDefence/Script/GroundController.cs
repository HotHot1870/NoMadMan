using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    [SerializeField] private GameObject m_Mud;
    public void EmitMud(Vector3 pos){
        var mudEmitter = Instantiate(m_Mud, this.transform);
        mudEmitter.transform.position = pos;
        mudEmitter.GetComponent<ParticleSystem>().Play();
        Destroy(mudEmitter,5);
    }
}
