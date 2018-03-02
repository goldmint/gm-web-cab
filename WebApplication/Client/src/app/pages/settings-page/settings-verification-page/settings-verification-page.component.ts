import { Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy, ChangeDetectorRef, EventEmitter, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
// import { PhoneNumberComponent } from 'ngx-international-phone-number';
import 'rxjs/add/operator/finally';

import { APIResponse, Country, Region, KYCStart, KYCAgreementResend } from '../../../interfaces';
import { APIService, MessageBoxService } from '../../../services';
import { KYCProfile } from '../../../models/kyc-profile';

import * as countries from '../../../../assets/data/countries.json';

enum Phase { Start, Basic, Agreement, KYCPending, KYC, Finished }

@Component({
  selector: 'app-settings-verification-page',
  templateUrl: './settings-verification-page.component.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SettingsVerificationPageComponent implements OnInit {
  // @ViewChild("pho", {read: PhoneNumberComponent}) pho: PhoneNumberComponent;

  public _phase = Phase;

  public loading = true;
  public processing = false;
  public repeat = Array;

  public phase: Phase;
  public countries: Country[];
  public regions: Region[];
  public errors = [];

  public kycProfile = new KYCProfile();
  public dateOfBirth: { day: number, month: number, year: number | '' };
  public minBirthYear = 1999;

  public agreementResend: KYCAgreementResend = null;

  private selectedCountry;

  constructor(
    private _apiService: APIService,
    private _cdRef: ChangeDetectorRef,
    private _messageBox: MessageBoxService) {

    this.dateOfBirth = { day: 1, month: 1, year: '' };
    this.countries = <Country[]><any>countries;

    this.phase = Phase.Start;

    this._apiService.getKYCProfile()
      .finally(() => {
        this.loading = false;
        this._cdRef.detectChanges();
      })
      .subscribe(
      (res: APIResponse<KYCProfile>) => {
        this.kycProfile = res.data;

        if (this.kycProfile.dob) {
          this.dateOfBirth = {
            day: this.kycProfile.dob.getDate(),
            month: this.kycProfile.dob.getMonth() + 1,
            year: this.kycProfile.dob.getFullYear()
          };
        }

        this.onCountrySelect(false);

        this.onPhaseUpdate();
      },
      err => { });
  }

  ngOnInit() {
  }

  onPhaseUpdate() {
    if (!this.kycProfile.isFormFilled) {
      this.phase = Phase.Start;
    }
    else if (!this.kycProfile.isAgreementSigned) {
      this.phase = Phase.Agreement;
    }
    else if (this.kycProfile.isKYCPending) {
      this.phase = Phase.KYCPending;
    }
    else if (!this.kycProfile.isKYCFinished) {
      this.phase = Phase.KYC;
    }
    else {
      this.phase = Phase.Finished;
    }
    this._cdRef.markForCheck();
  }

  setVerificationPhase(phase: Phase) {
    this.phase = phase;
  }

  onCountrySelect(reset: boolean = true) {
    const country = <Country>this.countries.find(country => country.countryShortCode === this.kycProfile.country);

    if (reset) this.kycProfile.state = null;

    if (country != null) {
      this.selectedCountry = country;
      this.regions = country.regions;
      if (!this.kycProfile.isFormFilled) {
        this.kycProfile.phoneNumber = country['phoneCode'];
      }
    }
  }

  onPhoneNumberChanged(event) {
    event.target.value = event.target.value.replace(/(?!^\+)[^\d]/g, '');

    if (this.kycProfile.phoneNumber.indexOf(this.selectedCountry.phoneCode)) {
      event.target.value = this.selectedCountry.phoneCode;
    }
  }

  submit(kycForm?: NgForm) {

    if (this.phase == Phase.Basic && kycForm) {
      this.submitBasicInformation(kycForm);
    }

    if (this.phase == Phase.Agreement) {
      this.resendAgreement();
    }

    if (this.phase == Phase.KYC) {
      this.startKYCVerification();
    }

    console.log("SUBMIT");
  }

  submitBasicInformation(kycForm: NgForm) {
    this.processing = true;

    this.kycProfile.dob = new Date(<number>this.dateOfBirth.year, this.dateOfBirth.month - 1, this.dateOfBirth.day);

    this._apiService.updateKYCProfile(this.kycProfile)
      .finally(() => {
        kycForm.form.markAsPristine();

        this.processing = false;
        this._cdRef.detectChanges();
      })
      .subscribe(
      (res: APIResponse<KYCProfile>) => {
        this.kycProfile = res.data;
        // this.startKYCVerification();
        this.onPhaseUpdate();
      },
      err => {
        if (err.error.errorCode) {
          switch (err.error.errorCode) {
            case 100: // InvalidParameter
              for (let i = err.error.data.length - 1; i >= 0; i--) {
                this.errors[err.error.data[i].field] = err.error.data[i].desc;
              }
              break;

            default:
              this._messageBox.alert(err.error.errorDesk);
              break;
          }
        }
      });
  }

  startKYCVerification() {
    this.processing = true;

    this._apiService.startKYCVerification(window.location.href)
      .finally(() => {
        this.processing = false;
        this._cdRef.detectChanges();
      })
      .subscribe(
      (res: APIResponse<KYCStart>) => {
        this._messageBox.confirm('You\'ll be redirected to the verification service.')
          .subscribe(confirmed => {
            if (confirmed) {
              localStorage.setItem('gmint_kycTicket', String(res.data.ticketId));
              window.location.href = res.data.redirect;
            }
          });
      },
      err => { });
  }

  resendAgreement() {
    this.processing = true;

    this._apiService.resendKYCAgreement()
      .finally(() => {
        this.processing = false;
        this._cdRef.detectChanges();
      })
      .subscribe(
        res => {
          this.agreementResend = res.data;
        },
        err => { }
      );
  }

}
