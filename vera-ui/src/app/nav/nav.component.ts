import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }
  isIn = false;   // store state
  toggleState() { // click handler
    let bool = this.isIn;
    this.isIn = bool === false ? true : false;
  }
  public isCollapsed = true; //dropDown
  public isNavbarCollapsed = true; //NavBar
  public isIconCollapsed = true; //icon
  public collapse = true;
  public isSidebarCollapse = "none"; //sidebar
  toggle() {
    if (this.isSidebarCollapse == "none") {
      this.isSidebarCollapse = "block";
    } else {
      this.isSidebarCollapse = "none"
    }
  }
}
