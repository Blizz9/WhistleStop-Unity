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
        private List<UIReportingItem> _reportingItems;

        private Text _textFieldTemplate;

        private Vector3 _textFieldPosition;

        #region MonoBehaviour

        public void Awake()
        {
            _reportingItems = new List<UIReportingItem>();

            _textFieldTemplate = gameObject.Descendants().OfComponent<Text>().First();
            _textFieldPosition = _textFieldTemplate.rectTransform.position;
        }

        #endregion

        #region Reporting Items

        public void AddReportingItem(UIReportingItem reportingItem)
        {
            if (!_reportingItems.Contains(reportingItem))
                _reportingItems.Add(reportingItem);

            reportingItem.TextField = Instantiate(_textFieldTemplate, _textFieldTemplate.transform.parent);
            reportingItem.TextField.gameObject.SetActive(true);
            reportingItem.TextField.rectTransform.position = _textFieldPosition;
            _textFieldPosition = new Vector3(_textFieldPosition.x, _textFieldPosition.y - 38, _textFieldPosition.z);
        }

        public void RemoveReportingItem(UIReportingItem reportingItem)
        {
            _reportingItems.Remove(reportingItem);

            foreach (Text textField in gameObject.Descendants().OfComponent<Text>().Where(t => t.isActiveAndEnabled))
                GameObject.Destroy(textField.gameObject);

            _textFieldPosition = _textFieldTemplate.rectTransform.position;

            foreach (UIReportingItem currentReportingItem in _reportingItems)
                AddReportingItem(currentReportingItem);
        }

        public void IncrementReportingItem(UIReportingItem reportingItem)
        {
            reportingItem.Value++;
            reportingItem.TextField.text = string.Format("{0}: {1}", reportingItem.Name, reportingItem.Value);
        }

        public void SetReportingItemValue(UIReportingItem reportingItem, long value)
        {
            reportingItem.Value = value;
            reportingItem.TextField.text = string.Format("{0}: {1}", reportingItem.Name, reportingItem.Value);
        }

        #endregion
    }

    public class UIReportingItem
    {
        public string Name;
        public long Value;
        public Text TextField;
    }
}
