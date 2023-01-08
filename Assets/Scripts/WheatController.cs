using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class WheatController : MonoBehaviour
{
    public RectTransform[] wheats;
    public MMAudioAnalyzer analyzer;
    [Range(0f, 1f)] public float lerpSpeed = 1f;

    [Header("生长曲线")] public AnimationCurve growCurve;
    public float multipile = 1;

    [Header("Harvest")] public Animator animator;
    public KeyCode key;
    public float high;
    public float delay;

    [Header("Score")] public TextMeshProUGUI scoreText;
    [Range(0f, 1f)] public float lerpScoreSpeed;

    private static readonly int USE = Animator.StringToHash("use");
    private bool isCooldown = false;
    private float cacheScore;
    private float score;
    //完成数量
    private int finishCount;
    private static readonly int FINISH = Animator.StringToHash("finish");

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        for (int i = 0; i < analyzer.NumberOfBands; i++)
        {
            var high = analyzer.BandLevels[i] * multipile;
            
            high *= 1f - growCurve.Evaluate(high / multipile);
            wheats[i].sizeDelta = new Vector2(100f, Mathf.Lerp(wheats[i].rect.height, high, lerpSpeed));
        }

        if (Input.GetKeyDown(key))
        {
            if (isCooldown)
                return;
            animator.SetTrigger(USE);

            for (int i = 0; i < analyzer.NumberOfBands; i++)
            {
                if (wheats[i].rect.height > high)
                {
                    var wheatGrow = wheats[i].gameObject.GetComponent<WheatGrow>();
                    if (!wheatGrow.IsClip)
                    {
                        wheatGrow.Clip();
                        score += wheatGrow.Score;
                        //提高倍率使剩余的小麦更容易完成
                        multipile *= 1.2f;
                        finishCount++;
                    }
                }
            }

            isCooldown = true;
            StartCoroutine(UseDelay());

            if (finishCount == analyzer.NumberOfBands)
                animator.SetBool(FINISH, true);
        }

        cacheScore = Mathf.Lerp(cacheScore, score, lerpScoreSpeed);

        //更新分数
        if (scoreText != null)
            scoreText.text = Mathf.Floor(cacheScore).ToString(CultureInfo.InvariantCulture);
    }

    IEnumerator UseDelay()
    {
        yield return new WaitForSeconds(delay);
        isCooldown = false;
    }
}