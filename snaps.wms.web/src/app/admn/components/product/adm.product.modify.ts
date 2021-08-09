import { Component, OnInit, Input, Output, OnDestroy } from '@angular/core';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { adminService } from '../../../admn/services/account.service';
import { lov } from '../../../helpers/lov';
import { product_ls, product_md, product_pm } from '../../models/adm.product.model';
import { admproductService } from '../../services/adm.product.service';
import { ThrowStmt } from '@angular/compiler';
import { NgbDateAdapter, NgbDateParserFormatter, NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/helpers/ngx-bootstrap.config';
import { textChangeRangeIsUnchanged } from 'typescript';
import { shareService } from 'src/app/share.service';
import { barcode_ls, barcode_pm } from '../../models/adm.barcode.model';
import { admbarcodeService } from '../../services/adm.barcode.service';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { route_thsum } from 'src/app/outbound/Models/oub.route.model';
import { ConvertActionBindingResult } from '@angular/compiler/src/compiler_util/expression_converter';
@Component({
  selector: 'appadm-productmodify',
  templateUrl: 'adm.product.modify.html',
  styles: ['.dg4line {  height:247px !important; } ','.dglines { height:138px !important; }'],
  providers: [NgbPaginationConfig,
    {provide: NgbDateAdapter, useClass: CustomAdapter},
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}] 

})
export class admproductmodifyComponent implements OnInit, OnDestroy {
   //@Input() crproduct: product_md;
     @Input() IsWritable: boolean;

    ison:boolean=false;
    public lsproduct:product_ls[] = new Array();
    
    public crproduct:product_md = new product_md();
    public pmproduct:product_pm = new product_pm();

    public slctype:lov = new lov(); // select type
    public slctypemanage:lov = new lov(); // Selective type manage
    public slcunitmanage:lov = new lov(); // Seletive unit stock
    public slcunitprep:lov = new lov(); // Selective unit preparartion
    public slcunitreceipt:lov = new lov(); // Selective unit receipt
    public slcunitdesc:lov = new lov(); // Selective unit desceriptive
    public slcroomtype:lov = new lov(); // Selective room type
    public slchutype:lov = new lov(); //selective hu

    public slczoneputway:lov = new lov(); //Putwayt zone 
    public slczoneprep:lov = new lov(); //Preparation zone
    public slczonedist:lov = new lov();//Distribution zone
    public slcsharedist:lov = new lov();

    public msdstate:lov[] = new Array();
    public msdtype:lov[] = new Array();
    public msdtypemanage:lov[] = new Array();
    public msdstockunit:lov[] = new Array();
    public msdstockdesc:lov[] = new Array();
    public msdunitdesc:lov[] = new Array();
    public msdroomtype:lov[] = new Array();
    public msdhutype:lov[] = new Array();
    public msdzone:lov[]      = new Array();   //Zone of storage 
    public msdzoneprep:lov[]  = new Array();   //Zone of preparation
    public msdzonedist:lov[]  = new Array();   //Zone of distribute
    public msdsharedist:lov[] = new Array();   //Share of distribution

    public crstate:boolean = true;
    public spcareadsc:string;
    public formatdatelong:string;

    public pmbarcode:barcode_pm = new barcode_pm();
    public lsbarcode:barcode_ls[] = new Array();

    //public IsWritable:boolean = true;
    public allowchangehirachy:boolean;
    public allowchangedimension:boolean;
    public allowchangedlc:boolean;
    public allowchangeunit:boolean;
    public allowchangebarcode:boolean;
    
    constructor(
        private sv: admproductService,
        private av: authService,
        private mv: shareService,
        private bv: admbarcodeService,
        private toastr: ToastrService,
        private ngPopups: NgPopupsService) {
        this.av.retriveAccess(); 
        this.pmproduct.orgcode = this.av.crProfile.orgcode;
        this.pmproduct.site = this.av.crRole.site;
        this.pmproduct.depot = this.av.crRole.depot;
        this.formatdatelong = this.av.crProfile.formatdatelong;
        //this.crproduct = new product_md();
        this.getmaster();
    }

    ngOnInit() { }

    ngAfterViewInit(){ } 
    ngselccmpare(item, selected) { return item.value === selected.value } //compare selected object with ng-select
    ngselcstockdesc(){ this.msdstockdesc = this.msdunitdesc.filter(x=>x.valopnfirst == this.slcunitmanage.value); }
    ngsetmodify(o:product_md){ 
      this.crproduct = o;this.fndBarcode();
      this.crstate = (this.crproduct.tflow == "IO") ? true : false;
      this.slchutype = (this.crproduct.hucode != "") ? this.msdhutype.find(x=>x.value == this.crproduct.hucode) : null;
      this.slctype = (this.crproduct.hucode != "") ? this.msdtype.find(x=>x.value == this.crproduct.articletype) : null;
      this.slctypemanage = (this.crproduct.typemanage != "") ? this.msdtypemanage.find(x=>x.value == this.crproduct.typemanage) : null;
      this.slcunitmanage = (this.crproduct.unitmanage != "") ? this.msdstockunit.find(x=>x.value == this.crproduct.unitmanage) : null;
      this.slcunitreceipt = (this.crproduct.unitreceipt != "") ? this.msdstockunit.find(x=>x.value == this.crproduct.unitreceipt) : null;
      this.slcunitprep = (this.crproduct.unitprep != "") ? this.msdstockunit.find(x=>x.value == this.crproduct.unitprep) : null;
      this.slcunitdesc = (this.crproduct.unitdesc != "") ? this.msdunitdesc.find(x=>x.value == this.crproduct.unitdesc) : null;
      this.slczonedist = (this.crproduct.spcdistzone != "") ? this.msdzonedist.find(x=>x.value == this.crproduct.spcdistzone) : null;
      this.slczoneprep = (this.crproduct.spcprepzone != "") ? this.msdzoneprep.find(x=>x.value == this.crproduct.spcprepzone) : null;
      this.slczoneputway = (this.crproduct.spcrecvzone != "") ? this.msdzone.find(x=>x.value == this.crproduct.spcrecvzone) : null;
      this.slcroomtype = (this.crproduct.roomtype != "") ? this.msdroomtype.find(x=>x.value == this.crproduct.roomtype) : null;
      this.slcsharedist = (this.crproduct.spcdistshare != "") ? this.msdsharedist.find(e=>e.value == this.crproduct.spcdistshare) : null;
    }

    getmaster() { 
      Promise.all([
        this.mv.lov("ALL","FLOW").toPromise(), 
        this.mv.lov("ARTICLE","ROOMTYPE").toPromise(), 
        this.mv.hu().toPromise(),
        this.mv.storagezone().toPromise(),
        this.mv.prepzonestock().toPromise(),
        this.mv.prepzonedist().toPromise(),
        this.mv.sharedist().toPromise(),
        this.mv.lov("ARTICLE","TYPE").toPromise(),
        this.mv.lov("ARTICLE","MANAGE").toPromise(),
        this.mv.lov("UNIT","KEEP").toPromise(),
        this.mv.lov("UNIT","STOCKDESC").toPromise()

      ]).
      then((res) => {
        this.msdstate = res[0];
        this.msdstate.push({ value : 'NW', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
        this.msdstate.push({ value : 'false', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});

        this.msdroomtype = res[1];
        this.msdhutype = res[2];
        this.msdzone = res[3];
        this.msdzoneprep = res[4];
        this.msdzonedist = res[5];
        this.msdsharedist = res[6];
        this.msdtype = res[7];
        this.msdtypemanage = res[8];
        this.msdstockunit = res[9];
        this.msdunitdesc = res[10];

      });


      // this.mv.lov("ALL","FLOW").pipe().subscribe(
      //   (res) => { this.msdstate = res;
      //     this.msdstate.push({ value : 'NW', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
      //     this.msdstate.push({ value : 'false', desc : 'New product', icon : 'fas fa-plus-square text-primary', valopnfirst : '', valopnsecond : '', valopnthird :'', valopnfour:''});
      //   },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );

      // this.mv.lov("ARTICLE","ROOMTYPE").pipe().subscribe(
      //   (res) => { this.msdroomtype = res;  },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );

      // this.mv.hu().pipe().subscribe(
      //   (res) => { this.msdhutype = res;  },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );

      // this.mv.storagezone().pipe().subscribe(
      //   (res) => { this.msdzone = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );

      // this.mv.prepzonestock().pipe().subscribe(
      //   (res) => { this.msdzoneprep = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );

      // this.mv.prepzonedist().pipe().subscribe(
      //   (res) => { this.msdzonedist = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
      // this.mv.sharedist().pipe().subscribe((res)=> { this.msdsharedist = res; });

      // this.mv.lov("ARTICLE","TYPE").pipe().subscribe(
      //   (res) => { this.msdtype = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );

      // this.mv.lov("ARTICLE","MANAGE").pipe().subscribe(
      //   (res) => { this.msdtypemanage = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );

      // this.mv.lov("UNIT","KEEP").pipe().subscribe(
      //   (res) => { this.msdstockunit = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );
      
      // this.mv.lov("UNIT","STOCKDESC").pipe().subscribe(
      //   (res) => { this.msdunitdesc = res; },
      //   (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
      //   () => { }
      // );            

    }


    dscicon(o:string) { 
      return this.msdstate.find(x=>x.value == o).icon;
    }
    newproduct() { 
        this.crproduct.orgcode = this.av.crProfile.orgcode;
        this.crproduct.site = this.av.crRole.site;
        this.crproduct.depot = this.av.crRole.depot;

        this.crproduct.spcarea = "";
        this.crproduct.article = "";
        this.crproduct.articletype = "";
        this.crproduct.pv= 0;
        this.crproduct.lv= 0;
        this.crproduct.descalt = "";
        this.crproduct.thcode = "";
        this.crproduct.tflow = "";
        this.crproduct.cronhand = 0;
        this.crproduct.cronblock = 0;
        this.crproduct.crincmng= 0;
        this.crproduct.croudeliv= 0;	
        this.crproduct.description = "";
        this.crproduct.dlcall= 0;
        this.crproduct.dlcfactory= 0;
        this.crproduct.dlcwarehouse= 0;
        this.crproduct.dlcshop= 0;
        this.crproduct.dlconsumer= 0;
        this.crproduct.hdivison = "";
        this.crproduct.hdepartment = "";
        this.crproduct.hsubdepart = "";
        this.crproduct.hclass = "";
        this.crproduct.hsubclass = "";
        this.crproduct.typemanage = "";
        this.crproduct.unitmanage = "";
        this.crproduct.unitreceipt = "";
        this.crproduct.unitprep = "";
        this.crproduct.unitsale = "";
        this.crproduct.unitstock = "";
        this.crproduct.unitdesc = "";
        this.crproduct.unitweight = "";
        this.crproduct.unitdimension = "";
        this.crproduct.unitvolume = "";
        this.crproduct.hucode = "";
        this.crproduct.skulength= 0;
        this.crproduct.skuwidth= 0;
        this.crproduct.skuheight= 0;
        this.crproduct.skuweight= 0;
        this.crproduct.skuvolume= 0;
        this.crproduct.rtoskuofpu= 0;
        this.crproduct.rtoskuofipck= 0;
        this.crproduct.rtoskuofpck= 0;
        this.crproduct.rtoskuoflayer= 0;
        this.crproduct.rtoskuofhu= 0;
        this.crproduct.innaturalloss= 0;
        this.crproduct.ounaturalloss= 0;
        this.crproduct.costavg= 0;
        this.crproduct.pulength= 0;
        this.crproduct.puwidth= 0;
        this.crproduct.puheight= 0;
        this.crproduct.pugrossweight= 0;
        this.crproduct.puweight= 0;
        this.crproduct.puvolume= 0;
        this.crproduct.ipcklength= 0;
        this.crproduct.ipckwidth= 0;
        this.crproduct.ipckheight= 0;
        this.crproduct.ipckgrossweight= 0;
        this.crproduct.ipckweight= 0;
        this.crproduct.ipckvolume= 0;
        this.crproduct.pcklength= 0;
        this.crproduct.pckwidth= 0;
        this.crproduct.pckheight= 0;
        this.crproduct.pckgrossweight= 0;
        this.crproduct.pckweight= 0;
        this.crproduct.pckvolume= 0;
        this.crproduct.layerlength= 0;
        this.crproduct.layerwidth= 0;
        this.crproduct.layerheight= 0;
        this.crproduct.layergrossweight= 0;
        this.crproduct.layerweight= 0;
        this.crproduct.layervolume= 0;
        this.crproduct.hulength= 0;
        this.crproduct.huwidth= 0;
        this.crproduct.huheight= 0;
        this.crproduct.hugrossweight= 0;
        this.crproduct.huweight= 0;
        this.crproduct.huvolume= 0;
        this.crproduct.tempmin= 0;
        this.crproduct.tempmax= 0;
        this.crproduct.isdangerous= false;
        this.crproduct.ishighvalue= false;;
        this.crproduct.isfastmove= false;
        this.crproduct.isslowmove= false;
        this.crproduct.isprescription= false;
        this.crproduct.isdlc= false;
        this.crproduct.ismaterial= false;
        this.crproduct.isunique= false;
        this.crproduct.isalcohol= false;
        this.crproduct.istemperature= false;
        this.crproduct.ismixingprep= false;
        this.crproduct.isfinishgoods= false;
        this.crproduct.isnaturalloss = false;
        this.crproduct.alcmanage = "";
        this.crproduct.alccategory = "";
        this.crproduct.alccontent = "";
        this.crproduct.alccolor = "";
        this.crproduct.dangercategory = "";
        this.crproduct.dangerlevel = "";
        this.crproduct.laslotno = "";
        this.crproduct.lasdatemfg = new Date(); 
        this.crproduct.lasdateexp = new Date();
        this.crproduct.lasserialno = "";
        this.crproduct.datecreate = new Date();
        this.crproduct.accncreate = "";
        this.crproduct.datemodify = new Date();
        this.crproduct.accnmodify = "";
        this.crproduct.procmodify = "";

        this.slcunitmanage = null;
        this.slctype = null;
        this.slctypemanage = null;
        this.slcunitdesc = null;
        this.slcunitprep = null;
        this.slcunitreceipt = null;

        this.crproduct.rtoskuofipck = 0;
        this.crproduct.rtoskuofpck = 0;
        this.crproduct.rtoskuoflayer = 0;
        this.crproduct.rtoskuofhu = 0;        
        
        this.toastr.info("<span class='fn-07e'>New product is ready to setup</span>",null,{ enableHtml : true });
        //console.log(this.dscloc(this.crproduct.lsformat,"AA","","","",""));
    }

    decbool(o:boolean):number {  return (o==true) ? 1 : 0 ; }
    encbool(o:number):boolean { return (o==1)? true : false; }
    selproduct(o:product_ls){ 
      this.sv.get(o).pipe().subscribe(            
        (res) => { 
          this.crproduct = res; 
          this.crstate = (this.crproduct.tflow == "IO") ? true : false;

          //switch box assignment
          // this.crisdangerous = this.encbool(this.crproduct.isdangerous);
          // this.crishighvalue = this.encbool(this.crproduct.ishighvalue);
          // this.crisfastmove = this.encbool(this.crproduct.isfastmove);
          // this.crisslowmove = this.encbool(this.crproduct.isslowmove);
          // this.crisprescription = this.encbool(this.crproduct.isprescription);
          // this.crisdlc = this.encbool(this.crproduct.isdlc);
          // this.crismaterial = this.encbool(this.crproduct.ismaterial);
          // this.crisunique = this.encbool(this.crproduct.isunique);
          // this.crisalcohol = this.encbool(this.crproduct.isalcohol);
          // this.cristemperature = this.encbool(this.crproduct.istemperature);
          // this.crismixingprep = this.encbool(this.crproduct.ismixingprep);
          // this.crisfinishgoods = this.encbool(this.crproduct.isfinishgoods);
          // this.crisnaturalloss = this.encbool(this.crproduct.isnaturalloss);
          // this.crismeasurement = this.encbool(this.crproduct.ismeasurement);

          
          //LOV assignment '
          this.slchutype = (this.crproduct.hucode != "") ? this.msdhutype.find(x=>x.value == this.crproduct.hucode) : null;
          this.slctype = (this.crproduct.hucode != "") ? this.msdtype.find(x=>x.value == this.crproduct.articletype) : null;
          this.slctypemanage = (this.crproduct.typemanage != "") ? this.msdtypemanage.find(x=>x.value == this.crproduct.typemanage) : null;
          this.slcunitmanage = (this.crproduct.unitmanage != "") ? this.msdstockunit.find(x=>x.value == this.crproduct.unitmanage) : null;
          this.slcunitreceipt = (this.crproduct.unitreceipt != "") ? this.msdstockunit.find(x=>x.value == this.crproduct.unitreceipt) : null;
          this.slcunitprep = (this.crproduct.unitprep != "") ? this.msdstockunit.find(x=>x.value == this.crproduct.unitprep) : null;
          this.slcunitdesc = (this.crproduct.unitdesc != "") ? this.msdunitdesc.find(x=>x.value == this.crproduct.unitdesc) : null;
          this.slczonedist = (this.crproduct.spcdistzone != "") ? this.msdzone.find(x=>x.value == this.crproduct.spcdistzone) : null;
          this.slczoneprep = (this.crproduct.spcprepzone != "") ? this.msdzone.find(x=>x.value == this.crproduct.spcprepzone) : null;
          this.slczoneputway = (this.crproduct.spcrecvzone != "") ? this.msdzone.find(x=>x.value == this.crproduct.spcrecvzone) : null;
        },
        (err) => { this.toastr.error(err.error.message); this.ison = false; },
        () => { }
    );
    }

    rounddigit(o:number){       
      return Number(o.toFixed(2));
    }
    caldimension() { 
      
      //Ratio calculate 
      this.crproduct.ipckweight = (this.crproduct.rtoskuofipck != 0) ? this.rounddigit(this.crproduct.rtoskuofipck * this.crproduct.skuweight) : 0 ;
      this.crproduct.rtoskuofipck = (this.crproduct.rtoskuofipck != 0) ? this.crproduct.rtoskuofipck : 0 ;

      this.crproduct.rtoskuofpck = (this.crproduct.rtoipckofpck != 0) ? this.crproduct.rtoipckofpck * this.crproduct.rtoskuofipck : 0 ; 
      this.crproduct.pckweight = (this.crproduct.rtoipckofpck != 0) ? this.rounddigit(this.crproduct.rtoskuofpck * this.crproduct.skuweight) : 0 ;

      this.crproduct.rtoskuoflayer = (this.crproduct.rtopckoflayer != 0) ? this.crproduct.rtopckoflayer * this.crproduct.rtoskuofpck : 0 ; 
      this.crproduct.layerweight =  (this.crproduct.rtopckoflayer != 0) ? this.rounddigit(this.crproduct.rtoskuoflayer * this.crproduct.skuweight) : 0 ;

      this.crproduct.rtoskuofhu = (this.crproduct.rtolayerofhu != 0) ? this.crproduct.rtoskuoflayer * this.crproduct.rtolayerofhu : 0 ; 
      this.crproduct.huweight =  (this.crproduct.rtolayerofhu != 0) ? this.rounddigit(this.crproduct.rtoskuofhu * this.crproduct.skuweight) : 0 ;


      // if(this.crproduct.skulength && this.crproduct.skuwidth && this.crproduct.skuheight) { 
      //   console.log("Cal Dimension");
      //   this.crproduct.skuvolume = this.rounddigit((
      //     parseFloat(this.crproduct.skulength.toString()) * 
      //     parseFloat(this.crproduct.skuwidth.toString()) * 
      //     parseFloat(this.crproduct.skuheight.toString())/1000));

      //   this.crproduct.ipcklength = this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuofipck);
      //   this.crproduct.ipckwidth = this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuofipck);
      //   this.crproduct.ipckheight = this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuofipck);
      //   this.crproduct.ipckvolume = this.rounddigit(this.crproduct.skuvolume * this.crproduct.rtoskuofipck);
        
      //   this.crproduct.pcklength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuofpck);
      //   this.crproduct.pckwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuofpck);
      //   this.crproduct.pckheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuofpck);
      //   this.crproduct.pckvolume = this.rounddigit(this.crproduct.skuvolume * this.crproduct.rtoskuofpck);

      //   this.crproduct.layerlength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuoflayer);
      //   this.crproduct.layerwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuoflayer);
      //   this.crproduct.layerheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuoflayer);
        

      //   this.crproduct.hulength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuofhu);
      //   this.crproduct.huwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuofhu);
      //   this.crproduct.huheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuofhu);
      // }else {
      //   console.log("Not cal Dimension");
      // }

    }
    caldimensionsku() { 
      
      if(this.crproduct.skulength && this.crproduct.skuwidth && this.crproduct.skuheight) { 
        console.log("Cal Dimension");
        this.crproduct.skuvolume = this.rounddigit((
        parseFloat(this.crproduct.skulength.toString()) * 
        parseFloat(this.crproduct.skuwidth.toString()) * 
        parseFloat(this.crproduct.skuheight.toString())/1000));

      
        this.crproduct.layerlength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuoflayer);
        this.crproduct.layerwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuoflayer);
        this.crproduct.layerheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuoflayer);
        

        this.crproduct.hulength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuofhu);
        this.crproduct.huwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuofhu);
        this.crproduct.huheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuofhu);
      }
    }

    caldimensionipck() { 
      
      if(this.crproduct.skulength && this.crproduct.skuwidth && this.crproduct.skuheight) { 
        console.log("Cal Dimension");
        this.crproduct.ipckvolume = this.rounddigit((
          parseFloat(this.crproduct.ipcklength.toString()) * 
          parseFloat(this.crproduct.ipckwidth.toString()) * 
          parseFloat(this.crproduct.ipckheight.toString())/1000));

        this.crproduct.layerlength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuoflayer);
        this.crproduct.layerwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuoflayer);
        this.crproduct.layerheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuoflayer);
        
        this.crproduct.hulength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuofhu);
        this.crproduct.huwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuofhu);
        this.crproduct.huheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuofhu);
      }
    }

    caldimensionpck() { 
      
      if(this.crproduct.skulength && this.crproduct.skuwidth && this.crproduct.skuheight) { 
        console.log("Cal Dimension");
        this.crproduct.pckvolume = this.rounddigit((
          parseFloat(this.crproduct.pcklength.toString()) * 
          parseFloat(this.crproduct.pckwidth.toString()) * 
          parseFloat(this.crproduct.pckheight.toString())/1000));     

        this.crproduct.layerlength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuoflayer);
        this.crproduct.layerwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuoflayer);
        this.crproduct.layerheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuoflayer);
        

        this.crproduct.hulength =  this.rounddigit(this.crproduct.skulength * this.crproduct.rtoskuofhu);
        this.crproduct.huwidth =  this.rounddigit(this.crproduct.skuwidth * this.crproduct.rtoskuofhu);
        this.crproduct.huheight =  this.rounddigit(this.crproduct.skuheight * this.crproduct.rtoskuofhu);
      }
    }

    caldlc(o:string) { 
      if (o == "factory") { 
        if ((Number(this.crproduct.dlcwarehouse) + Number(this.crproduct.dlcshop) + Number(this.crproduct.dlconsumer) + Number(this.crproduct.dlcfactory) ) > 100 ) { 
          this.toastr.error("DLC of factory is over 100%, please recheck again"); 
          this.crproduct.dlcfactory = 100 - (Number(this.crproduct.dlcwarehouse) + Number(this.crproduct.dlcshop) + Number(this.crproduct.dlconsumer));
        }
      }else if (o == "warehouse") { 
        if ((Number(this.crproduct.dlcfactory) + Number(this.crproduct.dlcshop) + Number(this.crproduct.dlconsumer) + Number(this.crproduct.dlcwarehouse) ) > 100 ) { 
          this.toastr.error("DLC of warehouse is over 100%, please recheck again"); 
          this.crproduct.dlcwarehouse = 100 - (Number(this.crproduct.dlcfactory) + Number(this.crproduct.dlcshop) + Number(this.crproduct.dlconsumer));
        }
      }else if (o == "shop"){
        if ((Number(this.crproduct.dlcfactory) + Number(this.crproduct.dlcwarehouse) + Number(this.crproduct.dlconsumer) + Number(this.crproduct.dlcshop) ) > 100 ) { 
          this.toastr.error("DLC of shop is over 100%, please recheck again"); 
          this.crproduct.dlcshop = 100 - (Number(this.crproduct.dlcfactory) + Number(this.crproduct.dlcwarehouse) + Number(this.crproduct.dlconsumer));
        }
      }else if (o == "consumer") { 
        if ((Number(this.crproduct.dlcfactory) + Number(this.crproduct.dlcwarehouse) + Number(this.crproduct.dlcshop) + Number(this.crproduct.dlconsumer) ) > 100 ) { 
          this.toastr.error("DLC of consumer is over 100%, please recheck again"); 
          this.crproduct.dlconsumer = 100 - (Number(this.crproduct.dlcfactory) + Number(this.crproduct.dlcwarehouse) + Number(this.crproduct.dlcshop));
        }
      }
    }

    validate() {
        this.crproduct.tflow = (this.crstate == true) ? "IO" : "XX";
        this.crproduct.articletype = this.slctype.value;
        this.crproduct.typemanage = this.slctypemanage.value;
        this.crproduct.unitmanage = this.slcunitmanage.value;
        this.crproduct.unitprep = this.slcunitprep.value;
        this.crproduct.unitreceipt = this.slcunitreceipt.value;
        this.crproduct.unitdesc = this.slcunitdesc.value;
        this.crproduct.roomtype = this.slcroomtype.value;
        this.crproduct.hucode = this.slchutype.value;
        this.crproduct.spcrecvzone = this.slczoneputway.value;
        this.crproduct.spcprepzone = this.slczoneprep.value;
        this.crproduct.spcdistzone = this.slczonedist.value;
        this.crproduct.spcdistshare = this.slcsharedist.value;
        this.ngPopups.confirm('Do you accept change configuration of Product ?')
            .subscribe(res => {
                if (res) {
                    this.ison = true;
                    
                    this.sv.upsert(this.crproduct).pipe().subscribe(            
                        (res) => { this.toastr.success("<span class='fn-07e'>Save product success</span>",null,{ enableHtml : true }); this.ison = false; },
                        (err) => { this.toastr.error("<span class='fn-07e'>"+((err.error == undefined) ? err.message : err.error.message)+"</span>",null,{ enableHtml : true });  },
                        () => { }
                    );
                             
                } 
            });
    }
    valaisle(type:string){
      this.mv.valaisle((type=='from') ? this.crproduct.spcrecvaisle : this.crproduct.spcrecvaisleto).toPromise().then((res)=> { 
        if (res == false) { 
          this.toastr.success("<span class='fn-07e'>Aisle incorrect</span>",null,{ enableHtml : true }); 
          if (type=='from'){ 
            this.crproduct.spcrecvaisle = "";
          }else {
            this.crproduct.spcrecvaisleto = "";
          }
        }
      });
    }
    valbay(type:string){
      this.mv.valbay((type=='from') ? this.crproduct.spcrecvbay : this.crproduct.spcrecvbayto).toPromise().then((res)=> { 
        if (res == false) { 
          this.toastr.success("<span class='fn-07e'>Bay incorrect</span>",null,{ enableHtml : true }); 
          if (type=='from'){ 
            this.crproduct.spcrecvbay = "";
          }else {
            this.crproduct.spcrecvbayto = "";
          }
        }
      });
    }
    vallevel(type:string){ 
      this.mv.vallevel((type=='from') ? this.crproduct.spcrecvlevel : this.crproduct.spcrecvlevelto).toPromise().then((res)=> { 
        if (res == false) { 
          this.toastr.success("<span class='fn-07e'>Level incorrect</span>",null,{ enableHtml : true }); 
          if (type=='from'){ 
            this.crproduct.spcrecvlevel = "";
          }else {
            this.crproduct.spcrecvlevelto = "";
          }
        }
      });
    }
    vallocation(type:string){ 
      this.mv.vallevel(this.crproduct.spcrecvlocation).toPromise().then((res)=> { 
        if (res == false) { 
          this.toastr.success("<span class='fn-07e'>Location incorrect</span>",null,{ enableHtml : true }); 
          this.crproduct.spcrecvlocation = "";
        }
      });
    }

    fndBarcode(){
      this.pmbarcode.article = this.crproduct.article;
      this.pmbarcode.pv = this.crproduct.pv;
      this.pmbarcode.lv = this.crproduct.lv;
      this.bv.find(this.pmbarcode).pipe().subscribe( (res) => { this.lsbarcode = res; } );     
    }

    setBarcode(o:barcode_ls) { 
      if (this.allowchangebarcode === false) {
        this.toastr.warning("<span class='fn-07e'>System not allow to set primary</span>",null,{ enableHtml : true });
      }else {
        this.ngPopups.confirm("Do you confirm set barcode to primary select")
        .subscribe((res) => { 
          if(res) {
            this.bv.setPrimary(o).subscribe((res) => { 
              this.toastr.success("<span class='fn-07e'>Set barcode primary success</span>",null,{ enableHtml : true });
            })
          }
        });
      }
    }

    ngDecColor(o:number){ return (o >= 1) ? "text-primary" : "tx-mute"; }
    ngOnDestroy():void { 
      this.lsproduct = null;     delete this.lsproduct;
      this.crproduct = null;     delete this.crproduct;
      this.pmproduct = null;     delete this.pmproduct;
      this.slchutype = null;     delete this.slchutype;
      this.slctypemanage = null; delete this.slctypemanage;
      this.slcunitmanage = null; delete this.slcunitmanage;
      this.slcunitprep = null;   delete this.slcunitprep;
      this.slcunitreceipt = null;delete this.slcunitreceipt;
      this.slcunitdesc = null;   delete this.slcunitdesc;
      this.slcroomtype = null;   delete this.slcroomtype;
      this.slchutype = null;     delete this.slchutype;
      this.slczoneputway = null; delete this.slczoneputway;
      this.slczoneprep = null;   delete this.slczoneprep;
      this.slczonedist = null;   delete this.slczonedist;
      this.msdstate = null;      delete this.msdstate;
      this.msdtype = null;       delete this.msdtype;
      this.msdtypemanage = null; delete this.msdtypemanage;
      this.msdstockunit = null;  delete this.msdstockunit;
      this.msdstockdesc = null;  delete this.msdstockdesc;
      this.msdunitdesc = null;   delete this.msdunitdesc;
      this.msdroomtype = null;   delete this.msdroomtype;
      this.msdhutype = null;     delete this.msdhutype;
      this.msdzone = null;       delete this.msdzone;
      this.msdzoneprep = null;   delete this.msdzoneprep;
      this.msdzonedist = null;   delete this.msdzonedist;
      this.crstate = null;       delete this.crstate;
      this.spcareadsc = null;    delete this.spcareadsc;
      this.lsbarcode = null;     delete this.lsbarcode;
      this.lsbarcode = null;	 delete this.lsbarcode;
    }
}
