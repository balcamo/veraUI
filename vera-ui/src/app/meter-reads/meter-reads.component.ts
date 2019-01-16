import { Component, OnInit } from '@angular/core';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Constants } from '../classes/constants';
import { UserService } from '../service/app.service.user';
import { User, Auth } from '../classes/user';
import { Meters, MeterRead } from '../classes/meter-reads';


@Component({
  selector: 'app-meter-reads',
  templateUrl: './meter-reads.component.html',
  styleUrls: ['./meter-reads.component.scss']
})
export class MeterReadsComponent implements OnInit {

  userService: UserService;
  user = new User();
  consts = new Constants();
  http: Http;
  meters: Meters[];
  viewReads = "none";
  reads: MeterRead[];
  oldReads: MeterRead[];

  constructor(http: Http, userService: UserService, private httpClient: HttpClient, ) {
    this.http = http;
    this.userService = userService;

  }

  ngOnInit() {
    this.getGoAPI()
  }
  getGoAPI() {
    console.log("in the button function");
    this.http.get("http://localhost:3000/meters").subscribe((data) => this.waitForMeters(data));
  }

  waitForMeters(data:any) {
    console.log("setting this.meters to the response");
    this.meters = JSON.parse(data.text()) as Meters[];
    if (this.meters.length == 0) {
      console.log("Data returned is null");
    } else {
      console.log("finishing waitForHttp");
    }

  }

  setMeter(meter: Meters) {
    this.reads = meter.Reads;
    console.log(meter.Reads);
    if (this.viewReads == "none") {
      this.viewReads = "block";

    } else if (this.reads !== this.oldReads) {
      this.viewReads = "block";

    } else if (this.reads == this.oldReads) {
      this.viewReads = "none";

    }

    this.oldReads = this.reads
  }
}
