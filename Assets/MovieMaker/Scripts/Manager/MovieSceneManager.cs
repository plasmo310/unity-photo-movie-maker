using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace MovieMaker.Manager
{
    /// <summary>
    /// MovieScene管理クラス
    /// </summary>
    public class MovieSceneManager : MonoBehaviour
    {
        /// <summary>
        /// スライド表示UI
        /// </summary>
        [SerializeField] private GameObject _slideCanvas;
        [SerializeField] private GameObject _slideImagePrefab;
        [SerializeField] private GameObject _slideTextPrefab;

        /// <summary>
        /// アルバム再生用タイムライン
        /// </summary>
        [SerializeField] private PlayableDirector _albumTimelineDirector;

        /// <summary>
        /// スライドUIを表示する最大数
        /// この数だけ起動時にプレハブを生成する
        /// </summary>
        [SerializeField] private int _slideUiMaxCount = 10;

        private void Start()
        {
            // 表示用のプレハブを生成
            for (var i = 0; i < _slideUiMaxCount; i++)
            {
                Instantiate(_slideImagePrefab, _slideCanvas.transform);
                Instantiate(_slideTextPrefab, _slideCanvas.transform);
            }

            StartCoroutine(StartMovieCoroutine());
        }

        /// <summary>
        /// ムービーの再生
        /// </summary>
        private IEnumerator StartMovieCoroutine()
        {
            // 同期させるために一回止める
            _albumTimelineDirector.Play();
            _albumTimelineDirector.Stop();

            yield return new WaitForSeconds(0.5f);
            _albumTimelineDirector.Play();
        }
    }
}
