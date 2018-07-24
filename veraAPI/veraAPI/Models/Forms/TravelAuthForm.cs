using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using veraAPI.Models.Forms;

namespace veraAPI.Models
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

      /*  public TravelAuthForm(object tempform)
        {
            Preparer = tempform.Preparer;
            FirstName = tempform.FirstName;
            LastName = tempform.LastName;
            Phone = tempform.Phone;
            Email = tempform.Email;
            EventTitle = tempform.EventTitle;
            Location = tempform.Location;
            TravelBegin = tempform.TravelBegin;
            TravelEnd = tempform.TravelEnd;
            DistVehicle = tempform.DistVehicle;
            DistVehicleNum = tempform.DistVehicleNum;
            RegistrationCost = tempform.RegistrationCost;
            Airfare = tempform.Airfare;
            RentalCar = tempform.RentalCar;
            FuelParking = tempform.FuelParking;
            Mileage = tempform.Mileage;
            Lodging = tempform.Lodging;
            PerDiem = tempform.PerDiem;
            FullDays = tempform.FullDays;
            Misc = tempform.Misc;
            total = tempform.total;
            Advance = tempform.Advance;
            AdvanceAmount = tempform.AdvanceAmount;
            Policy = tempform.Policy;
            SubmitterSig = tempform.SubmitterSig;
        }   */
    }
}