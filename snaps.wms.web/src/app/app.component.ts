import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
declare var $: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent  {
  title = 'snaps-wms-web';
  constructor(private translate: TranslateService) {
    translate.setDefaultLang('en');
  }
  useLanguage(language: string) {
    this.translate.use(language);
 }
}
