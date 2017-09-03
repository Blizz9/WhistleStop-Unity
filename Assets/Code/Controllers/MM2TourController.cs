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
            public bool IsCompleted;
            public bool IsVirtuallyCompleted;

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
        private const short ATOMIC_FIRE_AMMO = 0x009C;
        private const short AIR_SHOOTER_AMMO = 0x009D;
        private const short LEAF_SHIELD_AMMO = 0x009E;
        private const short BUBBLE_LEAD_AMMO = 0x009F;
        private const short QUICK_BOOMERANG_AMMO = 0x00A0;
        private const short TIME_STOPPER_AMMO = 0x00A1;
        private const short METAL_BLADE_AMMO = 0x00A2;
        private const short CRASH_BOMB_AMMO = 0x00A3;
        private const short ITEM_1_AMMO = 0x00A4;
        private const short ITEM_2_AMMO = 0x00A5;
        private const short ITEM_3_AMMO = 0x00A6;
        private const short ENERGY_TANKS = 0x00A7;
        private const short LIVES = 0x00A8;
        private const short GAME_STATE_COUNTER = 0x00FD;
        private const short GAME_STATE = 0x00FE;
        private const short GAME_START = 0x00FF;
        private const short MEGA_MAN_HP = 0x06C0;

        private const byte FULL_MEGA_MAN_HP = 28;
        private const byte FULL_AMMO = 28;

        private List<RobotManifest> _robotManifests;
        private int _tourStopIndex;

        public void ButtonClick1()
        {
            //Singleton.CSLibretro.WriteRAM(new byte[] { 96 }, COMPLETION_PROGRESS);
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, UNLOCKED_ITEMS);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ATOMIC_FIRE_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, AIR_SHOOTER_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, LEAF_SHIELD_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, BUBBLE_LEAD_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, QUICK_BOOMERANG_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, TIME_STOPPER_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, METAL_BLADE_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, CRASH_BOMB_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_1_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_2_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_3_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, ENERGY_TANKS);
            Singleton.CSLibretro.WriteRAM(new byte[] { 3 }, LIVES);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_MEGA_MAN_HP }, MEGA_MAN_HP);
        }

        #region MonoBehaviour

        public override void Start()
        {
            base.Start();

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //for (int i = 256; i < 288; i++)
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
                addReportingItem(ATOMIC_FIRE_AMMO, "Atomic Fire Ammo");
                addReportingItem(AIR_SHOOTER_AMMO, "Air Shooter Ammo");
                addReportingItem(LEAF_SHIELD_AMMO, "Leaf Shield Ammo");
                addReportingItem(BUBBLE_LEAD_AMMO, "Bubble Lead Ammo");
                addReportingItem(QUICK_BOOMERANG_AMMO, "Quick Boomerang Ammo");
                addReportingItem(TIME_STOPPER_AMMO, "Time Stopper Ammo");
                addReportingItem(METAL_BLADE_AMMO, "Metal Blade Ammo");
                addReportingItem(CRASH_BOMB_AMMO, "Crash Bomb Ammo");
                addReportingItem(ITEM_1_AMMO, "Item 1 Ammo");
                addReportingItem(ITEM_2_AMMO, "Item 2 Ammo");
                addReportingItem(ITEM_3_AMMO, "Item 3 Ammo");
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
                addReportingItem(0x00CC, "???");
                addReportingItem(0x00CD, "???");
                addReportingItem(0x00CE, "???");
                addReportingItem(0x00CF, "???");
                addReportingItem(0x00D0, "???");
                addReportingItem(0x00D1, "???");
                addReportingItem(0x00D2, "???");
                addReportingItem(0x00D3, "???");
                addReportingItem(0x00D4, "???");
                addReportingItem(0x00D5, "???");
                addReportingItem(0x00D6, "???");
                addReportingItem(0x00D7, "???");
                addReportingItem(0x00D8, "???");
                addReportingItem(0x00D9, "???");
                addReportingItem(0x00DA, "???");
                addReportingItem(0x00DB, "???");
                addReportingItem(0x00DC, "???");
                addReportingItem(0x00DD, "???");
                addReportingItem(0x00DE, "???");
                addReportingItem(0x00DF, "???");
                addReportingItem(0x00E0, "???");
                addReportingItem(0x00E1, "???");
                addReportingItem(0x00E2, "???");
                addReportingItem(0x00E3, "???");
                addReportingItem(0x00E4, "???");
                addReportingItem(0x00E5, "???");
                addReportingItem(0x00E6, "???");
                addReportingItem(0x00E7, "???");
                addReportingItem(0x00E8, "???");
                addReportingItem(0x00E9, "???");
                addReportingItem(0x00EB, "???");
                addReportingItem(0x00EC, "???");
                addReportingItem(0x00ED, "???");
                addReportingItem(0x00EE, "???");
                addReportingItem(0x00EF, "???");
                addReportingItem(0x00F0, "???");
                addReportingItem(0x00F1, "???");
                addReportingItem(0x00F2, "???");
                addReportingItem(0x00F3, "???");
                addReportingItem(0x00F4, "???");
                addReportingItem(0x00F5, "???");
                addReportingItem(0x00F6, "???");
                addReportingItem(0x00F7, "Graphics Set");
                addReportingItem(0x00F8, "???");
                addReportingItem(0x00F9, "???");
                addReportingItem(0x00FA, "???");
                addReportingItem(0x00FB, "???");
                addReportingItem(0x00FC, "???");
                addReportingItem(0x00FD, "Weapon Selected on Menu");
                addReportingItem(0x00FE, "Menu Page");
                addReportingItem(GAME_START, "Game State");
                addReportingItem(0x049F, "Player X");
                addReportingItem(0x049F, "Enemy 1 speed");
                addReportingItem(0x04A0, "Player Y");
                addReportingItem(0x04BF, "Enemy 1 Y");
                addReportingItem(MEGA_MAN_HP, "Mega Man HP");
            }

            #endregion

            _robotManifests = new List<RobotManifest>();
            _robotManifests.Add(new RobotManifest("Metal", (1 << 6), 7, 6, 11, 20));
            _robotManifests.Add(new RobotManifest("Flash", (1 << 5), 6, 5, 8, 18));
            //_robotManifests.Last().IsCompleted = true;
            _robotManifests.Add(new RobotManifest("Quick", (1 << 4), 3, 4, 7, 22));
            _robotManifests.Add(new RobotManifest("Wood", (1 << 2), 4, 2, 9, 22));

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
                Singleton.CSLibretro.SaveScreenshot(screenshotFilename);
                Debug.Log("Screenshot taken: " + screenshotFilename);
            }
        }

        #endregion

        #region Callbacks
        
        protected override void afterRunFrame()
        {
            base.afterRunFrame();

            //if ((_lastFrameRAM[0x0023] == 0) && (_ram[0x0023] == 8) && (_ram[0x00F0] == 135) && (_ram[0x00F1] == 190))
            //{
            //    Singleton.CSLibretro.IsStepping = true;
            //}

            if ((_lastFrameRAM[LEVEL] == 0) && (_ram[LEVEL] == 5))
            {
                Debug.Log("Game Started, Loading Tour Stop: Level Select");
                loadStateWithInjection(string.Format("{0}Level Select.{1}", TOUR_DIRECTORY, STATE_EXTENSION));

                byte completion = 0;
                foreach (RobotManifest robot in _robotManifests.Where(rm => rm.IsCompleted))
                    completion |= robot.CompletionFlag;

                Singleton.CSLibretro.WriteRAM(new byte[] { completion }, COMPLETION_PROGRESS);
            }

            // make this a graphic on screen, this also tends to trigger more than it needs to
            if ((_lastFrameRAM[LEVEL] != _ram[LEVEL]) && (_ram[LEVEL] != 0))
            {
                RobotManifest recommendedRobot = _robotManifests.Where(rm => !rm.IsCompleted).FirstOrDefault();

                if ((recommendedRobot != null) && (_ram[LEVEL] != recommendedRobot.MenuLevel))
                    Singleton.UI.SetStatus(string.Format("{0} Man is the recommended robot to fight", recommendedRobot.Name));
            }

            if (_ram[ATOMIC_FIRE_AMMO] < FULL_AMMO / 2)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO / 2 }, ATOMIC_FIRE_AMMO);

            if (_ram[AIR_SHOOTER_AMMO] < FULL_AMMO / 2)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO / 2 }, AIR_SHOOTER_AMMO);

            if (_ram[LEAF_SHIELD_AMMO] < FULL_AMMO / 2)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO / 2 }, LEAF_SHIELD_AMMO);

            if (_ram[BUBBLE_LEAD_AMMO] < FULL_AMMO / 2)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO / 2 }, BUBBLE_LEAD_AMMO);

            if (_ram[QUICK_BOOMERANG_AMMO] < FULL_AMMO / 2)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO / 2 }, QUICK_BOOMERANG_AMMO);

            if (_ram[METAL_BLADE_AMMO] < FULL_AMMO / 2)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO / 2 }, METAL_BLADE_AMMO);

            if (_ram[CRASH_BOMB_AMMO] < FULL_AMMO / 2)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO / 2 }, CRASH_BOMB_AMMO);

            if (_ram[ITEM_1_AMMO] < FULL_AMMO)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_1_AMMO);

            if (_ram[ITEM_2_AMMO] < FULL_AMMO)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_2_AMMO);

            if (_ram[ITEM_3_AMMO] < FULL_AMMO)
                Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_3_AMMO);

            if (_lastFrameRAM[LEVEL] != _ram[LEVEL]) // this doesn't quite work, can trigger on select screen when moving around
            {
                RobotManifest robot = _robotManifests.Where(rm => (rm.MenuLevel == _lastFrameRAM[LEVEL]) && (rm.InGameLevel == _ram[LEVEL])).FirstOrDefault();
                if (robot != null)
                    tourStopIndexChanged(_robotManifests.IndexOf(robot) * 3);
            }

            if ((_lastFrameRAM[LEVEL_PROGRESS_HIGH] + 1) == _ram[LEVEL_PROGRESS_HIGH])
            {
                RobotManifest robot = _robotManifests.Where(rm => rm.InGameLevel == _ram[LEVEL]).First();
                if (_ram[LEVEL_PROGRESS_HIGH] == robot.BossProgress)
                    tourStopIndexChanged(_robotManifests.IndexOf(robot) * 3 + 2);
                else if (_ram[LEVEL_PROGRESS_HIGH] == robot.CheckpointProgress)
                    tourStopIndexChanged(_robotManifests.IndexOf(robot) * 3 + 1);
            }

            if ((_lastFrameRAM[LIVES] - 1) == _ram[LIVES])
                loadTourStopWithInjection(_tourStops[_tourStopIndex], false); // there really needs to be 2 starting tour stops as to not how the robot intro on death

            // suggest weapon unlock and switch to weapon in boss room

            // quickly move to stage select and not wait for full item unlock animations

        }

        #endregion

        #region Events

        public override void TourStopSelected(TourStopController tourStop)
        {
            base.TourStopSelected(tourStop);

            _tourStopIndex = _tourStops.IndexOf(tourStop);

            loadTourStopWithInjection(tourStop, true);
        }

        private void tourStopIndexChanged(int index)
        {
            _tourStopIndex = index;

            foreach (TourStopController tourStop in _tourStops)
                tourStop.Selected = false;
            _tourStops[_tourStopIndex].Selected = true;
        }

        #endregion

        #region Loading

        private void loadTourStopWithInjection(TourStopController tourStop, bool virtuallyCompleteLevels)
        {
            if (virtuallyCompleteLevels)
            {
                int targetRobotIndex = Mathf.FloorToInt(_tourStopIndex / 3);

                foreach (RobotManifest robot in _robotManifests)
                {
                    if (!robot.IsCompleted && (_robotManifests.IndexOf(robot) < targetRobotIndex))
                        robot.IsVirtuallyCompleted = true;
                    else
                        robot.IsVirtuallyCompleted = false;
                }
            }

            loadStateWithInjection(tourStop.FilePath);
        }

        private void loadStateWithInjection(string filePath)
        {
            byte[] state = File.ReadAllBytes(filePath);

            byte completion = 0;
            foreach (RobotManifest robot in _robotManifests.Where(rm => rm.IsCompleted || rm.IsVirtuallyCompleted))
                completion |= robot.CompletionFlag;

            state[COMPLETION_PROGRESS + RAM_OFFSET] = completion;

            // there is some issue with this and energy tanks not working at times?
            if (_ram[LIVES] < 4)
                state[LIVES + RAM_OFFSET] = 4;

            if (_ram[ENERGY_TANKS] < 3)
                state[ENERGY_TANKS + RAM_OFFSET] = 3;

            state[ATOMIC_FIRE_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[AIR_SHOOTER_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[LEAF_SHIELD_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[BUBBLE_LEAD_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[QUICK_BOOMERANG_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[TIME_STOPPER_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[METAL_BLADE_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[CRASH_BOMB_AMMO + RAM_OFFSET] = FULL_AMMO;

            state[UNLOCKED_ITEMS + RAM_OFFSET] = 7;
            state[ITEM_1_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[ITEM_2_AMMO + RAM_OFFSET] = FULL_AMMO;
            state[ITEM_3_AMMO + RAM_OFFSET] = FULL_AMMO;

            Singleton.CSLibretro.LoadState(state);
        }

        #endregion

        #region State Creation
        /*
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
                RobotManifest robot = _robotManifests.Where(rm => rm.InGameLevel == _ram[LEVEL]).First();

                string levelProgressSuffix = string.Empty;
                if (_ram[LEVEL_PROGRESS_HIGH] >= robotManifest.BossProgress)
                    levelProgressSuffix = " (boss)";
                else if (_ram[LEVEL_PROGRESS_HIGH] >= robotManifest.CheckpointProgress)
                    levelProgressSuffix = " (checkpoint)";

                _stateName = string.Format(@"{0} Man{1}", robotManifest.Name, levelProgressSuffix);
                saveState(string.Format("{0}{1}.{2}", STATES_DIRECTORY, _stateName, STATE_EXTENSION));
            }

            //if (_lastFrameRAM[0x00FE] != _ram[0x00FE])
            //    Debug.Log("State Value: " + _ram[0x00FE]);

            //if ((_lastFrameRAM[0x00FD] == 1) && (_ram[0x00FD] == 0))
            //    Debug.Log("Counter went down to zero, mode: " + _ram[0x00FE]);
        }

        private void saveState(string stateFilename)
        {
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, COMPLETION_PROGRESS);
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, UNLOCKED_ITEMS);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ATOMIC_FIRE_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, AIR_SHOOTER_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, LEAF_SHIELD_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, BUBBLE_LEAD_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, QUICK_BOOMERANG_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, TIME_STOPPER_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, METAL_BLADE_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, CRASH_BOMB_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_1_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_2_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_AMMO }, ITEM_3_AMMO);
            Singleton.CSLibretro.WriteRAM(new byte[] { 0 }, ENERGY_TANKS);
            Singleton.CSLibretro.WriteRAM(new byte[] { 3 }, LIVES);
            Singleton.CSLibretro.WriteRAM(new byte[] { FULL_MEGA_MAN_HP }, MEGA_MAN_HP);

            Singleton.CSLibretro.SaveState(stateFilename);
            Debug.Log("Saved state: " + stateFilename);
        }
        */
        #endregion
    }
}
