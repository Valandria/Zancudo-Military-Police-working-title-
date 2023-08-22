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
    
    [CalloutProperties("Military Police Active Shooter", "Valandria", "0.0.1")]
    public class MilitaryActiveShooter : Callout
    {
        private Ped suspect, vic1, vic2, vic3, vic4, vic5;
        private Vector3[] coordinates =
        {
            new Vector3(),
        };
        public MilitaryActiveShooter()
        {
            InitInfo(coordinates[RandomUtils.Random.Next(coordinates.Length + 30)]);
            ShortName = "MP - Active Shooter";
            CalloutDescription = "Reports of an active shooter near the front gate, neutralize target and minimize casulties.";
            ResponseCode = 3;
            StartDistance = 300f;
        }
        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~y~Officer ~b~" + displayName + ",~y~ several reports of an active shooter have come in!");
            suspect = await SpawnPed(RandomUtils.GetRandomPed(), Location);
            vic1 = await SpawnPed(RandomUtils.GetRandomPed(), Location + 1);
            vic2 = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            vic3 = await SpawnPed(RandomUtils.GetRandomPed(), Location + 4);
            vic4 = await SpawnPed(RandomUtils.GetRandomPed(), Location - 5);
            vic5 = await SpawnPed(RandomUtils.GetRandomPed(), Location - 1);
            //Suspect 1
            PedData data = new PedData();
            List<Item> items = new List<Item>();
            data.BloodAlcoholLevel = 0.08;
            Item Rifle = new Item {
                Name = "Rifle",
                IsIllegal = true
            };
            items.Add(Rifle);
            data.Items = items;
            Utilities.SetPedData(suspect.NetworkId,data);
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
            suspect.AttachBlip();
            suspect.Weapons.Give(WeaponHash.MarksmanRifle, 1000, true, true);
            suspect.Accuracy = 50;
            suspect.RelationshipGroup = 0xCE133D78;
            suspect.Task.FightAgainstHatedTargets(this.StartDistance);
            PedQuestion question = new PedQuestion();
            question.Question = "What're you shoot at people for?";
            question.Answers = new List<string>
            {
                "Go to hell pig!",
                "Fuck you!",
                "You suck at aiming!",
                "Why didn't you kill me?",
                "*Silence*",
                "*Silence in Spanish*",
                "*Glares*",
                "*Stares*"
            };
            PedQuestion question2 = new PedQuestion();
            question2.Question = "What the hell is wrong with you?";
            question2.Answers = new List<string>
            {
                "Go to hell pig!",
                "Fuck you!",
                "You suck at aiming!",
                "Why didn't you kill me?",
                "*Silence*",
                "*Silence in Spanish*",
                "*Glares*",
                "*Stares*"
            };
            PedQuestion question3 = new PedQuestion();
            question3.Question = "Why did you do this?";
            question3.Answers = new List<string>
            {
                "Go to hell pig!",
                "Fuck you!",
                "You suck at aiming!",
                "Why didn't you kill me?",
                "*Silence*",
                "*Silence in Spanish*",
                "*Glares*",
                "*Stares*"
            };
            vic1.Kill();
            vic2.Kill();
            vic3.Kill();
            vic4.Kill();
            vic5.Kill();
            vic1.AttachBlip();
            vic2.AttachBlip();
            vic3.AttachBlip();
            vic4.AttachBlip();
            vic5.AttachBlip();
            PedData data1 = await Utilities.GetPedData(suspect.NetworkId);
            string firstname = data1.FirstName;
            DrawSubtitle("~r~[" + firstname + "] ~s~I knew this was coming... DIE!", 5000);
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