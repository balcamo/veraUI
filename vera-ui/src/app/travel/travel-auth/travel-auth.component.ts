import { Component, OnInit } from '@angular/core';
import { AuthForm } from '../../classes/travel-auth-form';

@Component({
  selector: 'app-travel-auth',
  templateUrl: './travel-auth.component.html',
  styleUrls: ['./travel-auth.component.scss']
})
export class TravelAuthComponent implements OnInit {
  form = new AuthForm();
  total: number;

  constructor() { }

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
  }
}
