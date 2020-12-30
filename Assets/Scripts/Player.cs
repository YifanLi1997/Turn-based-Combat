using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] Slider turnSlider;
    [SerializeField] float speed = 0.1f;
    [SerializeField] GameObject selectionPanel;

    [Header("Health")]
    [SerializeField] Text healthText;
    [SerializeField] Slider healthSlider;
    [SerializeField] float fullHealth = 100f;
    Health health;
    [SerializeField] GameObject deathPanel;

    [Header("Attack and Defense")]
    GameObject attackTarget;
    bool isSelectingTarget = false;
    [SerializeField] float physicalAttackAbility = 50f;
    [SerializeField] float physicalDefenseAbility = 50f;
    //[SerializeField] float physicalDefenseAbilityInDefenseMode = 40f; // double
    [SerializeField] float magicalAttackAbility = 60f;
    [SerializeField] float magicalDefenseAbility = 30f;
    [SerializeField] float runAwayChance = 0.8f;

    GameController gameController;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        gameController = FindObjectOfType<GameController>();
        animator = GetComponent<Animator>();

        // health should be read from the playerProfile
        health.FullHealth = fullHealth;
        health.CurrentHealth = health.FullHealth;
        UpdateHealthInfoDisplay();

    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.IsTakingTurns)
        {
            turnSlider.value += speed * Time.deltaTime;
            if (turnSlider.value >= turnSlider.maxValue)
            {
                ShowSelectionPanel();
            }
        }

        if (isSelectingTarget)
            DetectMouseSelectingAttackTarget();
    }

    private void DetectMouseSelectingAttackTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("I am selecting attackTarget!");
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit)
            {
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    attackTarget = hit.transform.gameObject;
                    isSelectingTarget = false;
                }
            }
        }
    }

    public void PhysicalAttack()
    {
        HideSelectionPanel();
        isSelectingTarget = true;
        StartCoroutine(SelectAttackTarget());
    }

    IEnumerator SelectAttackTarget()
    {
        while (attackTarget == null)
        {
            yield return null;
        }
        attackTarget.GetComponent<Enemy>().TakePhysicalDamage(physicalAttackAbility);
        Debug.Log("I am attacking " + attackTarget.name);
        ResetAttackTarget();
        ResetTurnSlider();
    }

    private void ResetAttackTarget()
    {
        attackTarget = null;
    }

    public void MagicalAttack()
    {
        Debug.Log("I am attacking magically!");
        ResetTurnSlider();
        HideSelectionPanel();
    }

    public void Defense()
    {
        Debug.Log("I am defensing!");
        ResetTurnSlider();
        HideSelectionPanel();
    }

    public void RunAway()
    {
        // Debug.Log("I am running away!");

        float ran = UnityEngine.Random.Range(0f, 1f);
        if (ran <= runAwayChance)
        {
            Debug.Log("I ran away successfully. Combat End!");
        }
        else
        {
            Debug.Log("Fail to run away. Combat continues.");
            ResetTurnSlider();
            HideSelectionPanel();
        }
    }

    public void TakePhysicalDamage(float damage)
    {
        health.CurrentHealth -= (damage - physicalDefenseAbility) > 0? (damage - physicalDefenseAbility): 1;
        UpdateHealthInfoDisplay();
        animator.SetTrigger("IsTakingDamage");
        CheckDeath();
    }

    public void TakeMagicalDamage(float damage)
    {
        health.CurrentHealth -= (damage - magicalDefenseAbility) > 0 ? (damage - magicalDefenseAbility) : 1;
        UpdateHealthInfoDisplay();
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (health.CurrentHealth <= 0f)
        {
            StartCoroutine(WaitAndDisplayDeathPanel());
        }
    }

    IEnumerator WaitAndDisplayDeathPanel()
    {
        health.CurrentHealth = 0f;
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log(gameObject.name + " died!");
        deathPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void UpdateHealthInfoDisplay()
    {
        healthText.text = health.CurrentHealth.ToString();
        healthSlider.value = health.CurrentHealth / health.FullHealth;
    }

    private void ResetTurnSlider()
    {
        turnSlider.value = 0f;
        gameController.IsTakingTurns = true;
        Time.timeScale = 1f;
    }

    private void HideSelectionPanel()
    {
        selectionPanel.SetActive(false);
    }

    private void ShowSelectionPanel()
    {
        selectionPanel.SetActive(true);
        gameController.IsTakingTurns = false;
        Time.timeScale = 0f;
    }
}
