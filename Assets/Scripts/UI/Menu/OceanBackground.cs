using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OceanBackground : MonoBehaviour
{
    /*VideoPlayer _videoPlayer;
    private float _playbackSpeed;
    private bool _isPlayingForward = true;

    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _playbackSpeed = _videoPlayer.playbackSpeed;
    }
    void Update()
    {
        if (_videoPlayer.isPlaying)
        {
            if (_isPlayingForward && _videoPlayer.time >= _videoPlayer.clip.length)
            {
                _videoPlayer.playbackSpeed = -_playbackSpeed;
                _isPlayingForward = false;
            }
            else if (!_isPlayingForward && _videoPlayer.time <= 0)
            {
                _videoPlayer.playbackSpeed = _playbackSpeed;
                _isPlayingForward = true;
            }
        }
        else
        {
            _videoPlayer.Play();
        }
    }*/
}