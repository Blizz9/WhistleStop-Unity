using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/UI Controller")]
    public class UIController : MonoBehaviour
    {
        public GameObject TourStopPrefab;
        public GameObject ReportingItemPrefab;

        private GameObject _tourStopParent;
        private GameObject _reportingItemParent;

        private Text _status;

        #region MonoBehaviour

        public void Awake()
        {
            _tourStopParent = gameObject.Descendants().Where(d => d.name == "TourStopContent").First();
            _reportingItemParent = gameObject.Descendants().Where(d => d.name == "ReportingContent").First();

            _status = gameObject.Descendants().Where(d => d.name == "Status").First().GetComponent<Text>();
            _status.color = new Color(_status.color.r, _status.color.g, _status.color.b, 0f);
        }

        public void Update()
        {
            if (_status.color.a > 0f)
                _status.color = new Color(_status.color.r, _status.color.g, _status.color.b, Mathf.Clamp01(_status.color.a - (Time.deltaTime * 0.35f)));
        }

        #endregion

        #region Tour Stops

        public TourStopController CreateTourStop()
        {
            return (Instantiate(TourStopPrefab, _tourStopParent.transform).GetComponent<TourStopController>());
        }

        #endregion

        #region Reporting Items

        public ReportingItemController CreateReportingItem(string name)
        {
            ReportingItemController reportingItem = Instantiate(ReportingItemPrefab, _reportingItemParent.transform).GetComponent<ReportingItemController>();
            reportingItem.Name = name;
            return (reportingItem);
        }

        #endregion

        #region Status

        public void SetStatus(string statusMessage)
        {
            _status.text = statusMessage;
            _status.color = new Color(_status.color.r, _status.color.g, _status.color.b, 1f);
        }

        #endregion
    }
}
