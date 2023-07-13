using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public abstract class Utils 
{
    public static bool IsOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static void DestroySelf(GameObject go, float destroyDelay = 0f) {
        UnityEngine.Object.Destroy(go, destroyDelay);
    }

    public static void RunAfterDelay(MonoBehaviour monoBehaviour, float delay, Action task)
    {
        monoBehaviour.StartCoroutine(RunAfterDelayCoroutine(delay, task));
    }

    private static IEnumerator RunAfterDelayCoroutine(float delay, Action task)
    {
        yield return new WaitForSeconds(delay);
        task.Invoke();
    }
}
