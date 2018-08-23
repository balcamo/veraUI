using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Forms
{
    public class TravelAuthForm : BaseForm
    {
        public string Preparer { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string TravelBegin { get; set; } = string.Empty;
        public string TravelEnd { get; set; } = string.Empty;
        public string DistVehicle { get; set; } = string.Empty;
        public string DistVehicleNum { get; set; } = string.Empty;
        public string RegistrationCost { get; set; } = string.Empty;
        public string Airfare { get; set; } = string.Empty;
        public string RentalCar { get; set; } = string.Empty;
        public string FuelParking { get; set; } = string.Empty;
        public string Mileage { get; set; } = string.Empty;
        public string Lodging { get; set; } = string.Empty;
        public string PerDiem { get; set; } = string.Empty;
        public string FullDays { get; set; } = string.Empty;
        public string Misc { get; set; } = string.Empty;
        public string total { get; set; } = string.Empty;
        public string Advance { get; set; } = string.Empty;
        public string AdvanceAmount { get; set; } = string.Empty;
        public string Policy { get; set; } = string.Empty;
        public string SubmitterSig { get; set; } = string.Empty;
        public string RecapRegistrationCost { get; set; } = string.Empty;
        public string RecapAirfare { get; set; } = string.Empty;
        public string RecapRentalCar { get; set; } = string.Empty;
        public string RecapFuelParking { get; set; } = string.Empty;
        public string RecapMileage { get; set; } = string.Empty;
        public string RecapLodging { get; set; } = string.Empty;
        public string RecapPerDiem { get; set; } = string.Empty;
        public string RecapFullDays { get; set; } = string.Empty;
        public string RecapMisc { get; set; } = string.Empty;
        public string TotalRecap { get; set; } = string.Empty;
        public string TotalReimburse { get; set; } = string.Empty;
        public string Supervisor { get; set; } = string.Empty;

        public TravelAuthForm() { }

        public TravelAuthForm(int formDataID)
        {
            this.FormDataID = formDataID;
        }
    }
}