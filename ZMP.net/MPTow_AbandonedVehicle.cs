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
            new Vector3(-1582.39f, 3082.07f, 31.37f),
            new Vector3(-1591.7f, 3116.52f, 32.76f),
            new Vector3(-1591.9f, 3169.57f, 30.13f),
            new Vector3(-1625.31f, 3205.78f, 30.04f),
            new Vector3(-1638f, 3182.51f, 30.04f),
            new Vector3(-1616.81f, 3133.3f, 31.21f),
            new Vector3(-1669.42f, 3149.87f, 31.66f),
            new Vector3(-1669.42f, 3149.87f, 31.7f),
            new Vector3(-1661.86f, 3117.91f, 31.71f),
            new Vector3(-1662.6f, 3083.84f, 31.71f),
            new Vector3(-1671.92f, 3060.73f, 31.71f),
            new Vector3(-1645.69f, 3042.39f, 31.58f),
            new Vector3(-1603.62f, 3092.1f, 32.65f),
            new Vector3(-1632.66f, 2988.54f, 32.7f),
            new Vector3(-1609.99f, 2947.02f, 32.21f),
            new Vector3(-1622.98f, 2958.5f, 32.21f),
            new Vector3(-1535.05f, 2898.26f, 30.59f),
            new Vector3(-1472.8f, 2804.38f, 23.29f),
            new Vector3(-1491.53f, 2786.98f, 20.68f),
            new Vector3(-1482.1f, 2767.15f, 19.17f),
            new Vector3(-1411.85f, 2701.09f, 4.53f),
            new Vector3(-1417.62f, 2699.65f, 4.53f),
            new Vector3(-1445.2f, 2697.84f, 4.53f),
            new Vector3(-1466.05f, 2693.44f, 4.53f),
            new Vector3(-1480.09f, 2689.1f, 3.7f),
            new Vector3(-1470.57f, 2678.23f, 3.7f),
            new Vector3(-1509.96f, 2690.55f, 3.7f),
            new Vector3(-1528.27f, 2702.09f, 3.7f),
            new Vector3(-1532.07f, 2685.42f, 3.7f),
            new Vector3(-1530.65f, 2657.67f, 1.09f),
            new Vector3(-1492.88f, 2650.75f, 0.91f),
            new Vector3(-1442.79f, 2651.52f, 1.94f),
            new Vector3(-1577.58f, 2683f, 3.22f),
            new Vector3(-1611.17f, 2683.29f, 0.83f),
            new Vector3(-1623.2f, 2758.19f, 9.05f),
            new Vector3(-1654.36f, 2729.79f, 5.59f),
            new Vector3(-1655.82f, 2689.58f, 2.7f),
            new Vector3(-1575.28f, 2604.67f, 3.36f),
            new Vector3(-1614.26f, 2579.7f, 2.95f),
            new Vector3(-1714.96f, 2619.95f, 2.99f),
            new Vector3(-1826.3f, 2634.68f, 2.87f),
            new Vector3(-1789.78f, 2701.37f, 4.58f),
            new Vector3(-1848.22f, 2699.8f, 4.32f),
            new Vector3(-1881.72f, 2667.41f, 2.79f),
            new Vector3(-1904.88f, 2702.8f, 4.38f),
            new Vector3(-1937.89f, 2721.54f, 3.97f),
            new Vector3(-1980.36f, 2704.4f, 3.97f),
            new Vector3(-2029.16f, 2717.96f, 3.97f),
            new Vector3(-2067.6f, 2622.06f, 1.39f),
            new Vector3(-2086.21f, 2666.37f, 2.82f),
            new Vector3(-2141.43f, 2661.38f, 3.98f),
            new Vector3(-2216.73f, 2682.15f, 2.72f),
            new Vector3(-2157.14f, 2714.95f, 4.09f),
            new Vector3(-2196.88f, 2726.79f, 1.25f),
            new Vector3(-2299.25f, 2821.08f, 3.67f),
            new Vector3(-2341.78f, 2788.51f, 3.02f),
            new Vector3(-2392.45f, 2846.77f, 3.5f),
            new Vector3(-2417.5f, 2795.83f, 2.81f),
            new Vector3(-2453.99f, 2799.06f, 3.27f),
            new Vector3(-2502.06f, 2815.33f, 3.27f),
            new Vector3(-2555.57f, 2830.18f, 3.27f),
            new Vector3(-2597.58f, 2893.94f, 4.34f),  //Under Great Ocean Highway, need to continue West towards beach.
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
            new Vector3(),
        };
        public MilitaryAbaondonedVehicle()
        {
            InitInfo(coordinates[RandomUtils.Random.Next(coordinates.Length + 30)]);

            ShortName = "MP Tow - Abandoned Vehicle";
            CalloutDescription = "An abandoned vehicle has been spotted near the base, investigate and report back.";
            ResponseCode = 1;
            StartDistance = 200f;
        }

        public override async Task OnAccept()
        {
            InitBlip(25);

            var cars = new[]  //Separate var for surveillance van?
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
            World.ShootBullet(Location, car1.Position, Game.PlayerPed, WeaponHash.RayPistol, 0);  //Should be optional for actual abandoned vehicle, not surveillance van.
            World.ShootBullet(Location, car1.Position, Game.PlayerPed, WeaponHash.RayPistol, 0);  //Should be optional for actual abandoned vehicle, not surveillance van.
            car1.Deform(Location, 10000, 100);
            car1.EngineHealth = 5;
            car1.BodyHealth = 1;

            API.Wait(2);
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);

            car1.Deform(Location, 10000, 100);
            World.ShootBullet(Location, car1.Position, Game.PlayerPed, WeaponHash.RayPistol, 0);  //Should be optional for actual abandoned vehicle, not surveillance van.
        }
    }
}