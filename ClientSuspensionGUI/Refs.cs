using CarModelSystem;
using CarX;
using CodeStage.AntiCheat.Detectors;
using GameOverlay;
using SyncMultiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VinylSystem;

namespace ClientSuspensionGUIRefs
{
    public class Refs : MonoBehaviour
    {
        #region[LOCAL_PLAYER]

        public static string SteamID { get => Overlay.instance.localUniqId.ToString(); }
        public static PlayerCarControl PlayerCarControl { get => PlayerCarControl.instance; }
        public static Rigidbody LP_Rigidbody { get => LP_CARXCar.getRigidbody; }
        public static Transform LP_Transform { get => LP_CARXCar.getTransform; }
        public static RaceCar LP_RaceCar { get => PlayerCarControl.car; }
        public static Car LP_Car { get => LP_RaceCar.GetCar(); } 
        public static CARXCar LP_CARXCar { get => LP_RaceCar.GetComponentInParent<CARXCar>(); }
        public static GameObject LP_CarGameObject { get => LP_RaceCar.gameObject; }
        public static CarEngineController LP_CarEngineController { get => LP_RaceCar.engineController; }
        public static CarModel LP_CarModel { get => LP_RaceCar.carModel; }
        public static Livery LP_Livery { get => LP_CarModel.livery; }
        public static CarSkeleton LP_Skeleton { get => LP_CarModel.skeleton; }
        public static CarLightController LP_LightCont { get => LP_RaceCar.carLightController; }
        public static CarResourceProvider LP_ResProvider { get => FindObjectOfType<CarResourceProvider>(); }
        public static ParticleSystem LP_CollisionSparks { get => LP_ResProvider.collisionSparks; }
        public static ParticleSystem LP_ExhaustFlame { get => LP_ResProvider.exhaustFlame; }
        public static ParticleSystem LP_ExhaustSmoke { get => LP_ResProvider.exhaustSmoke; }
        public static GameObject LP_DriverPrefab { get => LP_ResProvider.driverPrefab; }
        public static DriftController LP_DriftController { get => PlayerCarControl.car.GetComponentInParent<DriftController>(); }


        #endregion

        #region[NETWORK]
        public static NetworkController NetworkController { get => FindObjectOfType<NetworkController>(); }
        #endregion

        #region[Day-Night/Light Scene Controllers]
        public static DayNightController DayNightCont { get => FindObjectOfType<DayNightController>(); }
        public static LightVariantController LightVariant { get => FindObjectOfType<LightVariantController>(); }
        public static string Active_SceneName { get => SceneManager.GetActiveScene().name; }
        public static string Active_LightSceneName { get => DayNightCont.m_lastSceneLightingName; set => DayNightCont.TryApplyMode(value, IsNight); }
        public static bool IsNight { get => !DayNightCont.IsDayLight; set => DayNightCont.SetDayNightMode(value); }
        #endregion

        #region[Map]
        public static CARXSurfaceManager SurfaceManager { get => FindObjectOfType<CARXSurfaceManager>(); }
        public static CameraRotation CamRotation { get => CameraRotation.instance; }
        #endregion

        #region[Random]
        public static DynoParamsList[] DynoParams_List { get => FindObjectsOfType<DynoParamsList>(); }
        public static ObscuredCheatingDetector AC_Obscr { get => FindObjectOfType<ObscuredCheatingDetector>(); }
        public static VinylSystemConfig VinylConfig { get => VinylSystemConfig.config; }
        public static DontDestroyOnLoad HDRPCustomPasses { get => FindObjectOfType<DontDestroyOnLoad>(); }
        public static RoomUILine[] MP_Rooms { get => FindObjectsOfType<RoomUILine>(); }
        public static string Game_Version { get => GameVersion.versionAsString; }
        public static GameManager Game_Manager { get => FindObjectOfType<GameManager>(); }
        public static GamePrefs Game_Prefs { get => FindObjectOfType<GamePrefs>(); }
        public static GameExpController Exp_Controller { get => FindObjectOfType<GameExpController>(); }
        public static TimeController Time_Controller { get => FindObjectOfType<TimeController>(); }
        public static NetworkController Network_Controller { get => FindObjectOfType<NetworkController>(); }
        public static UIMessageBox UI_MessageBox { get => FindObjectOfType<UIMessageBox>(); }
        public static VinylMetaProvider Vinyl_Meta_Provider { get => FindObjectOfType<VinylMetaProvider>(); }
        public static AFKController AFK_Controller { get => FindObjectOfType<AFKController>(); }
        public static RestoreManager Restore_Manager { get => FindObjectOfType<RestoreManager>(); }
        public static void ShowText(string text, float time) { GUICommonGodVoice.ShowText(text, time); }
        #endregion

    }
}
