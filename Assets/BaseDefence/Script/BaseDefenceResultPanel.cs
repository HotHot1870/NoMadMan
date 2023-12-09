using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BaseDefenceResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    private void Start()
    {
        m_Self.SetActive(false);
    }
}
