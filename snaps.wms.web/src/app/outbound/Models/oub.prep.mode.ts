import { outbound_ls } from './oub.order.model';

export class prep_ls {
  orgcode: string;
  site: string;
  depot: string;
  spcarea: string;
  routeno: string;
  huno: string;
  preptype: string;
  prepno: string;
  prepdate: Date | string | null;
  priority: number;
  thcode: string;
  spcorder: string;
  spcarticle: string;
  tflow: string;
  capacity: number;
  thname: string;
  picker: string;
  preppct: number;
  preptypename: string;
  przone: string;
}
export class prep_pm {
  orgcode: string;
  site: string;
  depot: string;
  spcarea: string;
  routeno: string;
  huno: string;
  preptype: string;
  prepno: string;
  prepdate: Date | string | null;
  priority: number;
  thcode: string;
  spcorder: string;
  spcarticle: string;
  tflow: string;
  capacity: number;
  dateassign: Date | string | null;
  deviceID: string;
  oupromo: string;
  dateprepfrom: Date | string | null;
  dateprepto: Date | string | null;
  dateorderfrom: Date | string | null;
  dateorderto: Date | string | null;

  ouflag: string;
  ouorder: string;
  article: string;
  setno: string;
}
export interface prep_ix extends prep_ls {}
export interface prep_md extends prep_ls {
  dateassign: Date | string | null;
  datestart: Date | string | null;
  dateend: Date | string | null;
  deviceID: string;
  picker: string;
  datecreate: Date | string | null;
  accncreate: string;
  datemodify: Date | string | null;
  accnmodify: string;
  procmodify: string;

  lines: prln_md[];
}

export interface prep_stock {
  orgcode: string;
  site: string;
  depot: string;
  article: string;
  pv: number;
  lv: number;
  loccode: string;
  datemfg: Date | string | null;
  dateexp: Date | string | null;
  batchno: string;
  lotno: string;
  stockid: number;
  huno: string;
  qtysku: number;
  qtypu: number;
  serialno: string;
  spcarea: string;
}

export class prln_ls {
  orgcode: string;
  site: string;
  depot: string;
  spcarea: string;
  article: string;
  description: string;
  qtyskuorder: number;
  qtypuorder: number;
  qtypuops: number;
  tflow: string;
  datemodify: Date | string | null;
  unitprep: string;
  unitname: string;
  qtyskuops: number;
  prepno: string;
  prepln: number;
  preptypeops: string;
  preplineops: number;
}
export class prln_pm extends prln_ls {}
export class prln_ix extends prln_ls {}
export class prln_md extends prln_ls {
  huno: string;
  hunosource: string;
  loczone: string;
  loccode: string;
  locseq: number;
  locdigit: string;
  ouorder: string;
  ouln: number;
  barcode: string;
  pv: number;
  lv: number;
  stockid: number;
  qtyweightorder: number;
  qtyvolumeorder: number;
  qtyweightops: number;
  qtyvolumeops: number;
  batchno: string;
  lotno: string;
  datemfg: Date | string | null;
  dateexp: Date | string | null;
  serialno: string;
  picker: string;
  datepick: Date | string | null;
  devicecode: string;
  datecreate: Date | string | null;
  accncreate: string;
  accnmodify: string;
  procmodify: string;
  rtoskuofpu: number;
  skipdigit: string;
}

export class prepset {
  orgcode: string;
  site: string;
  depot: string;
  spcarea: string;
  setno: string;
  datestart: Date | string | null;
  datefinish: Date | string | null;
  opsperform: string;
  opsorder: number;
  datecreate: Date | string | null;
  accncreate: string;
  procmodify: string;
  orders: prepsln[] = new Array();
  slcouorder: string;
  distb: outbound_ls[] = new Array();
}
export class prepsln {
  orgcode: string;
  site: string;
  depot: string;
  spcarea: string;
  routeno: string;
  thcode: string;
  ouorder: string;
  ouln: number;
  barcode: string;
  article: string;
  pv: number;
  lv: number;
  unitprep: string;
  qtyskuorder: number;
  qtypuorder: number;
  qtyweightorder: number;
  qtyvolumeorder: number;
  qtyskuops: number;
  qtypuops: number;
  qtyweightops: number;
  qtyvolumeops: number;
  batchno: string;
  lotno: string;
  datemfg: Date | string | null;
  dateexp: Date | string | null;
  serialno: string;
  description: string;
  result: string;
  tflow: string;
  errmsg: string;
}

export interface ouselect {
  orgcode: string;
  site: string;
  depot: string;
  spcarea: string;
  ouorder: string;
  outype: string;
  ousubtype: string;
  thcode: string;
  selected: number;
  selectdate: string | null;
  selectaccn: string;
  selectflow: string;
}
