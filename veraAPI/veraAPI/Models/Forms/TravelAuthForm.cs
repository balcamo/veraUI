using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VeraAPI.Models.Forms;

namespace VeraAPI.Models
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
        public string DistVehicleNum;
        public string RegistrationCost;
        public string Airfare;
        public string RentalCar;
        public string FuelParking;
        public string Mileage;
        public string Lodging;
        public string PerDiem;
        public string FullDays;
        public string Misc;
        public string total;
        public string Advance;
        public string AdvanceAmount;
        public string Policy;
        public string SubmitterSig;

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
            if (DistVehicleNum == null) { DistVehicleNum = string.Empty; }
            if (RegistrationCost == null) { RegistrationCost = string.Empty; }
            if (Airfare == null) { Airfare = string.Empty; }
            if (RentalCar == null) { RentalCar = string.Empty; }
            if (FuelParking == null) { FuelParking = string.Empty; }
            if (Mileage == null) { Mileage = string.Empty; }
            if (Lodging == null) { Lodging = string.Empty; }
            if (PerDiem == null) { PerDiem = string.Empty; }
            if (FullDays == null) { FullDays = string.Empty; }
            if (Misc == null) { Misc = string.Empty; }
            if (total == null) { total = string.Empty; }
            if (Advance == null) { Advance = string.Empty; }
            if (AdvanceAmount == null) { AdvanceAmount = string.Empty; }
            if (Policy == null) { Policy = string.Empty; }
            if (SubmitterSig == null) { SubmitterSig = string.Empty; }
        }
    }
}