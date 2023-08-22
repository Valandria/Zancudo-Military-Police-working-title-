using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;

namespace ZancudoMilitaryPolice
{

    [CalloutProperties("Military Police Heavy Active Shooters", "Valandria", "0.0.1")]
    public class ActiveHeavyShooters : Callout
    {
        Ped MPHASped1, MPHASped2, MPHASped3, MPHASped4;
        private Vector3[] coordinates =
        {
            new Vector3(),
        };

        public ActiveHeavyShooters()
        {
            InitInfo(coordinates[RandomUtils.Random.Next(coordinates.Length + 30)]);

            ShortName = "MP - Heavily Weaponised Active Shooters";
            CalloutDescription = "Several soldiers have gone AWOL near the armory, neutralize all targets and minimize casulties.";
            ResponseCode = 3;
            StartDistance = 200f;
        }

        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            MPHASped1 = await SpawnPed(RandomUtils.GetRandomPed(), Location);
            MPHASped2 = await SpawnPed(RandomUtils.GetRandomPed(), Vector3Extension.Around(MPHASped1.Position, 5f));
            MPHASped3 = await SpawnPed(RandomUtils.GetRandomPed(), Vector3Extension.Around(MPHASped1.Position, 5f));
            MPHASped4 = await SpawnPed(RandomUtils.GetRandomPed(), Vector3Extension.Around(MPHASped1.Position, 6f));
            MPHASped1.AlwaysKeepTask = true;
            MPHASped1.BlockPermanentEvents = true;
            MPHASped2.AlwaysKeepTask = true;
            MPHASped2.BlockPermanentEvents = true;
            MPHASped3.AlwaysKeepTask = true;
            MPHASped3.BlockPermanentEvents = true;
            MPHASped4.AlwaysKeepTask = true;
            MPHASped4.BlockPermanentEvents = true;
            MPHASped1.IsPersistent = true;
            MPHASped2.IsPersistent = true;
            MPHASped3.IsPersistent = true;
            MPHASped4.IsPersistent = true;
            MPHASped1.RelationshipGroup = 0xCE133D78;
            MPHASped1.Task.FightAgainstHatedTargets(this.StartDistance);
            MPHASped2.RelationshipGroup = 0xCE133D78;
            MPHASped2.Task.FightAgainstHatedTargets(this.StartDistance);
            MPHASped3.RelationshipGroup = 0xCE133D78;
            MPHASped3.Task.FightAgainstHatedTargets(this.StartDistance);
            MPHASped4.RelationshipGroup = 0xCE133D78;
            MPHASped4.Task.FightAgainstHatedTargets(this.StartDistance);
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            
            MPHASped1.Weapons.Give(WeaponHash.Minigun, 9999, true, true);
            MPHASped1.Task.ShootAt(Game.PlayerPed);
            MPHASped2.Weapons.Give(WeaponHash.MarksmanRifleMk2, 9999, true, true);
            MPHASped2.Task.ShootAt(Game.PlayerPed);
            MPHASped3.Weapons.Give(WeaponHash.CombatMGMk2, 9999, true, true);
            MPHASped3.Task.ShootAt(Game.PlayerPed);
            MPHASped4.Weapons.Give(WeaponHash.AssaultRifleMk2, 9999, true, true);
            MPHASped4.Task.ShootAt(Game.PlayerPed);

            MPHASped1.Armor = 7000;
            MPHASped2.Armor = 7000;
            MPHASped3.Armor = 7000;
            MPHASped4.Armor = 7000;

            MPHASped1.Accuracy = 40;
            MPHASped2.Accuracy = 60;
            MPHASped3.Accuracy = 70;
            MPHASped4.Accuracy = 70;

            MPHASped1.ShootRate = 1000;
            MPHASped1.FiringPattern = FiringPattern.FullAuto;

            MPHASped1.AttachBlip();
            MPHASped2.AttachBlip();
            MPHASped3.AttachBlip();
            MPHASped4.AttachBlip();
        }
    }
}
