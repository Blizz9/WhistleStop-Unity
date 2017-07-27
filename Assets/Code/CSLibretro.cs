using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using CSLibretro;
using UnityEngine;

namespace com.PixelismGames.WhistleStop
{
    [AddComponentMenu("Pixelism Games/CSLibretro")]
    public class CSLibretro : MonoBehaviour
    {
        public GameObject sprite;

        private const string DLL_NAME = "snes9x_libretro.dll";
        //private const string DLL_NAME = "nestopia_libretro.dll";
        //private const string DLL_NAME = "gambatte_libretro.dll";

        private Wrapper _wrapper;

        private double _framePeriodNanoseconds = 1000000000 / 60.0988118623484;
        private double _frameLeftoverNanoseconds = 0;

        private System.Diagnostics.Stopwatch _stopwatch;

        #region MonoBehaviour

        public void Awake()
        {
            _stopwatch = new System.Diagnostics.Stopwatch();
        }

        public void Start()
        {
            _wrapper = new Wrapper(Callback);
            //_s.Start();
            //Debug.Log(QualitySettings.vSyncCount);
            //Debug.Log("Start Thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        public void Update()
        {
            StartCoroutine(clockFrame());
            //Debug.Log("Here: " + Time.frameCount + " | " + Time.deltaTime);

            //60.0988118623484

            //float milli = 17f - Time.deltaTime * 1000f;
            //Debug.Log(milli + " | " + (int)milli);

            //if (milli >= 0)
            //    System.Threading.Thread.Sleep((int)milli);

            //_s.Stop();
            //Debug.Log(_s.ElapsedMilliseconds);
            //Debug.Log("Update Frame: " + Time.frameCount);
            _wrapper.Run();

            //_s.Reset();
            //_s.Start();
            //Debug.Log("Update Thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        #endregion

        private IEnumerator clockFrame()
        {
            yield return new WaitForEndOfFrame();

            _stopwatch.Stop();

            double frameElapsedNanoseconds = ((double)_stopwatch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency) * 1000000000;
            _frameLeftoverNanoseconds += _framePeriodNanoseconds - frameElapsedNanoseconds;
            if (_frameLeftoverNanoseconds > 0)
            {
                Thread.Sleep((int)(_frameLeftoverNanoseconds / 1000000));
                _frameLeftoverNanoseconds %= 1000000;
            }
            else
            {
                _frameLeftoverNanoseconds = 0;
            }

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public void Callback(IntPtr videoBufferAddress, int width, int height, int stride)
        {
            Texture2D videoTexture = new Texture2D(width, height, TextureFormat.RGB565, false);

            int rowSize = width * sizeof(short);
            if (rowSize == stride)
            {
                videoTexture.LoadRawTextureData(videoBufferAddress, (height * stride));
            }
            else
            {
                // if the data also contains the back buffer, we have to rip out just the first frame
                int newVideoBufferSize = height * width * sizeof(short);
                byte[] newVideoBuffer = new byte[newVideoBufferSize];

                for (int i = 0; i < height; i++)
                {
                    IntPtr rowAddress = (IntPtr)((int)videoBufferAddress + (i * stride));
                    int newRowIndex = i * rowSize;
                    Marshal.Copy(rowAddress, newVideoBuffer, newRowIndex, rowSize);
                }

                videoTexture.LoadRawTextureData(newVideoBuffer);
            }

            videoTexture.Apply();

            if (sprite.GetComponent<SpriteRenderer>().sprite == null)
            {
                sprite.GetComponent<SpriteRenderer>().sprite = Sprite.Create(videoTexture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 1f);
                sprite.GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Point;
            }
            else
            {
                Graphics.CopyTexture(videoTexture, sprite.GetComponent<SpriteRenderer>().sprite.texture);
            }

            //Debug.Log("Callback Thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //Debug.Log("Callback Frame: " + Time.frameCount);
        }
    }
}
