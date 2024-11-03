using UnityEngine;
using UnityEngine.Playables;

namespace MovieMaker.Slide
{
    /// <summary>
    /// スライドデータPlayableAsset
    /// </summary>
    public class SlidePlayableAsset : PlayableAsset
    {
        /// <summary>
        /// スライド情報(初期設定用)
        /// </summary>
        [SerializeField] private SlideData.Param _slideData;

        /// <summary>
        /// スライド情報の設定
        /// </summary>
        /// <param name="slideData"></param>
        public void SetSlideData(SlideData.Param slideData)
        {
            _slideData = slideData;
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // playableのBehaviourにデータを設定して返却
            var playable = ScriptPlayable<SlidePlayableBehaviour>.Create(graph);
            var playableBehaviour = playable.GetBehaviour();
            playableBehaviour.SlideData = _slideData;
            return playable;
        }
    }
}