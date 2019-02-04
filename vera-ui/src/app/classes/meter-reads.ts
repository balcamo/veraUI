
export class Meters
{
  MeterNumber:string;
  Manufactuer:string;
  ModelNum: string;
  SerialNum:string;
  Substation:string;
  Feeder:string;
  Address:string;
  Location:string;
  BillingType:string;
  BillingCycle:string;
  Multiplier:string;
  Route:string;
  LogDate:string;
  Endpoint: string;

  checkIncludes(filterVal: string) {
    if (this.MeterNumber.includes(filterVal) || this.Manufactuer.includes(filterVal) ||
      this.ModelNum.includes(filterVal) || this.SerialNum.includes(filterVal) ||
      this.Substation.includes(filterVal) || this.Feeder.includes(filterVal) ||
      this.Address.includes(filterVal) || this.Location.includes(filterVal) ||
      this.BillingType.includes(filterVal) || this.BillingCycle.includes(filterVal) ||
      this.Multiplier.includes(filterVal) || this.Route.includes(filterVal) ||
      this.LogDate.includes(filterVal) || this.Endpoint.includes(filterVal)) {
      return true;
    } else {
      return false
    }
  }
}

export class MeterRead
{
  Date: number;
  Kwh: number;
}
