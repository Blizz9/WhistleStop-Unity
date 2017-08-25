using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.PixelismGames.WhistleStop.Utilities;
using UnityEngine;

namespace com.PixelismGames.WhistleStop.Controllers
{
    [AddComponentMenu("Pixelism Games/Controllers/Tours/MM2 Tour Controller")]
    public class MM2TourController : TourController
    {
        private const string TOUR_DIRECTORY = "./Contrib/MM2Tour/";

        private class RobotManifest
        {
            public string Name;
            public byte CompletionFlag;
            public int MenuLevel;
            public int InGameLevel;
            public int CheckpointProgress;
            public int BossProgress;
            public bool IsComplete;

            public RobotManifest(string name, byte completionFlag, int menuLevel, int inGameLevel, int checkpointProgress, int bossProgress)
            {
                Name = name;
                CompletionFlag = completionFlag;
                MenuLevel = menuLevel;
                InGameLevel = inGameLevel;
                CheckpointProgress = checkpointProgress;
                BossProgress = bossProgress;
            }
        }

        private const byte RAM_OFFSET = 0x5D;
        private const short LEVEL_PROGRESS_LOW = 0x001F;
        private const short LEVEL_PROGRESS_HIGH = 0x0020;
        private const short LEVEL = 0x002A;
        private const short COMPLETION_PROGRESS = 0x009A;
        private const short UNLOCKED_ITEMS = 0x009B;
        private const short ENERGY_TANKS = 0x00A7;
        private const short LIVES = 0x00A8;
        private const short MEGA_MAN_HP = 0x06C0;

        private const byte FULL_MEGA_MAN_HP = 28;

        private List<RobotManifest> _robotManifests;
        private int _tourStopIndex;

        public void ButtonClick1()
        {
            Singleton.CSLibretro.WriteRAM(new byte[] { 255 }, COMPLETION_PROGRESS);
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, UNLOCKED_ITEMS);
            // default all the item ammo here
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, ENERGY_TANKS);
            Singleton.CSLibretro.WriteRAM(new byte[] { 3 }, LIVES);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_MEGA_MAN_HP }, MEGA_MAN_HP);
        }

        #region MonoBehaviour

        public override void Start()
        {
            base.Start();

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //for (int i = 169; i < 256; i++)
            //{
            //    sb.AppendLine("addReportingItem(0x" + i.ToString("X4") + ", \"???\");");
            //}
            //System.IO.File.WriteAllText("./Contrib/temp.txt", sb.ToString());

            #region Reporting Items

            if (ShowReporting)
            {
                //addReportingItem(0x0003, "???");
                //addReportingItem(0x0004, "???");
                //addReportingItem(0x0005, "???");
                //addReportingItem(0x0006, "???");
                //addReportingItem(0x0007, "???");
                //addReportingItem(0x0008, "???");
                //addReportingItem(0x0009, "???");
                //addReportingItem(0x000A, "???");
                //addReportingItem(0x000B, "???");
                //addReportingItem(0x000E, "???");
                //addReportingItem(0x000F, "???");
                //addReportingItem(0x0010, "???");
                //addReportingItem(0x0011, "???");
                //addReportingItem(0x0012, "???");
                //addReportingItem(0x0013, "???");
                //addReportingItem(0x0014, "???");
                //addReportingItem(0x0015, "???");
                //addReportingItem(0x0016, "???");
                //addReportingItem(0x0017, "???");
                //addReportingItem(0x0018, "???");
                //addReportingItem(0x0019, "???");
                //addReportingItem(0x001A, "???");
                //addReportingItem(0x001B, "???");
                //addReportingItem(0x001C, "???");
                //addReportingItem(0x001D, "???");
                //addReportingItem(0x001E, "???");
                addReportingItem(LEVEL_PROGRESS_LOW, "Level Progress Low");
                addReportingItem(LEVEL_PROGRESS_HIGH, "Level Progress High");
                //addReportingItem(0x0021, "???");
                //addReportingItem(0x0022, "Vertical Screen Scroll");
                addReportingItem(0x0023, "Controller 1 Poll");
                //addReportingItem(0x0024, "???");
                //addReportingItem(0x0025, "Controller 1 Mirror");
                //addReportingItem(0x0026, "???");
                //addReportingItem(0x0027, "Controller 1 Poll 2");
                //addReportingItem(0x0028, "???");
                //addReportingItem(0x0029, "???");
                addReportingItem(LEVEL, "Level");
                //addReportingItem(0x002B, "???");
                //addReportingItem(0x002C, "???");
                //addReportingItem(0x002D, "Player X in Room");
                //addReportingItem(0x002E, "Player Y");
                //addReportingItem(0x002F, "Player Y");
                //addReportingItem(0x0030, "Gravity");
                //addReportingItem(0x0031, "???");
                //addReportingItem(0x0032, "???");
                //addReportingItem(0x0033, "???");
                //addReportingItem(0x0034, "???");
                //addReportingItem(0x0035, "???");
                //addReportingItem(0x0036, "???");
                //addReportingItem(0x0037, "???");
                //addReportingItem(0x0038, "Screen Height?");
                //addReportingItem(0x0039, "Screen Height?");
                //addReportingItem(0x003A, "???");
                //addReportingItem(0x003B, "???");
                //addReportingItem(0x003C, "???");
                //addReportingItem(0x003D, "???");
                //addReportingItem(0x003E, "???");
                //addReportingItem(0x003F, "???");
                //addReportingItem(0x0040, "???");
                //addReportingItem(0x0041, "???");
                //addReportingItem(0x0042, "???");
                //addReportingItem(0x0043, "???");
                //addReportingItem(0x0044, "???");
                //addReportingItem(0x0045, "???");
                //addReportingItem(0x0046, "???");
                //addReportingItem(0x0047, "???");
                //addReportingItem(0x0048, "Enemy Counts?");
                //addReportingItem(0x0049, "Enemy Counts?");
                //addReportingItem(0x004A, "???");
                addReportingItem(0x004B, "Invincibility Timer");
                addReportingItem(COMPLETION_PROGRESS, "Completion Progress");
                addReportingItem(UNLOCKED_ITEMS, "Unlocked Items");
                addReportingItem(0x009C, "Atomic Fire");
                addReportingItem(0x009D, "Air Shooter");
                addReportingItem(0x009E, "Leaf Shield");
                addReportingItem(0x009F, "Bubble Lead");
                addReportingItem(0x00A0, "Quick Boomerang");
                addReportingItem(0x00A1, "Time Stopper");
                addReportingItem(0x00A2, "Metal Blade");
                addReportingItem(0x00A3, "Crash Bomb");
                addReportingItem(0x00A4, "Item 1");
                addReportingItem(0x00A5, "Item 2");
                addReportingItem(0x00A6, "Item 3");
                addReportingItem(ENERGY_TANKS, "Energy Tanks");
                addReportingItem(LIVES, "Lives");
                addReportingItem(0x00A9, "Selected Weapon");
                //addReportingItem(0x00AA, "???");
                //addReportingItem(0x00AB, "???");
                addReportingItem(0x00AC, "Weapon Charge");
                //addReportingItem(0x00AD, "???");
                //addReportingItem(0x00AE, "???");
                //addReportingItem(0x00AF, "???");
                //addReportingItem(0x00B0, "???");
                //addReportingItem(0x00B1, "???");
                //addReportingItem(0x00B2, "???");
                //addReportingItem(0x00B3, "???");
                //addReportingItem(0x00B4, "???");
                //addReportingItem(0x00B5, "???");
                //addReportingItem(0x00B6, "???");
                //addReportingItem(0x00B7, "???");
                //addReportingItem(0x00B8, "???");
                //addReportingItem(0x00B9, "???");
                //addReportingItem(0x00BA, "???");
                //addReportingItem(0x00BB, "???");
                //addReportingItem(0x00BC, "???");
                //addReportingItem(0x00BD, "???");
                //addReportingItem(0x00BE, "???");
                //addReportingItem(0x00BF, "???");
                //addReportingItem(0x00C0, "???");
                //addReportingItem(0x00C1, "???");
                //addReportingItem(0x00C2, "???");
                //addReportingItem(0x00C3, "???");
                //addReportingItem(0x00C4, "???");
                //addReportingItem(0x00C5, "???");
                //addReportingItem(0x00C6, "???");
                //addReportingItem(0x00C7, "???");
                addReportingItem(0x00C8, "Intro");
                addReportingItem(0x00C9, "Intro");
                addReportingItem(0x00CA, "Intro");
                addReportingItem(0x00CB, "Difficulty");
                //addReportingItem(0x00CC, "???");
                //addReportingItem(0x00CD, "???");
                //addReportingItem(0x00CE, "???");
                //addReportingItem(0x00CF, "???");
                //addReportingItem(0x00D0, "???");
                //addReportingItem(0x00D1, "???");
                //addReportingItem(0x00D2, "???");
                //addReportingItem(0x00D3, "???");
                //addReportingItem(0x00D4, "???");
                //addReportingItem(0x00D5, "???");
                //addReportingItem(0x00D6, "???");
                //addReportingItem(0x00D7, "???");
                //addReportingItem(0x00D8, "???");
                //addReportingItem(0x00D9, "???");
                //addReportingItem(0x00DA, "???");
                //addReportingItem(0x00DB, "???");
                //addReportingItem(0x00DC, "???");
                //addReportingItem(0x00DD, "???");
                //addReportingItem(0x00DE, "???");
                //addReportingItem(0x00DF, "???");
                //addReportingItem(0x00E0, "???");
                //addReportingItem(0x00E1, "???");
                //addReportingItem(0x00E2, "???");
                //addReportingItem(0x00E3, "???");
                //addReportingItem(0x00E4, "???");
                //addReportingItem(0x00E5, "???");
                //addReportingItem(0x00E6, "???");
                //addReportingItem(0x00E7, "???");
                //addReportingItem(0x00E8, "???");
                //addReportingItem(0x00E9, "???");
                //addReportingItem(0x00EA, "???");
                //addReportingItem(0x00EB, "???");
                //addReportingItem(0x00EC, "???");
                //addReportingItem(0x00ED, "???");
                //addReportingItem(0x00EE, "???");
                //addReportingItem(0x00EF, "???");
                //addReportingItem(0x00F0, "???");
                //addReportingItem(0x00F1, "???");
                //addReportingItem(0x00F2, "???");
                //addReportingItem(0x00F3, "???");
                //addReportingItem(0x00F4, "???");
                //addReportingItem(0x00F5, "???");
                //addReportingItem(0x00F6, "???");
                //addReportingItem(0x00F7, "Graphics Set");
                //addReportingItem(0x00F8, "???");
                //addReportingItem(0x00F9, "???");
                //addReportingItem(0x00FA, "???");
                //addReportingItem(0x00FB, "???");
                //addReportingItem(0x00FC, "???");
                addReportingItem(0x00FD, "Weapon Selected on Menu");
                addReportingItem(0x00FE, "Menu Page");
                //addReportingItem(0x00FF, "???");
                //addReportingItem(0x049F, "Player X");
                //addReportingItem(0x049F, "Enemy 1 speed");
                //addReportingItem(0x04A0, "Player Y");
                //addReportingItem(0x04BF, "Enemy 1 Y");
                addReportingItem(MEGA_MAN_HP, "Mega Man HP");
            }

            #endregion

            _robotManifests = new List<RobotManifest>();
            _robotManifests.Add(new RobotManifest("Metal", (1 << 6), 7, 6, 11, 20));
            _robotManifests.Add(new RobotManifest("Flash", (1 << 5), 6, 5, 8, 18));
            _robotManifests.Add(new RobotManifest("Quick", (1 << 4), 3, 4, 7, 22));

            _tourStops = new List<TourStopController>();
            foreach (RobotManifest robotManifest in _robotManifests)
            {
                for (int i = 0; i <= 2; i++)
                {
                    string levelProgressSuffix = string.Empty;
                    if (i == 1)
                        levelProgressSuffix = " (checkpoint)";
                    else if (i == 2)
                        levelProgressSuffix = " (boss)";

                    TourStopController tourStop = Singleton.UI.CreateTourStop();
                    tourStop.Tour = this;
                    tourStop.FilePath = string.Format("{0}{1} Man{2}.{3}", TOUR_DIRECTORY, robotManifest.Name, levelProgressSuffix, STATE_EXTENSION);
                    tourStop.LoadScreenshot(string.Format("{0}{1} Man{2}.{3}", TOUR_DIRECTORY, robotManifest.Name, levelProgressSuffix, SCREENSHOT_EXTENSION));
                    tourStop.Description = string.Format("{0} Man{1}", robotManifest.Name, levelProgressSuffix);
                    _tourStops.Add(tourStop);
                }
            }
            _tourStops[_tourStopIndex].Selected = true;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Singleton.CSLibretro.SaveState(STATES_DIRECTORY + "temp1." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Singleton.CSLibretro.SaveState(STATES_DIRECTORY + "temp2." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                Singleton.CSLibretro.SaveState(STATES_DIRECTORY + "temp3." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                Singleton.CSLibretro.SaveState(STATES_DIRECTORY + "temp4." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha5))
                Singleton.CSLibretro.SaveState(STATES_DIRECTORY + "temp5." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha6))
                Singleton.CSLibretro.LoadState(STATES_DIRECTORY + "temp1." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha7))
                Singleton.CSLibretro.LoadState(STATES_DIRECTORY + "temp2." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha8))
                Singleton.CSLibretro.LoadState(STATES_DIRECTORY + "temp3." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha9))
                Singleton.CSLibretro.LoadState(STATES_DIRECTORY + "temp4." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Alpha0))
                Singleton.CSLibretro.LoadState(STATES_DIRECTORY + "temp5." + STATE_EXTENSION);

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                string screenshotFilename = string.Format("{0}Screenshot.{1}", STATES_DIRECTORY, SCREENSHOT_EXTENSION);

                if (!string.IsNullOrEmpty(_stateName))
                    screenshotFilename = string.Format("{0}{1}.{2}", STATES_DIRECTORY, _stateName, SCREENSHOT_EXTENSION);

                Singleton.CSLibretro.SaveScreenshot(screenshotFilename);
                Debug.Log("Screenshot taken: " + screenshotFilename);
            }
        }

        #endregion

        #region Callbacks
        /*
        protected override void afterRunFrame()
        {
            base.afterRunFrame();

            if (_ram[0x009C] < 14)
                Singleton.CSLibretro.WriteRAM(new byte[] { 14 }, 0x009C);
        }
        */
        #endregion

        #region Events

        public override void TourStopSelected(TourStopController tourStop)
        {
            base.TourStopSelected(tourStop);

            //_tourStopIndex = _tourStops.IndexOf(tourStop);

            loadTourStopWithInjection(tourStop);
        }

        #endregion

        #region Loading

        private void loadTourStopWithInjection(TourStopController tourStop)
        {
            byte[] state = File.ReadAllBytes(tourStop.FilePath);

            //if (_quickManComplete)
            //{
            //    state[0x009A + RAM_OFFSET] = 16;
            //}
            //if (_ram[LIVES] > 4)
            //    state[LIVES + RAM_OFFSET] = _ram[LIVES];

            //if (_ram[COINS] > 90)
            //{
            //    state[COINS + RAM_OFFSET] = _ram[COINS];
            //    state[COINS_DISPLAY_TENS + RAM_OFFSET] = _ram[COINS_DISPLAY_TENS];
            //    state[COINS_DISPLAY_ONES + RAM_OFFSET] = _ram[COINS_DISPLAY_ONES];
            //}

            //state[SCORE_MILLIONS + RAM_OFFSET] = _ram[SCORE_MILLIONS];
            //state[SCORE_HUNDRED_THOUSANDS + RAM_OFFSET] = _ram[SCORE_HUNDRED_THOUSANDS];
            //state[SCORE_TEN_THOUSANDS + RAM_OFFSET] = _ram[SCORE_TEN_THOUSANDS];
            //state[SCORE_THOUSANDS + RAM_OFFSET] = _ram[SCORE_THOUSANDS];
            //state[SCORE_HUNDREDS + RAM_OFFSET] = _ram[SCORE_HUNDREDS];
            //state[SCORE_TENS + RAM_OFFSET] = _ram[SCORE_TENS];

            Singleton.CSLibretro.LoadState(state);
        }

        #endregion

        #region State Creation

        private int _lastLevelChangeMenuLevel;
        private int _lastLevelChangeInGameLevel;
        private string _stateName;

        protected override void afterRunFrame()
        {
            base.afterRunFrame();

            if ((_lastFrameRAM[LEVEL_PROGRESS_HIGH] == 0) && (_ram[LEVEL_PROGRESS_HIGH] == 1))
            {
                if (_lastLevelChangeMenuLevel != 0)
                {
                    RobotManifest robotManifest = _robotManifests.Where(rm => (rm.MenuLevel == _lastLevelChangeMenuLevel) && (rm.InGameLevel == _lastLevelChangeInGameLevel)).FirstOrDefault();
                    if (robotManifest != null)
                    {
                        _stateName = string.Format(@"{0} Man", robotManifest.Name);
                        saveState(string.Format("{0}{1}.{2}", STATES_DIRECTORY, _stateName, STATE_EXTENSION));
                    }
                }
            }

            if (_lastFrameRAM[LEVEL] != _ram[LEVEL])
            {
                _lastLevelChangeMenuLevel = _lastFrameRAM[LEVEL];
                _lastLevelChangeInGameLevel = _ram[LEVEL];
            }

            if ((_lastFrameRAM[LIVES] - 1) == _ram[LIVES])
            {
                RobotManifest robotManifest = _robotManifests.Where(rm => rm.InGameLevel == _ram[LEVEL]).First();

                string levelProgressSuffix = string.Empty;
                if (_ram[LEVEL_PROGRESS_HIGH] >= robotManifest.BossProgress)
                    levelProgressSuffix = " (boss)";
                else if (_ram[LEVEL_PROGRESS_HIGH] >= robotManifest.CheckpointProgress)
                    levelProgressSuffix = " (checkpoint)";

                _stateName = string.Format(@"{0} Man{1}", robotManifest.Name, levelProgressSuffix);
                saveState(string.Format("{0}{1}.{2}", STATES_DIRECTORY, _stateName, STATE_EXTENSION));
            }
        }

        private void saveState(string stateFilename)
        {
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, COMPLETION_PROGRESS);
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, UNLOCKED_ITEMS);
            // default all the item ammo here
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, ENERGY_TANKS);
            Singleton.CSLibretro.WriteRAM(new byte[] { 3 }, LIVES);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_MEGA_MAN_HP }, MEGA_MAN_HP);

            Singleton.CSLibretro.SaveState(stateFilename);
            Debug.Log("Saved state: " + stateFilename);
        }

        #endregion
    }
}
