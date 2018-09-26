import { environment } from '../../environments/environment';

export class Constants {
  mileageRate = 0.545;
  firstLastDayFood = .75 * 2;
  customer = [];
  employee = [
    { name: "Home", route: "./home", icon: "../../assets/HomeBrown.png" },
    { name: "Travel", route: "./travel", icon: "../../assets/TravelIconBrown.png" },
  ];
  crew = [];

  // Change url based on what server deploying to
  url = 'https://bigfoot.verawp.local/api/';
  //url = 'https://localhost:64154/api/';
}
