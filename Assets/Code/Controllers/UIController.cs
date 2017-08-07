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
        private const int REPORTING_ITEM_Y_STRIDE = 26;

        private List<UIReportingItem> _reportingItems;
        private Text _reportingTextTemplate;

        public GameObject TourStopPrefab;
        [HideInInspector] public GameObject TourStopParent;

        #region MonoBehaviour

        public void Awake()
        {
            _reportingItems = new List<UIReportingItem>();

            _reportingTextTemplate = gameObject.Descendants().Where(d => d.name == "ReportingTextTemplate").First().GetComponent<Text>();

            TourStopParent = gameObject.Descendants().Where(d => d.name == "TourStopContent").First();
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
    }

    public class UIReportingItem
    {
        public string Name;
        public long Value;
        public Text Text;
    }
}
