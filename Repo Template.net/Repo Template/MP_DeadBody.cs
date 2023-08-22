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
    [CalloutProperties("Military Police Dead Body Callout", "Valandria", "0.0.1")]
    public class DeadBodyCallout : Callout
    {
        Ped body;
        private Vector3[] coordinates = 
        {
            new Vector3(),
        };

        public DeadBodyCallout()
        {

            InitInfo(coordinates[RandomUtils.Random.Next(coordinates.Length + 30)]);

            ShortName = "MP - Dead Body";
            CalloutDescription = "Someone has reported a dead body. Investigate immediately.";
            ResponseCode = 3;
            StartDistance = 200f;
        }
        public async override Task OnAccept()
        {
            InitBlip(20);
            UpdateData();
        }
        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            body = await SpawnPed(RandomUtils.GetRandomPed(), Location);
            body.Kill();


        }

    }
}
