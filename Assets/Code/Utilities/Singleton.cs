using com.PixelismGames.WhistleStop.Controllers;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Utilities
{
    public static class Singleton
    {
        public const float PIXELS_PER_UNIT = 1f;

        private static Camera _camera;
        private static SpriteRenderer _screen;
        private static CSLibretroController _csLibretro;
        private static UIController _ui;

        #region Singletons

        public static Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    Debug.Log("ERROR | Singleton: Camera not yet provided");
                    Debug.Break();
                }

                return (_camera);
            }
        }

        public static SpriteRenderer Screen
        {
            get
            {
                if (_screen == null)
                {
                    Debug.Log("ERROR | Singleton: Screen not yet provided");
                    Debug.Break();
                }

                return (_screen);
            }
        }

        public static CSLibretroController CSLibretro
        {
            get
            {
                if (_csLibretro == null)
                {
                    Debug.Log("ERROR | Singleton: CSLibretro not yet provided");
                    Debug.Break();
                }

                return (_csLibretro);
            }
        }

        public static UIController UI
        {
            get
            {
                if (_ui == null)
                {
                    Debug.Log("ERROR | Singleton: UI not yet provided");
                    Debug.Break();
                }

                return (_ui);
            }
        }

        #endregion

        #region Provide Routines

        public static void ProvideCamera(Camera camera)
        {
            if (_camera == null)
            {
                _camera = camera;
            }
            else
            {
                Debug.Log("ERROR | Singleton: Camera already provided");
                Debug.Break();
            }
        }

        public static void ProvideScreen(SpriteRenderer screen)
        {
            if (_screen == null)
            {
                _screen = screen;
            }
            else
            {
                Debug.Log("ERROR | Singleton: Screen already provided");
                Debug.Break();
            }
        }

        public static void ProvideCSLibretro(CSLibretroController csLibretro)
        {
            if (_csLibretro == null)
            {
                _csLibretro = csLibretro;
            }
            else
            {
                Debug.Log("ERROR | Singleton: CSLibretro already provided");
                Debug.Break();
            }
        }

        public static void ProvideUI(UIController ui)
        {
            if (_ui == null)
            {
                _ui = ui;
            }
            else
            {
                Debug.Log("ERROR | Singleton: UI already provided");
                Debug.Break();
            }
        }

        #endregion
    }
}
