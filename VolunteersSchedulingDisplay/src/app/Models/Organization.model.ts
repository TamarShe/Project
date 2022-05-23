export class Organization
{
  org_code!:number;
  org_name!:string;
  org_platform!:string;
  need_scheduling!:boolean;
  org_min_age!:number;
  activity_start_date!:Date;
  activity_end_date!:Date;
  avg_volunteering_time?:number;
}
