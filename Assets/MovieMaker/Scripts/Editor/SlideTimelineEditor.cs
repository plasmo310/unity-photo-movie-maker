using System.Collections.Generic;
using System.IO;
using MovieMaker.Slide;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace MovieMaker.Editor
{
    /// <summary>
    /// スライドTimelineエディタ拡張
    /// ScriptableObjectデータからKeyframe用のGUIを表示する
    /// </summary>
    [CustomEditor(typeof(TimelineAsset))]
    public class SlideTimelineEditor : UnityEditor.Editor
    {
        /// <summary>
        /// スライド情報（内部保持用）
        /// </summary>
        private SlideData _slideData;

        /// <summary>
        /// Timeline格納パス
        /// </summary>
        private static readonly string TimelineDirPath = "Assets/MovieMaker/Timeline";

        /// <summary>
        /// Slideトラック格納パス
        /// ※トラック生成時に保存される
        /// </summary>
        private static readonly string SlideTracksDirPath = $"{TimelineDirPath}/SlideTracks";

        /// <summary>
        /// DummyTimelineアセットパス
        /// ※更新後のタイムライン表示を切り替えるために一時的に切り替える
        /// </summary>
        private static readonly string DummyTimelineAssetPath = $"{TimelineDirPath}/Dummy/DummyTimeline.playable";

        /// <summary>
        /// 生成対象のトラック名
        /// </summary>
        private const string SlideTrackName = "SlidePlayableTrack";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // キーフレーム作成用のGUI表示
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Slide Track Generate", EditorStyles.boldLabel);

            // TODO: ObjectFieldだと保存されないので、本当はSerializedFieldを使いたい.
            // TODO: しかしその場合にはCustomのTimelineAssetを作成する必要があり、Timelineアセットの生成など面倒なことになりそうなので保留.
            _slideData = (SlideData) EditorGUILayout.ObjectField("Slide Data", _slideData, typeof(SlideData), false);

            // Generateボタン
            if (GUILayout.Button("Generate"))
            {
                // Timelineの内容が動的に更新されないため、一時的に他のアセットを選択
                var dummy = AssetDatabase.LoadAssetAtPath<TimelineAsset>(DummyTimelineAssetPath);
                Selection.activeObject = dummy;

                // トラック生成
                var timelineAsset = (TimelineAsset) target;
                GenerateTracks(timelineAsset, _slideData);
                EditorUtility.SetDirty(timelineAsset);

                // 選択しなおすことでTimelineを強制再描画
                Selection.activeObject = timelineAsset;
            }
        }

        /// <summary>
        /// PlayableTrack生成
        /// 管理を簡潔にするために 1スライド/1トラック で生成する
        /// </summary>
        /// <param name="playableTrack">PlayableTrack</param>
        /// <param name="slideDaraParam">スライド情報データ</param>
        private static void GenerateTrack(PlayableTrack playableTrack, SlideData.Param slideDaraParam)
        {
            // SlidePlayableAssetの作成
            var slidePlayableAsset = ScriptableObject.CreateInstance<SlidePlayableAsset>();
            slidePlayableAsset.SetSlideData(slideDaraParam);
            var slidePlayableAssetPath = Path.Join(SlideTracksDirPath, $"{playableTrack.name}.asset");
            AssetDatabase.CreateAsset(slidePlayableAsset, slidePlayableAssetPath);

            // Clipに設定
            var clip = playableTrack.CreateClip<SlidePlayableAsset>();
            clip.asset = slidePlayableAsset;
            clip.start = slideDaraParam.Frame;
            clip.duration = slideDaraParam.ShowDuration;
        }

        /// <summary>
        /// 全てのPlayableTrack生成
        /// </summary>
        /// <param name="timelineAsset">Timelineアセット</param>
        /// <param name="slideData">スライド情報データ</param>
        private static void GenerateTracks(TimelineAsset timelineAsset, SlideData slideData)
        {
            if (slideData == null)
            {
                Debug.LogError("please set slide data.");
                return;
            }

            // 作成済のキーフレームを削除する
            var deleteTracks = new List<TrackAsset>();
            foreach (var outputTrack in timelineAsset.GetOutputTracks())
            {
                if (outputTrack.name.Contains(SlideTrackName))
                {
                    deleteTracks.Add(outputTrack);
                }
            }
            foreach (var deleteTrack in deleteTracks)
            {
                var deleteTrackName = deleteTrack.name;
                timelineAsset.DeleteTrack(deleteTrack);
                Debug.Log($"delete track => {deleteTrackName}");
            }

            // 作成済のスライド情報アセットを削除
            var slideDataAssetGuids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { SlideTracksDirPath });
            foreach (var slideDataAssetGuid in slideDataAssetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(slideDataAssetGuid);
                AssetDatabase.DeleteAsset(assetPath);
                Debug.Log($"delete asset => {assetPath}");
            }

            // AssetDatabaseを更新
            AssetDatabase.Refresh();

            // パラメータごとにTrackを生成する
            var slideTrackIndex = 1;
            foreach (var sheet in slideData.sheets)
            {
                // パラメータからTrackを生成
                foreach (var param in sheet.list)
                {
                    var trackName = $"{SlideTrackName}_{slideTrackIndex}";
                    var track = timelineAsset.CreateTrack<PlayableTrack>(null, trackName);
                    GenerateTrack(track, param);
                    slideTrackIndex++;
                    Debug.Log($"create track => {trackName}");
                }
            }
        }
    }
}
