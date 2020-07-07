using UnityEngine;

namespace EVRC
{
    using static KeyboardInterface;
    using CockpitMode = CockpitUIMode.CockpitMode;

    /**
     * A button type that uses a ControlButtonAsset and outputs keyboard commands
     * to control ED.
     */
    public class SpeedControlButton : ControlButton
    {
        public short throttleValue;
        private static VirtualThrottle throttle;
        protected override void OnEnable()
        {
            base.OnEnable();
            if(controlButtonAsset is SpeedControlButtonAsset) {
                throttleValue = (controlButtonAsset as SpeedControlButtonAsset).throttleSpeed;
                Debug.Log("Speedctrl at" + throttleValue);
            } else {
                Debug.Log("Not a speedControl");
            }
            CockpitUIMode.ModeChanged.Listen(OnCockpitUIModeChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CockpitUIMode.ModeChanged.Remove(OnCockpitUIModeChanged);
        }

        private void OnCockpitUIModeChanged(CockpitMode mode) {
            throttle = FindObjectOfType<VirtualThrottle>();
            if(throttle) {
                Debug.Log("throttle found");
            } else {
                Debug.Log("throttle not found");
            }
        }

        public override bool IsValid()
        {
            //if has throttle... then
            return base.IsValid();
        }

        protected override Unpress Activate()
        {
            Debug.Log("HEYO");
                        if(throttle) {
                Debug.Log("throttle found");
            } else {
                Debug.Log("throttle not found");
            }
            var control = controlButtonAsset.GetControl();
            KeyCombo? defaultKeycombo = controlButtonAsset.GetDefaultKeycombo();
            var unpress = CallbackPress(EDControlBindings.GetControlButton(control, defaultKeycombo));
            throttle?.SetHandle(throttleValue);
            return () => unpress();
        }
    }
}
