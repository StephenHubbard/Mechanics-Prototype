using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Utils
{
    public static bool IsOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
