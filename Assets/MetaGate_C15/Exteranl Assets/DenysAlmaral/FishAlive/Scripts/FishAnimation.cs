using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace FishAlive
{

    public class FishAnimation : MonoBehaviour
    {
        private const float FacingCosAng = 0.5f;
        private const float VerticalPrecisionMul = 0.3f;
        public GameObject Target;
        public SwimConfig Config;
        [SerializeField] private Animator _animator;
        //[SerializeField] private AnimationCurve TurnEasing;
        //[SerializeField] private AnimationCurve BendEasing;

        private Vector3 _targetPos;
        //private float Config.TargetPingFrequency = 0.5f;
        private float TargetPositionPrecision = 0.4f; //meters
        private float TargetPingPrecision = 0.3f;  //seconds
        private float _targetPingTimer = 0;


        private int swimSpeedParam;
        private int bendParam;
        private float acceleration;
        private float finalAcceleration;
        private float accelFadeInSpeed = 20.0f;
        private bool turning = false;
        private Vector3 _turnTarget;
        private float speed;
        private Transform _transform;
        private Quaternion _turnTargetRotation;
        private Quaternion _turnStartRotation;
        private float _turnAngleTotal;
        private float _turnTimeTotal;
        private float _turningTime;
        private float _turningSide;
        private float _currTurningAngle;

        private void Awake()
        {
            _transform = transform;
            swimSpeedParam = Animator.StringToHash("swimSpeed");
            bendParam = Animator.StringToHash("turn");
            if (!Config)
            {
                Debug.LogError("A reference to Scriptable Object instance of SwimConfig is needed.", gameObject);
            }
            if (!_animator)
            {
                if (!TryGetComponent<Animator>(out _animator))
                {
                    Debug.LogError("The Animator component is missing.", gameObject);
                }
            }
        }

        void Start()
        {
            _animator.SetFloat(swimSpeedParam, 1f);
            _animator.SetFloat(bendParam, 0.5f);
            _animator.CrossFade(Animator.StringToHash("Swim"), 0, 0, Random.value);


            acceleration = 0f;
            finalAcceleration = Config.NormalAcceleration;
            speed = 0;

            PingTarget();
            //StartTurnTowards();
            turning = false;
        }


        public void OnGUI()
        {
            //string s = acceleration.ToString();
            //GUI.Label(new Rect(25, 25, 100, 30), s);
            //GUI.Label(new Rect(25, 45, 100, 50), Time.deltaTime.ToString());
        }

        void OnDrawGizmos()
        {
            //Gizmos.DrawSphere(_targetPos, 0.02f); 

        }

        private void SetAnimationSwimSpeed(float speed)
        {
            //Converting the real speed to a value between 0-1 to fit adecuately in the animation...
            //that's why some values here are magically hard-coded as a result of trial and error
            //TODO: use AnimationCurve later maybe
            speed *= 2f;
            speed += acceleration / 4;
            if (speed <= 0)
            {
                _animator.speed = 1.0f;
                _animator.SetFloat(swimSpeedParam, 0);
            }
            else if (speed >= 1.0f)
            {
                _animator.SetFloat(swimSpeedParam, 1);
                _animator.speed = speed;
            }
            else
            {
                _animator.speed = 1.0f;
                _animator.SetFloat(swimSpeedParam, speed);
            }

        }

        private float ApplySoftAcceleration(float dt, float accel)
        {
            float step = accelFadeInSpeed * dt;
            if (accel < finalAcceleration - step)
            {
                accel += step;
            }
            else if (accel > finalAcceleration + step)
            {
                accel -= step;
            }
            else
            {
                accel = finalAcceleration;
            }
            return accel;
        }

        void PingTarget()
        {
            if (Target)
            {
                Vector3 rng = Random.insideUnitSphere * TargetPositionPrecision;
                rng.y *= VerticalPrecisionMul;
                _targetPos = Target.transform.position + rng;
                StartTurnTowards(_targetPos);
                var toTarget = _targetPos - _transform.position;
                var d = toTarget.magnitude;
                var facingTarget = Vector3.Dot(toTarget, _transform.forward) > FacingCosAng;
                if ((d > Config.TargetFar))
                {
                    //go fast only if facing target;
                    if (facingTarget) finalAcceleration = Config.FastAcceleration;

                }
                else if (d > Config.TargetClose)
                {
                    finalAcceleration = Config.NormalAcceleration;
                }
                else
                {
                    finalAcceleration = Config.SlowAcceleration;
                }
            }
        }

        void SetBendAnimation(float t)
        {
            float v = Config.BendEasing.Evaluate(t);
            v = v * (_turnAngleTotal / Config.MaxBendAtAngle); //how much bend, afected by the total angle
            if (v > 1.0f) v = 1.0f;
            //easing result is from 0 - 1, we need it from
            //0 - 0.5 if turning left,
            //0.5 - 1 if turning right        
            v = v / 2;
            if (_turningSide > 0) v = 0.5f + v;
            else if (_turningSide < 0) v = 0.5f - v;
            else v = 0.5f;

            _animator.SetFloat(bendParam, v);
        }

        void Motion(float dt)
        {
            //directional motion
            acceleration = ApplySoftAcceleration(dt, acceleration);
            speed += acceleration * dt;
            speed -= Config.LiquidDrag * speed * dt;
            if (speed < 0) speed = 0;
            Vector3 dir = _transform.forward;
            _transform.position += dir * speed * dt;

            //turning towards target
            if (turning)
            {
                _turningTime = _turningTime + dt;
                var normalizedTime = (_turningTime / _turnTimeTotal);
                _currTurningAngle = Config.TurnEasing.Evaluate(normalizedTime) * _turnAngleTotal;
                var turn = Quaternion.RotateTowards(_turnStartRotation, _turnTargetRotation, _currTurningAngle);
                _transform.rotation = turn;
                if (_turningTime >= _turnTimeTotal)
                {
                    turning = false;
                    _turningSide = 0;
                }
                SetBendAnimation(normalizedTime);

            }

            SetAnimationSwimSpeed(speed);
        }

        void Timers(float dt)
        {
            _targetPingTimer -= dt;
            if (_targetPingTimer < 0)
            {
                PingTarget();
                _targetPingTimer = Config.TargetPingFrequency + Random.value * TargetPingPrecision;
            }
        }

        void StartTurnTowards(Vector3 target)
        {
            if (!turning)
            {
                turning = true;
                var lookat = target - _transform.position;
                if (lookat.sqrMagnitude > 0)
                {
                    lookat.Normalize();
                    _turnTargetRotation = Quaternion.LookRotation(lookat);
                    _turnStartRotation = _transform.rotation;
                    _turnAngleTotal = Quaternion.Angle(_turnStartRotation, _turnTargetRotation);
                    //_turnTimeTotal = _turnAngleTotal / maxTurningSpeed;
                    _turnTimeTotal = Config.TurningTimeTotal;
                    _turningTime = 0.0f;

                    var cross = Vector3.Cross(_transform.forward, lookat);
                    _turningSide = Vector3.Dot(cross, Vector3.up);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            Motion(Time.deltaTime);
            Timers(Time.deltaTime);
        }
    }

}