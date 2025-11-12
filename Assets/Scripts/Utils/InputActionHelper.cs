using UnityEngine;

namespace VampireSurvivor.Utils
{
    /// <summary>
    /// Helper for setting up Unity Input System
    /// Provides guidance for Input Actions configuration
    /// </summary>
    public static class InputActionHelper
    {
        /// <summary>
        /// Required Input Actions for the game:
        ///
        /// Action Map: "Player"
        /// - Move (Vector2, Gamepad: Left Stick, Touch: Virtual Joystick)
        /// - Aim (Vector2, Gamepad: Right Stick, Touch: Optional)
        /// - Fire (Button, not needed if auto-fire is on)
        /// - Dodge (Button, Gamepad: B/Circle, Touch: Button)
        /// - Ability1 (Button, Gamepad: X/Square, Touch: Button)
        /// - Ability2 (Button, Gamepad: Y/Triangle, Touch: Button)
        /// - Ability3 (Button, Gamepad: LB/L1, Touch: Button)
        /// - Ability4 (Button, Gamepad: RB/R1, Touch: Button)
        ///
        /// Action Map: "UI"
        /// - Navigate (Vector2)
        /// - Submit (Button)
        /// - Cancel (Button)
        /// - Pause (Button, Gamepad: Start/Options, Touch: Button)
        ///
        /// Mobile Considerations:
        /// - Use Unity's On-Screen Controls package for virtual joystick/buttons
        /// - Enable touch simulation in Input System settings
        /// - Consider swipe gestures for dodge
        /// </summary>

        public const string PLAYER_ACTION_MAP = "Player";
        public const string UI_ACTION_MAP = "UI";

        // Player Actions
        public const string MOVE_ACTION = "Move";
        public const string AIM_ACTION = "Aim";
        public const string FIRE_ACTION = "Fire";
        public const string DODGE_ACTION = "Dodge";
        public const string ABILITY1_ACTION = "Ability1";
        public const string ABILITY2_ACTION = "Ability2";
        public const string ABILITY3_ACTION = "Ability3";
        public const string ABILITY4_ACTION = "Ability4";

        // UI Actions
        public const string NAVIGATE_ACTION = "Navigate";
        public const string SUBMIT_ACTION = "Submit";
        public const string CANCEL_ACTION = "Cancel";
        public const string PAUSE_ACTION = "Pause";
    }
}
