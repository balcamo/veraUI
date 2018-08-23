using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Forms
{
    public class TravelAuthForm : BaseForm
    {
        public string Preparer;
        public string FirstName;
        public string LastName;
        public string Phone;
        public string Email;
        public string EventTitle;
        public string Location;
        public string TravelBegin;
        public string TravelEnd;
        public string DistVehicle;
        public string RegistrationCost;
        public string Airfare;
        public string RentalCar;
        public string Fuel;
        public string ParkingTolls;
        public string Mileage;
        public string Lodging;
        public string PerDiem;
        public string FullDays;
        public string Misc;
        public string MiscExplain;
        public string TotalEstimate;
        public string Advance;
        public string AdvanceAmount;
        public string Policy;
        public string SubmitterSig;
        public string RecapRegistrationCost;
        public string RecapAirfare;
        public string RecapRentalCar;
        public string RecapFuel;
        public string RecapParkingTolls;
        public string RecapMileage;
        public string RecapLodging;
        public string RecapPerDiem;
        public string RecapFullDays;
        public string RecapMisc;
        public string TotalRecap;
        public string TotalReimburse;
        public string Supervisor;

        public TravelAuthForm() { }

        public TravelAuthForm(int formDataID)
        {
            this.FormDataID = formDataID;
        }

        public void setNulls()
        {
            if (Preparer == null) { Preparer = string.Empty; }
            if (FirstName == null) { FirstName = string.Empty; }
            if (LastName == null) { LastName = string.Empty; }
            if (Phone == null) { Phone = string.Empty; }
            if (Email == null) { Email = string.Empty; }
            if (EventTitle == null) { EventTitle = string.Empty; }
            if (Location == null) { Location = string.Empty; }
            if (TravelBegin == null) { TravelBegin = string.Empty; }
            if (TravelEnd == null) { TravelEnd = string.Empty; }
            if (DistVehicle == null) { DistVehicle = string.Empty; }
            if (RegistrationCost == null) { RegistrationCost = string.Empty; }
            if (Airfare == null) { Airfare = string.Empty; }
            if (RentalCar == null) { RentalCar = string.Empty; }
            if (Fuel == null) { Fuel = string.Empty; }
            if (ParkingTolls == null) { ParkingTolls = string.Empty; }
            if (Mileage == null) { Mileage = string.Empty; }
            if (Lodging == null) { Lodging = string.Empty; }
            if (PerDiem == null) { PerDiem = string.Empty; }
            if (FullDays == null) { FullDays = string.Empty; }
            if (Misc == null) { Misc = string.Empty; }
            if (TotalEstimate == null) { TotalEstimate = string.Empty; }
            if (Advance == null) { Advance = string.Empty; }
            if (AdvanceAmount == null) { AdvanceAmount = string.Empty; }
            if (Policy == null) { Policy = string.Empty; }
            if (SubmitterSig == null) { SubmitterSig = string.Empty; }
            if (RecapRegistrationCost == null) { RecapRegistrationCost = string.Empty; }
            if (RecapRentalCar == null) { RecapRentalCar = string.Empty; }
            if (RecapAirfare == null) { RecapAirfare = string.Empty; }
            if (RecapFuel == null) { RecapFuel = string.Empty; }
            if (RecapParkingTolls == null) { RecapParkingTolls = string.Empty; }
            if (RecapLodging == null) { RecapLodging = string.Empty; }
            if (RecapPerDiem == null) { RecapPerDiem = string.Empty; }
            if (RecapFullDays == null) { RecapFullDays = string.Empty; }
            if (RecapMisc == null) { RecapMisc = string.Empty; }
            if (TotalRecap == null) { TotalRecap = string.Empty; }
            if (TotalReimburse == null) { TotalReimburse = string.Empty; }
            if (Supervisor == null) { Supervisor = string.Empty; }
        }
    }
}