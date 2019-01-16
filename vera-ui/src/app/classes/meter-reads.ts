
export class Meters
{
  MeterNumber: number;
  Endpoint: number;
  Reads: MeterRead[];
  Substation: number;
  Freq: number;
  MaxkW: number;
  Phase: string;
}

export class MeterRead
{
  Date: number;
  Kwh: number;
}
