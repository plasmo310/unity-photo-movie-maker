using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MovieMaker.Slide.UI
{
    /// <summary>
    /// スライド画像UI
    /// </summary>
    public class SlideImage : MonoBehaviour
    {
        /// <summary>
        /// スライドImageUI
        /// </summary>
        [SerializeField] private Image _slideImage;

        /// <summary>
        /// スライドRectTransform
        /// </summary>
        [SerializeField] private RectTransform _slideRectTransform;

        /// <summary>
        /// 表示中か？
        /// </summary>
        private bool _isShowing = false;
        public bool IsShowing => _isShowing;

        private void Start()
        {
            // 最初は非表示
            Hide();
        }

        /// <summary>
        /// スライドデータの設定
        /// </summary>
        /// <param name="slideData">画像データ</param>
        public void SetData(SlideData.Param slideData)
        {
            ClearData();

            // 名前からテクスチャを取得
            var newTexture = Resources.Load<Texture2D>(slideData.TexturePath);
            if (newTexture == null)
            {
                Debug.LogError($"not found texture => {slideData.TexturePath}");
                return;
            }

            // スプライトの設定
            var newSprite = Sprite.Create(
                newTexture,
                new Rect(0, 0, newTexture.width, newTexture.height),
                new Vector2(0.5f, 0.5f));
            _slideImage.sprite = newSprite;
            _slideRectTransform.localPosition = new Vector3(
                (float) slideData.PositionX * (Screen.width / 2.0f),
                (float) slideData.PositionY * (Screen.height / 2.0f),
                0.0f);
            _slideRectTransform.sizeDelta = new Vector2(
                (float) slideData.Scale * newTexture.width,
                (float) slideData.Scale * newTexture.height);
        }

        /// <summary>
        /// 画像データのクリア
        /// </summary>
        public void ClearData()
        {
            _slideImage.sprite = null;
            _slideRectTransform.localPosition = Vector3.zero;
            _slideRectTransform.sizeDelta = Vector2.one;
        }

        /// <summary>
        /// 表示処理
        /// </summary>
        /// <param name="slideData">画像データ</param>
        public void Show(SlideData.Param slideData)
        {
            _isShowing = true;

            // 画像データ設定
            SetData(slideData);

            // アニメーション開始
            var sequence = DOTween.Sequence();

            // 移動処理
            var moveSpeedX = (float) slideData.MoveSpeedX;
            var moveSpeedY = (float) slideData.MoveSpeedY;
            var movePosition = new Vector3(moveSpeedX, moveSpeedY, 0f) * (float) slideData.ShowDuration;
            if (movePosition.sqrMagnitude > Mathf.Epsilon)
            {
                transform.DOMove(movePosition, (float)slideData.ShowDuration)
                    .SetRelative()
                    .SetEase(Ease.Linear);
            }

            // フェードイン
            var tmpColor = _slideImage.color;
            tmpColor.a = 0.0f;
            _slideImage.color = tmpColor;
            sequence.Append(DOTween.ToAlpha(
                () => _slideImage.color,
                color => _slideImage.color = color,
                1.0f,
                (float) slideData.FadeInDuration
                ));

            // 表示待機時間
            var showDuration = slideData.ShowDuration - slideData.FadeInDuration - slideData.FadeOutDuration;
            sequence.AppendInterval((float) showDuration);

            // フェードアウト
            sequence.Append(DOTween.ToAlpha(
                () => _slideImage.color,
                color => _slideImage.color = color,
                0.0f,
                (float) slideData.FadeOutDuration
            ));

            // 表示フラグをオフにする
            sequence.AppendCallback(() =>
            {
                _isShowing = false;
            });
        }

        /// <summary>
        /// 非表示処理
        /// </summary>
        public void Hide()
        {
            ClearData();

            // 画像非表示
            var tmpColor = _slideImage.color;
            tmpColor.a = 0.0f;
            _slideImage.color = tmpColor;
        }
    }
}
