import { Injectable } from '@angular/core';
import { Meters } from '../classes/meter-reads';
@Injectable()
export class MeterService  {
  private _session: Meters[];

  constructor() {

  }

  public setMeterList(meterList:Meters[]) {
    this._session = meterList;
  }

  public getMeterList(): Meters[] {
    return this._session
  }
}
