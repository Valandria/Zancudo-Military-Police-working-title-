using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZancudoMilitaryPolice
{
    [CalloutProperties("Military Police Drinking on Base", "Valandria", "0.0.1")]
    public class DrinkingonBase : Callout
    {
        public JObject GetJsonData()
        {
            string mpdobpath = "/callouts/VRRC/VRRCConfig.json";
            string mpdobdata = API.LoadResourceFile(API.GetCurrentResourceName(), mpdobpath);
            JObject mpdobjsonData = JObject.Parse(mpdobdata);

            foreach (var mpdobDepartment in mpdobjsonData["DrinkingonBase-Department"])
            {
                int.TryParse((string)mpdobDepartment[0], out int mpdobdeptID);
                _assignedDeptarments.Add(mpdobdeptID);
            }

            List<Vector3> mpdobcoords = new List<Vector3>();
            foreach (var mpdobcoordinate in mpdobjsonData["DrinkingonBase-Coordinates"])
            {
                mpdobcoords.Add(JsonConvert.DeserializeObject<Vector3>(mpdobcoordinate.ToString()));
            }
            _mpdobcoordinates = mpdobcoords.SelectRandom();

            Dictionary<string, PedHash> mpdobPrimaryHashes = new Dictionary<string, PedHash>();
            string[] mpdobPrimaryJSON = JsonConvert.DeserializeObject<string[]>(mpdobjsonData["DrinkingonBase-Primary"].ToString());
            foreach (string mpdobPrimaryhash in mpdobPrimaryJSON)
            {
                int mpdobPrimaryhashKey = API.GetHashKey(mpdobPrimaryhash);
                mpdobPrimaryHashes.Add(mpdobPrimaryhash, (PedHash)mpdobPrimaryhashKey);
            }
            _mpdobPrimaryHash = mpdobPrimaryHashes.SelectRandom().Value;

            Dictionary<string, PedHash> mpdobSecondaryHashes = new Dictionary<string, PedHash>();
            string[] mpdobSecondaryJSON = JsonConvert.DeserializeObject<string[]>(mpdobjsonData["DrinkingonBase-Secondary"].ToString());
            foreach (string mpdobSecondaryhash in mpdobSecondaryJSON)
            {
                int mpdobPrimaryhashKey = API.GetHashKey(mpdobSecondaryhash);
                mpdobPrimaryHashes.Add(mpdobSecondaryhash, (PedHash)mpdobPrimaryhashKey);
            }
            _mpdobSecondaryHash = mpdobSecondaryHashes.SelectRandom().Value;

            Dictionary<string, VehicleHash> mpdobvehicleHashes = new Dictionary<string, VehicleHash>();
            string[] mpdobvehicleJSON = JsonConvert.DeserializeObject<string[]>(mpdobjsonData["ZMP-Vehicles"].ToString());
            foreach (string mpdobhash in mpdobvehicleJSON)
            {
                int hashKey = API.GetHashKey(mpdobhash);
                mpdobvehicleHashes.Add(mpdobhash, (VehicleHash)hashKey);
            }
            _mpdobvehicleHash = mpdobvehicleHashes.SelectRandom().Value;

            return mpdobjsonData;
        }

        public override async Task<bool> CheckRequirements()
        {
            var mpdobplayerDept = Utilities.GetPlayerData().DepartmentID;
            if (_assignedDeptarments.Contains(mpdobplayerDept))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<int> _assignedDeptarments = new List<int>();
        private Vector3 _mpdobcoordinates;
        private Vector3 FinalDrivingLocation = new Vector3(2009.56f, 3062.47f, 47.05f);
        private PedHash _mpdobPrimaryHash;
        private PedHash _mpdobSecondaryHash;
        private VehicleHash _mpdobvehicleHash;

        private Vehicle _mpdobVehicle;

        Ped _mpdobPrimary;
        Ped _mpdobSecondary;

        public DrinkingonBase()
        {
            _ = GetJsonData();
            InitInfo(World.GetNextPositionOnStreet(_mpdobcoordinates));
            ResponseCode = 2;
            StartDistance = 200f;
        }
        
        public async override void OnStart(Ped player)
        {
            base.OnStart(player);

            Random mpdobscenariochooser = new Random();
            int mpdobscenariochoice = mpdobscenariochooser.Next(1, 100 + 1);

            if (mpdobscenariochoice <= 50)
            {
                _mpdobPrimary = await SpawnPed(_mpdobPrimaryHash, Location + 2);
                _mpdobSecondary = await SpawnPed(_mpdobSecondaryHash, Location + 1);
                _mpdobVehicle = await SpawnVehicle(_mpdobvehicleHash, Location);
                _mpdobPrimary.SetIntoVehicle(_mpdobVehicle, VehicleSeat.Driver);
                _mpdobSecondary.SetIntoVehicle(_mpdobVehicle, VehicleSeat.Passenger);

                PedData mpdobPrimaryData = await Utilities.GetPedData(_mpdobPrimary.NetworkId);
                PedData mpdobSecondaryData = await Utilities.GetPedData(_mpdobSecondary.NetworkId);
                VehicleData mpdobVehicleData = await Utilities.GetVehicleData(_mpdobVehicle.NetworkId);
                _mpdobPrimary.AlwaysKeepTask = true;
                _mpdobSecondary.AlwaysKeepTask = true;
                _mpdobPrimary.IsPersistent = true;
                _mpdobSecondary.IsPersistent = true;
                _mpdobPrimary.BlockPermanentEvents = true;
                _mpdobSecondary.BlockPermanentEvents = true;
                _mpdobVehicle.IsPersistent = true;

                _mpdobPrimary.DrivingStyle = DrivingStyle.AvoidTrafficExtremely;
                _mpdobPrimary.DrivingSpeed = 200;
                _mpdobPrimary.Task.DriveTo(_mpdobVehicle, FinalDrivingLocation, 0, 200);
                _mpdobVehicle.AttachBlip();
                Pursuit.RegisterPursuit(_mpdobPrimary);

                if (_mpdobVehicle.Speed <= 0)
                {
                    _mpdobPrimary.Task.LeaveVehicle();
                    _mpdobSecondary.Task.LeaveVehicle();
                    _mpdobPrimary.Task.FleeFrom(player);
                    _mpdobSecondary.Task.FleeFrom(player);
                }

                Random mpdobPBAI = new Random();
                int mpdobPBAL = mpdobPBAI.Next(1, 100 + 1);
                if (mpdobPBAL <= 25)
                {
                    mpdobPrimaryData.BloodAlcoholLevel = 0.11;
                }
                if (mpdobPBAL > 25 && mpdobPBAL <= 50)
                {
                    mpdobPrimaryData.BloodAlcoholLevel = 0.07;
                }
                if (mpdobPBAL > 50 && mpdobPBAL <= 75)
                {
                    mpdobPrimaryData.BloodAlcoholLevel = 0.03;
                }
                if (mpdobPBAL > 75)
                {
                    mpdobPrimaryData.BloodAlcoholLevel = 0.14;
                }

                Random mpdobSBAI = new Random();
                int mpdobSBAL = mpdobPBAI.Next(1, 100 + 1);
                if (mpdobSBAL <= 25)
                {
                    mpdobSecondaryData.BloodAlcoholLevel = 0.11;
                }
                if (mpdobSBAL > 25 && mpdobSBAL <= 50)
                {
                    mpdobSecondaryData.BloodAlcoholLevel = 0.07;
                }
                if (mpdobSBAL > 50 && mpdobSBAL <= 75)
                {
                    mpdobSecondaryData.BloodAlcoholLevel = 0.03;
                }
                if (mpdobSBAL > 75)
                {
                    mpdobSecondaryData.BloodAlcoholLevel = 0.14;
                }

                Item mpdobdogtags = new Item
                {
                    Name = "Dog tags matching ID",
                    IsIllegal = false
                };
                Item mpdobmilID = new Item
                {
                    Name = "Military ID (Drivers License)",
                    IsIllegal = false,
                };

                Item mpdobemptybeercan = new Item
                {
                    Name = "Empty beer can",
                    IsIllegal = true
                };
                Item mpdobwrench = new Item
                {
                    Name = "Wrench",
                    IsIllegal = false
                };

                mpdobPrimaryData.Items.Add(mpdobmilID);
                mpdobPrimaryData.Items.Add(mpdobdogtags);
                mpdobSecondaryData.Items.Add(mpdobmilID);
                mpdobSecondaryData.Items.Add(mpdobdogtags);
                mpdobVehicleData.Items.Add(mpdobwrench);
                mpdobVehicleData.Items.Add(mpdobemptybeercan);

                Utilities.SetPedData(_mpdobPrimary.NetworkId, mpdobPrimaryData);
                Utilities.SetPedData(_mpdobSecondary.NetworkId, mpdobSecondaryData);
                Utilities.SetVehicleData(_mpdobVehicle.NetworkId, mpdobVehicleData);
                Utilities.ExcludeVehicleFromTrafficStop(_mpdobVehicle.NetworkId, true);
            }

            if (mpdobscenariochoice > 50)
            {
                _mpdobPrimary = await SpawnPed(_mpdobPrimaryHash, Location.Around(15f));
                _mpdobSecondary = await SpawnPed(_mpdobSecondaryHash, Location.Around(10f));
                _mpdobPrimary.AlwaysKeepTask = true;
                _mpdobSecondary.AlwaysKeepTask = true;
                _mpdobPrimary.IsPersistent = true;
                _mpdobSecondary.IsPersistent = true;
                _mpdobPrimary.BlockPermanentEvents = true;
                _mpdobSecondary.BlockPermanentEvents = true;
                API.SetPedIsDrunk(_mpdobPrimary.NetworkId, true);
                API.SetPedIsDrunk(_mpdobSecondary.NetworkId, true);
                _mpdobPrimary.Task.WanderAround();
                _mpdobSecondary.Task.WanderAround();

                PedData mpdobPrimaryData = await Utilities.GetPedData(_mpdobPrimary.NetworkId);
                PedData mpdobSecondaryData = await Utilities.GetPedData(_mpdobSecondary.NetworkId);

                Random mpdobPBAI = new Random();
                int mpdobPBAL = mpdobPBAI.Next(1, 100 + 1);
                if (mpdobPBAL <= 25)
                {
                    mpdobPrimaryData.BloodAlcoholLevel = 0.11;
                }
                if (mpdobPBAL > 25 && mpdobPBAL <= 50)
                {
                    mpdobPrimaryData.BloodAlcoholLevel = 0.07;
                }
                if (mpdobPBAL > 50 && mpdobPBAL <= 75)
                {
                    mpdobPrimaryData.BloodAlcoholLevel = 0.03;
                }
                if (mpdobPBAL > 75)
                {
                    mpdobPrimaryData.BloodAlcoholLevel = 0.14;
                }

                Random mpdobSBAI = new Random();
                int mpdobSBAL = mpdobPBAI.Next(1, 100 + 1);
                if (mpdobSBAL <= 25)
                {
                    mpdobSecondaryData.BloodAlcoholLevel = 0.11;
                }
                if (mpdobSBAL > 25 && mpdobSBAL <= 50)
                {
                    mpdobSecondaryData.BloodAlcoholLevel = 0.07;
                }
                if (mpdobSBAL > 50 && mpdobSBAL <= 75)
                {
                    mpdobSecondaryData.BloodAlcoholLevel = 0.03;
                }
                if (mpdobSBAL > 75)
                {
                    mpdobSecondaryData.BloodAlcoholLevel = 0.14;
                }

                Utilities.SetPedData(_mpdobPrimary.NetworkId, mpdobPrimaryData);
                Utilities.SetPedData(_mpdobSecondary.NetworkId, mpdobSecondaryData);
            }
        }

        public async override Task OnAccept()
        {
            InitBlip(50f);
            UpdateData();
        }
    }
}