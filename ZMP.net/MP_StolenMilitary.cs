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
    [CalloutProperties("Military Police Stolen Military Vehicle Callout", "Valandria", "0.0.1")]
    public class StolenMilitaryVehicle : Callout
    {
        public JObject GetJsonData()
        {
            string mpsmvpath = "/callouts/VRRC/VRRCConfig.json";
            string mpsmvdata = API.LoadResourceFile(API.GetCurrentResourceName(), mpsmvpath);
            JObject mpsmvjsonData = JObject.Parse(mpsmvdata);

            foreach (var mpsmvDepartment in mpsmvjsonData["StolenMilitaryVehicle-Department"])
            {             
                int.TryParse((string)mpsmvDepartment[0], out int mpsmvdeptID);
                _assignedDeptarments.Add(mpsmvdeptID);
            }

            List<Vector3> mpsmvcoords = new List<Vector3>();
            foreach (var mpsmvcoordinate in mpsmvjsonData["StolenMilitaryVehicle-Coordinates"])
            {
                mpsmvcoords.Add(JsonConvert.DeserializeObject<Vector3>(mpsmvcoordinate.ToString()));
            }
            _mpsmvcoordinates = mpsmvcoords.SelectRandom();

            Dictionary<string, PedHash> mpsmvDriverHashes = new Dictionary<string, PedHash>();
            string[] mpsmvDriverJSON = JsonConvert.DeserializeObject<string[]>(mpsmvjsonData["StolenMilitaryVehicle-Driver"].ToString());
            foreach (string mpsmvDriverhash in mpsmvDriverJSON)
            {
                int mpsmvDriverhashKey = API.GetHashKey(mpsmvDriverhash);
                mpsmvDriverHashes.Add(mpsmvDriverhash, (PedHash)mpsmvDriverhashKey);
            }
            _mpsmvDriverHash = mpsmvDriverHashes.SelectRandom().Value;

            //----VEHICLE DATA----
            Dictionary<string, VehicleHash> mpsmvvehicleHashes = new Dictionary<string, VehicleHash>();
            string[] mpsmvvehicleJSON = JsonConvert.DeserializeObject<string[]>(mpsmvjsonData["ZMP-Vehicles"].ToString());
            foreach (string mpsmvhash in mpsmvvehicleJSON)
            {
                int hashKey = API.GetHashKey(mpsmvhash);
                mpsmvvehicleHashes.Add(mpsmvhash, (VehicleHash)hashKey);
            }
            _mpsmvvehicleHash = mpsmvvehicleHashes.SelectRandom().Value;

            return mpsmvjsonData;
        }

        public override async Task<bool> CheckRequirements()
        {
            var mpsmvplayerDept = Utilities.GetPlayerData().DepartmentID;
            if (_assignedDeptarments.Contains(mpsmvplayerDept))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<int> _assignedDeptarments = new List<int>();
        private Vector3 _mpsmvcoordinates;
        private PedHash _mpsmvDriverHash;
        private VehicleHash _mpsmvvehicleHash;

        private Vehicle _mpsmvVehicle;

        Ped _mpsmvDriver;

        public StolenMilitaryVehicle()
        {
            _ = GetJsonData();
            InitInfo(World.GetNextPositionOnStreet(_mpsmvcoordinates));
            ShortName = "MP - Stolen Military Vehicle";
            CalloutDescription = "Someone stole a vehicle from the motor pool!";
            ResponseCode = 3;
            StartDistance = 250f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            _mpsmvDriver = await SpawnPed(_mpsmvDriverHash, Location + 2);
            _mpsmvVehicle = await SpawnVehicle(_mpsmvvehicleHash, Location);
            _mpsmvDriver.SetIntoVehicle(_mpsmvVehicle, VehicleSeat.Driver);

            VehicleData mpsmvVehicleData = await Utilities.GetVehicleData(_mpsmvVehicle.NetworkId);
            Utilities.SetVehicleData(_mpsmvVehicle.NetworkId, mpsmvVehicleData);
            Utilities.ExcludeVehicleFromTrafficStop(_mpsmvVehicle.NetworkId,true);
            _mpsmvDriver.AlwaysKeepTask = true;
            _mpsmvDriver.IsPersistent = true;
            _mpsmvDriver.BlockPermanentEvents = true;
            _mpsmvVehicle.IsPersistent = true;

            _mpsmvDriver.DrivingStyle = DrivingStyle.AvoidTrafficExtremely;
            _mpsmvDriver.DrivingSpeed = 200;
            _mpsmvDriver.Task.FleeFrom(player);
            _mpsmvVehicle.AttachBlip();
            Pursuit.RegisterPursuit(_mpsmvDriver);
        }
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
        }
    }
}