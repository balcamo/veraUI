import { environment } from '../../environments/environment';

export class Constants {
  mileageRate = 0.545;
  firstLastDayFood = .75 * 2;
  customer = [];
  employee = [
    { name: "Home", route: "./home", icon: "fa fa-home" },
    { name: "Travel", route: "./travel", icon: "fa fa-plane" },
  ];
  crew = [];

  // Change url based on what server deploying to
  url = 'https://bigfoot.verawp.local/api/';
  //url = 'https://localhost:64154/api/';
}
