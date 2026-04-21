using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchlightScript : MonoBehaviour
{
    public Light pointLight;
    public bool lighting = true;
    public void ToggleLight(bool light)
    {
        this.lighting = light;
        pointLight.enabled = light;
    }
    public void ToggleLight()
    {
        this.lighting = !this.lighting;
        pointLight.enabled = lighting;
    }
}
