import { Component, OnInit } from '@angular/core';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Constants } from '../../classes/constants';
import { UserService } from '../../service/app.service.user';
import { User, Auth } from '../../classes/user';
import { Meters, MeterRead } from '../../classes/meter-reads';
import { MeterService } from '../../service/app.service.meters';


@Component({
  selector: 'app-meters',
  templateUrl: './meters.component.html',
  styleUrls: ['./meters.component.scss']
})
export class MetersComponent implements OnInit {



  userService: UserService;
  user = new User();
  consts = new Constants();
  http: Http;
  meterService: MeterService;
  meters: Meters[];
  filteredMeters: Meters[];
  viewReads = "none";
  reads: MeterRead[];
  oldReads: MeterRead[];
  meterSelected = false;
  currentMeter: Meters;
  filterOpts = this.consts.meter_categories;
  filterCategory: string;
  filterString: string;

  constructor(http: Http, userService: UserService, private httpClient: HttpClient, meterService: MeterService) {
    this.http = http;
    this.userService = userService;
    this.meterService = meterService;
    this.meters = this.meterService.getMeterList();
    this.filteredMeters = this.meters;
  }

  ngOnInit() {
    if (this.meters.length == 0) {
      this.getGoAPI();
    } else {
      console.log("the value of meters[0] in init is: " + this.meters[0].MeterNumber);
    }
  }
  getGoAPI() {
    console.log("in the button function");
    //this.http.get("http://vm-apiary.verawp.local:3000/meters").subscribe((data) => console.log(data));
    this.http.get("http://vm-apiary.verawp.local:3000/meters").subscribe((data) => this.waitForMeters(data));
    console.log("after the get");

  }

  waitForMeters(data: any) {
    console.log("setting this.meters to the response");
    this.meters = JSON.parse(data.text()) as Meters[];
    this.meterService.setMeterList(this.meters);
    this.filteredMeters = this.meters;
    if (this.meters.length == 0) {
      console.log("Data returned is null");
    } else {
      console.log("the value of meters[0] is: " + this.meters[0].MeterNumber);
      console.log("finishing waitForHttp");
    }

  }

  setMeter(meter: Meters) {
    this.meterSelected = true;
    this.currentMeter = meter;
    console.log(meter);
    this.oldReads = this.reads
  }

  allMeters() {
    this.meterSelected = false;
  }
  // filter meters based on entered string
  filterData(value: any) {
    this.filterString = value;
    if (this.filterString == "") {
      this.filteredMeters = this.meters
    } else {
      if (this.filterCategory == this.filterOpts[0]) {
        this.filteredMeters = this.meters.filter(s => s.MeterNumber.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[1]) {
        this.filteredMeters = this.meters.filter(s => s.Address.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[2]) {
        this.filteredMeters = this.meters.filter(s => s.Substation.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[3]) {
        this.filteredMeters = this.meters.filter(s => s.Location.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[4]) {
        this.filteredMeters = this.meters.filter(s => s.BillingType.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[5]) {
        this.filteredMeters = this.meters.filter(s => s.Multiplier.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[6]) {
        this.filteredMeters = this.meters.filter(s => s.LogDate.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[7]) {
        this.filteredMeters = this.meters.filter(s => s.Endpoint.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[8]) {
        this.filteredMeters = this.meters.filter(s => s.BillingCycle.includes(this.filterString));
      }
      else if (this.filterCategory == this.filterOpts[9]) {
        this.filteredMeters = this.meters.filter(s => s.BillingCycle.includes(this.filterString));
      } else {
        this.filteredMeters = this.meters.filter(s => (this.checkIncludes(s, this.filterString)));
      }

    }
    console.log(this.filterString);

  }

  checkIncludes(meter: Meters, filterVal: string) {
    if (meter.MeterNumber.includes(filterVal) || meter.Substation.includes(filterVal) ||
      meter.Address.includes(filterVal) || meter.Location.includes(filterVal) ||
      meter.BillingType.includes(filterVal) || meter.BillingCycle.includes(filterVal) ||
      meter.Multiplier.includes(filterVal) || meter.Route.includes(filterVal) ||
      meter.LogDate.includes(filterVal) || meter.Endpoint.includes(filterVal)) {
      return true;
    } else {
      return false
    }
  }

  // designate which colomn things are being filterd on
  filterMeters(value: any) {
    if (value == "undefined") {
      this.filterCategory = "0"
    }
    else {
      this.filterCategory = value;
    }
    console.log("filter cat " + this.filterCategory);
    if (this.filterString != "undefined") {
      this.filterData(this.filterString);
    }
    else {
      this.filteredMeters = this.meters;
    }
  }

}