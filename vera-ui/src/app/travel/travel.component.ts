import { Component, OnInit, Input } from '@angular/core';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { Constants } from '../classes/constants';
import { AuthForm } from '../classes/travel-auth-form';
import { User, Auth } from '../classes/user';
import { UserService } from '../service/app.service.user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-travel',
  templateUrl: './travel.component.html',
  styleUrls: ['./travel.component.scss']
})
export class TravelComponent implements OnInit {
  // variables needed for class 
  authDisplay = "none";
  allAuthDisplay = "none";
  approveAuthDisplay = "none";
  financeAuthDisplay = "none";
  consts = new Constants();
  http: Http;
  authForms = [];
  @Input() user;
  userService: UserService;
  approver = false;
  formsList = false;
  finance = false;

  constructor(private router: Router, http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;
    this.user = this.userService.getUser();
  }
  
  ngOnInit() {
    if (this.user.EntryGroup[3] == 1 || this.user.EntryGroup[3] == 2 || this.user.EntryGroup[3] == 99) {
      this.approver = true;
    }
    if (this.user.EntryGroup[2] == 1 ||  this.user.EntryGroup[3] == 99) {
      this.finance = true;
    }
  }

  /**
   * This function will toggle the display of
   * the new travel authorization form
   **/
  displayAuth() {
    this.user = this.userService.getUser();
    if (this.authDisplay == "none") {
      this.allAuthDisplay = "none";
      this.approveAuthDisplay = "none";
      this.financeAuthDisplay = "none";
      this.authDisplay = "block";
    } else {
      this.authDisplay = "none";
    }
  }

  /**
   * this displays all active authforms found on in the database
   * */
  displayAllAuth() {
    this.user = this.userService.getUser();
    this.authForms = [];
    this.formsList = false;

    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    console.log(this.consts.url + 'TravelAuth?restUserID=' + this.user.UserID);
    this.http.get(this.consts.url + 'TravelAuth?restUserID=' + this.user.UserID)
      .subscribe((data) => this.waitForHttp(data));
    if (this.authForms.length == 0) {
      console.log("Data returned is null");
      this.formsList = false;
    } else {
      this.formsList = true;
      console.log("finishing displayAllAuth");
    }
    console.log("in all auth formsList is : " + this.formsList);
    if (this.allAuthDisplay == "none") {
      this.authDisplay = "none";
      this.approveAuthDisplay = "none";
      this.financeAuthDisplay = "none";
      this.allAuthDisplay = "block";
    } else {
      this.allAuthDisplay = "none";
    }
  }
  /**
   * this displays all active authforms to be approved found on in the database
   * */
  displayApproveAuth() {
    this.user = this.userService.getUser();
    this.authForms = [];
    this.formsList = false;

    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });

    this.http.get(this.consts.url + 'TravelApproval?restUserID=' + this.user.UserID)
      .subscribe((data) => this.waitForHttp(data));
    if (this.authForms.length == 0) {
      console.log("Data returned is null");
      this.formsList = false;
    } else {
      this.formsList = true;
      console.log("finishing displayAllAuth");
    }
    console.log("in approve auth formsList is : " + this.formsList);
    if (this.approveAuthDisplay == "none") {
      this.allAuthDisplay = "none";
      this.authDisplay = "none";
      this.financeAuthDisplay = "none";
      this.approveAuthDisplay = "block";
    } else {
      this.approveAuthDisplay = "none";
    }
  }

 /**
   * this displays all active forms for finance to be approved found in the database
   * */
  displayFinanceAuth() {
    this.user = this.userService.getUser();
    this.authForms = [];
    this.formsList = false;

    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
   // this.http.get(this.consts.url + 'TravelAuth?restUserID=' + this.user.UserID)
    this.http.get(this.consts.url + 'TravelFinance?restUserID=' + this.user.UserID)
      .subscribe((data) => this.waitForHttp(data));
    if (this.authForms.length == 0) {
      console.log("Data returned is null");
      this.formsList = false;
    } else {
      this.formsList = true;
      console.log("finishing displayAllAuth");
    }
    console.log("in approve auth formsList is : " + this.formsList);
    if (this.financeAuthDisplay == "none") {
      this.allAuthDisplay = "none";
      this.authDisplay = "none";
      this.approveAuthDisplay = "none";
      this.financeAuthDisplay = "block";
    } else {
      this.financeAuthDisplay = "none";
    }
  }

  /**
   * this function will allow necessary item to be done after
   * the http request has returned data
   * @param data : return from the http request
   */
  waitForHttp(data: any) {
    console.log("the data : " + data.text());
    this.authForms = JSON.parse(data.text()) as AuthForm[];
    if (this.authForms.length == 0) {
      console.log("Data returned is null");
      this.formsList = false;
    } else {
      this.formsList = true;
      console.log("finishing waitForHttp");
    }
  }

}
