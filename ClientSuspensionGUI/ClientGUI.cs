using CarX;
using ClientSuspensionGUIRefs;
using System;
using UnityEngine;

namespace ClientSuspensionGUI
{
    internal class ClientGUI : MonoBehaviour
    {
        /// <summary>
        /// ClientSuspension window's content
        /// </summary>
        /// <param name="windowID"></param>
        public static void ClientSusWindow(int windowID)
        {
            GUIStyle style1 = new GUIStyle(GUI.skin.label) // judge me later :kek: 
            {
                fontStyle = FontStyle.Bold
            };
            GUIStyle style2 = new GUIStyle()
            {
                fontStyle = FontStyle.Bold
            };
            GUIStyle style3 = new GUIStyle(GUI.skin.toggle)
            {
                fontStyle = FontStyle.Bold
            };
            GUIStyle style4 = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 20
            };

            style1.normal.textColor = Color.yellow; // ik redundant shit all over lol THE PROJECT IS OLD!
            style2.normal.textColor = Color.white;
            style3.normal.textColor = Color.yellow;
            style4.normal.textColor = Color.yellow;

            Main.Front = GUILayout.Toggle(Main.Front, "Front", style3);
            GUILayout.Label("Use front suspension together!", style2);

            Main.Back = GUILayout.Toggle(Main.Back, "Back", style3);
            GUILayout.Label("Use back suspension together", style2);

            Main.Together = GUILayout.Toggle(Main.Together, "Together", style3);
            GUILayout.Label("Use all suspension together", style2);

            var eventCurrent = Event.current;
            Main.ControlCurrent = GUI.GetNameOfFocusedControl();

            GUILayout.Label("Window Keybind", style1);
            GUI.SetNextControlName("windowControl");
            Main.Window = GUILayout.TextField(Main.Window, GUILayout.Width(100));
            var textBoxFocused = Main.ControlCurrent == "windowControl";
            if (textBoxFocused && eventCurrent.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
            }
            if (textBoxFocused && eventCurrent.type == EventType.KeyUp)
            {
                Main.Window = eventCurrent.keyCode.ToString();
                PlayerPrefs.SetString("windowKeyBind", Main.Window);
                if (Enum.TryParse(PlayerPrefs.GetString("windowKeyBind"), out KeyCode newKeyCode))
                {
                    Main.WindowKey = newKeyCode;
                }
            }

            GUILayout.Label("UP Keybind", style1);
            GUI.SetNextControlName("upControl");
            Main.Up = GUILayout.TextField(Main.Up, GUILayout.Width(100));
            bool textBoxFocused1 = Main.ControlCurrent == "upControl";
            if (textBoxFocused1 && eventCurrent.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
            }
            if (textBoxFocused1 && eventCurrent.type == EventType.KeyUp)
            {
                Main.Up = eventCurrent.keyCode.ToString();
                PlayerPrefs.SetString("upKeyBind", Main.Up);
                if (Enum.TryParse(PlayerPrefs.GetString("upKeyBind"), out KeyCode newKeyCode))
                {
                    Main.UpKey = newKeyCode;
                }
            }

            GUILayout.Label("Down Keybind", style1);
            GUI.SetNextControlName("downControl");
            Main.Down = GUILayout.TextField(Main.Down, GUILayout.Width(100));
            var textBoxFocused2 = Main.ControlCurrent == "downControl";
            if (textBoxFocused2 && eventCurrent.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
            }
            if (textBoxFocused2 && eventCurrent.type == EventType.KeyUp)
            {
                Main.Down = eventCurrent.keyCode.ToString();
                PlayerPrefs.SetString("downKeyBind", Main.Down);
                if (Enum.TryParse(PlayerPrefs.GetString("downKeyBind"), out KeyCode newKeyCode))
                {
                    Main.DownKey = newKeyCode;
                }
            }

            GUILayout.Label("Jump Keybind", style1);
            GUI.SetNextControlName("jumpControl");
            Main.Jump = GUILayout.TextField(Main.Jump, GUILayout.Width(100));
            bool textBoxFocused3 = Main.ControlCurrent == "jumpControl";
            if (textBoxFocused3 && eventCurrent.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
            }
            if (textBoxFocused3 && eventCurrent.type == EventType.KeyUp)
            {
                Main.Jump = eventCurrent.keyCode.ToString();
                PlayerPrefs.SetString("jumpKeyBind", Main.Jump);
                if (Enum.TryParse(PlayerPrefs.GetString("jumpKeyBind"), out KeyCode newKeyCode))
                {
                    Main.JumpKey = newKeyCode;
                }
            }

            GUILayout.Label("Spring Ammount: [0.001f - 0.05f]", style1);
            GUILayout.Label("Amount to go Up/Down per Update() call", style2);
            Main.SpringStepString = GUILayout.TextField(Main.SpringStepString, GUILayout.Width(100));
            float.TryParse(Main.SpringStepString, out float result);
            Main.SpringStep = Mathf.Clamp(result, 0.001f, 0.05f);
            PlayerPrefs.SetFloat("springStep", Main.SpringStep);

            GUILayout.Label("Spring Jump: [0f - 10f]", style1);
            GUILayout.Label("Amount to jump by", style2);
            Main.SpringJumpString = GUILayout.TextField(Main.SpringJumpString, GUILayout.Width(100));
            float.TryParse(Main.SpringJumpString, out float result1);
            Main.SpringJump = Mathf.Clamp(result1, 0f, 10f);
            PlayerPrefs.SetFloat("springJump", result1);

            GUILayout.Label("Spring Min: [-4f - 10f]", style1);
            GUILayout.Label("Amount to extend springs Min", style2);
            Main.SpringMinString = GUILayout.TextField(Main.SpringMinString, GUILayout.Width(100));
            float.TryParse(Main.SpringMinString, out float result2);
            Main.SpringMin = Mathf.Clamp(result2, -4f, 10f);
            PlayerPrefs.SetFloat("springMin", result2);

            GUILayout.Label("Spring Max: [-4f - 10f]", style1);
            GUILayout.Label("Amount to extend springs Max", style2);
            Main.SpringMaxString = GUILayout.TextField(Main.SpringMaxString, GUILayout.Width(100));
            float.TryParse(Main.SpringMaxString, out float result3);
            Main.SpringMax = Mathf.Clamp(result3, -4f, 10f);
            PlayerPrefs.SetFloat("springMax", result3);
            
            if (GUI.changed)
            {
                PlayerPrefs.Save();
            }
            if (GUILayout.Button("Store default Suspension", style4, GUILayout.ExpandWidth(true)))
            {
                GetValues();
            }
            if (GUILayout.Button("Restore default suspension", style4, GUILayout.ExpandWidth(true)))
            {
                RestoreValues();
            }

            GUI.DragWindow();
        }

        /// <summary>
        /// Restores our localPlayer's initial suspension stored in UnityEngine.PlayerPrefs
        /// </summary>
        internal static void RestoreValues()
        {
            if (PlayerPrefs.HasKey("IniFL") && PlayerPrefs.HasKey("IniFR") && PlayerPrefs.HasKey("IniBL") && PlayerPrefs.HasKey("IniBR"))
            {
                PlayerPrefs.GetFloat("IniFL", Main.IniFl);
                PlayerPrefs.GetFloat("IniFR", Main.IniFr);
                PlayerPrefs.GetFloat("IniBL", Main.IniBL);
                PlayerPrefs.GetFloat("IniBR", Main.IniBR);

                var frontLeftWheel = Refs.LP_CARXCar.GetWheel(WheelIndex.FrontLeft);
                var frontRightWheel = Refs.LP_CARXCar.GetWheel(WheelIndex.FrontRight);
                var rearLeftWheel = Refs.LP_CARXCar.GetWheel(WheelIndex.RearLeft);
                var rearRightWheel = Refs.LP_CARXCar.GetWheel(WheelIndex.RearRight);

                frontLeftWheel.maxSpringLen = Main.IniFl;
                frontRightWheel.maxSpringLen = Main.IniFr;
                rearLeftWheel.maxSpringLen = Main.IniBL;
                rearRightWheel.maxSpringLen = Main.IniBR;

                Debug.LogWarning($"[Restoring Values]: Found Initial Suspension Values, Restoring These --> FrontLeft: {Main.IniFl}f, FrontRight: {Main.IniFr}f, BackLeft: {Main.IniBL}f And BackRight: {Main.IniBR}f");
            }
            else
            {
                Debug.LogError("[Restoring Values]: Failed To Restore Suspension Values, They Don't Exist!");
            }
        }

        /// <summary>
        /// Gets our localPlayer's current suspension set up and stores it in UnityEngine.PlayerPrefs
        /// </summary>
        internal static void GetValues()
        {
            try
            {
                Main.IniFl = Refs.LP_CARXCar.GetWheel(WheelIndex.FrontLeft).maxSpringLen;
                Main.IniFr = Refs.LP_CARXCar.GetWheel(WheelIndex.FrontRight).maxSpringLen;
                Main.IniBL = Refs.LP_CARXCar.GetWheel(WheelIndex.RearLeft).maxSpringLen;
                Main.IniBR = Refs.LP_CARXCar.GetWheel(WheelIndex.RearRight).maxSpringLen;

                PlayerPrefs.SetFloat("IniFL", Main.IniFl);
                PlayerPrefs.SetFloat("IniFR", Main.IniFr);
                PlayerPrefs.SetFloat("IniBL", Main.IniBL);
                PlayerPrefs.SetFloat("IniBR", Main.IniBR);
                PlayerPrefs.Save();

                Debug.LogWarning($"[Getting Initial Suspension]: Storing Initial Suspension Values Now, FrontLeft: {Main.IniFl}f, FrontRight: {Main.IniFr}f, BackLeft: {Main.IniBL}f And BackRight: {Main.IniBR}f");
            }
            catch (Exception)
            {
                Debug.LogError("[Getting Initial Suspension]: Failed To Store Suspension Values, Could Not Find Wheel Index!");
            }
        }
    }
}
