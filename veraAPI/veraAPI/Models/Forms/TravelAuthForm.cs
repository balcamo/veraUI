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
        public string Int1 { get; set; } = string.Empty;        // Phone
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
        public string String8 { get; set; } = string.Empty;     // Supervisor

        public TravelAuthForm() { }

        public TravelAuthForm(int formDataID)
        {
            this.FormDataID = formDataID;
        }

        public void setNulls()
        {
            if (Bool1 == null) { Bool1 = string.Empty; }
            if (String1 == null) { String1 = string.Empty; }
            if (String2 == null) { String2 = string.Empty; }
            if (Int1 == null) { Int1 = string.Empty; }
            if (String3 == null) { String3 = string.Empty; }
            if (String4 == null) { String4 = string.Empty; }
            if (String5 == null) { String5 = string.Empty; }
            if (Date1 == null) { Date1 = string.Empty; }
            if (Date2 == null) { Date2 = string.Empty; }
            if (Bool2 == null) { Bool2 = string.Empty; }
            if (Decimal2 == null) { Decimal2 = string.Empty; }
            if (Decimal3 == null) { Decimal3 = string.Empty; }
            if (Decimal4 == null) { Decimal4 = string.Empty; }
            if (Decimal5 == null) { Decimal5 = string.Empty; }
            if (Decimal6 == null) { Decimal6 = string.Empty; }
            if (Decimal7 == null) { Decimal7 = string.Empty; }
            if (Decimal8 == null) { Decimal8 = string.Empty; }
            if (Decimal9 == null) { Decimal9 = string.Empty; }
            if (Decimal10 == null) { Decimal10 = string.Empty; }
            if (Decimal11 == null) { Decimal11 = string.Empty; }
            if (String6 == null) { String6 = string.Empty; }
            if (Decimal12 == null) { Decimal12 = string.Empty; }
            if (Bool3 == null) { Bool3 = string.Empty; }
            if (Decimal13 == null) { Decimal13 = string.Empty; }
            if (Bool4 == null) { Bool4 = string.Empty; }
            if (String7 == null) { String7 = string.Empty; }
            if (Decimal14 == null) { Decimal14 = string.Empty; }
            if (Decimal15 == null) { Decimal15 = string.Empty; }
            if (Decimal16 == null) { Decimal16 = string.Empty; }
            if (Decimal17 == null) { Decimal17 = string.Empty; }
            if (Decimal18 == null) { Decimal18 = string.Empty; }
            if (Decimal19 == null) { Decimal19 = string.Empty; }
            if (Decimal20 == null) { Decimal20 = string.Empty; }
            if (Decimal21 == null) { Decimal21 = string.Empty; }
            if (Decimal22 == null) { Decimal22 = string.Empty; }
            if (Decimal23 == null) { Decimal23 = string.Empty; }
            if (Decimal24 == null) { Decimal24 = string.Empty; }
            if (Decimal25 == null) { Decimal25 = string.Empty; }
            if (String8 == null) { String8 = string.Empty; }
        }
    }
}