using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    [SerializeField] Slider turnSlider;
    [SerializeField] float speed = 0.15f;

    [SerializeField] float fullHealth = 200f;
    [SerializeField] float currentHealth = 150f;

    Player[] attackTargets;
    [SerializeField] float physicalAttackAbility = 40f;
    [SerializeField] float physicalDefenseAbility = 20f;
    [SerializeField] float magicalAttackAbility = 80f;
    [SerializeField] float magicalDefenseAbility = 20f;
    [SerializeField] float physicalAttackRatio = 0.8f;

    GameController gameController;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        attackTargets = FindObjectsOfType<Player>();
        gameController = FindObjectOfType<GameController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.IsTakingTurns)
        {
            turnSlider.value += speed * Time.deltaTime;
            if (turnSlider.value >= turnSlider.maxValue)
            {
                StartCoroutine(TakeAttackActions());  
            }
        }
    }

    IEnumerator TakeAttackActions()
    {
        gameController.IsTakingTurns = false;
        //Time.timeScale = 0f;

        animator.SetTrigger("IsAttacking");

        if (UnityEngine.Random.Range(0f, 1f) <= physicalAttackRatio)
        {
            DoPhysicalAttack();
        }
        else
        {
            DoMagicalAttack();
        }

        yield return new WaitForSecondsRealtime(1f);

        gameController.IsTakingTurns = true;
        //Time.timeScale = 1f;
        turnSlider.value = 0f;
    }

    private void DoPhysicalAttack()
    {   
        attackTargets[UnityEngine.Random.Range(0, attackTargets.Length)].TakePhysicalDamage(physicalAttackAbility);
    }

    private void DoMagicalAttack()
    {
        attackTargets[UnityEngine.Random.Range(0, attackTargets.Length)].TakeMagicalDamage(magicalAttackAbility);
    }

    public void TakePhysicalDamage(float damage)
    {
        currentHealth -= (damage - physicalDefenseAbility) > 0 ? (damage - physicalDefenseAbility) : 1;
        animator.SetTrigger("IsTakingDamage");
        CheckDeath();
    }

    public void TakeMagicalDamage(float damage)
    {
        currentHealth -= (damage - magicalDefenseAbility) > 0 ? (damage - magicalDefenseAbility) : 1;
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (currentHealth <= 0f)
        {
            Debug.Log(gameObject.name + " died!");
        }
    }
}
