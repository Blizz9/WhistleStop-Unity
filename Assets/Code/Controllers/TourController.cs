using System.Collections.Generic;
using com.PixelismGames.WhistleStop.Utilities;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    // TODO : Implement a visual progress system in the UI, like cookie trail to show when you pass each screen in SMB?
    // TODO : Implement a go-back position (with screenshot) when the user clicks a tour stop?
    [AddComponentMenu("")]
    public class TourController : MonoBehaviour
    {
        public const string STATES_DIRECTORY = "./Contrib/States/";
        public const string STATE_EXTENSION = "state";
        public const string SCREENSHOT_EXTENSION = "png";

        protected List<ReportingItemController> _reportingItems;
        protected byte[] _ram;
        protected byte[] _lastFrameRAM;

        protected List<TourStopController> _tourStops;

        public bool ShowReporting;

        #region MonoBehaviour

        public virtual void Awake()
        {
            _reportingItems = new List<ReportingItemController>();
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
            _lastFrameRAM = _ram == null ? Singleton.CSLibretro.ReadRAM() : _ram;
            _ram = Singleton.CSLibretro.ReadRAM();

            foreach (ReportingItemController reportingItem in _reportingItems)
                reportingItem.Value = _ram[reportingItem.Address.Value];
        }

        #endregion

        #region Reporting

        protected void addReportingItem(int address, string name)
        {
            ReportingItemController  reportingItem = Singleton.UI.CreateReportingItem(name);
            reportingItem.Address = address;

            _reportingItems.Add(reportingItem);
        }

        #endregion

        #region Events

        public virtual void TourStopSelected(TourStopController tourStop)
        {
            foreach (TourStopController otherTourStop in _tourStops)
                otherTourStop.Selected = otherTourStop == tourStop;
        }

        #endregion
    }
}
