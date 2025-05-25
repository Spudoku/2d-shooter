using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPoof : MonoBehaviour
{
    /*[SerializeField] private float timer;

    private void Awake()
    {
        StartCoroutine(KillTimer(timer));
    }

    private IEnumerator KillTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log($"{name}: my position is {transform.position}");
        Destroy(gameObject);
    }*/
    //public float explosionSize;     // radius in units
    [SerializeField] private float duration;    // duration of effect

    private Material mat;
    Color currentColor;
    Color endColor;



    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
        endColor = Color.black;
        endColor.a = 0f;


        //transform.localScale = new Vector3(explosionSize * 2, explosionSize * 2, explosionSize * 2);

        StartCoroutine(FadeFromColorToNormal(mat.color, endColor, duration));
    }

    // fade to become completely transparent, then destroy self
    // Code by Unity Forum user 'StarManta'
    IEnumerator FadeFromColorToNormal(Color start, Color end, float duration)
    {

        Color currentColor = start;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            currentColor = Color.Lerp(currentColor, end, normalizedTime);
            mat.color = currentColor;
            yield return null;
        }
        //currentColor = end; //without this, the value will end at something like 0.9992367
        Destroy(gameObject);
    }
}
