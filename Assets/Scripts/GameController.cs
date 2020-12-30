using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool IsTakingTurns { get; set; }

    [SerializeField] Text timerText;
    [SerializeField] float combatTimeInSeconds = 180f;

    void Start()
    {
        IsTakingTurns = true;
        UpdateTimer();
    }

    void Update()
    {
        combatTimeInSeconds -= Time.deltaTime;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timerText.text = Mathf.Floor(combatTimeInSeconds / 60).ToString("00") + ":" + Mathf.Floor(combatTimeInSeconds % 60).ToString("00");
    }


}
