using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider2D))]
public class TimelineController : MonoBehaviour {
    public enum TriggerType {
        Once, Everytime,
    }

    public GameObject triggeringGameObject;
    public PlayableDirector director;
    public TriggerType triggerType;
    public UnityEvent OnDirectorPlay;
    public UnityEvent OnDirectorFinish;


    protected bool isAlreadyTriggered;


    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject != triggeringGameObject || triggerType == TriggerType.Once && isAlreadyTriggered)
            return;

        director.Play();
        isAlreadyTriggered = true;
        OnDirectorPlay.Invoke();
        Invoke(nameof(FinishInvoke), (float)director.duration);
    }

    private void FinishInvoke() {
        OnDirectorFinish.Invoke();
    }

}