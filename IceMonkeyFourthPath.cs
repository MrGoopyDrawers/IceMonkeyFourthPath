using MelonLoader;
using BTD_Mod_Helper;
using IceMonkeyFourthPath;
using PathsPlusPlus;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api.Enums;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using JetBrains.Annotations;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppSystem.IO;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using Il2CppAssets.Scripts.Models.TowerSets;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using UnityEngine;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using System.Runtime.CompilerServices;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

[assembly: MelonInfo(typeof(IceMonkeyFourthPath.IceMonkeyFourthPath), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace IceMonkeyFourthPath;

public class IceMonkeyFourthPath : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<IceMonkeyFourthPath>("IceMonkeyFourthPath loaded!");
    }
    public class FourthPath2 : PathPlusPlus
    {
        public override string Tower => TowerType.IceMonkey;
        public override int UpgradeCount => 5;

    }
    public class SnowballThrower : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 550;
        public override int Tier => 1;
        public override string Icon => VanillaSprites.DartMonkeySnowballsIcon;

        public override string Description => "Throws snowballs very rapidly, and in a longer range.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var newAttackModel = Game.instance.model.GetTowerFromId("DartMonkey-001").GetAttackModel().Duplicate();
            newAttackModel.weapons[0].projectile.pierce = 1;
            newAttackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 10f;
            newAttackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.Frozen | BloonProperties.Lead | BloonProperties.White;
            newAttackModel.weapons[0].projectile.ApplyDisplay<snowball>();
            newAttackModel.weapons[0].projectile.scale *= 0.5f;
            newAttackModel.weapons[0].rate *= 0.25f;
            newAttackModel.name = "snowballLauncher";
            towerModel.AddBehavior(newAttackModel);
        }
    }
    public class IceCompromise : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 550;
        public override int Tier => 2;
        public override string Icon => VanillaSprites.SoColdIcon;

        public override string Description => "Snowballs can pop frozen bloons.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var attackModel in towerModel.GetAttackModels())
            {
                if (attackModel.name.Contains("snowballLauncher"))
                {
                    attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.Lead | BloonProperties.White;
                }

            }
        }
    }
    public class FreezingSnowballs : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 1550;
        public override int Tier => 3;
        public override string Icon => VanillaSprites.NinjaMonkeySnowflakesIcon;

        public override string Description => "Snowballs pop camo and lead bloons. They have more pierce and can freeze bloons.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var attackModel in towerModel.GetAttackModels())
            {
                if (attackModel.name.Contains("snowballLauncher"))
                {
                    attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.White;
                    attackModel.weapons[0].projectile.pierce += 2;
                    towerModel.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
                    attackModel.weapons[0].projectile.collisionPasses = new int[] { -1, 0, 1 };
                    attackModel.weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("IceMonkey-320").GetAttackModel().weapons[0].projectile.GetBehavior<FreezeModel>().Duplicate());
                    attackModel.weapons[0].projectile.GetBehavior<FreezeModel>().lifespan *= 0.1f;
                }

            }
        }
    }
    public class SnowGunner : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 5000;
        public override int Tier => 4;
        public override string Icon => "snowballLauncher";

        public override string Description => "Rapidly shoots snowballs in a larger range. They can freeze MOAB class.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var attackModel in towerModel.GetAttackModels())
            {
                if (attackModel.name.Contains("snowballLauncher"))
                {
                    attackModel.weapons[0].projectile.collisionPasses = new int[] { -1, 0, 1 };
                    attackModel.weapons[0].rate *= 0.2f;
                    attackModel.range += 30;
                    attackModel.weapons[0].projectile.GetDamageModel().damage += 3;
                    attackModel.weapons[0].projectile.GetBehavior<FreezeModel>().canFreezeMoabs = true;
                }

            }
        }
    }
    public class Snowman : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 50000;
        public override int Tier => 5;
        public override string Icon => VanillaSprites.MortarSnowExplosionIcon;

        public override string Description => "Snowballs launch so fast they get more pierce and damage. Snowball Storm ability - Sends alot of snowballs at once, all around the track.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var attackModel in towerModel.GetAttackModels())
            {
                if (attackModel.name.Contains("snowballLauncher"))
                {
                    attackModel.weapons[0].projectile.collisionPasses = new int[] { -1, 0, 1 };
                    attackModel.weapons[0].rate *= 0.6f;
                    attackModel.weapons[0].projectile.GetDamageModel().damage += 10;
                    attackModel.weapons[0].projectile.pierce += 4;
                    towerModel.AddBehavior(Game.instance.model.GetTowerFromId("TackShooter-050").GetBehavior<AbilityModel>().Duplicate());
                    towerModel.GetBehavior<AbilityModel>().icon = GetSpriteReference(mod, "snowballLauncher");
                    towerModel.GetAbility().name = "Snowball Storm";
                    var activateAttack = towerModel.GetBehavior<AbilityModel>().GetBehavior<ActivateAttackModel>();
                    activateAttack.attacks[0].weapons[0].projectile = attackModel.weapons[0].projectile.Duplicate();
                }

            }
        }
    }
    public class snowball : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "snowball");
        }
    }
}