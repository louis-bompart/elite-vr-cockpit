﻿using UnityEngine;

namespace EVRC
{
    using EDControlButton = EDControlBindings.EDControlButton;

    /**
     * A control button asset with static text and textures
     */
    [CreateAssetMenu(fileName = "SpeedControlButtonAsset", menuName = "EVRC/ControlButtonAssets/SpeedControlButtonAsset", order = 3)]
    public class SpeedControlButtonAsset : SimpleControlButtonAsset
    {
        public short throttleSpeed;
        // public string text;
        // public Texture texture;
        // public EDControlButton control;
        // public string defaultKeycombo;

        // public override string GetText()
        // {
        //     return text;
        // }

        // public override Texture GetTexture()
        // {
        //     return texture;
        // }

        // public override EDControlButton GetControl()
        // {
        //     return control;
        // }

        // public override KeyboardInterface.KeyCombo? GetDefaultKeycombo()
        // {
        //     return ParseKeycomboString(defaultKeycombo);
        // }
    }
}
