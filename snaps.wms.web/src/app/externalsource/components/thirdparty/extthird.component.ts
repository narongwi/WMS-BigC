import { Component, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../../../auth/services/auth.service';
import { lov } from '../../../helpers/lov';
import { externalSourcelService } from '../../services/app-external.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { shareService } from 'src/app/share.service';
import { extbarsource } from '../barcode/extbar.source';
declare var $: any;
@Component({
  selector: 'app-extthirdparty',
  templateUrl: 'extthird.component.html'

})
export class extthirdpartyComponent implements OnInit {
  @ViewChild('line') line:extbarsource;
  //List file update
  public files: any = [];
  //PageNavigate
  public lsrowlmt:lov[] = new Array();
  public pageSize = 200;
  //Date format
  public dateformat:string;
  public dateformatlong:string;
  //Tab
  public crtab:number = 1;

  //Upload process 
  fileUploadForm : FormGroup;
  fileInputLabel : string;

  //Filter file format 
  fileformat = ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-excel','application/json'];
  constructor(private sv: externalSourcelService, 
      private av: authService, 
      private ss: shareService,
      private toastr: ToastrService,
      private formBuilder : FormBuilder ) { 
  this.ss.ngSetup(); this.av.retriveAccess();
  this.dateformat = this.av.crProfile.formatdate;
  this.dateformatlong = this.av.crProfile.formatdatelong;
  }

  ngOnInit(): void {
    this.fileUploadForm = this.formBuilder.group({ myFile:[''] });
   }

  ngAfterViewInit(){  this.setupJS(); /*setTimeout(this.toggle, 1000);*/   }
  setupJS() { 
    // sidebar nav scrolling
    $('#accn-list .sidebar-scroll').slimScroll({
      height: '95%',
      wheelStep: 5,
      touchScrollStep: 50,
      color: '#cecece'
    });   
  }
  //toggle(){ $('.snapsmenu').click();  }

  meUpload(){ 
    // const formData = new FormData();
    // formData.append('formFile', this.fileUploadForm.get('myfile').value);
    // this.sv.uploadbarcode(formData,this.ngDecFiletype(file.type)).subscribe(            
    //   (res) => { this.toastr.success("Upload completed");  },
    //   (err) => { this.toastr.error((err.error == undefined) ? err.message : err.error.message);  },
    //   () => { } 
    // );
  }

  meUploadEvent(event){
    if (event.length > 0) {
      const  file = event[0];  
      if (!this.fileformat.includes(file.type)) {
        this.toastr.warning("We support excel file only ");
      } else {
        const formData = new FormData();
        formData.set('file', file, file.filename);
        this.sv.uploadTHParty(formData,this.ngDecFiletype(file.type)).subscribe(            
          (res) => { this.toastr.success("Upload completed");  this.line.meFnd(); this.crtab = 1; },
          (err) => { this.toastr.error((err.error == undefined) ? err.message : err.error.message);  },
          () => { } 
        );
      }
    }  
  }
  meUnload(index) { this.files.splice(index, 1); }
  
  ngDecFiletype(o:string) { 
    switch(o){
      case "application/vnd.ms-excel" : return "csv";
      case "application/json" : return "json";
      default : return "excel"
    }
  }

  ngOnDestroy():void {  
    this.files = null;          delete this.files;
    this.lsrowlmt = null;       delete this.lsrowlmt;
    this.pageSize = null;       delete this.pageSize;
    this.dateformat = null;     delete this.dateformat;
    this.dateformatlong = null; delete this.dateformatlong;
    this.crtab = null;          delete this.crtab;
  }

}
