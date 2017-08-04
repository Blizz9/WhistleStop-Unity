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

        private bool _preRoundStarted;
        private bool _roundStarted;
        private bool _deathOccurred;
        private bool _waitingOnLoadingLevelTimer;

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
            //if (Input.GetKeyDown(KeyCode.U))
            //{
            //    byte[] saveState = System.IO.File.ReadAllBytes("smb.ss.base");

            //    saveState[93 + 0x071A] = 5;
            //    saveState[93 + 0x071B] = 6;
            //    saveState[93 + 0x072C] = 42;
            //    saveState[93 + 0x0750] = 0xA5;
            //    saveState[93 + 0x075A] = 8;
            //    //saveState[93 + 0x0756] = 2;

            //    System.IO.File.WriteAllBytes("game.ss", saveState);
            //    Singleton.CSLibretro.LoadState("game.ss");
            //}

            //if (Input.GetKeyDown(KeyCode.I))
            //{
            //    byte[] saveState = System.IO.File.ReadAllBytes("game.ss");

            //    saveState[93 + 0x0754] = 0;
            //    saveState[93 + 0x0756] = 2;
            //    saveState[93 + 0x075A] = 8;

            //    System.IO.File.WriteAllBytes("game.ss", saveState);
            //    Singleton.CSLibretro.LoadState("game.ss");
            //}

            //if (Input.GetKeyDown(KeyCode.U))
            //{
            //    Singleton.CSLibretro.LoadState("game.ss");
            //}

            if (Input.GetKeyDown(KeyCode.Y))
            {
                Singleton.CSLibretro.LoadState("game.ss");
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                _deathOccurred = false;
                _waitingOnLoadingLevelTimer = false;

                Debug.Log("Reset death watch");
            }
        }

        #endregion

        #region Callbacks

        private void _afterRunFrame()
        {
            byte[] ram = Singleton.CSLibretro.ReadRAM();

            foreach (MemoryAnalysisReportingItem analysisReportingItem in _analysisReportingItems)
                analysisReportingItem.TextField.text = string.Format("{0:X4} {1}: {2}", analysisReportingItem.Address, analysisReportingItem.Name, ram[analysisReportingItem.Address]);

            if (!_preRoundStarted && ram[0x0770] == 1)
            {
                Debug.Log("Pre-round started");
                _preRoundStarted = true;
            }

            if (_preRoundStarted && !_roundStarted && ram[0x0772] == 3)
            {
                Debug.Log("Round started");
                _roundStarted = true;
            }

            if (_roundStarted && !_deathOccurred && ram[0x0772] == 0)
            {
                Debug.Log("Death occurred");
                _deathOccurred = true;
            }

            if (_deathOccurred && !_waitingOnLoadingLevelTimer && ram[0x0772] == 1)
            {
                Debug.Log("Set restart values, waiting on proper loading level timer");
                Singleton.CSLibretro.WriteRAM(new byte[] { 4 }, 0x075A);
                _waitingOnLoadingLevelTimer = true;
            }

            if (_waitingOnLoadingLevelTimer && ram[0x07A0] == 3)
            {
                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, 0x0754);
                Singleton.CSLibretro.WriteRAM(new byte[] { 2 }, 0x0756);

                bool isPastCheckpoint = ram[0x071A] > 2 ? true : false;
                int world = ram[0x075F] + 1;
                int level = ram[0x0760] + 1;

                string saveStateFilename = string.Format("SMB {0}-{1}{2}.ss", world, level, isPastCheckpoint ? " (checkpoint)" : "");

                Singleton.CSLibretro.SaveState(saveStateFilename);

                Debug.Log("State saved: " + saveStateFilename + ", waiting on next death");

                _roundStarted = false;
                _deathOccurred = false;
                _waitingOnLoadingLevelTimer = false;
            }
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
