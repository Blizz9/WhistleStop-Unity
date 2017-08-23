using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.PixelismGames.WhistleStop.Utilities;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/Tours/SMB Tour Controller")]
    public class SMBTourController : TourController
    {
        private const string TOUR_DIRECTORY = "./Contrib/SMBTour/";

        private class TourStopManifest
        {
            public int World;
            public int WorldDisplay;
            public int Level;
            public int LevelDisplay;
            public int CheckpointScreen;
            public TourStopController TourStop;

            public TourStopManifest(int world, int worldDisplay, int level, int levelDisplay, int checkpointScreen)
            {
                World = world;
                WorldDisplay = worldDisplay;
                Level = level;
                LevelDisplay = levelDisplay;
                CheckpointScreen = checkpointScreen;
            }
        }

        private const byte RAM_OFFSET = 0x5D;
        private const short MOVING_DIRECTION = 0x0045;
        private const short CURRENT_SCREEN = 0x071A;
        private const short INPUTS = 0x074A;
        private const short PLAYER_STATE = 0x0754; // 0 = big, 1 = small
        private const short POWERUP_STATE = 0x0756; // 0 = small, 1 = big, >=2 = fiery
        private const short LIVES = 0x075A;
        private const short COINS = 0x075E;
        private const short WORLD = 0x075F;
        private const short LEVEL = 0x0760;
        private const short GAME_LOADING_STATE = 0x0770; // 0 = not started, 1 = started
        private const short LEVEL_LOADING_STATE = 0x0772; // 0 = restart, 1 = starting, 3 = in-level
        private const short PLAYER_COUNT = 0x077A;
        private const short LEVEL_LOADING_TIMER = 0x07A0;
        private const short SCORE_MILLIONS = 0x07DD;
        private const short SCORE_HUNDRED_THOUSANDS = 0x07DE;
        private const short SCORE_TEN_THOUSANDS = 0x07DF;
        private const short SCORE_THOUSANDS = 0x07E0;
        private const short SCORE_HUNDREDS = 0x07E1;
        private const short SCORE_TENS = 0x07E2;
        private const short COINS_DISPLAY_TENS = 0x07ED;
        private const short COINS_DISPLAY_ONES = 0x07EE;

        private List<TourStopManifest> _tourStopManifests;
        private int _tourStopIndex;

        private bool _skipFrame;

        #region MonoBehaviour

        public override void Start()
        {
            base.Start();

            #region Reporting Items

            if (ShowReporting)
            {
                addReportingItem(0x0009, "Frames");
                addReportingItem(0x000E, "Player State");
                addReportingItem(0x0033, "Facing");
                addReportingItem(MOVING_DIRECTION, "Moving Direction");
                addReportingItem(0x0057, "Horizontal Speed");
                addReportingItem(0x006D, "Horizontal Position");
                addReportingItem(0x0086, "X on Screen");
                addReportingItem(0x00CE, "Y on Screen");
                addReportingItem(0x00E7, "Level Layout Address");
                addReportingItem(CURRENT_SCREEN, "Current Screen");
                addReportingItem(0x071B, "Next Screen");
                addReportingItem(0x071C, "Screen Edge");
                addReportingItem(0x072C, "Level Layout Index?");
                addReportingItem(INPUTS, "Inputs");
                addReportingItem(0x0750, "Area Offset");
                addReportingItem(POWERUP_STATE, "Powerup State");
                addReportingItem(0x0757, "Pre-level Flag");
                addReportingItem(LIVES, "Lives");
                addReportingItem(LEVEL, "Level");
                addReportingItem(GAME_LOADING_STATE, "Game Loading State");
                addReportingItem(LEVEL_LOADING_STATE, "Level Loading State");
                addReportingItem(0x0773, "Level Palette");
                addReportingItem(PLAYER_COUNT, "Player Count");
                addReportingItem(LEVEL_LOADING_TIMER, "Level Loading Timer");
            }

            #endregion

            _tourStopManifests = new List<TourStopManifest>();
            _tourStopManifests.Add(new TourStopManifest(0, 1, 0, 1, 5));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 0, 1, 0));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 2, 2, 6));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 2, 2, 0));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 3, 3, 4));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 3, 3, 0));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 4, 4, 0));
            _tourStopManifests.Add(new TourStopManifest(1, 2, 0, 1, 6));
            _tourStopManifests.Add(new TourStopManifest(1, 2, 0, 1, 0));
            _tourStopManifests.Add(new TourStopManifest(1, 2, 2, 2, 5));
            _tourStopManifests.Add(new TourStopManifest(1, 2, 2, 2, 0));
            _tourStopManifests.Add(new TourStopManifest(1, 2, 3, 3, 7));
            _tourStopManifests.Add(new TourStopManifest(1, 2, 3, 3, 0));
            _tourStopManifests.Add(new TourStopManifest(1, 2, 4, 4, 0));
            _tourStopManifests.Add(new TourStopManifest(2, 3, 0, 1, 6));
            _tourStopManifests.Add(new TourStopManifest(2, 3, 0, 1, 0));
            _tourStopManifests.Add(new TourStopManifest(2, 3, 1, 2, 6));
            _tourStopManifests.Add(new TourStopManifest(2, 3, 1, 2, 0));
            _tourStopManifests.Add(new TourStopManifest(2, 3, 2, 3, 4));
            _tourStopManifests.Add(new TourStopManifest(2, 3, 2, 3, 0));
            _tourStopManifests.Add(new TourStopManifest(2, 3, 3, 4, 0));
            _tourStopManifests.Add(new TourStopManifest(3, 4, 0, 1, 6));
            _tourStopManifests.Add(new TourStopManifest(3, 4, 0, 1, 0));
            _tourStopManifests.Add(new TourStopManifest(3, 4, 2, 2, 6));
            _tourStopManifests.Add(new TourStopManifest(3, 4, 2, 2, 0));
            _tourStopManifests.Add(new TourStopManifest(3, 4, 3, 3, 4));
            _tourStopManifests.Add(new TourStopManifest(3, 4, 3, 3, 0));
            _tourStopManifests.Add(new TourStopManifest(3, 4, 4, 4, 0));

            _tourStops = new List<TourStopController>();
            foreach (TourStopManifest tourStopManifest in _tourStopManifests)
            {
                TourStopController tourStop = Singleton.UI.CreateTourStop();
                tourStop.Tour = this;

                string checkpointSuffix = string.Empty;
                if ((tourStopManifest.LevelDisplay != 4) && (tourStopManifest.CheckpointScreen == 0))
                    checkpointSuffix = " (checkpoint)";

                tourStop.FilePath = string.Format("{0}{1}-{2}{3}.{4}", TOUR_DIRECTORY, tourStopManifest.WorldDisplay, tourStopManifest.LevelDisplay, checkpointSuffix, STATE_EXTENSION);
                tourStop.LoadScreenshot(string.Format("{0}{1}-{2}{3}.{4}", TOUR_DIRECTORY, tourStopManifest.WorldDisplay, tourStopManifest.LevelDisplay, checkpointSuffix, SCREENSHOT_EXTENSION));
                tourStop.Description = string.Format("World {0}-{1}{2}", tourStopManifest.WorldDisplay, tourStopManifest.LevelDisplay, checkpointSuffix);

                _tourStops.Add(tourStop);

                tourStopManifest.TourStop = tourStop;
            }

            _tourStops[_tourStopIndex].Selected = true;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Singleton.CSLibretro.SaveState(STATES_DIRECTORY + "temp" + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Singleton.CSLibretro.LoadState(STATES_DIRECTORY + "temp" + STATE_EXTENSION);

            //if (Input.GetKeyDown(KeyCode.Alpha3) && !string.IsNullOrEmpty(_stateName))
            //{
            //    string screenshotFilename = string.Format("{0}{1}.{2}", STATES_DIRECTORY, _stateName, SCREENSHOT_EXTENSION);
            //    Singleton.CSLibretro.SaveScreenshot(screenshotFilename);

            //    Debug.Log("Screenshot taken: " + screenshotFilename);
            //}

            if (Input.GetKeyDown(KeyCode.Alpha4))
                Singleton.CSLibretro.LoadState(TOUR_DIRECTORY + "3-4." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha5))
                Singleton.CSLibretro.LoadState(TOUR_DIRECTORY + "4-1." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha6))
                Singleton.CSLibretro.LoadState(TOUR_DIRECTORY + "4-2." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha7))
                Singleton.CSLibretro.LoadState(TOUR_DIRECTORY + "4-3." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha8))
                Singleton.CSLibretro.LoadState(TOUR_DIRECTORY + "4-4." + STATE_EXTENSION);
        }

        #endregion

        #region Callbacks
        
        protected override void afterRunFrame()
        {
            base.afterRunFrame();

            if (_skipFrame)
            {
                _skipFrame = false;
                return;
            }

            if ((_lastFrameRAM[PLAYER_COUNT] == 0) && (_ram[PLAYER_COUNT] == 1))
                Singleton.UI.SetStatus("Sorry, this tour does not support 2 players");

            if ((_lastFrameRAM[GAME_LOADING_STATE] == 0) && (_ram[GAME_LOADING_STATE] == 1))
            {
                Debug.Log("Game Started, Loading Tour Stop: Start");
                Singleton.CSLibretro.LoadState(string.Format("{0}Start.{1}", TOUR_DIRECTORY, STATE_EXTENSION));
            }

            if ((_ram[LEVEL_LOADING_STATE] == 3) && ((_lastFrameRAM[CURRENT_SCREEN] + 1) == _ram[CURRENT_SCREEN]) && (_ram[CURRENT_SCREEN] == _tourStopManifests[_tourStopIndex].CheckpointScreen))
            {
                _tourStopIndex++;

                foreach (TourStopController tourStop in _tourStops)
                    tourStop.Selected = false;
                _tourStops[_tourStopIndex].Selected = true;

                Debug.Log("Reached Checkpoint: Tour Stop Changed to " + _tourStops[_tourStopIndex].Description);
            }

            if ((_ram[LEVEL_LOADING_STATE] == 0) && ((_lastFrameRAM[LIVES] - 1) == _ram[LIVES]))
            {
                Debug.Log("Player Died, Loading State: " + _tourStops[_tourStopIndex].Description);
                loadTourStopWithInjection(_tourStops[_tourStopIndex]);
            }

            if (_ram[LEVEL_LOADING_STATE] == 1)
            {
                if ((_ram[WORLD] != _tourStopManifests[_tourStopIndex].World) || (_ram[LEVEL] != _tourStopManifests[_tourStopIndex].Level))
                {
                    TourStopManifest tourStopManifest = _tourStopManifests.Where(tsm => ((tsm.World == _ram[WORLD]) && (tsm.Level == _ram[LEVEL]))).FirstOrDefault();

                    if (tourStopManifest != null)
                    {
                        _tourStopIndex = _tourStopManifests.IndexOf(tourStopManifest);

                        foreach (TourStopController tourStop in _tourStops)
                            tourStop.Selected = false;
                        _tourStops[_tourStopIndex].Selected = true;

                        Debug.Log("Level Changed: Tour Stop Changed to " + _tourStops[_tourStopIndex].Description);
                    }
                }
            }
        }
        
        #endregion

        #region Events

        public override void TourStopSelected(TourStopController tourStop)
        {
            base.TourStopSelected(tourStop);

            _tourStopIndex = _tourStops.IndexOf(tourStop);

            loadTourStopWithInjection(tourStop);

            _skipFrame = true;
        }

        #endregion

        #region Loading

        private void loadTourStopWithInjection(TourStopController tourStop)
        {
            byte[] state = File.ReadAllBytes(tourStop.FilePath);

            if (_ram[LIVES] > 4)
                state[LIVES + RAM_OFFSET] = _ram[LIVES];

            if (_ram[COINS] > 90)
            {
                state[COINS + RAM_OFFSET] = _ram[COINS];
                state[COINS_DISPLAY_TENS + RAM_OFFSET] = _ram[COINS_DISPLAY_TENS];
                state[COINS_DISPLAY_ONES + RAM_OFFSET] = _ram[COINS_DISPLAY_ONES];
            }

            state[SCORE_MILLIONS + RAM_OFFSET] = _ram[SCORE_MILLIONS];
            state[SCORE_HUNDRED_THOUSANDS + RAM_OFFSET] = _ram[SCORE_HUNDRED_THOUSANDS];
            state[SCORE_TEN_THOUSANDS + RAM_OFFSET] = _ram[SCORE_TEN_THOUSANDS];
            state[SCORE_THOUSANDS + RAM_OFFSET] = _ram[SCORE_THOUSANDS];
            state[SCORE_HUNDREDS + RAM_OFFSET] = _ram[SCORE_HUNDREDS];
            state[SCORE_TENS + RAM_OFFSET] = _ram[SCORE_TENS];

            Singleton.CSLibretro.LoadState(state);
        }

        #endregion

        #region State Creation
        /*
        private bool _deathOccurred;
        private int? _screenshotTimer;
        private string _stateName;

        protected override void afterRunFrame()
        {
            base.afterRunFrame();

            if ((_ram[LEVEL_LOADING_STATE] == 0) && ((_lastFrameRAM[LIVES] - 1) == _ram[LIVES]))
                _deathOccurred = true;

            if (_deathOccurred && (_ram[LEVEL_LOADING_STATE] == 1))
            {
                _deathOccurred = false;
                _screenshotTimer = 0;

                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, PLAYER_STATE);
                Singleton.CSLibretro.WriteRAM(new byte[] { 2 }, POWERUP_STATE);
                Singleton.CSLibretro.WriteRAM(new byte[] { 4 }, LIVES);
                Singleton.CSLibretro.WriteRAM(new byte[] { 90 }, COINS);
                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, SCORE_MILLIONS);
                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, SCORE_HUNDRED_THOUSANDS);
                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, SCORE_TEN_THOUSANDS);
                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, SCORE_THOUSANDS);
                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, SCORE_HUNDREDS);
                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, SCORE_TENS);
                Singleton.CSLibretro.WriteRAM(new byte[] { 9 }, COINS_DISPLAY_TENS);
                Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, COINS_DISPLAY_ONES);

                TourStopManifest tourStopManifest = _tourStopManifests.Where(tsm => ((tsm.World == _ram[WORLD]) && (tsm.Level == _ram[LEVEL]))).FirstOrDefault();
                bool isPastCheckpoint = false;
                if (tourStopManifest.CheckpointScreen > 0)
                    isPastCheckpoint = _ram[CURRENT_SCREEN] >= tourStopManifest.CheckpointScreen ? true : false;

                _stateName = string.Format(@"{0}-{1}{2}", tourStopManifest.WorldDisplay, tourStopManifest.LevelDisplay, isPastCheckpoint ? " (checkpoint)" : "");
                string stateFilename = string.Format("{0}{1}.{2}", STATES_DIRECTORY, _stateName, STATE_EXTENSION);
                Singleton.CSLibretro.SaveState(stateFilename);

                Debug.Log("Saved state: " + stateFilename);
            }

            if (_screenshotTimer.HasValue && (_ram[LEVEL_LOADING_STATE] == 3))
            {
                _screenshotTimer++;

                if (_screenshotTimer == 4)
                {
                    _screenshotTimer = null;

                    string screenshotFilename = string.Format("{0}{1}.{2}", STATES_DIRECTORY, _stateName, SCREENSHOT_EXTENSION);
                    Singleton.CSLibretro.SaveScreenshot(screenshotFilename);

                    Debug.Log("Screenshot taken: " + screenshotFilename);
                }
            }
        }
        */
        #endregion
    }
}
