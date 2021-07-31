using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorConstants
{
    public static class Parameters
    {
        public const string VelocityMultiplier = "VelocityMultiplier";
        public const string ForwardVelocity = "VelocityZ";
        public const string SideVelocity = "VelocityX";
        public const string Moving = "Moving";
        public const string TurnValue = "TurnValue";
        public const string Reloading = "Reloading";
        public const string Attacking = "Attacking";
    }
    public static class Layers
    {
        public const string UpperBody = "UpperBody Layer";
        public const string Attack = "Attack Additive Layer";
    }
}
