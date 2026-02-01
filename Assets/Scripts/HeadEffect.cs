using UnityEngine;
using UnityEngine.UI;

public class HeadEffect : MonoBehaviour
{
    [SerializeField]
    float duration;
    [SerializeField]
    float fadeDuration;
    float passedtime = 0;

    Image image;
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        passedtime += Time.deltaTime;
        transform.localScale = new Vector3(Mathf.Lerp(0.0f, 5.0f, Mathf.Clamp(passedtime / duration, 0, 1)), Mathf.Lerp(0.0f, 3.0f, Mathf.Clamp(passedtime / duration, 0, 1)), Mathf.Lerp(0.0f, 3.0f, Mathf.Clamp(passedtime / duration, 0, 1)));

        image.color = new Color(255, 255, 255, Mathf.Lerp(255.0f, 0.0f, Mathf.Clamp(passedtime / fadeDuration, 0, 1)));
        if (passedtime > duration)
            Destroy(gameObject);
    }
}
