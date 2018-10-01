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
  consts = new Constants();
  http: Http;
  authForms = [];
  @Input() user;
  userService: UserService;

  constructor(private router: Router, http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;
  }
  
  ngOnInit() {

  }

  /**
   * This function will toggle the display of
   * the new travel authorization form
   **/
  displayAuth() {
    this.user = this.userService.getUser();
    if (this.authDisplay == "none") {
      this.authDisplay = "block";
      this.allAuthDisplay = "none";
      this.approveAuthDisplay = "none";
    } else {
      this.authDisplay = "none";
    }
  }

  /**
   * this displays all active authforms found on in the database
   * */
  displayAllAuth() {
    this.user = this.userService.getUser();
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });

    this.http.get(this.consts.url + 'TravelAuth?tokenHeader=' + this.user.UserID)
      .subscribe((data) => this.waitForHttp(data));
      //.subscribe((data) => console.log("the return from the get "+data.text()));

    if (this.allAuthDisplay == "none") {
      this.allAuthDisplay = "block";
      this.authDisplay = "none";
      this.approveAuthDisplay = "none";
    } else {
      this.allAuthDisplay = "none";
    }
  }  /**
   * this displays all active authforms to be approved found on in the database
   * */
  displayApproveAuth() {
    this.user = this.userService.getUser();
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });

    this.http.get(this.consts.url + 'TravelApproval?tokenHeader=' + this.user.UserID)
      .subscribe((data) => this.waitForHttp(data));
      //.subscribe((data) => console.log("the return from the get "+data.text()));

    if (this.allAuthDisplay == "none") {
      this.allAuthDisplay = "none";
      this.authDisplay = "none";
      this.approveAuthDisplay = "block";
    } else {
      this.approveAuthDisplay = "none";
    }
  }

  /**
   * this function will allow necessary item to be done after
   * the http request has returned data
   * @param data : return from the http request
   */
  waitForHttp(data: any) {
    console.log("the data : " + data.text());
    if (JSON.parse(data.text()) == []) {
      console.log("Data returned is null");

    } else {
      this.authForms = JSON.parse(data.text()) as AuthForm[];

      console.log("finishing waitForHttp");
    }
  }

}
