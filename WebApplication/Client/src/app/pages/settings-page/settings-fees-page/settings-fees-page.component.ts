import {ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, ViewEncapsulation} from '@angular/core';
import {APIService} from "../../../services";

@Component({
  selector: 'app-settings-fees-page',
  templateUrl: './settings-fees-page.component.html',
  styleUrls: ['./settings-fees-page.component.sass'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SettingsFeesPageComponent implements OnInit {

  public currencyTypeList = ['fiat', 'crypto']
  public currentCurrencyType = this.currencyTypeList[0];
  public isDataLoaded = false;
  public fees: object;
  public isMobile: boolean;

  constructor(private _cdRef: ChangeDetectorRef,
              private _apiService: APIService,) { }

  ngOnInit() {
    this.isMobile = (window.innerWidth <= 576);
    window.onresize = () => {
      this.isMobile = window.innerWidth <= 576 ? true : false;
      this._cdRef.markForCheck();
    };

    this._apiService.getFees().subscribe(data => {
      this.fees = data.data;
      this.isDataLoaded = true;
      this._cdRef.detectChanges();
    });
    this._cdRef.markForCheck();
  }

  chooseCurrencyType(type) {
    if (this.currentCurrencyType !== type) {
      this.currentCurrencyType = type;
      this._cdRef.detectChanges();
    }
  }
}
