using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;
using CitizenFX.Core.Native;


namespace ZancudoMilitaryPolice
{
    [CalloutProperties("Military Police Rogue Soldier", "Valandria", "0.0.2")]
    public class RogueSoldier : Callout
    {
        Ped MPRSPed;
        private Vector3[] coordinates = {
            new Vector3(-2156.1f, 3240.397f, 32.81042f),
            new Vector3(-1963.363f, 3135.539f, 32.81038f),
            new Vector3(-1952.621f, 2986.537f, 32.81018f),
            new Vector3(-1809.317f, 2878.269f, 32.80949f),
            new Vector3(-1736.669f, 2898.073f, 32.8085f),
            new Vector3(-1825.416f, 2943.042f, 33.15864f),
            new Vector3(-2395.029f, 2989.533f, 32.86604f),
            new Vector3(-2449.016f, 2963.492f, 32.8145f),
            new Vector3(-2482.605f, 2934.713f, 32.81105f),
        };

        public RogueSoldier()
        {
            InitInfo(coordinates[RandomUtils.Random.Next(coordinates.Length + 30)]);
            ShortName = "MP - Rogue Military Soldier";
            CalloutDescription = "A soldier has reportedly gone AWOL near the armory, neutralize target and minimize casulties.";
            ResponseCode = 3;
            StartDistance = 100f;
        }

        public override async Task OnAccept()
        {
            var suspects = new[]
          {
               PedHash.MilitaryBum,
               PedHash.Armymech01SMY,
               PedHash.ExArmy01,
               PedHash.Armoured01,
               PedHash.Pilot01SMM,
               PedHash.Pilot01SMY,
               PedHash.Pilot02SMM
           };

            var guns = new[]
          {
               WeaponHash.RPG,
               WeaponHash.HeavySniperMk2,
               WeaponHash.HomingLauncher,
               WeaponHash.Minigun,
               WeaponHash.Railgun,
           };

            base.InitBlip(25);
            MPRSPed = await SpawnPed(suspects[RandomUtils.Random.Next(suspects.Length)], Location + 20);
            MPRSPed.Weapons.Give(guns[RandomUtils.Random.Next(guns.Length)], 9999, true, true);
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            MPRSPed.Accuracy = 80;
            MPRSPed.FiringPattern = FiringPattern.FullAuto;
            MPRSPed.ShootRate = 1000;
            MPRSPed.RelationshipGroup = 0xCE133D78;
            MPRSPed.Task.FightAgainstHatedTargets(this.StartDistance);
            MPRSPed.ArmorFloat = 1000;
            MPRSPed.Armor = 1000;
            ShowNetworkedNotification("The suspect should be considered armed and dangerous with heavy weaponry.", "CHAR_CALL911", "CHAR_CALL911", "Dispatch", "", 15f);
            ShowNetworkedNotification("Clear the area before making contact.", "CHAR_CALL911", "CHAR_CALL911", "Dispatch", "", 15f);
        }
    }
}
