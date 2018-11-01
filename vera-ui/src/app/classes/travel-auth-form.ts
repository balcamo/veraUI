

export class AuthForm {
  Preparer = false;      // Preparer
  FirstName: string;    // FirstName
  LastName: string;    // LastName
  Phone: number;      // Phone
  Email: string;    // Email
  EventTitle: string;    // EventTitle
  Location: string;    // Location
  TravelBegin: Date;        // TravelBegin
  TravelEnd: Date;        // TravelEnd
  DistVehicle: boolean;     // DistVehicle
  RegistrationCost: number;   // RegistrationCost
  Airfare: number;   // Airfare
  RentalCar: number;   // RentalCar
  Fuel: number;   // Fuel
  ParkingTolls: number;   // ParkingTolls
  Mileage: number;   // Mileage
  Lodging: number;   // Lodging
  PerDiem: number;   // PerDiem
  TravelDays:number  // days of travel
  FullDays: number;  // FullDays of travel
  Misc: number;  // Misc
  MiscExplain: string;    // MiscExplain
  TotalEstimate: number;  // TotalEstimate
  Advance: boolean;     // Advance
  AdvanceAmount: number;  // AdvanceAmount
  AdvanceDate: Date;
  AdvanceStatus: number; // approval status of the advance
  Policy: boolean;     // Policy
  SubmitterSig: number;    // SubmitterSig
  SubmitDate: Date;
  RecapRegistrationCost: number;  // RecapRegistrationCost
  RecapAirfare: number;  // RecapAirfare
  RecapRentalCar: number;  // RecapRentalCar
  RecapFuel: number;  // RecapFuel
  RecapParkingTolls: number;  // RecapParkingTolls
  RecapMileage: number;  // RecapMileage
  RecapMileageAmount: number;  // RecapMileage
  RecapLodging: number;  // RecapLodging
  RecapPerDiem: number;  // RecapPerDiem
  RecapTravelDays: number;  // RecapTravelDays
  RecapFullDays: number;  // RecapFullDays
  RecapMisc: number;  // RecapMisc
  TotalRecap: number;  // TotalRecap
  RecapDate: Date;
  RecapStatus: number; // whether the recap has been approved or not
  TotalReimburse: number;  // TotalReimburse
  ApprovalStatus: string;    // ApprovalStatus
  ApprovalDate: Date;
  DHApproval: string;     // Department head approval DHApproval
  DHApprovalDate: Date;
  DHID: number;    // Department head ID DHID
  DHEmail: string;
  GMApproval: string;     // GM approval GMApproval
  GMApprovalDate: Date;
  GMID: number;   // GMID
  GMEmail: string;
  CloseDate: Date;
  CloseStatus: number;
  MailMessage: string; // message to be sent to the traveler
}
