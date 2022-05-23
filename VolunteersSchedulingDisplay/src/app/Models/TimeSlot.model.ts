import { Time } from "@angular/common";

export class TimeSlot
{
  time_slot_code!:number;
  day_of_week!:number;
  start_at_date!:Date;
  end_at_date!:Date;
  start_at_hour!:number;
  end_at_hour!:number;
}
