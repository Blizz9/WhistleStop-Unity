using System.Collections.Generic;
using com.PixelismGames.WhistleStop.Utilities;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("")]
    public class TourController : MonoBehaviour
    {
        protected List<TourReportingItem> _reportingItems;
        protected byte[] _ram;

        public bool ShowReporting;

        #region MonoBehaviour

        public void Awake()
        {
            _reportingItems = new List<TourReportingItem>();
        }

        public virtual void Start()
        {
            Singleton.CSLibretro.BeforeRunFrame += beforeRunFrame;
            Singleton.CSLibretro.AfterRunFrame += afterRunFrame;
        }

        #endregion

        #region Callbacks

        protected virtual void beforeRunFrame()
        {
        }

        protected virtual void afterRunFrame()
        {
            _ram = Singleton.CSLibretro.ReadRAM();

            foreach (TourReportingItem reportingItem in _reportingItems)
                reportingItem.TextField.text = string.Format("{0:X4} {1}: {2}", reportingItem.Address, reportingItem.Name, _ram[reportingItem.Address]);
        }

        #endregion

        #region Reporting

        protected void addReportingItem(int address, string name)
        {
            TourReportingItem reportingItem = new TourReportingItem();
            reportingItem.Address = address;
            reportingItem.Name = name;
            Singleton.UI.AddReportingItem(reportingItem);

            _reportingItems.Add(reportingItem);
        }

        #endregion
    }

    public class TourReportingItem : UIReportingItem
    {
        public int Address;
    }
}
