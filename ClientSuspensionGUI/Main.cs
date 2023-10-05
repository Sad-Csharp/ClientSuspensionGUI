using BepInEx;
using BepInEx.Logging;
using CarX;
using ClientSuspensionGUIRefs;
using HarmonyLib;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClientSuspensionGUI
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "ClientSuspensionGUI",
            AUTHOR = "Mizar-Valid",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0";

        internal readonly ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;
        public readonly string modFolder;

        /// <summary>
        /// booleans
        /// </summary>
        public static bool Front;
        public static bool Back;
        public static bool Together;
        public static bool _showGui1;

        /// <summary>
        /// Used to store car suspension values to later restore!
        /// </summary>
        public static float IniFL;
        public static float IniFR;
        public static float IniBL;
        public static float IniBR;

        /// <summary>
        /// Used to label input fields
        /// </summary>
        public static string Spring_Step = "0.02";
        public static string Spring_Jump = "1";
        public static string Spring_Min = "0.02";
        public static string Spring_Max = "3";
        public static string Spring_Jump_Speed = "0.1";
        public static string Up = "Q";
        public static string Down = "E";
        public static string Jump = "Space";
        public static string Window = "Backspace";
        public static string _controlCurrent;

        /// <summary>
        /// Used to store values entered in our textfields
        /// </summary>
        public static float springStep;
        public static float springJump;
        public static float springMin;
        public static float springMax;
        public static float springjumpSpeed;

        /// <summary>
        /// GUI/Black list
        /// </summary>
        public static Rect windowRect = new Rect(0, 0, 350, 600);
        public static string[] SceneBlackList = { "Start", "Startup", "Starting", "Begin", "Load", "Loading", "SelectCar", "Empty", "vp_showroom" };
        public static KeyCode windowKey = KeyCode.Backspace;
        public static KeyCode upKey = KeyCode.Q;
        public static KeyCode downKey = KeyCode.E;
        public static KeyCode jumpKey = KeyCode.Space;
        public static Scene scene1;

        public static Car LCCar;
        public static CARXCar LCXCar;
        public static Wheel FrontLeft;
        public static Wheel FrontRight;
        public static Wheel BackLeft;
        public static Wheel BackRight;
        public static RaceCar car;
        public static RaceCar[] allCars;
        public static void CarSearch()
        {
            allCars = Object.FindObjectsOfType<RaceCar>();
            for (int i = 0; i < allCars.Length; i++)
            {
                if (!allCars[i].isNetworkCar)
                {
                    car = allCars[i];
                    LCCar = allCars[i].GetComponentInParent<Car>();
                    LCXCar = allCars[i].GetComponentInParent<CARXCar>();
                    FrontLeft = LCXCar.GetWheel(WheelIndex.FrontLeft);
                    FrontRight = LCXCar.GetWheel(WheelIndex.FrontRight);
                    BackLeft = LCXCar.GetWheel(WheelIndex.RearLeft);
                    BackRight = LCXCar.GetWheel(WheelIndex.RearRight);
                    return;
                }
            }
        }

        #endregion

        public Main()
        {
            log = Logger;
            harmony = new Harmony(GUID);
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);
        }

        public void Awake()
        {

        }

        public void Start()
        {
            GetPlayerPrefs();
            windowRect = CenterWindow(windowRect);
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
                windowRect = GUILayout.Window(0, windowRect, ClientGUI.ClientSusWindow, "Client Suspension", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                Refs.MyCar().getRigidbody.detectCollisions = true;
            }
        }

        public void Update()
        {
            if (SceneBlackList.Contains(scene1.name) == false)
            {
                if (Input.GetKey(downKey))
                {
                    CarSearch();
                    if (Together == true)
                    {
                        Front = false;
                        Back = false;
                        FrontLeft.maxSpringLen -= springStep;

                        if (FrontLeft.maxSpringLen < springMin)
                        { FrontLeft.maxSpringLen = springMin; }

                        FrontRight.maxSpringLen -= springStep;

                        if (FrontRight.maxSpringLen < springMin)
                        { FrontRight.maxSpringLen = springMin; }

                        BackLeft.maxSpringLen -= springStep;

                        if (BackLeft.maxSpringLen < springMin)
                        { BackLeft.maxSpringLen = springMin; }

                        BackRight.maxSpringLen -= springStep;

                        if (BackRight.maxSpringLen < springMin)
                        { BackRight.maxSpringLen = springMin; }
                    }
                    else if (Front == true)
                    {
                        Together = false;
                        Back = false;
                        FrontLeft.maxSpringLen -= springStep;

                        if (FrontLeft.maxSpringLen < springMin)
                        {
                            FrontLeft.maxSpringLen = springMin;
                        }

                        FrontRight.maxSpringLen -= springStep;

                        if (FrontRight.maxSpringLen < springMin)
                        {
                            FrontRight.maxSpringLen = springMin;
                        }
                    }
                    else if (Back == true)
                    {
                        Together = false;
                        Front = false;

                        BackLeft.maxSpringLen -= springStep;

                        if (BackLeft.maxSpringLen < springMin)
                        {
                            BackLeft.maxSpringLen = springMin;
                        }

                        BackRight.maxSpringLen -= springStep;

                        if (BackRight.maxSpringLen < springMin)
                        {
                            BackRight.maxSpringLen = springMin;
                        }
                    }
                }
                if (Input.GetKey(upKey))
                {
                    CarSearch();
                    if (Together == true)
                    {
                        Front = false;
                        Back = false;

                        FrontLeft.maxSpringLen += springStep;

                        if (FrontLeft.maxSpringLen > springMax)
                        { FrontLeft.maxSpringLen = springMax; }

                        FrontRight.maxSpringLen += springStep;

                        if (FrontRight.maxSpringLen > springMax)
                        { FrontRight.maxSpringLen = springMax; }

                        BackLeft.maxSpringLen += springStep;

                        if (BackLeft.maxSpringLen > springMax)
                        { BackLeft.maxSpringLen = springMax; }

                        BackRight.maxSpringLen += springStep;

                        if (BackRight.maxSpringLen > springMax)
                        { BackRight.maxSpringLen = springMax; }
                    }
                    else if (Front == true)
                    {
                        Together = false;
                        Back = false;

                        FrontLeft.maxSpringLen += springStep;

                        if (FrontLeft.maxSpringLen > springMax)
                        { FrontLeft.maxSpringLen = springMax; }

                        FrontRight.maxSpringLen += springStep;

                        if (FrontRight.maxSpringLen > springMax)
                        { FrontRight.maxSpringLen = springMax; }
                    }
                    else if (Back == true)
                    {
                        Together = false;
                        Front = false;

                        BackLeft.maxSpringLen += springStep;

                        if (BackLeft.maxSpringLen > springMax)
                        { BackLeft.maxSpringLen = springMax; }

                        BackRight.maxSpringLen += springStep;

                        if (BackRight.maxSpringLen > springMax)
                        { BackRight.maxSpringLen = springMax; }
                    }
                }
                if (Input.GetKeyDown(jumpKey))
                {
                    CarSearch();
                    StartCoroutine(UP());
                }
            }
            if (Input.GetKeyDown(windowKey))
            {
                _showGui1 = !_showGui1;
            }
        }

        IEnumerator UP()
        {
            if (Together == true)
            {
                Front = false;
                Back = false;
                FrontLeft.maxSpringLen = springJump;
                FrontRight.maxSpringLen = springJump;
                BackRight.maxSpringLen = springJump;
                BackLeft.maxSpringLen = springJump;
            }
            else if (Front == true)
            {
                Together = false;
                Back = false;
                FrontLeft.maxSpringLen = springJump;
                FrontRight.maxSpringLen = springJump;
            }
            else if (Back == true)
            {
                Together = false;
                Front = false;
                BackLeft.maxSpringLen = springJump;
                BackRight.maxSpringLen = springJump;
            }

            yield return new WaitForSeconds(0.1f);

            if (Together == true)
            {
                Front = false;
                Back = false;
                FrontLeft.maxSpringLen = springJump;
                FrontRight.maxSpringLen = springJump;
                BackRight.maxSpringLen = springJump;
                BackLeft.maxSpringLen = springJump;
            }
            else if (Front == true)
            {
                Together = false;
                Back = false;
                FrontLeft.maxSpringLen = springJump;
                FrontRight.maxSpringLen = springJump;
            }
            else if (Back == true)
            {
                Together = false;
                Front = false;
                BackLeft.maxSpringLen = springJump;
                BackRight.maxSpringLen = springJump;
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
            if (PlayerPrefs.HasKey("springStep") || PlayerPrefs.HasKey("springJump") || PlayerPrefs.HasKey("springMin") || PlayerPrefs.HasKey("springMax") || PlayerPrefs.HasKey("springJumpSpeed") || PlayerPrefs.HasKey("upKey") || PlayerPrefs.HasKey("downKey") || PlayerPrefs.HasKey("jumpKey") || PlayerPrefs.HasKey("windowKey"))
            {
                Spring_Step = PlayerPrefs.GetFloat("springStep").ToString();
                Spring_Jump = PlayerPrefs.GetFloat("springJump").ToString();
                Spring_Min = PlayerPrefs.GetFloat("springMin").ToString();
                Spring_Max = PlayerPrefs.GetFloat("springMax").ToString();
                Spring_Jump_Speed = PlayerPrefs.GetFloat("springJumpSpeed").ToString();
                if (KeyCode.TryParse(PlayerPrefs.GetString("upKeyBind"), out KeyCode keyCode))
                {
                    upKey = keyCode;
                    Up = upKey.ToString();
                }
                if (KeyCode.TryParse(PlayerPrefs.GetString("downKeyBind"), out KeyCode keyCode1))
                {
                    downKey = keyCode1;
                    Down = downKey.ToString();
                }
                if (KeyCode.TryParse(PlayerPrefs.GetString("jumpKeyBind"), out KeyCode keyCode2))
                {
                    jumpKey = keyCode2;
                    Jump = jumpKey.ToString();
                }
                if (KeyCode.TryParse(PlayerPrefs.GetString("windowKeyBind"), out KeyCode keyCode3))
                {
                    windowKey = keyCode3;
                    Window = windowKey.ToString();
                }
            }
        }
    }
}
