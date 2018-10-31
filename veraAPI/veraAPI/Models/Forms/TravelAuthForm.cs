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
        public string RentalCar { get; set; } = string.Empty;           // RentalCar
        public string Fuel { get; set; } = string.Empty;                // Fuel
        public string ParkingTolls { get; set; } = string.Empty;        // ParkingTolls
        public string Mileage { get; set; } = string.Empty;             // Mileage
        public string Lodging { get; set; } = string.Empty;             // Lodging
        public string PerDiem { get; set; } = string.Empty;             // PerDiem
        public string TravelDays { get; set; } = string.Empty;            // TravelDays
        public string FullDays { get; set; } = string.Empty;            // FullDays
        public string Misc { get; set; } = string.Empty;                // Misc
        public string MiscExplain { get; set; } = string.Empty;         // MiscExplain
        public string TotalEstimate { get; set; } = string.Empty;       // TotalEstimate
        public string Advance { get; set; } = string.Empty;             // Advance
        public string AdvanceAmount { get; set; } = string.Empty;       // AdvanceAmount
        public string AdvanceDate { get; set; } = string.Empty;
        public string AdvanceStatus { get; set; } = string.Empty;
        public string Policy { get; set; } = string.Empty;              // Policy
        public string SubmitterSig { get; set; } = string.Empty;        // Integer
        public string SubmitDate { get; set; } = string.Empty;
        public string RecapRegistrationCost { get; set; } = string.Empty;   // RecapRegistrationCost
        public string RecapAirfare { get; set; } = string.Empty;        // RecapAirfare
        public string RecapRentalCar { get; set; } = string.Empty;      // RecapRentalCar
        public string RecapFuel { get; set; } = string.Empty;           // RecapFuel
        public string RecapParkingTolls { get; set; } = string.Empty;   // RecapParkingTolls
        public string RecapMileage { get; set; } = string.Empty;        // RecapMileage
        public string RecapMileageAmount { get; set; } = string.Empty;        // RecapMileageAmount
        public string RecapLodging { get; set; } = string.Empty;        // RecapLodging
        public string RecapPerDiem { get; set; } = string.Empty;        // RecapPerDiem
        public string RecapTravelDays { get; set; } = string.Empty;       // RecapTravelDays
        public string RecapFullDays { get; set; } = string.Empty;       // RecapFullDays
        public string RecapMisc { get; set; } = string.Empty;           // RecapMisc
        public string TotalRecap { get; set; } = string.Empty;          // TotalRecap
        public string RecapDate { get; set; } = string.Empty;
        public string RecapStatus { get; set; } = string.Empty;
        public string TotalReimburse { get; set; } = string.Empty;      // TotalReimburse
        public string ApprovalStatus { get; set; } = string.Empty;      // Integer - {0 Denied Red} {1 Approved Green} {2 Pending Yellow}
        public string ApprovalDate { get; set; } = string.Empty;
        public string DHApproval { get; set; } = string.Empty;          // Integer - {0 Denied Red} {1 Approved Green} {2 Pending Yellow}
        public string DHApprovalDate { get; set; } = string.Empty;
        public string DHID { get; set; } = string.Empty;                // Integer department head userID
        public string DHEmail { get; set; } = string.Empty;
        public string GMApproval { get; set; } = string.Empty;          // Integer - {0 Denied Red} {1 Approved Green} {2 Pending Yellow}
        public string GMApprovalDate { get; set; } = string.Empty;
        public string GMID { get; set; } = string.Empty;                // Integer general manager userID
        public string GMEmail { get; set; } = string.Empty;
        public string CloseDate { get; set; } = string.Empty;
        public string CloseStatus { get; set; } = string.Empty;
        public string MailMessage { get; set; } = string.Empty;

        public TravelAuthForm() { }

        public TravelAuthForm(int formDataID)
        {
            this.FormDataID = formDataID;
        }
    }
}