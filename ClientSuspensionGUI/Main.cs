using System;
using CarX;
using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZML.API;

namespace ClientSuspensionGUI
{
    //[ZMLMod("plugin.revive.mod", "Suspension Mod", "2.1.2", "Mizar")] // pulled a you last night to have a friend test lol still no go :(
    [ZMLMod("plugin.Suspension.mod", "Suspension Mod", "2.1.2", "Mizar")]
    public class Main : BaseMod
    {
        #region[Declarations]

        public static bool Front;
        public static bool Back;
        public static bool Together;
        private static bool _showGui1;
        
        // Used to store CarxCar suspension values to later restore!
        public static float IniFl;
        public static float IniFr;
        public static float IniBL;
        public static float IniBR;
        
        // input field strings
        public static string SpringStepString = "0.02";
        public static string SpringJumpString = "1";
        public static string SpringMinString = "0.02";
        public static string SpringMaxString = "3";
        public static string Up = "Q";
        public static string Down = "E";
        public static string Jump = "Space";
        public static string Window = "Backspace";
        public static string? ControlCurrent;

        // input field stored values
        public static float SpringStep;
        public static float SpringJump;
        public static float SpringMin;
        public static float SpringMax;
        private static Rect _windowRect = new Rect(0, 0, 350, 600);
        private static readonly string[] SceneBlackList = { "Start", "Startup", "Starting", "Begin", "Load", "Loading", "SelectCar", "Empty", "vp_showroom" };
        public static KeyCode WindowKey = KeyCode.Backspace;
        public static KeyCode UpKey = KeyCode.Q;
        public static KeyCode DownKey = KeyCode.E;
        public static KeyCode JumpKey = KeyCode.Space;
        private static Scene _scene1;
        private static Wheel _fl;
        private static Wheel _fr;
        private static Wheel _bl;
        private static Wheel _br;

        private static RaceCar CarSearch()
        {
            return PlayerCarControl.instance
                ? PlayerCarControl.instance.car
                : GameObject.Find("CarPositionMarker").GetComponent<RaceCar>();
        }

        private static Wheel FrontLeft
        {
            get
            {
                if (CarSearch() == null) return _fl;
                var car = CarSearch().GetComponent<CARXCar>();
                _fl = car.GetWheel(WheelIndex.FrontLeft);
                return _fl;
            }
        }


        private static Wheel FrontRight
        {
            get
            {
                if (CarSearch() == null) return _fr;
                var car = CarSearch().GetComponent<CARXCar>();
                _fr = car.GetWheel(WheelIndex.FrontRight);
                return _fr;
            }
        }

        private static Wheel BackRight
        {
            get
            {
                if (CarSearch() == null) return _br;
                var car = CarSearch().GetComponent<CARXCar>();
                _br = car.GetWheel(WheelIndex.RearRight);
                return _br;
            }
        }

        private static Wheel BackLeft
        {
            get
            {
                if (CarSearch() == null) return _bl;
                var car = CarSearch().GetComponent<CARXCar>();
                _bl = car.GetWheel(WheelIndex.RearLeft);
                return _bl;
            }
        }

        #endregion

        public void Start()
        {
            NetSync.ProcessPacket += NetSync.HandleSync;
            GetPlayerPrefs();
            _windowRect = CenterWindow(_windowRect);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Applies methods on Scene change every time a scene changes
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (SceneBlackList.Contains(scene.name) == false)
            {
                ClientGUI.GetValues();
            }
        }

        public void OnGUI()
        {
            if (_showGui1)
            {
                _windowRect = GUILayout.Window(0, _windowRect, ClientGUI.ClientSusWindow, "Client Suspension",
                    GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
        }

        public void Update()
        {
            NetSync.Update();
            if (SceneBlackList.Contains(_scene1.name) == false)
            {
                if (Input.GetKey(DownKey))
                {
                    var frontLeft = FrontLeft;
                    var frontRight = FrontRight;
                    var backLeft = BackLeft;
                    var backRight = BackRight;

                    if (Together)
                    {
                        Front = false;
                        Back = false;
                        frontLeft.maxSpringLen -= SpringStep;

                        if (frontLeft.maxSpringLen < SpringMin)
                        {
                            frontLeft.maxSpringLen = SpringMin;
                        }

                        frontRight.maxSpringLen -= SpringStep;

                        if (frontRight.maxSpringLen < SpringMin)
                        {
                            frontRight.maxSpringLen = SpringMin;
                        }

                        backLeft.maxSpringLen -= SpringStep;

                        if (backLeft.maxSpringLen < SpringMin)
                        {
                            backLeft.maxSpringLen = SpringMin;
                        }

                        backRight.maxSpringLen -= SpringStep;

                        if (backRight.maxSpringLen < SpringMin)
                        {
                            backRight.maxSpringLen = SpringMin;
                        }

                        NetSync.SendSuspensionData(frontLeft, frontRight, backLeft,
                            backRight);
                    }
                    else if (Front)
                    {
                        Together = false;
                        Back = false;
                        frontLeft.maxSpringLen -= SpringStep;

                        if (frontLeft.maxSpringLen < SpringMin)
                        {
                            frontLeft.maxSpringLen = SpringMin;
                        }

                        frontRight.maxSpringLen -= SpringStep;

                        if (frontRight.maxSpringLen < SpringMin)
                        {
                            frontRight.maxSpringLen = SpringMin;
                        }
                        NetSync.SendSuspensionData(frontLeft, frontRight, backLeft,
                            backRight);
                    }
                    else if (Back)
                    {
                        Together = false;
                        Front = false;

                        backLeft.maxSpringLen -= SpringStep;

                        if (backLeft.maxSpringLen < SpringMin)
                        {
                            backLeft.maxSpringLen = SpringMin;
                        }

                        backRight.maxSpringLen -= SpringStep;

                        if (backRight.maxSpringLen < SpringMin)
                        {
                            backRight.maxSpringLen = SpringMin;
                        }
                        NetSync.SendSuspensionData(frontLeft, frontRight, backLeft,
                            backRight);
                    }
                }

                if (Input.GetKey(UpKey))
                {
                    var left = FrontLeft;
                    var right = FrontRight;
                    var bLeft = BackLeft;
                    var bRight = BackRight;

                    if (Together)
                    {
                        Front = false;
                        Back = false;

                        left.maxSpringLen += SpringStep;

                        if (left.maxSpringLen > SpringMax)
                        {
                            left.maxSpringLen = SpringMax;
                        }

                        right.maxSpringLen += SpringStep;

                        if (right.maxSpringLen > SpringMax)
                        {
                            right.maxSpringLen = SpringMax;
                        }

                        bLeft.maxSpringLen += SpringStep;

                        if (bLeft.maxSpringLen > SpringMax)
                        {
                            bLeft.maxSpringLen = SpringMax;
                        }

                        bRight.maxSpringLen += SpringStep;

                        if (bRight.maxSpringLen > SpringMax)
                        {
                            bRight.maxSpringLen = SpringMax;
                        }
                        NetSync.SendSuspensionData(left, right, bLeft,
                            bRight);
                    }
                    else if (Front)
                    {
                        Together = false;
                        Back = false;

                        left.maxSpringLen += SpringStep;

                        if (left.maxSpringLen > SpringMax)
                        {
                            left.maxSpringLen = SpringMax;
                        }

                        right.maxSpringLen += SpringStep;

                        if (right.maxSpringLen > SpringMax)
                        {
                            right.maxSpringLen = SpringMax;
                        }
                        NetSync.SendSuspensionData(left, right, bLeft,
                            bRight);
                    }
                    else if (Back)
                    {
                        Together = false;
                        Front = false;

                        bLeft.maxSpringLen += SpringStep;

                        if (bLeft.maxSpringLen > SpringMax)
                        {
                            bLeft.maxSpringLen = SpringMax;
                        }

                        bRight.maxSpringLen += SpringStep;

                        if (bRight.maxSpringLen > SpringMax)
                        {
                            bRight.maxSpringLen = SpringMax;
                        }
                        NetSync.SendSuspensionData(left, right, bLeft,
                            bRight);
                    }
                }

                if (Input.GetKeyDown(JumpKey))
                {
                    StartCoroutine(UP());
                }
            }

            if (Input.GetKeyDown(WindowKey))
            {
                _showGui1 = !_showGui1;
            }
        }


        IEnumerator UP()
        {
            var left = FrontLeft;
            var right = FrontRight;
            var bLeft = BackLeft;
            var bRight = BackRight;

            if (Together)
            {
                Front = false;
                Back = false;
                left.maxSpringLen = SpringJump;
                right.maxSpringLen = SpringJump;
                bRight.maxSpringLen = SpringJump;
                bLeft.maxSpringLen = SpringJump;
            }
            else if (Front)
            {
                Together = false;
                Back = false;
                _fl.maxSpringLen = SpringJump;
                _fr.maxSpringLen = SpringJump;
            }
            else if (Back)
            {
                Together = false;
                Front = false;
                _bl.maxSpringLen = SpringJump;
                _br.maxSpringLen = SpringJump;
            }

            yield return new WaitForSeconds(0.1f);

            if (Together)
            {
                Front = false;
                Back = false;
                left.maxSpringLen = SpringJump;
                right.maxSpringLen = SpringJump;
                bRight.maxSpringLen = SpringJump;
                bLeft.maxSpringLen = SpringJump;
            }
            else if (Front)
            {
                Together = false;
                Back = false;
                left.maxSpringLen = SpringJump;
                right.maxSpringLen = SpringJump;
            }
            else if (Back)
            {
                Together = false;
                Front = false;
                bLeft.maxSpringLen = SpringJump;
                bRight.maxSpringLen = SpringJump;
            }

            try
            {
                NetSync.SendSuspensionData(left, right,
                    bLeft, bRight);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Source);
            }

            ClientGUI.RestoreValues();
        }

        /// <summary>
        /// Makes any Rect you pass through, auto center to your screen.
        /// </summary>
        /// <param name="windowRect"></param>
        /// <returns>windowRect</returns>
        public static Rect CenterWindow(Rect windowRect)
        {
            windowRect.x = (Screen.width - windowRect.width) / 2;
            windowRect.y = (Screen.height - windowRect.height) / 2;
            return windowRect;
        }

        public static void GetPlayerPrefs()
        {
            if (PlayerPrefs.HasKey("springStep") || PlayerPrefs.HasKey("springJump") ||
                PlayerPrefs.HasKey("springMin") || PlayerPrefs.HasKey("springMax") ||
                PlayerPrefs.HasKey("springJumpSpeed") || PlayerPrefs.HasKey("upKey") || PlayerPrefs.HasKey("downKey") ||
                PlayerPrefs.HasKey("jumpKey") || PlayerPrefs.HasKey("windowKey"))
            {
                SpringStepString = PlayerPrefs.GetFloat("springStep").ToString(CultureInfo.CurrentCulture);
                SpringJumpString = PlayerPrefs.GetFloat("springJump").ToString(CultureInfo.CurrentCulture);
                SpringMinString = PlayerPrefs.GetFloat("springMin").ToString(CultureInfo.CurrentCulture);
                SpringMaxString = PlayerPrefs.GetFloat("springMax").ToString(CultureInfo.CurrentCulture);
                SpringJumpSpeed = PlayerPrefs.GetFloat("springJumpSpeed").ToString(CultureInfo.CurrentCulture);
                if (Enum.TryParse(PlayerPrefs.GetString("upKeyBind"), out KeyCode keyCode))
                {
                    UpKey = keyCode;
                    Up = UpKey.ToString();
                }

                if (Enum.TryParse(PlayerPrefs.GetString("downKeyBind"), out KeyCode keyCode1))
                {
                    DownKey = keyCode1;
                    Down = DownKey.ToString();
                }

                if (Enum.TryParse(PlayerPrefs.GetString("jumpKeyBind"), out KeyCode keyCode2))
                {
                    JumpKey = keyCode2;
                    Jump = JumpKey.ToString();
                }

                if (Enum.TryParse(PlayerPrefs.GetString("windowKeyBind"), out KeyCode keyCode3))
                {
                    WindowKey = keyCode3;
                    Window = WindowKey.ToString();
                }
            }
        }
    }
}