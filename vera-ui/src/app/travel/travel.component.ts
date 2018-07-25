import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-travel',
  templateUrl: './travel.component.html',
  styleUrls: ['./travel.component.scss']
})
export class TravelComponent implements OnInit {

  authDisplay = "none";
  constructor(private router: Router) { }

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
  reloadOverwrite() {
    console.log("this should print on refresh");
    this.router.navigate(['/'], { replaceUrl: true });
  }
}
