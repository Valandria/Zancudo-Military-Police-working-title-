using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;

namespace ZancudoMilitaryPolice
{
    [CalloutProperties("Military Police Drunk Person", "Valandria", "0.0.1")]
    public class NC_DrunkCallout : Callout
    {
        private Ped suspect, suspect2;
        private Vector3[] coordinates =
        {
            new Vector3(),
        };

        public NC_DrunkCallout()
        {
            InitInfo(coordinates[RandomUtils.Random.Next(coordinates.Length + 30)]);
            ResponseCode = 2;
            StartDistance = 200f;
        }
        
        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            suspect = await SpawnPed(RandomUtils.GetRandomPed(), Location);
            suspect2 = await SpawnPed(RandomUtils.GetRandomPed(), Location);

            //Suspect Data
            PedData data = new PedData();
            List<Item> items = new List<Item>();
            data.BloodAlcoholLevel = 0.25;
            Item SixPack = new Item {
                Name = "Six Pack",
                IsIllegal = false
            };
            items.Add(SixPack);
            data.Items = items;
            Utilities.SetPedData(suspect.NetworkId,data);
            PedQuestion spq1 = new PedQuestion();
            spq1.Question = "What's going on?";
            spq1.Answers = new List<string>
            {
                "Just out drinking with my buddy.",
                "Just enjoying the moment.",
                "Just hanging around with my broskies over brewskies.",
                "Just enjoying brewskies.",
                "What's it look like?"
            };
            AddPedQuestion(suspect, spq1);
            PedQuestion spq2 = new PedQuestion();
            spq2.Question = "How much have you had to drink?";
            spq2.Answers = new List<string>
            {
                "Just a six pack, this is our second.",
                "We've had a little bit, maybe a tall boy.",
                "We just started drinking.",
                "A lot.",
                "Probably too much.",
                "A few shots and a beer.",
                ""
            };
            AddPedQuestion(suspect,spq2);
            PedQuestion spq3 = new PedQuestion();
            spq3.Question = "We have reports of you messing with people.";
            spq3.Answers = new List<string>
            {
                "That's a lie, I got pushed so I shoved back.",
                "Oh they're just a bunch of wussies.",
                "Snowflakes don't know what a shove is.",
                "I was just making my way through a crowd.",
                "They need better balance."
            };
            AddPedQuestion(suspect,spq3);

            //Suspect2 Data
            PedData data2 = new PedData();
            List<Item> items2 = new List<Item>();
            data2.BloodAlcoholLevel = 0.18;
            Item Beer = new Item {
                Name = "Beer",
                IsIllegal = false
            };
            items.Add(Beer);
            data2.Items = items2;
            Utilities.SetPedData(suspect2.NetworkId,data2);
            AddPedQuestion(suspect2, spq1);
            AddPedQuestion(suspect2, spq2);
            AddPedQuestion(suspect2, spq3);

            //Tasks
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
            suspect2.AlwaysKeepTask = true;
            suspect2.BlockPermanentEvents = true;
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[Northern Command] ~y~Officer ~b~" + displayName + ",~y~ the suspects have been reported");
            Notify("~y~to be causing issues with other people and falling down!");
            API.SetPedIsDrunk(suspect.GetHashCode(), true);
            API.SetPedIsDrunk(suspect2.GetHashCode(), true);
            suspect.Task.WanderAround();
            suspect2.Task.WanderAround();
            suspect.AttachBlip();
            suspect2.AttachBlip();
            PedData data1 = await Utilities.GetPedData(suspect.NetworkId);
            string firstname = data1.FirstName;
            DrawSubtitle("~r~[" + firstname + "] ~s~Can I have a beer?", 5000);
            PedData data5 = await Utilities.GetPedData(suspect2.NetworkId);
            string firstname2 = data5.FirstName;
            DrawSubtitle("~r~[" + firstname + "] ~s~SURE!", 5000);
            suspect.Task.FleeFrom(player);
        }

        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
        }
        private void Notify(string message)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandThefeedPostTicker(false, true);
        }
        private void DrawSubtitle(string message, int duration)
        {
            API.BeginTextCommandPrint("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandPrint(duration, false);
        }
    }
}