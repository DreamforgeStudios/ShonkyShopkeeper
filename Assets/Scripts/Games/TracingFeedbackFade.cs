using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TracingFeedbackFade : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Color alpha;
    public GameObject cross;
    private float cooldown;
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cooldown = Time.time + 0.5f;
    }

    private void Update() {
        if (Time.time > cooldown) { 
        alpha = spriteRenderer.color;
            if (alpha.a > 0) {
                alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * 2.5f);
                spriteRenderer.color = alpha;
                if (alpha.a < 0.2) {
                    Instantiate(cross, transform.position, transform.rotation);
                }
            }
        }
    }
}
