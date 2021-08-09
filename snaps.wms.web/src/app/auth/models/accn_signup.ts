

export class accn_signup { 
    accncode: string; 
    accnname: string;
    accnsurname: string; 
    email: string; 
    password: string;
    lang: string;
    accstoken: string;
    passwordcn : string;
    site:string;
}

export class accn_acs { 
    public accscode: string; 
    public accnkey: string;
    constructor(token:string,accn:string) { this.accscode = token; this.accnkey = accn;  }
    
    public get getToken() { return this.accscode; }
    public get getKey() { return this.accnkey; }
}

