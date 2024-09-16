using System.Collections.Generic;
using UnityEngine;

public class ExplosionAudio : MonoBehaviour
{
    [SerializeField] private List<AudioSource> _exlposionAudioList;

    private void Start()
    {
        EventManager.DoneDestruction.AddListener(PlayEffectsExplosion);
    }

    private void PlayEffectsExplosion()
    {
        if (GameManager.Instance.DestroyedOnWave > 0)
        {
            int exlosionAudioIdx;
            if (GameManager.Instance.IsTopExplosion)
                exlosionAudioIdx = 0;
            else
                exlosionAudioIdx = 1;

            _exlposionAudioList[exlosionAudioIdx].Play();
        }
    }
}
