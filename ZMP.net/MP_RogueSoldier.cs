using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ZancudoMilitaryPolice
{
    [CalloutProperties("Military Police Rogue Soldier", "Valandria", "0.0.1")]
    public class RogueSoldier : Callout
    {
        public JObject GetJsonData()
        {
            string mprspath = "/callouts/VRRC/VRRCConfig.json";
            string mprsdata = API.LoadResourceFile(API.GetCurrentResourceName(), mprspath);
            JObject mprsjsonData = JObject.Parse(mprsdata);

            foreach (var mprsDepartment in mprsjsonData["StolenMilitaryVehicle-Department"])
            {
                int.TryParse((string)mprsDepartment[0], out int mprsdeptID);
                _assignedDeptarments.Add(mprsdeptID);
            }

            List<Vector3> mprscoords = new List<Vector3>();
            foreach (var mprscoordinate in mprsjsonData["RogueSoldier-Coordinates"])
            {
                mprscoords.Add(JsonConvert.DeserializeObject<Vector3>(mprscoordinate.ToString()));
            }
            _mprscoordinates = mprscoords.SelectRandom();

            Dictionary<string, PedHash> mprsRogueSoliderHashes = new Dictionary<string, PedHash>();
            string[] mprsRogueSoliderJSON = JsonConvert.DeserializeObject<string[]>(mprsjsonData["RogueSoldier-RogueSoldier"].ToString());
            foreach (string mprsRogueSoliderhash in mprsRogueSoliderJSON)
            {
                int mprsRogueSoliderhashKey = API.GetHashKey(mprsRogueSoliderhash);

                mprsRogueSoliderHashes.Add(mprsRogueSoliderhash, (PedHash)mprsRogueSoliderhashKey);
            }
            _mprsRogueSoliderHash = mprsRogueSoliderHashes.SelectRandom().Value;

            Dictionary<string, WeaponHash> mprsweaponHashes = new Dictionary<string, WeaponHash>();
            string[] mprsweaponJSON = JsonConvert.DeserializeObject<string[]>(mprsjsonData["RogueSoldier-Weapons"].ToString());
            foreach (string mprsweaponhash in mprsweaponJSON)
            {
                int mprsweaponhashKey = API.GetHashKey(mprsweaponhash);

                mprsweaponHashes.Add(mprsweaponhash, (WeaponHash)mprsweaponhashKey);
            }
            _mprsweaponHash = mprsweaponHashes.SelectRandom().Value;

            //foreach (var mprsquestionJSON in mprsjsonData["RogueSoldier-Dialogue"])
            //{
            //    string question = mprsquestionJSON["Question"].ToString();
            //    string[] answers = JsonConvert.DeserializeObject<string[]>(mprsquestionJSON["Answers"].ToString());

            //    PedQuestion mprspq = new PedQuestion();
            //    mprspq.Question = question;
            //    mprspq.Answers = answers.ToList();

            //    _mprspedQuestions.Add(mprspq);
            //}

            return mprsjsonData;
        }

        public override async Task<bool> CheckRequirements()
        {
            var mprsplayerDept = Utilities.GetPlayerData().DepartmentID;
            if (_assignedDeptarments.Contains(mprsplayerDept))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<int> _assignedDeptarments = new List<int>();
        private Vector3 _mprscoordinates;
        private PedHash _mprsRogueSoliderHash;
        private WeaponHash _mprsweaponHash;

        //private PedData _mprsRogueSoldierData;

        //private List<PedQuestion> _mprspedQuestions = new List<PedQuestion>();

        Ped MPRSRogueSoldier;

        public RogueSoldier()
        {
            _ = GetJsonData();
            InitInfo(_mprscoordinates.Around(100f));
            ShortName = "MP - Reports of Armory Theft";
            CalloutDescription = "Armory Post reports a break in and the suspect was spotted, secure the area.";
            ResponseCode = 3;
            StartDistance = 1000f;
        }

        public override async Task OnAccept()
        {
            InitBlip(50);
        }

        public async override void OnStart(Ped player)
        {
            OnStart(player);

            MPRSRogueSoldier = await SpawnPed(_mprsRogueSoliderHash, _mprscoordinates.Around(30f));
            MPRSRogueSoldier.IsPersistent = true;
            MPRSRogueSoldier.AlwaysKeepTask = true;

            MPRSRogueSoldier.Weapons.Give(_mprsweaponHash, 9999, true, true);
            MPRSRogueSoldier.Accuracy = 80;
            MPRSRogueSoldier.FiringPattern = FiringPattern.FullAuto;
            MPRSRogueSoldier.ShootRate = 1000;
            MPRSRogueSoldier.RelationshipGroup = 0xCE133D78;
            MPRSRogueSoldier.Task.FightAgainstHatedTargets(StartDistance);
            MPRSRogueSoldier.ArmorFloat = 10000;
            MPRSRogueSoldier.Armor = 10000;
        }
    }
}
