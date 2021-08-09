import { correction_md } from "./inv.correction.mode";


export class transfer_md extends correction_md { 
    rsltaskno: string;
    rslhuno: string;
    rslstockid: number;

    sourcelocation: string;
    targetlocation: string;
    inagrn : string;
    ingrno : string;

    stocktflow :string;
}