using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MovieMaker.Slide.UI
{
    /// <summary>
    /// スライドテキストUI
    /// </summary>
    public class SlideText : MonoBehaviour
    {
        /// <summary>
        /// スライドテキストUI
        /// </summary>
        [SerializeField] private Text _slideText;

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
        /// <param name="slideData">スライドデータ</param>
        public void SetData(SlideData.Param slideData)
        {
            ClearData();

            // テキストの設定
            _slideText.text = slideData.Message;
            _slideRectTransform.localPosition = new Vector3(
                (float) slideData.PositionX * (Screen.width / 2.0f),
                (float) slideData.PositionY * (Screen.height / 2.0f),
                0.0f);
            _slideRectTransform.localScale *= (float) slideData.Scale;
        }

        /// <summary>
        /// データのクリア
        /// </summary>
        public void ClearData()
        {
            _slideText.text = null;
            _slideRectTransform.localPosition = Vector3.zero;
            _slideRectTransform.localScale = Vector3.one;
        }

        /// <summary>
        /// 表示処理
        /// </summary>
        /// <param name="slideData">スライドデータ</param>
        public void Show(SlideData.Param slideData)
        {
            _isShowing = true;

            // データ設定
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
            var tmpColor = _slideText.color;
            tmpColor.a = 0.0f;
            _slideText.color = tmpColor;
            sequence.Append(DOTween.ToAlpha(
                () => _slideText.color,
                color => _slideText.color = color,
                1.0f,
                (float) slideData.FadeInDuration
            ));

            // 表示待機時間
            var showDuration = slideData.ShowDuration - slideData.FadeInDuration - slideData.FadeOutDuration;
            sequence.AppendInterval((float) showDuration);

            // フェードアウト
            sequence.Append(DOTween.ToAlpha(
                () => _slideText.color,
                color => _slideText.color = color,
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
            var tmpColor = _slideText.color;
            tmpColor.a = 0.0f;
            _slideText.color = tmpColor;
        }
    }
}
