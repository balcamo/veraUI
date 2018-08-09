import { Component, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { User } from './classes/user';
import { UserService } from './service/app.service.user';
import { Observable } from 'rxjs';
import { Constants } from './classes/constants';
import { NavComponent } from './nav/nav.component';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  outputs: ['notify'],
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  userEntry = "block";
  mainPage = "none";
  userService: UserService;
  user = new User();;
  consts = new Constants();
  http: Http;
  notify: EventEmitter<string> = new EventEmitter<string>();
  nav: NavComponent;

  constructor(private router: Router, http: Http, userService: UserService) {
    this.userService = userService;
    this.http = http;

  }

  public emit_event(location: string) {
    this.notify.emit(location);
  }

  reloadOverwrite() {
    console.log("this should print on refresh");
    this.router.navigate([''], { replaceUrl: true });
  }

  submitUserName() {
    if (this.user.UserName == null) {
      alert("Please Enter your user name");
    } else {
     
      console.log("user name " + this.user.UserName);
      let params: URLSearchParams = new URLSearchParams();
      var pageHeaders = new Headers({
        'Content-Type': 'application/json'
      });
      let options = new RequestOptions({
        search: params,
        headers: pageHeaders
      });
      var body = JSON.stringify(this.user.UserName);
      console.log(this.consts.url + 'User');
      this.http.get(this.consts.url + 'User?userName={' + this.user.UserName + '}')
        .subscribe((data) => this.waitForHttp(data));

    }
  }

  waitForHttp(data: any) {
    if (data == undefined) {
      alert()
    } else {
      this.user.EntryGroup = data.text() as number;
      if (data.text() == 1) { this.user.nav = this.consts.employee; }
      this.userService.setUser(this.user);
      console.log("finishing waitForHttp");
      //this.nav = new NavComponent(this.userService);
      console.log("changing view");
      this.emit_event("nav");
      this.userEntry = "none";
      this.mainPage = "block";
    }
  }
  
}
