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
<<<<<<< HEAD
    this.getGoAPI()
  }
  getGoAPI() {
    console.log("in the button function");
    this.http.get("http://localhost:3000/meters").subscribe((data) => console.log(data));
    console.log("after the get");
//    this.http.get("http://vm-apiary.verawp.local:3000/meters").subscribe((data) => this.waitForMeters(data));
  }

  waitForMeters(data:any) {
    console.log("setting this.meters to the response");
    this.meters = JSON.parse(data.text()) as Meters[];
    this.filteredMeters = this.meters;
    if (this.meters.length == 0) {
      console.log("Data returned is null");
    } else {
      console.log("finishing waitForHttp");
    }

  }

  setMeter(meter: Meters) {
    this.meterSelected = true;
    this.currentMeter = meter;
    console.log(meter);
    this.oldReads = this.reads
  }
=======
>>>>>>> f349dd0fea26000ec32bd1195da1b2d1eb7ac2e7

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
