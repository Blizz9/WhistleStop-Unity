using UnityEngine;
using UnityEngine.UI;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/Reporting Item Controller")]
    public class ReportingItemController : MonoBehaviour
    {
        private string _name;
        private long _value;
        private int? _address;

        private Text _text;

        #region Properties

        public string Name
        {
            get { return (_name); }
            set
            {
                _name = value;
                setText();
            }
        }

        public long Value
        {
            get { return (_value); }
            set
            {
                if (_address.HasValue)
                {
                    if (value != _value)
                    {
                        _value = value;
                        _text.color = Color.red;
                        setText();
                    }
                }
                else
                {
                    _value = value;
                    setText();
                }
            }
        }

        public int? Address
        {
            get { return (_address); }
            set
            {
                _address = value;
                setText();
            }
        }

        #endregion

        #region MonoBehaviour

        public void Awake()
        {
            _text = GetComponent<Text>();
        }

        public void Update()
        {
            if (_text.color.g < 1f)
                _text.color = new Color(_text.color.r, Mathf.Clamp01(_text.color.g + (Time.deltaTime * 0.35f)), Mathf.Clamp01(_text.color.b + (Time.deltaTime * 0.35f)), _text.color.a);
        }

        #endregion

        #region Text

        private void setText()
        {
            if (_address.HasValue)
                _text.text = string.Format("{0:X4} {1}: {2}", _address, _name, _value);
            else
                _text.text = string.Format("{0}: {1}", _name, _value);
        }

        #endregion
    }
}
