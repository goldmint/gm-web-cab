import {
  Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy, HostBinding, TemplateRef, ChangeDetectorRef,
  OnDestroy
} from '@angular/core';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { MessageBoxService, EthereumService } from "../../services/index";
import { BigNumber } from 'bignumber.js'
import {Observable} from "rxjs/Observable";
import {APIService, UserService} from "../../services";
import {Subscription} from "rxjs/Subscription";

@Component({
  selector: 'app-transfer-page',
  templateUrl: './transfer-page.component.html',
  styleUrls: ['./transfer-page.component.sass'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TransferPageComponent implements OnInit, OnDestroy {
  @HostBinding('class') class = 'page';

  private _modalRef: BsModalRef;
  public amount: BigNumber = new BigNumber(0);
  public walletAddressVal: string = null;
  public walletAddress: string = null;

  amountUnset: boolean = true;
  goldBalance: BigNumber = null;
  goldHotBalance: BigNumber = null;
  goldMetamaskBalance: BigNumber = null;

  walletChecked:boolean = true;
  amountChecked: boolean = true;

  public amountValue: number;
  public ethAddress: string = '';
  public selectedWallet = 0;

  private sub1: Subscription;

  constructor(
    private _modalService: BsModalService,
    private _ethService: EthereumService,
    private _messageBox: MessageBoxService,
    private _apiService: APIService,
    private _userService: UserService,
    private _cdRef: ChangeDetectorRef
  ) { }

  ngOnInit() {
    Observable.combineLatest(
      this._ethService.getObservableHotGoldBalance(),
      this._ethService.getObservableGoldBalance(),
      this._ethService.getObservableEthAddress()
    ).subscribe(data => {
      if (this.ethAddress !== data[2]) {
        this.ethAddress = data[2];
        if (!this.ethAddress) {
          this.selectedWallet = 0;
        }
      }

      this.goldHotBalance = data[0];
      this.goldMetamaskBalance = data[1];
      this.goldBalance = this.selectedWallet == 0 ? data[0] : data[1];

      this.validateAmount();
      this._cdRef.markForCheck();
    });

    this.selectedWallet = this._userService.currentWallet.id === 'hot' ? 0 : 1;

    this.sub1 = this._userService.onWalletSwitch$.subscribe((wallet) => {
      this.selectedWallet = wallet['id'] === 'hot' ? 0 : 1;
      this._cdRef.markForCheck();
    });

  }

  onChangeWallet() {
    this.goldBalance  = this.selectedWallet == 0 ?  this.goldHotBalance : this.goldMetamaskBalance;
    this.validateAmount();
  }

  modal(template: TemplateRef<any>) {
    if (this.selectedWallet == 1) {
      if (this._modalRef) {
        this._modalRef.hide();
      }
      this._modalRef = this._modalService.show(template, { class: 'modal-lg' });
    } else {
      this.onHotWallet();
    }
  }

  onWalletAddressChanged(value: string) {
    this.walletAddress = null;
    this.walletChecked = false;

    if (this._ethService.isValidAddress(value)) {
      this.walletAddress = value;
      this.walletChecked = true;
    }
    this._cdRef.markForCheck();
  }

  onAmountChanged(value: string) {
    this.amountUnset = false;
    this.amount = new BigNumber(0);

    var testVal = value != null && value.length > 0 ? parseFloat(value) : 0;
    if (testVal > 0) {
      this.amount = new BigNumber(value);
      this.amount = this.amount.decimalPlaces(6, BigNumber.ROUND_DOWN);
    }
    this.validateAmount();
    this._cdRef.markForCheck();
  }

  validateAmount() {
    this.amountChecked = this.amountUnset || this.amount.gt(0) && this.goldBalance && this.amount.lte(this.goldBalance);
  }

  onMetamask() {
      var confText =
        "Target address: " + this.walletAddress + "<br/>" +
        "GOLD amount: " + this.amount + " GOLD<br/>"
      ;
      this._messageBox.confirm(confText).subscribe(ok => {
        if (ok) {
          this._ethService.transferGoldToWallet(this.ethAddress, this.walletAddress, this.amount);
          this.walletAddressVal = "";
          this.amount = new BigNumber(0);
          this.amountValue = null;
        }
        this._cdRef.markForCheck();
      });
  }

  onHotWallet() {
    var confText =
      "Target address: " + this.walletAddress + "<br/>" +
      "GOLD amount: " + this.amount + " GOLD<br/>";

    this._messageBox.confirm(confText).subscribe(ok => {
      if(ok) {
        this._apiService.goldTransferHwRequest(this.walletAddress, this.amount)
          .subscribe(() => {
            this._messageBox.alert('Your request is in progress now!');
            this.walletAddressVal = "";
            this.amount = new BigNumber(0);
            this.amountValue = null;
          });
      }
    });
  }

  ngOnDestroy() {
    this.sub1 && this.sub1.unsubscribe();
  }

}
