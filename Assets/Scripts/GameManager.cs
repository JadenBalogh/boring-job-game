using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Sprite hitFrame;
    [SerializeField] private Sprite idleFrame;
    [SerializeField] private GameObject clickTextPrefab;
    [SerializeField] private int jobEarningRate = 100;
    [SerializeField] private int goalAmount = 100;

    public int Score { get; private set; }
    public int ClickAmount { get; set; }
    public int PassiveAmount { get; set; }
    public int Hours { get; set; }
    public int TotalEarnings { get; set; }

    private TextMeshProUGUI scoreTextComp;
    private TextMeshProUGUI goalTextComp;
    private TextMeshProUGUI hoursTextComp;
    private TextMeshProUGUI freeTimeTextComp;
    private TextMeshProUGUI jobTextComp;
    private TextMeshProUGUI totalEarningsTextComp;
    private Button clickButton;
    private Image backgroundImage;
    private Transform textSpawnPoint;
    private Transform canvasParent;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene scene1, Scene scene2)
    {
        if (SceneManager.GetActiveScene().name == "EndOfDay")
        {
            freeTimeTextComp = GameObject.FindGameObjectWithTag("FreeTime").GetComponent<TextMeshProUGUI>();
            jobTextComp = GameObject.FindGameObjectWithTag("Job").GetComponent<TextMeshProUGUI>();
            totalEarningsTextComp = GameObject.FindGameObjectWithTag("TotalEarnings").GetComponent<TextMeshProUGUI>();

            int earnings = goalAmount * jobEarningRate;
            jobTextComp.text = "Job Earnings: $" + earnings.ToString();

            int randBonus = Random.Range(30, 50);
            int randEarnings = ((16 - Hours) * randBonus);
            freeTimeTextComp.text = "Free Time Bonus Earnings: $" + randEarnings.ToString();

            TotalEarnings += earnings + randEarnings;
            totalEarningsTextComp.text = "Total Earnings: $" + TotalEarnings.ToString();

            if (TotalEarnings >= 10000)
            {
                TextMeshProUGUI winTextComp = GameObject.FindGameObjectWithTag("Mortgage").GetComponent<TextMeshProUGUI>();
                winTextComp.text = "You successfully paid off your mortgage! Wow!";
            }
            else
            {
                StartCoroutine(LoadNextDay());
            }
        }
        else
        {
            Hours = 0;
            ClickAmount = 1;
            textSpawnPoint = GameObject.FindGameObjectWithTag("Spawn").transform;
            canvasParent = GameObject.FindGameObjectWithTag("Canvas").transform;
            GameObject clickZone = GameObject.FindGameObjectWithTag("ClickZone");
            backgroundImage = clickZone.GetComponent<Image>();
            clickButton = clickZone.GetComponent<Button>();
            clickButton.onClick.AddListener(HandleClick);
            scoreTextComp = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
            goalTextComp = GameObject.FindGameObjectWithTag("Goals").GetComponent<TextMeshProUGUI>();
            hoursTextComp = GameObject.FindGameObjectWithTag("Hours").GetComponent<TextMeshProUGUI>();
            goalTextComp.text = goalAmount.ToString();
            StartCoroutine(PassiveIncome());
            StartCoroutine(HoursLoop());
        }
    }

    private void AddScore(int score)
    {
        Score += score;
        scoreTextComp.text = Score.ToString();
        if (Score >= goalAmount)
        {
            // load end of day scene
            Score = 0;
            SceneManager.LoadScene("EndOfDay");
        }
    }

    public void HandleClick()
    {
        // Animate
        StartCoroutine(AnimateHit());

        // spawn click text
        Instantiate(clickTextPrefab, textSpawnPoint.position, Quaternion.identity, canvasParent);

        // add score
        AddScore(ClickAmount);
    }

    private IEnumerator PassiveIncome()
    {
        while (true)
        {
            AddScore(PassiveAmount);
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator AnimateHit()
    {
        backgroundImage.sprite = hitFrame;
        yield return new WaitForSeconds(0.2f);
        if (backgroundImage == null) yield break;
        backgroundImage.sprite = idleFrame;
    }

    private IEnumerator HoursLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            Hours++;
            hoursTextComp.text = Hours.ToString() + " hours";
        }
    }

    private IEnumerator LoadNextDay()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Clicker");
    }
}
