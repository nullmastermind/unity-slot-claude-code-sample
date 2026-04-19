using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SlotGameManager : MonoBehaviour
{
    [Header("UI References")]
    public Text pointsText;
    public Text resultText;
    public Button spinButton;
    public GameObject gameOverPanel;
    public Text gameOverText;
    public Button restartButton;
    public Button menuButton;

    [Header("Bet Controls")]
    public Button betMinusButton;
    public Button betPlusButton;
    public Text betText;

    [Header("Reel Containers")]
    public RectTransform[] reelStrips; // The moving strip inside each reel

    [Header("Settings")]
    public int startingPoints = 100;
    public int costPerSpin = 5;
    public int betStep = 5;
    public int minBet = 5;
    public float symbolHeight = 180f;

    private int currentPoints;
    private bool isSpinning;

    private string[] symbols = { "Cherry", "Lemon", "Orange", "Grape", "Bell", "Star", "Diamond", "Seven" };
    private int[] symbolWeights = { 25, 22, 20, 18, 10, 8, 5, 2 };
    private string[] symbolDisplay = { "\u2663", "\u2665", "\u2666", "\u2660", "\u266A", "\u2605", "\u25C6", "7" };
    private int stripSymbolCount = 20; // Number of symbols in each strip

    private Dictionary<string, int> payoutTable;

    void Awake()
    {
        payoutTable = new Dictionary<string, int>();
        payoutTable.Add("Cherry", 2);
        payoutTable.Add("Lemon", 3);
        payoutTable.Add("Orange", 4);
        payoutTable.Add("Grape", 5);
        payoutTable.Add("Bell", 8);
        payoutTable.Add("Star", 12);
        payoutTable.Add("Diamond", 20);
        payoutTable.Add("Seven", 50);
    }

    void Start()
    {
        currentPoints = GameData.StartingPoints;
        isSpinning = false;
        gameOverPanel.SetActive(false);
        RefreshPointsDisplay();
        resultText.text = "Press SPIN to play!";

        spinButton.onClick.AddListener(OnSpinPressed);
        restartButton.onClick.AddListener(OnRestartPressed);
        menuButton.onClick.AddListener(OnMenuPressed);

        if (betMinusButton != null)
            betMinusButton.onClick.AddListener(OnBetMinus);
        if (betPlusButton != null)
            betPlusButton.onClick.AddListener(OnBetPlus);

        RefreshBetDisplay();

        // Build the symbol strips for each reel
        for (int r = 0; r < reelStrips.Length; r++)
        {
            BuildReelStrip(reelStrips[r]);
        }
    }

    private void OnBetMinus()
    {
        costPerSpin -= betStep;
        if (costPerSpin < minBet) costPerSpin = minBet;
        RefreshBetDisplay();
    }

    private void OnBetPlus()
    {
        costPerSpin += betStep;
        if (costPerSpin > currentPoints) costPerSpin = currentPoints;
        RefreshBetDisplay();
    }

    private void RefreshBetDisplay()
    {
        if (betText != null)
            betText.text = "Bet: " + costPerSpin;

        if (betMinusButton != null)
            betMinusButton.interactable = (costPerSpin > minBet) && !isSpinning;

        if (betPlusButton != null)
            betPlusButton.interactable = (costPerSpin < currentPoints) && !isSpinning;
    }

    private void BuildReelStrip(RectTransform strip)
    {
        // Clear existing children
        for (int i = strip.childCount - 1; i >= 0; i--)
            Destroy(strip.GetChild(i).gameObject);

        // Set strip height
        float totalHeight = stripSymbolCount * symbolHeight;
        strip.sizeDelta = new Vector2(strip.sizeDelta.x, totalHeight);

        // Position strip so symbol 0 is visible (centered in mask)
        strip.anchoredPosition = new Vector2(0, 0);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        for (int i = 0; i < stripSymbolCount; i++)
        {
            GameObject symGO = new GameObject("Sym_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            symGO.transform.SetParent(strip, false);

            RectTransform rt = symGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0, -i * symbolHeight);
            rt.sizeDelta = new Vector2(0, symbolHeight);

            int symIdx = Random.Range(0, symbolDisplay.Length);
            Text txt = symGO.GetComponent<Text>();
            txt.text = symbolDisplay[symIdx];
            txt.fontSize = 80;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = font;
        }
    }

    private void RefreshPointsDisplay()
    {
        pointsText.text = "Points: " + currentPoints;
        RefreshBetDisplay();
    }

    private void OnSpinPressed()
    {
        if (isSpinning) return;
        if (currentPoints < costPerSpin)
        {
            DisplayGameOver();
            return;
        }

        // Disable bet buttons while spinning
        if (betMinusButton != null) betMinusButton.interactable = false;
        if (betPlusButton != null) betPlusButton.interactable = false;

        currentPoints -= costPerSpin;
        RefreshPointsDisplay();
        StartCoroutine(SpinAllReels());
    }

    private IEnumerator SpinAllReels()
    {
        isSpinning = true;
        spinButton.interactable = false;
        resultText.text = "Spinning...";

        int[] resultIndices = new int[reelStrips.Length];

        // Determine final symbols
        for (int r = 0; r < reelStrips.Length; r++)
            resultIndices[r] = GetWeightedIndex();

        // Rebuild strips with the target symbol at a known position
        for (int r = 0; r < reelStrips.Length; r++)
        {
            RebuildStripWithTarget(reelStrips[r], resultIndices[r], r);
        }

        // Start spinning all reels simultaneously, stop with cascade delay
        Coroutine[] spinCoroutines = new Coroutine[reelStrips.Length];
        for (int r = 0; r < reelStrips.Length; r++)
        {
            float spinDuration = 1.5f + r * 0.6f; // cascade: each reel stops later
            spinCoroutines[r] = StartCoroutine(SpinSingleReel(reelStrips[r], spinDuration));
        }

        // Wait for all reels to finish
        for (int r = 0; r < reelStrips.Length; r++)
            yield return spinCoroutines[r];

        // Small pause then evaluate
        yield return new WaitForSeconds(0.3f);

        CheckResults(resultIndices);

        isSpinning = false;
        spinButton.interactable = true;

        // Re-enable bet buttons and refresh display (clamps if balance changed)
        RefreshBetDisplay();

        if (currentPoints <= 0)
        {
            yield return new WaitForSeconds(1f);
            DisplayGameOver();
        }
    }

    private void RebuildStripWithTarget(RectTransform strip, int targetSymbolIndex, int reelIndex)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Clear
        for (int i = strip.childCount - 1; i >= 0; i--)
            Destroy(strip.GetChild(i).gameObject);

        // Set strip height
        float totalHeight = stripSymbolCount * symbolHeight;
        strip.sizeDelta = new Vector2(strip.sizeDelta.x, totalHeight);

        // Reset position to top
        strip.anchoredPosition = new Vector2(0, 0);

        // Place target symbol at a specific position (e.g., index = stripSymbolCount - 2)
        int targetPos = stripSymbolCount - 2;

        for (int i = 0; i < stripSymbolCount; i++)
        {
            GameObject symGO = new GameObject("Sym_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            symGO.transform.SetParent(strip, false);

            RectTransform rt = symGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0, -i * symbolHeight);
            rt.sizeDelta = new Vector2(0, symbolHeight);

            int symIdx;
            if (i == targetPos)
                symIdx = targetSymbolIndex;
            else
                symIdx = Random.Range(0, symbolDisplay.Length);

            Text txt = symGO.GetComponent<Text>();
            txt.text = symbolDisplay[symIdx];
            txt.fontSize = 80;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = font;
        }
    }

    private IEnumerator SpinSingleReel(RectTransform strip, float duration)
    {
        // Target Y: scroll down so targetPos symbol is visible in the mask center
        int targetPos = stripSymbolCount - 2;
        float targetY = targetPos * symbolHeight;

        float elapsed = 0f;
        float startY = strip.anchoredPosition.y;

        // Phase 1: Fast spin (constant speed scrolling through symbols)
        float fastDuration = duration * 0.65f;
        float fastSpeed = targetY / duration * 1.8f; // Fast enough to look like spinning

        while (elapsed < fastDuration)
        {
            elapsed += Time.deltaTime;
            float currentY = startY + fastSpeed * elapsed;
            // Wrap around to create infinite scroll illusion
            float wrappedY = currentY % (stripSymbolCount * symbolHeight);
            strip.anchoredPosition = new Vector2(0, wrappedY);
            yield return null;
        }

        // Phase 2: Decelerate to final position with ease-out
        float decelDuration = duration - fastDuration;
        float decelStart = strip.anchoredPosition.y;
        float decelElapsed = 0f;

        // Ensure we end exactly at targetY
        while (decelElapsed < decelDuration)
        {
            decelElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(decelElapsed / decelDuration);
            // Ease-out cubic
            float easeT = 1f - Mathf.Pow(1f - t, 3f);
            float currentY = Mathf.Lerp(decelStart, targetY, easeT);
            strip.anchoredPosition = new Vector2(0, currentY);
            yield return null;
        }

        // Snap to exact position
        strip.anchoredPosition = new Vector2(0, targetY);

        // Bounce effect
        float bounceAmount = 15f;
        float bounceDuration = 0.15f;

        // Overshoot
        float bounceElapsed = 0f;
        float bounceStart = targetY;
        float bounceTarget = targetY + bounceAmount;
        while (bounceElapsed < bounceDuration)
        {
            bounceElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(bounceElapsed / bounceDuration);
            float easeT = Mathf.Sin(t * Mathf.PI);
            strip.anchoredPosition = new Vector2(0, Mathf.Lerp(bounceStart, bounceTarget, easeT));
            yield return null;
        }

        // Snap final
        strip.anchoredPosition = new Vector2(0, targetY);
    }

    private int GetWeightedIndex()
    {
        int totalWeight = 0;
        for (int i = 0; i < symbolWeights.Length; i++)
            totalWeight += symbolWeights[i];

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        for (int i = 0; i < symbols.Length; i++)
        {
            cumulative += symbolWeights[i];
            if (roll < cumulative) return i;
        }
        return 0;
    }

    private void CheckResults(int[] indices)
    {
        string s0 = symbols[indices[0]];
        string s1 = symbols[indices[1]];
        string s2 = symbols[indices[2]];

        if (s0 == s1 && s1 == s2)
        {
            int payout = payoutTable[s0] * costPerSpin;
            currentPoints += payout;
            resultText.text = "JACKPOT! +" + payout + " points!";
            RefreshPointsDisplay();
            return;
        }

        if (s0 == s1 || s1 == s2 || s0 == s2)
        {
            string matched = (s0 == s1) ? s0 : (s1 == s2) ? s1 : s0;
            int payout = payoutTable[matched] * costPerSpin;
            currentPoints += payout;
            resultText.text = "Match! +" + payout + " points!";
            RefreshPointsDisplay();
            return;
        }

        resultText.text = "No match. Try again!";
    }

    private void DisplayGameOver()
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = "GAME OVER\n\nYou ran out of points!";
        spinButton.interactable = false;
    }

    private void OnRestartPressed()
    {
        SceneManager.LoadScene("SlotGame");
    }

    private void OnMenuPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
