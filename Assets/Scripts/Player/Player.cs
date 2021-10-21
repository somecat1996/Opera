using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    private PlayerAnimator playerAnimator;
    private PlayerMovement playerMovement;
    private PlayerStatus playerStatus;
    private MaterialController materialController;

    private void Awake()
    {
        instance = this;
        playerAnimator = gameObject.GetComponent<PlayerAnimator>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerStatus = gameObject.GetComponent<PlayerStatus>();
        materialController = gameObject.GetComponent<MaterialController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeCharacter(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeCharacter(int index)
    {
        playerAnimator.ChangeCharacter(index);
        materialController.ResearchRenderer(playerAnimator.animators[index].transform, playerAnimator.shadowAnimators[index].transform);
    }

    public void Walk(bool walk)
    {
        playerAnimator.SetBool("Walk", walk);
    }

    public void TriggerAnimation(int id)
    {
        playerAnimator.SetTrigger(id.ToString());
    }

    public Vector3 PlayerPosition()
    {
        return transform.position;
    }

    public bool IsStunImmunity()
    {
        return playerStatus.IsStunImmunity();
    }

    public void PlayerDie(TweenCallback _callbackFun = null, float _duration = 1.5f)
    {
        materialController.SetEnableDissolution(true, _duration, _callbackFun);
    }

    public void PlayerStealth(float duration)
    {
        materialController.SetEnableStealth(true, duration);
    }
}
