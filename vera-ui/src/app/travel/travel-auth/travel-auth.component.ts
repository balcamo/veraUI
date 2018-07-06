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
  districtVehicalBorder = "black";
  registrationBorder = "black";
  airfareBorder = "black";
  rentalBorder = "black";
  fuelBorder = "black";
  mileageBorder = "black";
  lodgingBorder = "black";
  perDiemBorder = "black";
  daysBorder = "black";
  miscBorder = "black";

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
      alert("success");
      this.setFormDefaults();
      this.nameBorder, this.phoneBorder = "black";
    } else {
      alert("Please fill in the required fields");
    }
    //this.http.get('/api/API')
      //.subscribe((data) => console.log(data));
  }

  checkRequired() {
    var valid = true;
    if (this.form.Name == null) {
      this.nameBorder = "red";
      valid = false;
    } if (this.form.Phone == null) {
      this.phoneBorder = "red";
      valid = false;
    } if (this.form.EventTitle == null) {
      this.eventTitleBorder = "red";
      valid = false;
    } if (this.form.Location == null) {
      this.eventLocBorder = "red";
      valid = false;
    } if (this.form.TravelBegin == null) {
      this.travelStartBorder = "red";
      valid = false;
    } if (this.form.TravelEnd == null) {
      this.travelEndBorder = "red";
      valid = false;
    } if (this.form.DistVehicle == null) {
      this.districtVehicalBorder = "red";
      valid = false;
    } if (this.form.RegistrationCost == null) {
      this.registrationBorder = "red";
      valid = false;
    } if (this.form.Airfare == null) {
      this.airfareBorder = "red";
      valid = false;
    } if (this.form.RentalCar == null) {
      this.rentalBorder = "red";
      valid = false;
    } if (this.form.FuelParking == null) {
      this.fuelBorder = "red";
      valid = false;
    } if (this.form.Mileage == null) {
      this.mileageBorder = "red";
      valid = false;
    } if (this.form.Lodging == null) {
      this.lodgingBorder = "red";
      valid = false;
    } if (this.form.PerDiem == null) {
      this.perDiemBorder = "red";
      valid = false;
    } if (this.form.FullDays == null) {
      this.daysBorder = "red";
      valid = false;
    } if (this.form.Misc == null) {
      this.miscBorder = "red";
      valid = false;
    }
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
