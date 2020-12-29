using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    bool isRunningTurns = true;
    [SerializeField] Slider turnSlider;
    [SerializeField] float speed = 0.1f;
    [SerializeField] GameObject selectionPanel;

    [SerializeField] Text healthText;
    [SerializeField] Slider healthSlider;
    [SerializeField] float fullHealth = 200f;
    [SerializeField] float currentHealth = 100f;

    GameObject attackTarget;
    bool isSelectingTarget = false;
    [SerializeField] float physicalAttackAbility = 50f;
    [SerializeField] float physicalDefenseAbility = 20f;
    //[SerializeField] float physicalDefenseAbilityInDefenseMode = 40f; // double
    [SerializeField] float magicalAttackAbility = 60f;
    [SerializeField] float magicalDefenseAbility = 30f;
    [SerializeField] float runAwayChance = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthInfoDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunningTurns)
        {
            turnSlider.value += speed * Time.deltaTime;
            if (turnSlider.value >= turnSlider.maxValue)
            {
                ShowSelectionPanel();
                isRunningTurns = false;
            }
        }

        DetectMouseSelectingAttackTarget();
    }

    private void DetectMouseSelectingAttackTarget()
    {
        if (isSelectingTarget == true)
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthInfoDisplay();
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (currentHealth <= 0f)
        {
            Debug.Log("I died!");
        }
    }


    private void UpdateHealthInfoDisplay()
    {
        healthText.text = currentHealth.ToString();
        healthSlider.value = currentHealth / fullHealth;
    }

    private void ResetTurnSlider()
    {
        turnSlider.value = 0f;
        isRunningTurns = true;
    }

    private void HideSelectionPanel()
    {
        selectionPanel.SetActive(false);
    }

    private void ShowSelectionPanel()
    {
        selectionPanel.SetActive(true);
    }
}
