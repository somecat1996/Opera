using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator[] animators;
    public Animator[] shadowAnimators;

    private Animator currentAnimator;
    private Animator currentShadowAnimator;

    private void Awake()
    {
        currentAnimator = animators[1];
        currentShadowAnimator = shadowAnimators[1];
    }

    public void ChangeCharacter(int index)
    {
        Debug.Log(index);
        if (currentAnimator)
        {
            currentAnimator.gameObject.SetActive(false);
        }
        if (currentShadowAnimator)
        {
            currentShadowAnimator.gameObject.SetActive(false);
        }

        currentAnimator = animators[index];
        currentShadowAnimator = shadowAnimators[index];

        currentAnimator.gameObject.SetActive(true);
        currentShadowAnimator.gameObject.SetActive(true);
    }

    public void SetTrigger(string name)
    {
        currentAnimator.SetTrigger(name);
        currentShadowAnimator.SetTrigger(name);
    }

    public void SetBool(string name, bool value)
    {
        currentAnimator.SetBool(name, value);
        currentShadowAnimator.SetBool(name, value);
    }
}
