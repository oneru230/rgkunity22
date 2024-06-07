using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RacingGameKit.UI
{
    [AddComponentMenu("")] //Menu removal
    public class RGKUI_DemoLoader:MonoBehaviour
    {
        public Slider m_LoadingSlider;
        public int m_TargetLevelIndex = 1;
        AsyncOperation m_AsyncLoadingProcess;

        void Start()
        {
            StartCoroutine(LoadLevel(m_TargetLevelIndex));
        }

        void Update()
        {
            if (m_LoadingSlider != null)
            {
                m_LoadingSlider.value = m_AsyncLoadingProcess.progress * 100;
            }
        }

        IEnumerator LoadLevel(int TrackIndex)
        {
            m_AsyncLoadingProcess = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(TrackIndex);

            yield return m_AsyncLoadingProcess;
        }
    }
}
