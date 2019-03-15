import { Component, OnInit, Input } from '@angular/core';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { Constants } from '../classes/constants';
import { AuthForm } from '../classes/travel-auth-form';
import { UserService } from '../service/app.service.user';
import { Router } from '@angular/router';


@Component({
  selector: 'app-meter-reads',
  templateUrl: './meter-reads.component.html',
  styleUrls: ['./meter-reads.component.scss']
})
export class MeterReadsComponent implements OnInit {

  userService: UserService;
  @Input() user;
  consts = new Constants();
  http: Http;
  metersDisplay = "block";
  reportsDisplay = "none";


  constructor(private router: Router, http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;
    this.user = this.userService.getUser();
  }
  ngOnInit() {

  }
  /**
 * This function will toggle the display of
 * all meters
 **/
  displayAllMeters() {
    this.user = this.userService.getUser();
    if (this.metersDisplay == "none") {
      this.reportsDisplay = "none";
      this.metersDisplay = "block";
    } else {
      this.metersDisplay = "none";
    }
  }
  /**
 * This function will toggle the display of
 * Meter Reports
 **/
  displayMeterReports() {
    this.user = this.userService.getUser();
    if (this.reportsDisplay == "none") {
      this.metersDisplay = "none";
      this.reportsDisplay = "block";
    } else {
      this.reportsDisplay = "none";
    }
  }

}
