using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VeraAPI.Models.Tools;

namespace VeraAPI.Models.Forms
{
    public class TravelAuthForm : BaseForm
    {
        public string Preparer { get; set; } = string.Empty;            // String
        public string FirstName { get; set; } = string.Empty;           // String
        public string LastName { get; set; } = string.Empty;            // String
        public string Phone { get; set; } = string.Empty;               // Int
        public string Email { get; set; } = string.Empty;               // String
        public string EventTitle { get; set; } = string.Empty;          // String
        public string Location { get; set; } = string.Empty;            // String
        public string TravelBegin { get; set; } = string.Empty;         // DateTime
        public string TravelEnd { get; set; } = string.Empty;           // DateTime
        public string DistVehicle { get; set; } = string.Empty;         // Bool
        public string RegistrationCost { get; set; } = string.Empty;    // Decimal
        public string Airfare { get; set; } = string.Empty;             // Decimal
        public string RentalCar { get; set; } = string.Empty;    // RentalCar
        public string Fuel { get; set; } = string.Empty;    // Fuel
        public string ParkingTolls { get; set; } = string.Empty;    // ParkingTolls
        public string Mileage { get; set; } = string.Empty;    // Mileage
        public string Lodging { get; set; } = string.Empty;    // Lodging
        public string PerDiem { get; set; } = string.Empty;    // PerDiem
        public string FullDays { get; set; } = string.Empty;   // FullDays
        public string Misc { get; set; } = string.Empty;   // Misc
        public string MiscExplain { get; set; } = string.Empty;     // MiscExplain
        public string TotalEstimate { get; set; } = string.Empty;   // TotalEstimate
        public string Advance { get; set; } = string.Empty;       // Advance
        public string AdvanceAmount { get; set; } = string.Empty;   // AdvanceAmount
        public string Policy { get; set; } = string.Empty;       // Policy
        public string SubmitterSig { get; set; } = string.Empty;     // SubmitterSig
        public string RecapRegistrationCost { get; set; } = string.Empty;   // RecapRegistrationCost
        public string RecapAirfare { get; set; } = string.Empty;   // RecapAirfare
        public string RecapRentalCar { get; set; } = string.Empty;   // RecapRentalCar
        public string RecapFuel { get; set; } = string.Empty;   // RecapFuel
        public string RecapParkingTolls { get; set; } = string.Empty;   // RecapParkingTolls
        public string RecapMileage { get; set; } = string.Empty;   // RecapMileage
        public string RecapLodging { get; set; } = string.Empty;   // RecapLodging
        public string RecapPerDiem { get; set; } = string.Empty;   // RecapPerDiem
        public string RecapFullDays { get; set; } = string.Empty;   // RecapFullDays
        public string RecapMisc { get; set; } = string.Empty;   // RecapMisc
        public string TotalRecap { get; set; } = string.Empty;   // TotalRecap
        public string TotalReimburse { get; set; } = string.Empty;   // TotalReimburse
        public string ApprovalStatus { get; set; } = string.Empty;     // ApprovalStatus - sets color of box on front end
        public string DeptHeadEmail { get; set; } = string.Empty;     // Department head approval - DHApproval
        public string DeptHeadID { get; set; } = string.Empty;    // Department head ID DHID
        public string GeneralManagerEmail { get; set; } = string.Empty;     // GMApproval
        public string GeneralManagerID { get; set; } = string.Empty;  // GMID

        public TravelAuthForm() { }

        public TravelAuthForm(int formDataID)
        {
            this.FormDataID = formDataID;
        }
    }
}