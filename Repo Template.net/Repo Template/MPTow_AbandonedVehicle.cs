using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;
using CitizenFX.Core.Native;

namespace ZancudoMilitaryPolice

{

    [CalloutProperties("Military Police Tow Abandoned Vehicle", "Valandria", "0.0.1")]
    public class MilitaryAbaondonedVehicle : FivePD.API.Callout
    {
        private Vehicle car1;
        private Vector3[] coordinates =
        {
            new Vector3(),
        };
        public MilitaryAbaondonedVehicle()
        {
            InitInfo(coordinates[RandomUtils.Random.Next(coordinates.Length + 30)]);

            ShortName = "MP Tow - Abandoned Vehicle";
            CalloutDescription = "An abandoned vehicle has been spotted, investigate and report back.";
            ResponseCode = 1;
            StartDistance = 200f;
        }

        public override async Task OnAccept()
        {
            InitBlip(25);

            var cars = new[]
              {
                   VehicleHash.Adder,
                   VehicleHash.CarbonRS,
                   VehicleHash.Oracle,
                   VehicleHash.Oracle2,
                   VehicleHash.Phoenix,
                   VehicleHash.Vigero,
                   VehicleHash.Zentorno,
                   VehicleHash.Youga2,
                   VehicleHash.Youga,
                   VehicleHash.Sultan,
                   VehicleHash.SultanRS,
                   VehicleHash.Sentinel,
                   VehicleHash.Sentinel2,
                   VehicleHash.Ruiner,
                   VehicleHash.Ruiner2,
                   VehicleHash.Ruiner3,
                   VehicleHash.Burrito,
                   VehicleHash.Burrito2,
                   VehicleHash.Burrito3,
                   VehicleHash.GBurrito,
                   VehicleHash.Bagger,
                   VehicleHash.Buffalo,
                   VehicleHash.Buffalo2,
                   VehicleHash.Comet2,
                   VehicleHash.Comet3,
                   VehicleHash.Felon,
               };

            car1 = await SpawnVehicle(cars[RandomUtils.Random.Next(cars.Length)], Location, 180);
            World.ShootBullet(Location, car1.Position, Game.PlayerPed, WeaponHash.RayPistol, 0);
            World.ShootBullet(Location, car1.Position, Game.PlayerPed, WeaponHash.RayPistol, 0);
            car1.Deform(Location, 10000, 100);
            car1.EngineHealth = 5;
            car1.BodyHealth = 1;

            API.Wait(2);
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);

            car1.Deform(Location, 10000, 100);
            World.ShootBullet(Location, car1.Position, Game.PlayerPed, WeaponHash.RayPistol, 0);
        }
    }
}