using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.PixelismGames.CSLibretro;
using com.PixelismGames.CSLibretro.Libretro;
using UnityEngine;
using UnityEngine.UI;

namespace com.PixelismGames.WhistleStop
{
    [AddComponentMenu("Pixelism Games/Controllers/CSLibretro Controller")]
    public class CSLibretroController : MonoBehaviour
    {
        private const float PIXELS_PER_UNIT = 1f;

        //private const string DLL_NAME = "snes9x_libretro.dll";
        private const string DLL_NAME = "fceumm_libretro.dll";
        //private const string DLL_NAME = "gambatte_libretro.dll";

        //private const string ROM_NAME = "smw.sfc";
        private const string ROM_NAME = "smb.nes";
        //private const string ROM_NAME = "sml.gb";

        public GameObject Screen; // move this out to a global singleton

        private Core _core;
        private SpriteRenderer _screenRenderer;
        private List<float> _audioSampleBuffer;
        private object _audioSync;

        public int Unders = 0;
        public int Extras = 0;

        #region MonoBehaviour

        public void Awake()
        {
            _screenRenderer = Screen.GetComponent<SpriteRenderer>();

            _audioSync = new object();
            lock (_audioSync)
            {
                _audioSampleBuffer = new List<float>();
            }

            gameObject.AddComponent<AudioSource>();
        }

        public void Start()
        {
            _core = new Core(DLL_NAME);

            _core.AudioSampleBatchHandler += audioSampleBatchHandler;
            _core.LogHandler += logHandler;
            _core.VideoFrameHandler += videoFrameHandler;

            _core.Load(ROM_NAME);

            Debug.Log(AudioSettings.outputSampleRate);

            AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
            //audioConfiguration.sampleRate = (int)_core.AudioSampleRate;
            audioConfiguration.sampleRate = 31550;
            AudioSettings.Reset(audioConfiguration);

            Debug.Log(AudioSettings.outputSampleRate);
            Debug.Log(_core.FrameRate);

            // this is required for OnAudioFilterRead to work and needs to be done after setting the AudioSettings.outputSampleRate
            gameObject.AddComponent<AudioSource>();
        }

        public void Update()
        {
            StartCoroutine(clockFrame());

            GameObject.Find("Unders").GetComponent<Text>().text = "Unders: " + Unders;
            GameObject.Find("Extras").GetComponent<Text>().text = "Extras: " + Extras;

            List<JoypadInputID> validInputs = new List<JoypadInputID>() { JoypadInputID.Up, JoypadInputID.Down, JoypadInputID.Left, JoypadInputID.Right, JoypadInputID.Start, JoypadInputID.Select, JoypadInputID.A, JoypadInputID.B, JoypadInputID.X, JoypadInputID.Y };
            foreach (CSLibretro.Input input in _core.Inputs.Where(i => (i.Port == 0) && (validInputs.Contains(i.JoypadInputID.Value))))
            {
                if (UnityEngine.Input.GetButtonDown(input.JoypadInputID.ToString()))
                    input.Value = 1;

                if (UnityEngine.Input.GetButtonUp(input.JoypadInputID.ToString()))
                    input.Value = 0;
            }

            _core.RunFrame();
        }

        public void OnAudioFilterRead(float[] data, int channels)
        {
            int sampleCount = data.Length;
            float[] sampleBuffer;

            lock (_audioSync)
            {
                if (!_audioSampleBuffer.Any())
                {
                    return;
                }
                else if (_audioSampleBuffer.Count >= sampleCount)
                {
                    sampleBuffer = new float[sampleCount];
                    Array.Copy(_audioSampleBuffer.ToArray(), data, sampleCount);
                    _audioSampleBuffer.RemoveRange(0, sampleCount);
                    return;
                }
                else
                {
                    //return;

                    //Array.Copy(_audioSampleBuffer.ToArray(), data, _audioSampleBuffer.Count);
                    //_audioSampleBuffer.RemoveRange(0, _audioSampleBuffer.Count);
                    //return;

                    sampleBuffer = new float[_audioSampleBuffer.Count];
                    Array.Copy(_audioSampleBuffer.ToArray(), sampleBuffer, _audioSampleBuffer.Count);
                    _audioSampleBuffer.RemoveRange(0, _audioSampleBuffer.Count);
                    Unders++;
                }
            }

            int frameCount = data.Length / channels;
            int bufferFrameCount = sampleBuffer.Length / channels;

            double step = (double)frameCount / ((double)frameCount - (double)bufferFrameCount);
            double start = step / 2;

            //Debug.Log(frameCount + " | " + bufferFrameCount + " | " + (frameCount - bufferFrameCount) + " | " + step + " | " + start);

            List<int> roundedFrames = new List<int>();
            double cumulativeStep = start;
            while (cumulativeStep < frameCount)
            {
                roundedFrames.Add((int)Math.Floor(cumulativeStep));
                cumulativeStep += step;
            }

            //Debug.Log((frameCount - bufferFrameCount) + " | " + roundedFrames.Count + " | " + System.Threading.Thread.CurrentThread.ManagedThreadId);

            //Debug.Log(sampleBuffer.Length + " | " + frameCount + " | " + bufferFrameCount + " | " + (frameCount - bufferFrameCount) + " | " + roundedFrames.Count);

            int bufferIndex = 0;
            for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
                int sampleIndex = frameIndex * channels;

                if (roundedFrames.Contains(frameIndex))
                {
                    for (int i = 0; i < channels; i++)
                    {
                        if (bufferIndex == 0)
                            data[sampleIndex + i] = sampleBuffer[bufferIndex + i];
                        else
                            data[sampleIndex + i] = (sampleBuffer[bufferIndex - channels + i] + sampleBuffer[bufferIndex + i]) / 2f;
                    }
                }
                else
                {
                    for (int i = 0; i < channels; i++)
                        data[sampleIndex + i] = sampleBuffer[bufferIndex + i];

                    bufferIndex += channels;
                }
            }
        }

        #endregion

        private IEnumerator clockFrame()
        {
            yield return new WaitForEndOfFrame();

            _core.StopFrameTiming();
            _core.SleepRemainingFrameTime();
            _core.StartFrameTiming();
        }

        private void audioSampleBatchHandler(short[] samples)
        {
            lock (_audioSync)
            {
                if (_core.FrameCount % 60 == 0)
                    Extras = _audioSampleBuffer.Count;

                _audioSampleBuffer.AddRange(samples.Select(s => (float)((double)s / (double)short.MaxValue)).ToList());
            }
        }

        private void logHandler(LogLevel level, string message)
        {
            Debug.Log(string.Format("[{0}] {1}", (int)level, message));
        }

        private void videoFrameHandler(int width, int height, byte[] frameBuffer)
        {
            TextureFormat textureFormat;

            switch (_core.PixelFormat)
            {
                case PixelFormat.RGB565:
                    textureFormat = TextureFormat.RGB565;
                    break;

                default:
                    textureFormat = TextureFormat.ARGB32;
                    break;
            }

            Texture2D videoFrameTexture = new Texture2D(width, height, textureFormat, false);
            videoFrameTexture.LoadRawTextureData(frameBuffer);
            videoFrameTexture.Apply();

            if (_screenRenderer.sprite == null)
            {
                _screenRenderer.sprite = Sprite.Create(videoFrameTexture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), PIXELS_PER_UNIT);
                _screenRenderer.sprite.texture.filterMode = FilterMode.Point;
            }
            else
            {
                Graphics.CopyTexture(videoFrameTexture, _screenRenderer.sprite.texture);
            }
        }
    }
}