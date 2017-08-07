using System.Linq;
using Unity.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/Tour Stop Controller")]
    public class TourStopController : MonoBehaviour
    {
        private Image _border;
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
            set { _border.enabled = value; }
        }

        #endregion

        #region MonoBehaviour

        public void Awake()
        {
            _border = gameObject.Descendants().Where(d => d.name == "Border").First().GetComponent<Image>();
            _border.enabled = false;

            _description = gameObject.Descendants().Where(d => d.name == "Description").First().GetComponent<Text>();
        }

        #endregion

        #region Events

        public void Clicked()
        {
            Tour.TourStopSelected(this);
        }

        #endregion
    }
}
