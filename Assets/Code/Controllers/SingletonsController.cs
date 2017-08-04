using com.PixelismGames.WhistleStop.Utilities;
using Unity.Linq;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/Singletons Controller")]
    public class SingletonsController : MonoBehaviour
    {
        #region MonoBehaviour

        public void Awake()
        {
            Singleton.ProvideCamera(gameObject.Children().OfComponent<Camera>().First());
            Singleton.ProvideScreen(gameObject.Children().OfComponent<SpriteRenderer>().First());
            Singleton.ProvideCSLibretro(gameObject.Children().OfComponent<CSLibretroController>().First());
            Singleton.ProvideUI(gameObject.Children().OfComponent<UIController>().First());
        }

        #endregion
    }
}