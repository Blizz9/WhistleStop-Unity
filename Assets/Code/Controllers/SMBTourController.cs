using System.Collections.Generic;
using System.Linq;
using com.PixelismGames.WhistleStop.Utilities;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    // TODO : Do a lot of the tour stop figuring without going in a state line, calculate each frame
    [AddComponentMenu("Pixelism Games/Controllers/Tours/SMB Tour Controller")]
    public class SMBTourController : TourController
    {
        private enum State
        {
            Startup = 0,
            TitleScreen,
            LoadingLevel,
            InLevel
        }

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

        private const short MOVING_DIRECTION = 0x0045;
        private const short CURRENT_SCREEN = 0x071A;
        private const short INPUTS = 0x074A;
        private const short PLAYER_STATE = 0x0754; // 0 = big, 1 = small
        private const short POWERUP_STATE = 0x0756; // 0 = small, 1 = big, >=2 = fiery
        private const short LIVES = 0x075A;
        private const short WORLD = 0x075F;
        private const short LEVEL = 0x0760;
        private const short LEVEL_LOADING_1 = 0x0770; // 1 = start
        private const short LEVEL_LOADING_STATE = 0x0772; // 0 = restart, 1 = starting, 3 = in-level
        private const short PLAYER_COUNT = 0x077A;
        private const short LEVEL_LOADING_TIMER = 0x07A0;

        private List<TourStopManifest> _tourStopManifests;

        private State _state;
        private int _tourStopIndex;

        #region MonoBehaviour

        public override void Awake()
        {
            base.Awake();

            _tourStopManifests = new List<TourStopManifest>();
            _tourStopManifests.Add(new TourStopManifest(0, 1, 0, 1, 5));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 0, 1, 0));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 2, 2, 6));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 2, 2, 0));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 3, 3, 4));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 3, 3, 0));
            _tourStopManifests.Add(new TourStopManifest(0, 1, 4, 4, 0));

            _tourStops = new List<TourStopController>();
            foreach (TourStopManifest tourStopManifest in _tourStopManifests)
            {
                TourStopController tourStop = Instantiate(Singleton.UI.TourStopPrefab, Singleton.UI.TourStopParent.transform).GetComponent<TourStopController>();
                tourStop.Tour = this;
                if ((tourStopManifest.LevelDisplay == 4) || (tourStopManifest.CheckpointScreen != 0))
                {
                    tourStop.FilePath = string.Format(@".\Contrib\SMBTour\SMB {0}-{1}.ss", tourStopManifest.WorldDisplay, tourStopManifest.LevelDisplay);
                    tourStop.Description = string.Format("World {0}-{1}", tourStopManifest.WorldDisplay, tourStopManifest.LevelDisplay);
                }
                else
                {
                    tourStop.FilePath = string.Format(@".\Contrib\SMBTour\SMB {0}-{1} (checkpoint).ss", tourStopManifest.WorldDisplay, tourStopManifest.LevelDisplay);
                    tourStop.Description = string.Format("World {0}-{1} (checkpoint)", tourStopManifest.WorldDisplay, tourStopManifest.LevelDisplay);
                }
                _tourStops.Add(tourStop);

                tourStopManifest.TourStop = tourStop;
            }

            _tourStops[_tourStopIndex].Selected = true;
        }

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
                addReportingItem(0x06D6, "Warpzone Control?");
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
                addReportingItem(LEVEL_LOADING_1, "Level Loading 1");
                addReportingItem(LEVEL_LOADING_STATE, "Level Loading State");
                addReportingItem(0x0773, "Level Palette");
                addReportingItem(PLAYER_COUNT, "Player Count");
                addReportingItem(LEVEL_LOADING_TIMER, "Level Loading Timer");
            }

            #endregion
        }

        #endregion

        #region Callbacks

        protected override void afterRunFrame()
        {
            base.afterRunFrame();

            if ((_lastFrameRAM[PLAYER_COUNT] == 0) && (_ram[PLAYER_COUNT] == 1))
                Singleton.UI.SetStatus("Sorry, this tour does not support 2 players");

            if ((_state == State.Startup) && (_ram[LEVEL_LOADING_STATE] == 3))
            {
                _state = State.TitleScreen;
                Debug.Log("State: Title Screen");
            }

            if ((_state == State.TitleScreen) && ((_ram[INPUTS] & 0x10) == 0x10))
            {
                Debug.Log("Game Started, Loading Tour Stop: " + _tourStops[_tourStopIndex].Description);

                _state = State.LoadingLevel;
                Singleton.CSLibretro.LoadState(_tourStops[_tourStopIndex].FilePath);
            }

            if ((_state == State.LoadingLevel) && (_ram[LEVEL_LOADING_STATE] == 3))
            {
                _state = State.InLevel;
                Debug.Log("State: In Level");
            }

            if ((_state == State.InLevel) && ((_lastFrameRAM[CURRENT_SCREEN] + 1) == _ram[CURRENT_SCREEN]) && (_ram[CURRENT_SCREEN] == _tourStopManifests[_tourStopIndex].CheckpointScreen))
            {
                _tourStopIndex++;

                foreach (TourStopController tourStop in _tourStops)
                    tourStop.Selected = false;
                _tourStops[_tourStopIndex].Selected = true;

                Debug.Log("Reached Checkpoint: Tour Stop Changed to " + _tourStops[_tourStopIndex].Description);
            }

            if ((_state == State.InLevel) && ((_lastFrameRAM[LIVES] - 1) == _ram[LIVES]))
            {
                Debug.Log("Player Died, Loading State: " + _tourStops[_tourStopIndex].Description);

                _state = State.LoadingLevel;
                Singleton.CSLibretro.LoadState(_tourStops[_tourStopIndex].FilePath);
            }

            if ((_state == State.InLevel) && (_ram[LEVEL_LOADING_STATE] == 1))
            {
                _state = State.LoadingLevel;
                Debug.Log("State: Loading Level");

                if ((_ram[WORLD] != _tourStopManifests[_tourStopIndex].World) || (_ram[LEVEL] != _tourStopManifests[_tourStopIndex].Level))
                {
                    TourStopManifest tourStopManifest = _tourStopManifests.Where(tsm => ((tsm.World == _ram[WORLD]) && (tsm.Level == _ram[LEVEL]))).FirstOrDefault();

                    if (tourStopManifest == null)
                    {
                        Debug.Log("Level Changed: No Tour Stop Found");
                    }
                    else
                    {
                        _tourStopIndex = _tourStopManifests.IndexOf(tourStopManifest);

                        foreach (TourStopController tourStop in _tourStops)
                            tourStop.Selected = false;
                        _tourStops[_tourStopIndex].Selected = true;

                        Debug.Log("Level Changed: Tour Stop Changed to " + _tourStops[_tourStopIndex].Description);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                Singleton.CSLibretro.SaveState(@".\Contrib\Saves\temp.ss");

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Singleton.CSLibretro.LoadState(@".\Contrib\Saves\temp.ss");
        }

        #endregion

        #region Save State Creation
        /*
        private bool _preRoundStarted;
        private bool _roundStarted;
        private bool _deathOccurred;
        private bool _waitingOnLoadingLevelTimer;

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
        */
        #endregion
    }
}
