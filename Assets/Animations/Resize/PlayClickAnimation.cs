using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayClickAnimation : MonoBehaviour
{
    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(() => {
            animator.Play("Click", -1, 0f);
        });
    }
}
