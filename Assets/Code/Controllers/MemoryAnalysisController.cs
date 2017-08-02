using System.Collections.Generic;
using System.Linq;
using com.PixelismGames.WhistleStop.Utilities;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/Memory Analysis Controller")]
    public class MemoryAnalysisController : MonoBehaviour
    {
        private List<MemoryAnalysisReportingItem> _analysisReportingItems;

        #region MonoBehaviour

        public void Awake()
        {
            _analysisReportingItems = new List<MemoryAnalysisReportingItem>();
        }

        public void Start()
        {
            Singleton.CSLibretro.AfterRunFrame += _afterRunFrame;

            AddAnalysisReportingItem(0x0009, "Frames");
            AddAnalysisReportingItem(0x000A, "AB Buttons");
            AddAnalysisReportingItem(0x000B, "UpDown Buttons");
            AddAnalysisReportingItem(0x000E, "Player State");
            AddAnalysisReportingItem(0x0033, "Facing");
            AddAnalysisReportingItem(0x0045, "Moving Direction");
            AddAnalysisReportingItem(0x0057, "Horizontal Speed");
            AddAnalysisReportingItem(0x006D, "Horizontal Position");
            AddAnalysisReportingItem(0x0086, "X on Screen");
            AddAnalysisReportingItem(0x00CE, "Y on Screen");
            AddAnalysisReportingItem(0x06D6, "Warpzone Control?");
            AddAnalysisReportingItem(0x071A, "Current Screen");
            AddAnalysisReportingItem(0x071B, "Next Screen");
            AddAnalysisReportingItem(0x071C, "Screen Edge");
            AddAnalysisReportingItem(0x072C, "Level Layout Index?");
            AddAnalysisReportingItem(0x074A, "Input");
            AddAnalysisReportingItem(0x0750, "Area Offset");
            AddAnalysisReportingItem(0x0756, "Powerup State");
            AddAnalysisReportingItem(0x0757, "Pre-level Flag");
            AddAnalysisReportingItem(0x075A, "Lives");
            AddAnalysisReportingItem(0x0760, "Level");
            AddAnalysisReportingItem(0x0770, "Level Loading 1");
            AddAnalysisReportingItem(0x0772, "Level Loading 2");
            AddAnalysisReportingItem(0x0773, "Level Palette");
            AddAnalysisReportingItem(0x07A0, "Level Loading Timer");
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                byte[] saveState = System.IO.File.ReadAllBytes("game.ss.base");

                saveState[93 + 0x071A] = 5;
                saveState[93 + 0x071B] = 5;

                System.IO.File.WriteAllBytes("game.ss", saveState);
                Singleton.CSLibretro.LoadState("game.ss");
            }
        }

        #endregion

            #region Callbacks

        private void _afterRunFrame()
        {
            byte[] ram = Singleton.CSLibretro.GetRAM();

            foreach (MemoryAnalysisReportingItem analysisReportingItem in _analysisReportingItems)
                analysisReportingItem.TextField.text = string.Format("{0:X4} {1}: {2}", analysisReportingItem.Address, analysisReportingItem.Name, ram[analysisReportingItem.Address]);

            //if ((ram[0x0757] == 1) && (ram[0x07A0] == 7))
            //{
            //    if (!System.IO.File.Exists("game.ss"))
            //        Singleton.CSLibretro.SaveState("game.ss");
            //}
        }

        #endregion

        #region Accessible Routines

        public void AddAnalysisReportingItem(int address, string name)
        {
            MemoryAnalysisReportingItem analysisReportingItem = new MemoryAnalysisReportingItem();
            analysisReportingItem.Address = address;
            analysisReportingItem.Name = name;
            Singleton.UI.AddReportingItem(analysisReportingItem);

            _analysisReportingItems.Add(analysisReportingItem);
        }

        #endregion
    }

    public class MemoryAnalysisReportingItem : UIReportingItem
    {
        public int Address;
    }
}
