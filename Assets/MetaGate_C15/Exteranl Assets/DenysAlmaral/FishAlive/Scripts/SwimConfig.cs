using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace FishAlive
{

    [CreateAssetMenu(fileName = "New Swim Config", menuName = "Swim Config")]
    public class SwimConfig : ScriptableObject
    {
        [Min(0)]
        [Tooltip("In seconds.")]
        public float TargetPingFrequency = 0.5f;
        [Min(0)]
        public float TargetPingPrecision = 0.3f;  //seconds    
        [Min(0)]
        public float TargetPositionPrecision = 0.4f;
        [Min(0)]
        public float TargetFar = 2;
        public float TargetClose = 0.5f;
        [Tooltip("Softly turning")]
        public AnimationCurve TurnEasing;
        [Tooltip("Softly bending")]
        public AnimationCurve BendEasing;
        [Range(0, 180)]
        [Tooltip("Which angle produce maximum bend")]
        public float MaxBendAtAngle = 90;
        [Min(0.1f)]
        public float TurningTimeTotal = 1.0f;
        [Min(0)]
        public float FastAcceleration = 3.0f;
        [Min(0)]
        public float NormalAcceleration = 1.0f;
        [Min(0)]
        public float SlowAcceleration = 0.2f;
        [Min(0)]
        public float LiquidDrag = 2f;

        public SwimConfig()
        {
            //You'll hate getting uninitialized curves.. so...
            Keyframe[] keys = new Keyframe[3];
            keys[0] = new Keyframe(0.0f, 0.0f, 0.0f, 0.0f);
            keys[1] = new Keyframe(0.4362354576587677f, 0.5005072355270386f, 2.776146173477173f, 2.776146173477173f);
            keys[2] = new Keyframe(1.0f, 1.0f, 0.0f, 0.0f);
            TurnEasing = new AnimationCurve(keys);

            Keyframe key1 = new Keyframe(-0.004464286f, 0f, 0f, 0.1697334f);
            Keyframe key2 = new Keyframe(0.2428975f, 0.9405391f, 1.279997f, 1.279997f);
            Keyframe key3 = new Keyframe(1f, 0f, 0.2333021f, 0f);

            BendEasing = new AnimationCurve(key1, key2, key3);
        }


    }
}