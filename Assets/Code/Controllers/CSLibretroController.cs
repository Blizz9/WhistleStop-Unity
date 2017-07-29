using System.Collections;
using com.PixelismGames.CSLibretro;
using com.PixelismGames.CSLibretro.Libretro;
using UnityEngine;

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

        #region MonoBehaviour

        public void Awake()
        {
            _screenRenderer = Screen.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            _core = new Core(DLL_NAME);

            _core.LogHandler += logHandler;
            _core.VideoFrameHandler += videoFrameHandler;

            _core.Load(ROM_NAME);
        }

        public void Update()
        {
            StartCoroutine(clockFrame());

            _core.RunFrame();
        }

        #endregion

        private IEnumerator clockFrame()
        {
            yield return new WaitForEndOfFrame();

            _core.StopFrameTiming();
            _core.SleepRemainingFrameTime();
            _core.StartFrameTiming();
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