using System.Collections;
using UnityEngine;

public class MonoScript : MonoBehaviour
{
    public static void StartExternalCoroutine(MonoBehaviour mono, IEnumerator enumerator) => mono.StartCoroutine(enumerator);
}
