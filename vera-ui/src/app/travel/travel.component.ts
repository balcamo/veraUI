import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-travel',
  templateUrl: './travel.component.html',
  styleUrls: ['./travel.component.scss']
})
export class TravelComponent implements OnInit {

  authDisplay = "none";
  constructor() { }

  ngOnInit() {
  }
  /**
   * This function will toggle the display of
   * the travel authorization form*/
  displayAuth() {
    if (this.authDisplay == "none") {
      this.authDisplay = "block";
    } else {
      this.authDisplay = "none"
    }
  }
}
