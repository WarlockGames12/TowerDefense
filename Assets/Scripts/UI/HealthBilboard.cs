using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBilboard : MonoBehaviour
{
    private Vector3 offset = new(0f, 1.25f, 0f);

    // Update is called once per frame
    private void LateUpdate()
    {
        var getParent = transform.parent;
        transform.position = getParent.position + offset;

        transform.rotation = Quaternion.identity;
    }
}
