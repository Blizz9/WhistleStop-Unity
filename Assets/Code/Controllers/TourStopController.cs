using System.Linq;
using Unity.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/Tour Stop Controller")]
    public class TourStopController : MonoBehaviour, IPointerClickHandler
    {
        private Image _border;
        private Image _screenshot;
        private Text _description;

        public TourController Tour;
        public string FilePath;

        #region Properties

        public string Description
        {
            get { return (_description.text); }
            set { _description.text = value; }
        }

        public bool Selected
        {
            get { return (_border.enabled); }
            set
            {
                _border.enabled = value;
                _screenshot.color = new Color(_screenshot.color.r, _screenshot.color.g, _screenshot.color.b, (value ? 1f : 0.5f));
            }
        }

        #endregion

        #region MonoBehaviour

        public void Awake()
        {
            _border = gameObject.Descendants().Where(d => d.name == "Border").First().GetComponent<Image>();
            _border.enabled = false;

            _screenshot = gameObject.Descendants().Where(d => d.name == "Screenshot").First().GetComponent<Image>();
            _screenshot.color = new Color(_screenshot.color.r, _screenshot.color.g, _screenshot.color.b, 0.5f);

            _description = gameObject.Descendants().Where(d => d.name == "Description").First().GetComponent<Text>();
        }

        #endregion

        #region Events

        public void OnPointerClick(PointerEventData eventData)
        {
            Tour.TourStopSelected(this);
        }

        #endregion
    }
}
