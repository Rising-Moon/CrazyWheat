using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WheatGrow : MonoBehaviour
{
    public Sprite bottomSprite;
    public Sprite topSprite;

    public int countOfBottom = 1;

    public Vector2 splitSize = new Vector2(100f, 100f);

    //分数
    private float score = 0;
    public float Score => score;

    [Header("Clip")] public Vector2 horForce;
    public float verForce;

    private List<RectTransform> splitTransforms = new List<RectTransform>();
    private float high;

    //是否被收割
    private bool isClip;
    public bool IsClip => isClip;

    private void Update()
    {
        if (bottomSprite == null || isClip)
            return;
        high = (transform as RectTransform).rect.height;
        //最少需要的数量
        var minCount = Mathf.Ceil(high / splitSize.y);

        for (int i = 0; i < 20 && splitTransforms.Count < minCount; i++)
        {
            CreateSplit();
        }

        //清理空成员
        for (int i = splitTransforms.Count - 1; i >= 0; i--)
        {
            if (splitTransforms[i] == null)
                splitTransforms.RemoveAt(i);
        }

        RefreshGrow();
    }

    //调整长势
    private void RefreshGrow()
    {
        for (int i = 0; i < splitTransforms.Count; i++)
        {
            var trans = splitTransforms[i];
            //设置起始位置
            trans.localPosition = new Vector3(0, i * splitSize.y);
            //设置大小
            var size = Mathf.Max(Mathf.Min(high - i * splitSize.y, splitSize.y), 0f);
            trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            //设置图片
            trans.gameObject.GetComponent<Image>().sprite =
                i >= countOfBottom && topSprite != null ? topSprite : bottomSprite;
        }
    }

    //创建新的一节
    private void CreateSplit()
    {
        var go = new GameObject();
        var rectTrans = go.AddComponent<RectTransform>();
        rectTrans.SetParent(transform);
        rectTrans.pivot = new Vector2(0.5f, 0f);
        rectTrans.anchorMax = rectTrans.anchorMin = new Vector2(0.5f, 0f);
        rectTrans.localPosition = Vector3.zero;
        rectTrans.localScale = Vector3.one;
        rectTrans.rotation = Quaternion.identity;
        rectTrans.AddComponent<Image>();
        splitTransforms.Add(rectTrans);
    }

    //切割
    public void Clip()
    {
        if (isClip)
            return;
        isClip = true;
        for (int i = countOfBottom; i < splitTransforms.Count; i++)
        {
            var trans = splitTransforms[i];
            var rb = trans.AddComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(Random.Range(horForce.x, horForce.y), verForce));
            score += trans.rect.height;
        }
    }
}