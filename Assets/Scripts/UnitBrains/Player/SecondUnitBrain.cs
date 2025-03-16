﻿using System.Collections.Generic;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> dangerTargets = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            float temperature = GetTemperature();

            if (temperature >= overheatTemperature)
            {
                return;
            }

            for (int i = 0; i <= temperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            IncreaseTemperature();
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           

            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            if (dangerTargets.Count == 0)
            {
                return unit.Pos;
            }
            return unit.Pos.CalcNextStepTowards(dangerTargets[0]);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            var result = new List<Vector2Int>();
            var targets = GetAllTargets();
            dangerTargets.Clear();


            var closestTarget = new Vector2Int();
            var minDistance = float.MaxValue;

            foreach (var target in targets)
            {
                float distance = DistanceToOwnBase(target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }

            }

            if (minDistance < float.MaxValue)
            {
                if (IsTargetInRange(closestTarget))
                {
                    result.Add(closestTarget);
                }
                else
                {
                    dangerTargets.Add(closestTarget);
                }
            }

            if (result.Count == 0)
            {
                var enemyBase = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.PlayerId : RuntimeModel.BotPlayerId];
                if (IsTargetInRange(enemyBase))
                {
                    result.Add(enemyBase);
                }
                else
                {
                    dangerTargets.Add(enemyBase);
                }
            }


            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}