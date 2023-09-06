using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SkyBoxUpdate : MonoBehaviour
{
    ReflectionProbe baker;
    // Start is called before the first frame update
    void Start()
    {
        baker = gameObject.AddComponent<ReflectionProbe>();
        InvokeRepeating("ChangeSkyBox",1f,0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeSkyBox() {
        RenderSettings.skybox = RenderSettings.skybox;
        DynamicGI.UpdateEnvironment();
        baker.cullingMask = 0;
        baker.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
        baker.mode = ReflectionProbeMode.Realtime;
        baker.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;

        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
        StartCoroutine(UpdateEnvironment());
    }

    IEnumerator UpdateEnvironment() {
        DynamicGI.UpdateEnvironment();
        baker.RenderProbe();
        yield return new WaitForEndOfFrame();
        RenderSettings.customReflectionTexture= baker.texture;
        }
    
}
