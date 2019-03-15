import { environment } from '../../environments/environment';

export class Constants {
  mileageRate = 0.545;
  travelDayFood = .75 ;
  customer = [];
  employee = [
    { name: "Home", route: "./home", icon: "../../assets/HomeBrown.png" },
    //{ name: "Travel", route: "./travel", icon: "../../assets/TravelIconBrown.png" },
    { name: "Meter Reads", route: "./meter-reads", icon: "../../assets/2hives2bee.png" },
  ];
  crew = [];

  // Change url based on what server deploying to
  //url = 'https://bigfoot.verawp.local/api/';
  url = 'https://bigfoot.verawaterandpower.com/api/';
  //url = 'https://localhost:64154/api/';

  meter_categories = ['Meter Number', 'Address', 'Substation', 'Meter Location', 'Billing Type', 'Multiplier', 'Last Logging Date YYYY-MM-DD', 'Endpoint','Billing Cycle','Usage'];
}
