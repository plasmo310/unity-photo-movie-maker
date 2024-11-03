using System.Linq;
using MovieMaker.Slide.UI;
using UnityEngine;
using UnityEngine.Playables;

namespace MovieMaker.Slide
{
    /// <summary>
    /// スライドデータPlayableBehavior
    /// </summary>
    public class SlidePlayableBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// スライドデータ
        /// </summary>
        public SlideData.Param SlideData { set; private get; }

        /// <summary>
        /// 文字列未設定時の値
        /// </summary>
        private const string StringNoneValue = "-";

        /// <summary>
        /// PlayableAsset再生時処理
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);

            if (SlideData == null)
            {
                Debug.LogError($"not set image data => {info.frameId}");
                return;
            }

            if (SlideData.TexturePath != StringNoneValue)
            {
                // ========== 画像の場合 ==========
                // Scene上からSlideImageオブジェクトを取得
                var slideImageArray = GameObject.FindObjectsByType<SlideImage>(FindObjectsSortMode.None);

                // 表示中でないオブジェクトに対してデータを設定して表示する
                var slideImage = slideImageArray.FirstOrDefault(s => !s.IsShowing);
                if (slideImage == null)
                {
                    Debug.LogError($"all image showing => {info.frameId}");
                    return;
                }
                slideImage.Show(SlideData);
            }
            else if (SlideData.Message != StringNoneValue)
            {
                // ========== テキストの場合 ==========
                // Scene上からSlideImageオブジェクトを取得
                var slideTextArray = GameObject.FindObjectsByType<SlideText>(FindObjectsSortMode.None);

                // 表示中でないオブジェクトに対してデータを設定して表示する
                var slideText = slideTextArray.FirstOrDefault(s => !s.IsShowing);
                if (slideText == null)
                {
                    Debug.LogError($"all text showing => {info.frameId}");
                    return;
                }
                slideText.Show(SlideData);
            }
        }
    }
}
