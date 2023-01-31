using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OceanBackground : MonoBehaviour
{
    VideoPlayer _videoPlayer;
    public List<VideoClip> _clips = new List<VideoClip>();
    private float _time;
    private float _playerSpeed;
    private int _clipPlaying;

    private void Start()
    {
        _clipPlaying = 0;
        _videoPlayer = GetComponent<VideoPlayer>();
        _playerSpeed = _videoPlayer.playbackSpeed;
    }
    void Update()
    {
        _time += Time.deltaTime;
        if (_videoPlayer.length / _playerSpeed <= _time)
        {
            _time = 0;
            _videoPlayer.clip = _clips[_clipPlaying];
            _clipPlaying++;
            _videoPlayer.Play();
        }
        if (_clipPlaying >= _clips.Count)
        {
            _clipPlaying = 0;
        }
    }
}