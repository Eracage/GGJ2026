using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public bool opposite = false;
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
        if (opposite)
            transform.Rotate(new Vector3(0, 180, 0), Space.Self);
    }
}
