import { Component, EventEmitter, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { User, Auth } from './classes/user';
import { UserService } from './service/app.service.user';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AdalService } from 'adal-angular4';
import { environment } from '../environments/environment';
import { MeterService } from './service/app.service.meters';
import { Meters } from './classes/meter-reads';
import { Observable } from 'rxjs';
import { Constants } from './classes/constants';
import { NavComponent } from './nav/nav.component';
import { Serialize, SerializeProperty, Serializable } from 'ts-serializer';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  outputs: ['notify'],
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  userEntry = "block";
  mainPage = "none";
  userService: UserService;
  user = new User();
  consts = new Constants();
  http: Http;
  notify: EventEmitter<string> = new EventEmitter<string>();
  nav: NavComponent;
  useremail: string;
  value = new Auth()
  password: string;
  url: URL;
  param: string;

  constructor(private router: Router, private route: ActivatedRoute, http: Http, userService: UserService,
              meterService:MeterService, private httpClient: HttpClient, private adalService: AdalService) {
    this.userService = userService;
    this.http = http;
    this.url = new URL(window.location.href);
    this.param = this.url.searchParams.get("route");
    console.log("current param: " + this.param);
    this.user.nav = this.consts.employee;
    if (meterService.getMeterList() == null) {
      meterService.setMeterList(new Array<Meters>());
    }

  }
  ngOnInit() {

  }
  public emit_event(location: string) {
    this.notify.emit(location);
  }

  /**
   * take imput for user email, send it to the server to check if valid
   * */
  getUser() {
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers();
    pageHeaders.append('Content-Type', 'application/json');
    pageHeaders.append('authorization', this.adalService.userInfo.token.toString());
    
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    var body = JSON.stringify({ UserName: this.adalService.userInfo.userName, UserPwd: this.adalService.userInfo.token });
    console.log(this.consts.url + 'Login');
    this.http.get(this.consts.url + 'Login').subscribe((data) =>console.log(data));
    //this.http.post(this.consts.url + 'Login', body)
    //  .subscribe((data) => this.waitForHttp(data));
  }

  waitForHttp(data: any) {
    console.log("data " + data.text());
    this.value = JSON.parse(data.text()) as Auth;
    console.log("in the wait " + this.value);
    this.user.EntryGroup = this.value.AccessKey;
    this.user.token = this.value.SessionKey;
    this.user.UserID = this.value.UserID;
    if (this.value.UserID== undefined) {
      alert("no data");
    } else if (this.user.EntryGroup[0] == 0) {
      alert("Not a valid email. Please try again");
    } else {
      if (this.user.EntryGroup[0] == 1 || this.user.EntryGroup[0] == 99) { this.user.nav = this.consts.employee; }
      this.userService.setUser(this.user);
      console.log("waitForHttp is complete");
      this.emit_event("nav");
      this.userEntry = "none";
      this.mainPage = "block";
      this.emit_event(this.param);
    }
    
  }
  get authenticated(): boolean {
    return this.adalService.userInfo.authenticated;
  }
  login() {
    this.adalService.login();
    this.getUser();
  }
  
}
