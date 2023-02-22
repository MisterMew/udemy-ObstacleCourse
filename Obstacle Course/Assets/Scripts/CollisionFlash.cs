using System.Collections;
using UnityEngine;

public static class CollisionFlash
{
    /* Variables */
    private static MonoBehaviour monoBehaviour = new MonoBehaviour();
    private static MeshRenderer meshRenderer = null;
    private static Color defaultColour = default;

    [Header("Flash Variables")]
    [SerializeField] private static float flashDuration = 1F; //0.024
    [SerializeField] private static float flashIntensity = 1F; //8
    [SerializeField] private static Color flashColour = default;
    private static float flashTimer = 0F;
    private static float lerp, intensity = 0F;


    public static void InitFlash(GameObject target, Color color)
    {
        if (target == null || color == null) return;

        meshRenderer = target.GetComponentInChildren<MeshRenderer>(); //Cache mesh renderer
        defaultColour = meshRenderer.material.color; //Cache original colour
        flashColour = color;

        flashTimer = flashDuration;

        MonoScript.StartExternalCoroutine(monoBehaviour, (IEnumerator)DoFlash());
    }

    private static IEnumerator DoFlash()
    {
        flashTimer -= Time.time; //Decrease flash timer

        lerp = Mathf.Clamp01(flashTimer / flashDuration);            //Get blending timer
        intensity = (lerp * flashIntensity) + 1F;                   //Calculate intensity
        meshRenderer.material.color = flashColour * intensity; //Set flash

        yield return new WaitForSeconds(flashDuration);

        meshRenderer.material.color = defaultColour; //Set Colour
    }
}
