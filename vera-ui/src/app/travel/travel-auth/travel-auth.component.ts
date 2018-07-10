import { Component, OnInit } from '@angular/core';
import { AuthForm } from '../../classes/travel-auth-form';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs';


@Component({
  selector: 'app-travel-auth',
  templateUrl: './travel-auth.component.html',
  styleUrls: ['./travel-auth.component.scss']
})
export class TravelAuthComponent implements OnInit {
  form = new AuthForm();
  total=0;
  http: Http;
  nameBorder = "black";
  phoneBorder = "black";
  eventTitleBorder = "black";
  eventLocBorder = "black";
  travelStartBorder = "blcak";
  travelEndBorder = "blcak";
  registrationBorder = "black";
  airfareBorder = "black";
  rentalBorder = "black";
  fuelBorder = "black";
  mileageBorder = "black";
  lodgingBorder = "black";
  perDiemBorder = "black";
  daysBorder = "black";
  miscBorder = "black";
  advanceColor = "black";
  distVehColor = "black";
  policyColor = "black";
  constructor(http: Http) {
    this.http = http;
  }

  ngOnInit() {
  }
  checkTot() {
    this.total = 0;
    this.total = this.form.RegistrationCost + this.form.Airfare + this.form.RentalCar +
      this.form.FuelParking + this.form.Mileage * 0.545 + this.form.Lodging + this.form.PerDiem * .75 * 2 +
      this.form.PerDiem * this.form.FullDays + this.form.Misc;
    this.form.total = this.total;
  }

  submitForm() {
    console.log("submitted");
    console.log(this.form);

    if (this.checkRequired()) {
      //this.http.get('bigfoot.verawp.local/api/API')
      //.subscribe((data) => alert(data));
      this.setFormDefaults();
      this.nameBorder, this.phoneBorder = "black";
    } else {
      alert("Please fill in the required fields");
    }
    
  }

  checkRequired() {
    var valid = true;
    if (this.form.Name == null || this.form.Phone == null || this.form.EventTitle == null
      || this.form.Location == null || this.form.TravelBegin == null || this.form.FullDays == null
      || this.form.Mileage == null || this.form.Lodging == null || this.form.PerDiem == null
      || this.form.RentalCar == null || this.form.FuelParking == null || this.form.Misc == null
      || this.form.TravelEnd == null || this.form.DistVehicle == null || this.form.Airfare == null
      || this.form.RegistrationCost == null || this.form.Advance == null || this.form.DistVehicle == null
      || this.form.Policy == null
      ) {
      valid = false;
    }

    this.nameBorder = (this.form.Name == null ? "red" : "black");
    this.phoneBorder = (this.form.Phone == null ? "red" : "black");
    this.eventTitleBorder = (this.form.EventTitle == null ? "red" : "black");
    this.eventLocBorder = (this.form.Location == null ? "red" : "black");
    this.travelStartBorder = (this.form.TravelBegin == null ? "red" : "black");
    this.travelEndBorder = (this.form.TravelEnd == null ? "red" : "black");
    this.distVehColor = (this.form.DistVehicle == null ? "red" : "black");
    this.registrationBorder = (this.form.RegistrationCost == null ? "red" : "black");
    this.airfareBorder = (this.form.Airfare == null ? "red" : "black");
    this.rentalBorder = (this.form.RentalCar == null ? "red" : "black");
    this.fuelBorder = (this.form.FuelParking == null ? "red" : "black");
    this.mileageBorder = (this.form.Mileage == null ? "red" : "black");
    this.lodgingBorder = (this.form.Lodging == null ? "red" : "black");
    this.perDiemBorder = (this.form.PerDiem == null ? "red" : "black");
    this.daysBorder = (this.form.FullDays == null ? "red" : "black");
    this.miscBorder = (this.form.Misc == null ? "red" : "black");
    this.advanceColor = (this.form.Advance == null ? "red" : "black");
    this.policyColor = (this.form.Policy == null ? "red" : "black");

    return valid;

  }
  setFormDefaults() {
    this.form.Name = null;
    this.form.Phone = null;
    this.form.Email = null;
    this.form.EventTitle = null;
    this.form.Location = null;
    this.form.TravelBegin = null;
    this.form.TravelEnd = null;
    this.form.DistVehicle = null;
    this.form.DistVehicleNum = null;
    this.form.RegistrationCost = 0;
    this.form.Airfare = 0;
    this.form.RentalCar = 0;
    this.form.FuelParking = 0;
    this.form.Mileage = 0;
    this.form.Lodging = 0;
    this.form.PerDiem = 0;
    this.form.FullDays = 0;
    this.form.Misc = 0;
    this.form.total = null;
    this.form.Advance = null;
    this.form.Policy = null;
  }
}