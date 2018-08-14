import { Component, OnInit, Input } from '@angular/core';
import { UserService } from '../../service/app.service.user';
import { User } from '../../classes/user';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { AuthForm } from '../../classes/travel-auth-form';
import { RecapForm } from '../../classes/recap-form';

@Component({
  selector: 'app-view-auth-forms',
  templateUrl: './view-auth-forms.component.html',
  styleUrls: ['./view-auth-forms.component.scss']
})
export class ViewAuthFormsComponent implements OnInit {
  //@Input() authForms;
  http: Http;
  userService: UserService
  user: User;
  displayForm = "none";
  displayRecap = "none";
  //temp
  authForms = [];
  form: AuthForm;
  recap: RecapForm;
  oldForm: AuthForm;
  auth1 = new AuthForm();
  auth2 = new AuthForm();
  constructor(http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;
    this.user = this.userService.getUser();
  }

  ngOnInit() {
    this.auth1.EventTitle = "Event 1";
    this.auth1.Supervisor = "red";
    this.auth2.EventTitle = "Event 2";
    this.auth2.Supervisor = "green";

    this.authForms.push(this.auth1);
    this.authForms.push(this.auth2);
  }
  displaySelected(authForm: AuthForm) {
    this.form = authForm;
    this.displayRecap = "none";
    console.log(this.form);
    if (this.displayForm == "none") {
      this.displayForm = "block";
    } else if (this.form !== this.oldForm) {
      this.displayForm = "block"
    } else if (this.form == this.oldForm) {
      this.displayForm = "none"
    } 
    this.oldForm = this.form
  }
  showRecap() {
    this.displayForm = "none";
    this.displayRecap = "block";

    this.recap.AdvanceTaken = this.form.AdvanceAmount;
    this.recap.FirstName = this.form.FirstName;
    this.recap.LastName = this.form.LastName;
    this.recap.Airfare = this.form.Airfare;
    this.recap.Email = this.form.Email;
    this.recap.Location = this.form.Location;
    this.recap.Phone = this.form.Phone;
    this.recap.EventTitle = this.form.EventTitle;
    this.recap.RegistrationCost = this.form.RegistrationCost;
    this.recap.RentalCar = this.form.RentalCar;
    this.recap.FuelParking = this.form.FuelParking;
    this.recap.Mileage = this.form.Mileage;
    this.recap.Lodging = this.form.Lodging;
    this.recap.FullDays = this.form.FullDays;
    this.recap.PerDiem = this.form.PerDiem;
    this.recap.Misc = this.form.Misc;
    this.recap.TotalRecap = this.form.total;

  }
}
