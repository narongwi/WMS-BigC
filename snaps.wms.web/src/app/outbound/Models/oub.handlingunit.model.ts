export class handerlingunit {
  orgcode: string;
  site: string;
  depot: string;
  spcarea: string;
  hutype: string;
  huno: string;
  loccode: string;
  thcode: string;
  routeno: string;
  mxsku: number;
  mxweight: number;
  mxvolume: number;
  crsku: number;
  crweight: number;
  crvolume: number;
  crcapacity: number;
  tflow: string;
  datecreate : Date | string | null; 
  accncreate: string;
  datemodify : Date | string | null; 
  accnmodify: string;
  procmodfiy: string;
  priority: number;
  promo: string;
  thname: string;
  hutypename: string;
  tflowname: string;
}
export class handerlingunit_gen extends handerlingunit {
  quantity: number;
  isallstore:number;
}

export class handerlingunit_item {
  prepno:string; 
  inorder:string;
  ouorder:string;
  loccode:string; 
  article: string;
  pv: number;
  lv: number;
  qtysku: number;
  qtypu: number;
  qtyweight: number;
  qtyvolume: number;
  descalt: string;
  batchno: string; 
  lotno: string; 
  datemfg: Date | string | null; 
  dateexp: Date | string | null; 
  serialno: string; 
  unitprep: string;
  tflow:string;
  ouln:string;
}
