using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    public class SpeedControlButtonManager : ControlButtonManager
    {
        public static new SpeedControlButtonManager _instance;
        public static new SpeedControlButtonManager instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[SpeedControlButtonManager]", false);
            }
        }

        /**
         * Add a new control button into the scene
         */
        public SpeedControlButton AddControlButton(SpeedControlButtonAsset controlButtonAsset) {
            return AddControlButton<SpeedControlButton, SpeedControlButtonAsset>(controlButtonAsset, controlButtonPrefab);
        }
    }
}
