using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace com.PixelismGames.WhistleStop.Controllers
{
    // TODO : Redesign reporting items to be selfsufficient like tour stops
    [AddComponentMenu("Pixelism Games/Controllers/UI Controller")]
    public class UIController : MonoBehaviour
    {
        private const int REPORTING_ITEM_Y_STRIDE = 26;

        private List<UIReportingItem> _reportingItems;
        private Text _reportingTextTemplate;

        public GameObject TourStopPrefab;
        [HideInInspector] public GameObject TourStopParent;

        private Text _status;

        #region MonoBehaviour

        public void Awake()
        {
            _reportingItems = new List<UIReportingItem>();

            _reportingTextTemplate = gameObject.Descendants().Where(d => d.name == "ReportingTextTemplate").First().GetComponent<Text>();

            TourStopParent = gameObject.Descendants().Where(d => d.name == "TourStopContent").First();

            _status = gameObject.Descendants().Where(d => d.name == "Status").First().GetComponent<Text>();
            _status.color = new Color(_status.color.r, _status.color.g, _status.color.b, 0f);
        }

        public void Update()
        {
            if (_status.color.a > 0f)
                _status.color = new Color(_status.color.r, _status.color.g, _status.color.b, Mathf.Clamp01(_status.color.a - (Time.deltaTime * 0.35f)));
        }

        #endregion

            #region Reporting Items

        public void AddReportingItem(UIReportingItem reportingItem)
        {
            if (!_reportingItems.Contains(reportingItem))
                _reportingItems.Add(reportingItem);

            reportingItem.Text = Instantiate(_reportingTextTemplate, _reportingTextTemplate.transform.parent);
            reportingItem.Text.gameObject.SetActive(true);
        }

        public void RemoveReportingItem(UIReportingItem reportingItem)
        {
            _reportingItems.Remove(reportingItem);
            GameObject.Destroy(reportingItem.Text.gameObject);
        }

        public void IncrementReportingItem(UIReportingItem reportingItem)
        {
            reportingItem.Value++;
            reportingItem.Text.text = string.Format("{0}: {1}", reportingItem.Name, reportingItem.Value);
        }

        public void SetReportingItemValue(UIReportingItem reportingItem, long value)
        {
            reportingItem.Value = value;
            reportingItem.Text.text = string.Format("{0}: {1}", reportingItem.Name, reportingItem.Value);
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

    public class UIReportingItem
    {
        public string Name;
        public long Value;
        public Text Text;
    }
}
