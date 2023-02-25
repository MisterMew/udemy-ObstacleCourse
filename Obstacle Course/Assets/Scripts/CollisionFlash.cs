using System.Collections;
using UnityEngine;

public  class CollisionFlash : MonoBehaviour
{
    /* Variables */
    private  MonoBehaviour monoBehaviour = default;

    private  MeshRenderer meshRenderer = null;
    private  Color defaultColour = default;

    [Header("Flash Variables")]
    private  float flashDuration = 1F; //0.024
    private  float flashIntensity = 1F; //8
    private  Color flashColour = default;
    private  float flashTimer = 0F;
    private  float lerp, intensity = 0F;

    public  void InitFlash(GameObject target, Color color)
    {
        if (target == null || color == null) return;

        meshRenderer = target.GetComponentInChildren<MeshRenderer>(); //Cache mesh renderer
        defaultColour = meshRenderer.material.color; //Cache original colour
        flashColour = color;

        flashTimer = flashDuration;

        /* How to use Coroutine when script doesn't inherit from MonoBehvaiour */
        ///
        /// if (monoBehaviour == null)
        ///     monoBehaviour = new GameObject().AddComponent<MonoScript>();
        ///
        /// MonoScript.StartExternalCoroutine(monoBehaviour, (IEnumerator)DoFlash());
        ///
        StartCoroutine(DoFlash());
    }

    private  IEnumerator DoFlash()
    {
        flashTimer -= Time.time; //Decrease flash timer

        lerp = Mathf.Clamp01(flashTimer / flashDuration);            //Get blending timer
        intensity = (lerp * flashIntensity) + 1F;                   //Calculate intensity
        meshRenderer.material.color = flashColour * intensity; //Set flash

        yield return new WaitForSeconds(flashDuration);

        meshRenderer.material.color = defaultColour; //Set Colour
    }
}
