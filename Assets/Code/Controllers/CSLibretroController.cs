using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using com.PixelismGames.CSLibretro;
using com.PixelismGames.CSLibretro.Libretro;
using com.PixelismGames.WhistleStop.Utilities;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/CSLibretro Controller")]
    public class CSLibretroController : MonoBehaviour
    {
        #if UNITY_STANDALONE_OSX
        //private const string DLL_NAME = @"./Contrib/Cores/snes9x_libretro.dylib";
        private const string DLL_NAME = @"./Contrib/Cores/fceumm_libretro.dylib";
        //private const string DLL_NAME = @"./Contrib/Cores/gambatte_libretro.dylib";
        //private const string ROM_NAME = @"./Contrib/ROMs/smw.sfc";
        private const string ROM_NAME = @"./Contrib/ROMs/smb.nes";
        //private const string ROM_NAME = @"./Contrib/ROMs/sml.gb";
        #else
        //private const string DLL_NAME = @".\Contrib\Cores\snes9x_libretro.dll";
        private const string DLL_NAME = @".\Contrib\Cores\fceumm_libretro.dll";
        //private const string DLL_NAME = @".\Contrib\Cores\gambatte_libretro.dll";
        //private const string ROM_NAME = @".\Contrib\ROMs\smw.sfc";
        private const string ROM_NAME = @".\Contrib\ROMs\smb.nes";
        //private const string ROM_NAME = @".\Contrib\ROMs\sml.gb";
        #endif

        private Core _core;

        private List<float> _audioSampleBuffer;
        private object _audioSync;

        public bool IsMuted;
        public bool IsStepping;
        public bool IsFastForwarding;
        public bool ShowReporting;

        private long _audioSmoothedCountValue;
        private ReportingItemController _audioSmoothedCount;
        private ReportingItemController _audioRemainingSamples;

        public event Action BeforeRunFrame;
        public event Action AfterRunFrame;

        #region Properties

        public long FrameCount
        {
            get { return (_core.FrameCount); }
        }

        public int ScreenHeight
        {
            get { return (_core.ScreenHeight); }
        }

        public int ScreenWidth
        {
            get { return (_core.ScreenWidth); }
        }

        #endregion

        #region MonoBehaviour

        public void Awake()
        {
            _audioSync = new object();
            lock (_audioSync)
            {
                _audioSampleBuffer = new List<float>();
            }
        }

        public void Start()
        {
			#if UNITY_STANDALONE_OSX
            _core = new Core(OS.OSX, DLL_NAME);
			#else
			_core = new Core(OS.Windows, DLL_NAME);
			#endif

            _core.AudioSampleBatchHandler += audioSampleBatchHandler;
            _core.LogHandler += logHandler;
            _core.VideoFrameHandler += videoFrameHandler;

            _core.Load(ROM_NAME);

            AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
            audioConfiguration.sampleRate = (int)_core.AudioSampleRate;
            AudioSettings.Reset(audioConfiguration);

            // this is required for OnAudioFilterRead to work and needs to be done after setting the AudioSettings.outputSampleRate
            gameObject.AddComponent<AudioSource>();

            if (ShowReporting)
            {
                _audioSmoothedCount = Singleton.UI.CreateReportingItem("Smoothed Count");
                _audioRemainingSamples = Singleton.UI.CreateReportingItem("Remaining Samples");
            }

            _core.StartFrameTiming();
        }

        public void Update()
        {
            List<JoypadInputID> validInputs = new List<JoypadInputID>() { JoypadInputID.Up, JoypadInputID.Down, JoypadInputID.Left, JoypadInputID.Right, JoypadInputID.Start, JoypadInputID.Select, JoypadInputID.A, JoypadInputID.B, JoypadInputID.X, JoypadInputID.Y, JoypadInputID.L, JoypadInputID.R };
            foreach (CSLibretro.Input input in _core.Inputs.Where(i => (i.Port == 0) && (validInputs.Contains(i.JoypadInputID.Value))))
            {
                switch (input.JoypadInputID)
                {
                    case JoypadInputID.Up:
                        input.Value = Convert.ToInt16(UnityEngine.Input.GetAxis("DPad Vertical Axis") > 0f);
                        break;

                    case JoypadInputID.Down:
                        input.Value = Convert.ToInt16(UnityEngine.Input.GetAxis("DPad Vertical Axis") < 0f);
                        break;

                    case JoypadInputID.Left:
                        input.Value = Convert.ToInt16(UnityEngine.Input.GetAxis("DPad Horizontal Axis") < 0f);
                        break;

                    case JoypadInputID.Right:
                        input.Value = Convert.ToInt16(UnityEngine.Input.GetAxis("DPad Horizontal Axis") > 0f);
                        break;

                    default:
                        string buttonName = input.JoypadInputID.ToString() + " Button";
                        if (UnityEngine.Input.GetButtonDown(buttonName))
                            input.Value = 1;
                        if (UnityEngine.Input.GetButtonUp(buttonName))
                            input.Value = 0;
                        break;
                }
            }

            if (_core.HasFramePeriodElapsed() && (!IsStepping || UnityEngine.Input.GetKeyDown(KeyCode.Space)))
            {
                if (ShowReporting)
                    _audioSmoothedCount.Value = _audioSmoothedCountValue;

                if (BeforeRunFrame != null)
                    BeforeRunFrame();

                _core.RunFrame();

                if (AfterRunFrame != null)
                    AfterRunFrame();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
                IsStepping = !IsStepping;

            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
                IsFastForwarding = true;

            if (UnityEngine.Input.GetKeyUp(KeyCode.F2))
                IsFastForwarding = false;
        }

        // the fickle timing of this makes the exact timing of audio in emulators very tough; another solution may need to be found
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
                    // this is the easy case, the core has provided us with enough samples, just copy them over
                    Array.Copy(_audioSampleBuffer.ToArray(), data, sampleCount);
                    _audioSampleBuffer.RemoveRange(0, sampleCount);
                    return;
                }

                // the core has not given us enough samples, copy what is there and attempt to smooth the results
                sampleBuffer = new float[_audioSampleBuffer.Count];
                Array.Copy(_audioSampleBuffer.ToArray(), sampleBuffer, _audioSampleBuffer.Count);
                _audioSampleBuffer.RemoveRange(0, _audioSampleBuffer.Count);
            }

            _audioSmoothedCountValue++;

            // smooth the data by duping (averaging) every X samples so that we have enough samples
            // this ultimately needs to be done via time stretching

            int frameCount = data.Length / channels;
            int bufferFrameCount = sampleBuffer.Length / channels;

            double step = (double)frameCount / ((double)frameCount - (double)bufferFrameCount);
            double start = step / 2;

            List<int> generatedFrames = new List<int>();
            double cumulativeStep = start;
            while (cumulativeStep < frameCount)
            {
                generatedFrames.Add((int)Math.Floor(cumulativeStep));
                cumulativeStep += step;
            }

            int bufferIndex = 0;
            for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
                int sampleIndex = frameIndex * channels;

                if (generatedFrames.Contains(frameIndex))
                {
                    for (int i = 0; i < channels; i++)
                    {
                        if (bufferIndex == 0)
                            data[sampleIndex + i] = sampleBuffer[bufferIndex + i];
                        else if (bufferIndex == sampleBuffer.Length)
                            data[sampleIndex + i] = sampleBuffer[bufferIndex - channels + i];
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

        #region Handlers

        private void audioSampleBatchHandler(short[] samples)
        {
            if (!IsMuted && !IsStepping && !IsFastForwarding)
            {
                lock (_audioSync)
                {
                    if (ShowReporting)
                        if (_core.FrameCount % 60 == 0)
                            _audioRemainingSamples.Value = _audioSampleBuffer.Count;

                    _audioSampleBuffer.AddRange(samples.Select(s => (float)((double)s / (double)short.MaxValue)).ToList());
                }
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

            if (Singleton.Screen.sprite == null)
            {
                Singleton.Screen.sprite = Sprite.Create(videoFrameTexture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), Singleton.PIXELS_PER_UNIT);
                Singleton.Screen.sprite.texture.filterMode = FilterMode.Point;
            }
            else
            {
                Graphics.CopyTexture(videoFrameTexture, Singleton.Screen.sprite.texture);
            }
        }

        #endregion

        #region Accessible Routines

        public byte[] ReadRAM()
        {
            return (_core.ReadRAM());
        }

        public void WriteRAM(byte[] data, int offset = 0)
        {
            _core.WriteRAM(data, offset);
        }

        public void LoadState(string stateFilePath)
        {
            _core.LoadState(stateFilePath);
        }

        public void LoadState(byte[] state)
        {
            GCHandle pinnedState = GCHandle.Alloc(state, GCHandleType.Pinned);
            _core.UnserializePassthrough(pinnedState.AddrOfPinnedObject(), (uint)state.Length);
            pinnedState.Free();
        }

        public void SaveState(string stateFilePath)
        {
            _core.SaveState(stateFilePath);
        }

        public void SaveScreenshot(string screenshotFilePath)
        {
            int width = Singleton.Screen.sprite.texture.width;
            int height = Singleton.Screen.sprite.texture.height;

            Texture2D rightedScreen = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    rightedScreen.SetPixel(x, (height - y - 1), Singleton.Screen.sprite.texture.GetPixel(x, y));

            rightedScreen.Apply();

            File.WriteAllBytes(screenshotFilePath, rightedScreen.EncodeToPNG());
        }

        #endregion
    }
}
