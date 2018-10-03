using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Forms
{
    public class TravelAuthForm : BaseForm
    {
        public string Bool1 { get; set; } = string.Empty;       // Preparer
        public string String1 { get; set; } = string.Empty;     // FirstName
        public string String2 { get; set; } = string.Empty;     // LastName
        public string Integer1 { get; set; } = string.Empty;        // Phone
        public string String3 { get; set; } = string.Empty;     // Email
        public string String4 { get; set; } = string.Empty;     // EventTitle
        public string String5 { get; set; } = string.Empty;     // Location
        public string Date1 { get; set; } = string.Empty;       // TravelBegin
        public string Date2 { get; set; } = string.Empty;       // TravelEnd
        public string Bool2 { get; set; } = string.Empty;       // DistVehicle
        public string Decimal2 { get; set; } = string.Empty;    // RegistrationCost
        public string Decimal3 { get; set; } = string.Empty;    // Airfare
        public string Decimal4 { get; set; } = string.Empty;    // RentalCar
        public string Decimal5 { get; set; } = string.Empty;    // Fuel
        public string Decimal6 { get; set; } = string.Empty;    // ParkingTolls
        public string Decimal7 { get; set; } = string.Empty;    // Mileage
        public string Decimal8 { get; set; } = string.Empty;    // Lodging
        public string Decimal9 { get; set; } = string.Empty;    // PerDiem
        public string Decimal10 { get; set; } = string.Empty;   // FullDays
        public string Decimal11 { get; set; } = string.Empty;   // Misc
        public string String6 { get; set; } = string.Empty;     // MiscExplain
        public string Decimal12 { get; set; } = string.Empty;   // TotalEstimate
        public string Bool3 { get; set; } = string.Empty;       // Advance
        public string Decimal13 { get; set; } = string.Empty;   // AdvanceAmount
        public string Bool4 { get; set; } = string.Empty;       // Policy
        public string String7 { get; set; } = string.Empty;     // SubmitterSig
        public string Decimal14 { get; set; } = string.Empty;   // RecapRegistrationCost
        public string Decimal15 { get; set; } = string.Empty;   // RecapAirfare
        public string Decimal16 { get; set; } = string.Empty;   // RecapRentalCar
        public string Decimal17 { get; set; } = string.Empty;   // RecapFuel
        public string Decimal18 { get; set; } = string.Empty;   // RecapParkingTolls
        public string Decimal19 { get; set; } = string.Empty;   // RecapMileage
        public string Decimal20 { get; set; } = string.Empty;   // RecapLodging
        public string Decimal21 { get; set; } = string.Empty;   // RecapPerDiem
        public string Decimal22 { get; set; } = string.Empty;   // RecapFullDays
        public string Decimal23 { get; set; } = string.Empty;   // RecapMisc
        public string Decimal24 { get; set; } = string.Empty;   // TotalRecap
        public string Decimal25 { get; set; } = string.Empty;   // TotalReimburse
        public string String8 { get; set; } = string.Empty;     // Approval Status - sets color of box on front end
        public string Bool5 { get; set; } = string.Empty;     // Department head approval
        public string Decimal26 { get; set; } = string.Empty;    // Department head ID
        public string Bool6 { get; set; } = string.Empty;     // GM approval
        public string Decimal27 { get; set; } = string.Empty;  // GM ID

        public TravelAuthForm() { }

        public TravelAuthForm(int formDataID)
        {
            this.FormDataID = formDataID;
        }
    }
}