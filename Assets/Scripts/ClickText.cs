using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float fadeTime = 2f;
    [SerializeField] private TextMeshProUGUI textComp;

    private float alpha = 1;

    void Update()
    {
        transform.position += new Vector3(0, moveSpeed, 0) * Time.deltaTime;
        textComp.color = new Color(0, 0, 0, alpha);
        alpha -= Time.deltaTime / fadeTime;
        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}
