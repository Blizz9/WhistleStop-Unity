using com.PixelismGames.WhistleStop.Utilities;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/Tours/SMB Tour Controller")]
    public class SMBTourController : TourController
    {
        private const short CURRENT_SCREEN = 0x071A;
        private const short PLAYER_STATE = 0x0754; // 0 = big, 1 = small
        private const short POWERUP_STATE = 0x0756; // 0 = small, 1 = big, >=2 = fiery
        private const short LIVES = 0x075A;
        private const short WORLD = 0x075F;
        private const short LEVEL = 0x0760;
        private const short LEVEL_LOADING_1 = 0x0770; // 1 = start
        private const short LEVEL_LOADING_2 = 0x0772; // 0 = restart, 1 = starting, 3 = in-level
        private const short LEVEL_LOADING_TIMER = 0x07A0;

        private bool _preRoundStarted;
        private bool _roundStarted;
        private bool _deathOccurred;
        private bool _waitingOnLoadingLevelTimer;

        public bool CreatingSaveStates;

        #region MonoBehaviour

        public override void Start()
        {
            base.Start();

            if (ShowReporting)
            {
                addReportingItem(0x0009, "Frames");
                addReportingItem(0x000A, "AB Buttons");
                addReportingItem(0x000B, "UpDown Buttons");
                addReportingItem(0x000E, "Player State");
                addReportingItem(0x0033, "Facing");
                addReportingItem(0x0045, "Moving Direction");
                addReportingItem(0x0057, "Horizontal Speed");
                addReportingItem(0x006D, "Horizontal Position");
                addReportingItem(0x0086, "X on Screen");
                addReportingItem(0x00CE, "Y on Screen");
                addReportingItem(0x06D6, "Warpzone Control?");
                addReportingItem(CURRENT_SCREEN, "Current Screen");
                addReportingItem(0x071B, "Next Screen");
                addReportingItem(0x071C, "Screen Edge");
                addReportingItem(0x072C, "Level Layout Index?");
                addReportingItem(0x074A, "Input");
                addReportingItem(0x0750, "Area Offset");
                addReportingItem(POWERUP_STATE, "Powerup State");
                addReportingItem(0x0757, "Pre-level Flag");
                addReportingItem(LIVES, "Lives");
                addReportingItem(LEVEL, "Level");
                addReportingItem(LEVEL_LOADING_1, "Level Loading 1");
                addReportingItem(LEVEL_LOADING_2, "Level Loading 2");
                addReportingItem(0x0773, "Level Palette");
                addReportingItem(LEVEL_LOADING_TIMER, "Level Loading Timer");
            }
        }

        #endregion

        #region Callbacks

        protected override void beforeRunFrame()
        {
            base.beforeRunFrame();

            if (Input.GetKeyDown(KeyCode.Y))
            {
                Singleton.CSLibretro.LoadState("game.ss");
                Debug.Log("Loaded save state game.ss");
            }
        }

        protected override void afterRunFrame()
        {
            base.afterRunFrame();

            if (CreatingSaveStates)
            {
                if (!_preRoundStarted && _ram[LEVEL_LOADING_1] == 1)
                {
                    Debug.Log("Pre-round started");
                    _preRoundStarted = true;
                }

                if (_preRoundStarted && !_roundStarted && _ram[LEVEL_LOADING_2] == 3)
                {
                    Debug.Log("Round started");
                    _roundStarted = true;
                }

                if (_roundStarted && !_deathOccurred && _ram[LEVEL_LOADING_2] == 0)
                {
                    Debug.Log("Death occurred");
                    _deathOccurred = true;
                }

                if (_deathOccurred && !_waitingOnLoadingLevelTimer && _ram[LEVEL_LOADING_2] == 1)
                {
                    Debug.Log("Set restart values, waiting on proper loading level timer");
                    Singleton.CSLibretro.WriteRAM(new byte[] { 4 }, LIVES);
                    _waitingOnLoadingLevelTimer = true;
                }

                if (_waitingOnLoadingLevelTimer && _ram[LEVEL_LOADING_TIMER] == 3)
                {
                    Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, PLAYER_STATE);
                    Singleton.CSLibretro.WriteRAM(new byte[] { 2 }, POWERUP_STATE);

                    bool isPastCheckpoint = _ram[CURRENT_SCREEN] > 2 ? true : false;
                    int world = _ram[WORLD] + 1;
                    int level = _ram[LEVEL] + 1;

                    string saveStateFilename = string.Format("SMB {0}-{1}{2}.ss", world, level, isPastCheckpoint ? " (checkpoint)" : "");

                    Singleton.CSLibretro.SaveState(saveStateFilename);

                    Debug.Log("State saved: " + saveStateFilename + ", waiting on next death");

                    _roundStarted = false;
                    _deathOccurred = false;
                    _waitingOnLoadingLevelTimer = false;
                }

                if (Input.GetKeyDown(KeyCode.U))
                {
                    _deathOccurred = false;
                    _waitingOnLoadingLevelTimer = false;
                    Debug.Log("Reset death watch");
                }
            }
        }

        #endregion
    }
}
