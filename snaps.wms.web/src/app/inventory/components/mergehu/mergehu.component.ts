import { AfterViewInit, Component, OnInit, OnDestroy } from '@angular/core';
import { MergehuService } from '../../services/mergehu.service';
import {
	merge_find,
	mergehu_ln,
	mergehu_md,
	merge_set,
	merge_md,
} from '../../models/inv.merge.model';
import { NgPopupsService } from 'ng-popups';
import { ToastrService } from 'ngx-toastr';
import { authService } from 'src/app/auth/services/auth.service';
import {
	NgbDateAdapter,
	NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import {
	CustomAdapter,
	CustomDateParserFormatter,
} from 'src/app/helpers/ngx-bootstrap.config';

// Jquery
declare var $: any;

@Component({
	selector: 'app-mergehu',
	templateUrl: './mergehu.component.html',
	styleUrls: ['./mergehu.component.scss'],
	providers: [
		MergehuService,
		{ provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
})
export class MergehuComponent implements OnInit, AfterViewInit, OnDestroy {
	public lsfn: merge_find = {};
	public lsmd: mergehu_md[] = [];
	public lnmd: mergehu_ln[] = [];
	public mdfn: merge_find = {};
	public mdsr: mergehu_ln[] = [];
	public mdtr: merge_md = {};
	public mdst: merge_set = {};
	public ctab: number = 1;

	// Date format
	public dateformat: string;
	public dateformatlong: string;
	public datecreate: Date | string | null;
	public lssel: Number;
	public lnsel: Number;
	public srsel: Number;
	public trsel: Number;
	//Toast Ref
	public toastRef: any;
	public txft: string;

	constructor(
		private av: authService,
		private sv: MergehuService,
		private toastr: ToastrService,
		// private mv: shareService,
		private ngPopups: NgPopupsService
	) {
		this.mdtr.lines = [];
		this.dateformat = this.av.crProfile.formatdate;
		this.dateformatlong = this.av.crProfile.formatdateshort;
		this.lsfn.orgcode = this.av.crProfile.orgcode;
		this.lsfn.site = this.av.crRole.site;
		this.lsfn.depot = this.av.crRole.depot;
		this.txft = '';
	}

	ngOnInit(): void { }
	public list() {
		this.sv.list(this.lsfn).subscribe(
			(res: mergehu_md[]) => {
				this.lsmd = res;
			},
			(err) => {
				this.toastr.error(
					"<span class='fn-07e'>" + err.message + '</span>',
					null,
					{ enableHtml: true }
				);
			}
		);
	}
	public line(humd: mergehu_md, ix: number) {
		this.lssel = ix;
		this.lnsel = -1;
		this.sv.line(humd).subscribe(
			(res: mergehu_ln[]) => {
				this.lnmd = res;
			},
			(err) => {
				this.toastr.error(
					"<span class='fn-07e'>" + err.message + '</span>',
					null,
					{ enableHtml: true }
				);
			}
		);
	}
	public linesl(ix: number) {
		this.lnsel = ix;
	}
	public srcsel(ix: number) {
		this.srsel = ix;
	}
	public tarsel(ix: number) {
		this.trsel = ix;
	}
	public edithu(ix: number) {
		if (this.lsmd[ix].tflow == 'IO') {
			this.mdtr = {};
			this.mdtr = Object.assign([], this.lsmd[ix]);
			this.mdst.orgcode = this.mdtr.orgcode;
			this.mdst.site = this.mdtr.site;
			this.mdst.depot = this.mdtr.depot;
			this.mdst.spcarea = this.mdtr.spcarea;
			this.mdst.huno = this.mdtr.hutarget;
			this.mdst.hutype = this.mdtr.hutype;
			this.mdst.loccode = this.mdtr.loccode;
			this.mdst.accncode = this.mdtr.accnmodify;
			this.mdst.mergeno = this.mdtr.mergeno;
			this.mdtr.lines = [];
			this.ctab = 2;
		} else {
			this.toastr.warning(
				"<span class='fn-07e'> HU status is " +
				this.lsmd[ix].tflowdes +
				' </span>',
				null,
				{ enableHtml: true }
			);
		}
	}
	public find() {
		this.sv.find(this.mdfn).subscribe(
			(res: mergehu_ln[]) => {
				this.mdsr = res;
			},
			(err) => {
				this.toastr.error(
					"<span class='fn-07e'>" + err.message + '</span>',
					null,
					{ enableHtml: true }
				);
			}
		);
	}
	public label() {
		if (this.mdtr.hutarget == null) {
			this.toastr.error(
				"<span class='fn-07e'>Please Generate Target HU !</span>",
				null,
				{ enableHtml: true }
			);
		} else if (this.mdtr.tflow == 'IO') {
			this.toastr.error(
				"<span class='fn-07e'>HU is waiting confirm !</span>",
				null,
				{ enableHtml: true }
			);
		} else if (this.mdtr.tflow == 'CL') {
			this.toastr.error("<span class='fn-07e'>HU is cancelled!</span>", null, {
				enableHtml: true,
			});
		} else {
			this.toastRef = this.toastr.warning(
				" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",
				null,
				{
					disableTimeOut: true,
					tapToDismiss: false,
					closeButton: false,
					positionClass: 'toast-bottom-right',
					enableHtml: true,
				}
			);
			this.sv
				.label(
					this.mdst.orgcode,
					this.mdst.site,
					this.mdst.depot,
					this.mdst.huno,
					'M'
				)
				.subscribe(
					(response) => {
						let blob: any = new Blob([response], { type: 'application/pdf' });
						const url = window.URL.createObjectURL(blob);
						let downloadLink = document.createElement('a');

						downloadLink.href = url;
						downloadLink.setAttribute(
							'download',
							'bgcwms_labelhu_' + this.mdst.huno + '.pdf'
						);
						document.body.appendChild(downloadLink);
						downloadLink.click();
						document.body.removeChild(downloadLink);
						this.toastr.clear(this.toastRef.ToastId);
					},
					(error) => {
						this.toastr.clear(this.toastRef.ToastId);
						this.toastr.warning(
							"<span class='fn-07e'> Label Data Not Found </span>",
							null,
							{ enableHtml: true }
						);
					}
				);
		}
	}
	public linklabel(ix: number) {
		let hu = this.lsmd[ix];
		if (hu.tflow == 'IO') {
			this.toastr.error(
				"<span class='fn-07e'>HU is waiting confirm !</span>",
				null,
				{ enableHtml: true }
			);
		} else if (hu.tflow == 'CL') {
			this.toastr.error("<span class='fn-07e'>HU is cancelled!</span>", null, {
				enableHtml: true,
			});
		} else {
			this.toastRef = this.toastr.warning(
				" &#128336; <span class='fn-07e'>Downloading ..... , Claim down wait a sec</span>",
				null,
				{
					disableTimeOut: true,
					tapToDismiss: false,
					closeButton: false,
					positionClass: 'toast-bottom-right',
					enableHtml: true,
				}
			);
			this.sv.label(hu.orgcode, hu.site, hu.depot, hu.hutarget, 'M').subscribe(
				(response) => {
					let blob: any = new Blob([response], { type: 'application/pdf' });
					const url = window.URL.createObjectURL(blob);
					let downloadLink = document.createElement('a');

					downloadLink.href = url;
					downloadLink.setAttribute(
						'download',
						'bgcwms_labelhu_' + this.mdst.huno + '.pdf'
					);
					document.body.appendChild(downloadLink);
					downloadLink.click();
					document.body.removeChild(downloadLink);
					this.toastr.clear(this.toastRef.ToastId);
				},
				(error) => {
					this.toastr.clear(this.toastRef.ToastId);
					this.toastr.warning(
						"<span class='fn-07e'> Label Data Not Found </span>",
						null,
						{ enableHtml: true }
					);
				}
			);
		}
	}
	public setup() {
		if (this.mdst.loccode == null || this.mdst.loccode == '') {
			this.toastr.error(
				"<span class='fn-07e'>Please Enter Target Location !</span>",
				null,
				{ enableHtml: true }
			);
		} else {
			this.ngPopups.confirm('Do you Generate New HU ?').subscribe((res) => {
				if (res) {
					this.sv.generate(this.mdst).subscribe(
						(res: merge_md) => {
							this.mdtr = res;
							this.mdst.orgcode = this.mdtr.orgcode;
							this.mdst.site = this.mdtr.site;
							this.mdst.depot = this.mdtr.depot;
							this.mdst.spcarea = this.mdtr.spcarea;
							this.mdst.huno = this.mdtr.hutarget;
							this.mdst.hutype = this.mdtr.hutype;
							this.mdst.loccode = this.mdtr.loccode;
							this.mdst.accncode = this.mdtr.accnmodify;
							this.mdst.mergeno = this.mdtr.mergeno;
							this.mdtr.lines = [];
						},
						(err) => {
							this.mdtr = {};
							this.mdtr.lines = [];

							this.toastr.error(
								"<span class='fn-07e'>" + err.message + '</span>',
								null,
								{ enableHtml: true }
							);
						}
					);
				}
			});
		}
	}

	public cancel() {
		if (this.mdtr.hutarget == null) {
			this.toastr.error(
				"<span class='fn-07e'>Please Generate Target HU !</span>",
				null,
				{ enableHtml: true }
			);
		} else {
			this.ngPopups.confirm('Do you cancel Merge HU ?').subscribe((res) => {
				if (res) {
					this.sv.cancel(this.mdst).subscribe(
						(res: merge_md) => {
							this.mdtr = {};
							this.mdst = {};
							this.mdtr.lines = [];
							this.list();
						},
						(err) => {
							this.toastr.error(
								"<span class='fn-07e'>" + err.message + '</span>',
								null,
								{ enableHtml: true }
							);
						}
					);
				}
			});
		}
	}

	public confirm() {
		if (this.mdtr.hutarget == null) {
			this.toastr.error(
				"<span class='fn-07e'>Please Generate Target HU !</span>",
				null,
				{ enableHtml: true }
			);
		} else if (this.mdtr.lines.length == 0) {
			this.toastr.error(
				"<span class='fn-07e'>error ,merge HU item is required !</span>",
				null,
				{ enableHtml: true }
			);
		} else {
			this.ngPopups.confirm('Do you confirm Merge HU ?').subscribe((res) => {
				if (res) {
					this.sv.merge(this.mdtr).subscribe(
						(res: merge_md) => {
							// print hu label
							this.label();
							this.mdtr = {};
							this.mdst = {};
							this.mdfn = {};
							this.mdtr.lines = [];
							this.list();
							this.toastr.success(
								"<span class='fn-07e'>Confirm Successfully!</span>",
								null,
								{ enableHtml: true }
							);
						},
						(err) => {
							this.toastr.error(
								"<span class='fn-07e'>" + err.message + '</span>',
								null,
								{ enableHtml: true }
							);
						}
					);
				}
			});
		}
	}

	public select(index: number) {
		if (this.mdtr.hutarget == null) {
			this.toastr.error(
				"<span class='fn-07e'>Please Generate Target HU !</span>",
				null,
				{ enableHtml: true }
			);
		} else {
			let sln = this.mdsr[index];
			sln.mergeno = this.mdtr.mergeno;
			if (
				this.mdtr.lines.filter(
					(e) => e.loccode == sln.loccode && e.huno == sln.huno
				).length > 0
			) {
				this.toastr.warning(
					"<span class='fn-07e'>" +
					sln.huno +
					' already exists in list , </span>',
					null,
					{ enableHtml: true }
				);
			} else {
				this.mdtr.lines.push(sln);
				this.mdsr.splice(index, 1);
			}
		}
	}
	public selectAll() {
		if (this.mdtr.hutarget == null) {
			this.toastr.error(
				"<span class='fn-07e'>Please Generate Target HU!</span>",
				null,
				{ enableHtml: true }
			);
		} else {
			this.mdsr.forEach((sln) => {
				if (
					this.mdtr.lines.filter(
						(e) => e.loccode == sln.loccode && e.huno == sln.huno
					).length == 0
				) {
					sln.mergeno = this.mdtr.mergeno;
					this.mdtr.lines.push(sln);
				}
			});
			this.mdsr = [];
		}
	}
	public remove(index: number) {
		this.mdtr.lines.splice(index, 1);
	}
	public removeAll() {
		this.mdtr.lines = [];
	}

	ngAfterViewInit() {
		this.setupJS();
		setTimeout(this.ngToggle, 1000);
	}
	ngToggle() {
		$('.snapsmenu').click();
	}
	setupJS() {
		$('#accn-list .sidebar-scroll').slimScroll({
			height: '95%',
			wheelStep: 5,
			touchScrollStep: 50,
			color: '#cecece',
		});
	}
	ngOnDestroy(): void {
		this.lsfn = null;
		this.lsmd = null;
		this.lnmd = null;
		this.mdfn = null;
		this.mdsr = null;
		this.mdtr = null;
		this.mdst = null;
		this.lssel = null;
		this.lnsel = null;
		this.srsel = null;
		this.trsel = null;
		delete this.lsfn;
		delete this.lsmd;
		delete this.lnmd;
		delete this.mdfn;
		delete this.mdsr;
		delete this.mdtr;
		delete this.mdst;
		delete this.lssel;
		delete this.lnsel;
		delete this.srsel;
		delete this.trsel;
	}
}
