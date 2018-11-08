using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

public class SceneAnimator : MonoBehaviour
{

    #region Common

    public void SetBool(string parameter, bool value, GameObject gameObject, float time)
    {
        Animator animator = gameObject.GetComponent<Animator>();

        if (!animator)
        {
            Debug.LogError(gameObject.name + " has no animator ");
            return;
        }

        StartCoroutine(SetBoolAfter(animator, parameter, value, time));
    }

    public void SetBool(string parameter, bool value, GameObject animatorGameObject)
    {
        if (animatorGameObject)
        {
            Animator animator = animatorGameObject.GetComponent<Animator>();

            if (!animator)
            {
                Debug.LogError(animatorGameObject.name + " has no animator ");
                return;
            }

            animator.SetBool(parameter, value);
        }
    }

    public void SetFloat(string parameter, float value, GameObject gameObject)
    {
        Animator animator = gameObject.GetComponent<Animator>();

        if (!animator)
        {
            Debug.LogError(gameObject.name + " has no animator ");
            return;
        }

        animator.SetFloat(parameter, value);
    }

    public void StartAnimation(string animName, GameObject gameObject)
    {
        StartCoroutine(AnimationCoroutine(animName, gameObject));
    }

    private IEnumerator AnimationCoroutine(string animName, GameObject gameObject)
    {
        Animator animator = gameObject.GetComponent<Animator>();

        if (animator)
        {

            float animLength = FindAnimLength(animator, animName);
            if (animLength != -1)
            {
                animator.SetBool(animName, true);
                yield return new WaitForSeconds(animLength);

                animator.SetBool(animName, false);
            }
            else
            {
                Debug.LogError(animName + " animation was not found in " + animator);
            }
        }
        else
        {
            Debug.LogError(gameObject.name + " has no animator ");

        }
    }

    private IEnumerator SetBoolAfter(Animator animator, string parameter, bool value, float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool(parameter, value);
    }

    #endregion

    #region Utils

    private float FindAnimLength(Animator animator, string clipName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        if (ac)
        {
            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name == clipName)
                {
                    return clip.length;
                }
            }
        }

        return -1;
    }

    #endregion

}
