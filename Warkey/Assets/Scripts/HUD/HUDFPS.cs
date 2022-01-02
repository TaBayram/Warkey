using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDFPS : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    private float deltaTime;
    private float count;
    private IEnumerator Start()
    {
        while (true)
        {
            if(Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
                count = (1 / Time.deltaTime);
                fpsText.text = Mathf.Ceil(count).ToString() + " FPS";
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
