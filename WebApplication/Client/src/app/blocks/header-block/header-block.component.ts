import {
  Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy, ChangeDetectorRef,
  OnDestroy
} from '@angular/core';
import { User } from '../../interfaces';
import { UserService, MessageBoxService, EthereumService, GoldrateService } from '../../services';
import {Subject} from "rxjs/Subject";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-header',
  templateUrl: './header-block.component.html',
  styleUrls: ['./header-block.component.sass'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderBlockComponent implements OnInit, OnDestroy {

  public gold_usd_rate: number;
  public user: User;
  public locale: string;
  public wallets = [
    {id: 'hot', name: 'HOT WALLET', account: ''},
    {id: 'metamask', name: 'METAMASK', account: ''},
  ];
  public activeWallet: Object = this.wallets[0];

  public metamaskAccount: string = null;
  public goldBalance: string|null = null;
  public hotGoldBalance: string|null = null;
  public usdBalance: number|null = null;
  public shortAdr: string;
  private destroy$: Subject<boolean> = new Subject<boolean>();


  constructor(
    private _ethService: EthereumService,
    private _goldrateService: GoldrateService,
    private _userService: UserService,
    private _cdRef: ChangeDetectorRef,
    private _messageBox: MessageBoxService,
    private _translate: TranslateService
  ) {
  }

  ngOnInit() {
    this._goldrateService.getObservableRate().takeUntil(this.destroy$).subscribe(data => {
      this.gold_usd_rate = data;
      this._cdRef.detectChanges();
    });

    this._userService.currentUser.takeUntil(this.destroy$).subscribe(currentUser => {
      this.user = currentUser;
      this._cdRef.detectChanges();
    });

    this._userService.currentLocale.takeUntil(this.destroy$).subscribe(currentLocale => {
      this.locale = currentLocale;
      this._cdRef.detectChanges();
    });

    this._ethService.getObservableEthAddress().takeUntil(this.destroy$).subscribe(ethAddr => {
      this.metamaskAccount = ethAddr;
      !this.metamaskAccount && this.activeWallet['id'] === 'metamask' && (this.activeWallet = this.wallets[0]);
      this.showShortAccount();
      this.wallets.forEach(item => {
        item.account = item.id === 'metamask' ? this.shortAdr : '';
      });
      this._cdRef.detectChanges();
    });

    this._ethService.getObservableGoldBalance().takeUntil(this.destroy$).subscribe(bal => {
      if (bal != null) {
        this.goldBalance = bal.toString().replace(/^(\d+\.\d\d)\d+$/, '$1');
        this._cdRef.detectChanges();
      }
    });

    this._ethService.getObservableHotGoldBalance().takeUntil(this.destroy$).subscribe(bal => {
      if (bal != null) {
        this.hotGoldBalance = bal.toString().replace(/^(\d+\.\d\d)\d+$/, '$1');
        this._cdRef.detectChanges();
      }
    });

    this._ethService.getObservableUsdBalance().takeUntil(this.destroy$).subscribe(bal => {
      this.usdBalance = bal;
      this._cdRef.detectChanges();
    });

    this._userService.currentWallet = this.activeWallet;
    this._cdRef.detectChanges();
  }

  onWalletSwitch(wallet) {
    if (wallet.id === 'metamask' && !this.metamaskAccount) {
      this._translate.get('MessageBox.MetaMask').subscribe(phrase => {
        this._messageBox.alert(phrase.Text, phrase.Heading);
      });
      return;
    }

    this._userService.currentWallet = this.activeWallet = wallet;
    this._userService.onWalletSwitch(wallet);

    this.showShortAccount();
    this._cdRef.detectChanges();
  }

  showShortAccount() {
    this.shortAdr = this.metamaskAccount ? ' (' + this.metamaskAccount.slice(0, 5) + ')...' : '';
  }

  showGoldRateInfo() {
    this._messageBox.alert(`${this.gold_usd_rate}`);
    this._cdRef.detectChanges();
  }

  private logout(e) {
    e.preventDefault();

    this._translate.get('MessageBox.logOut').subscribe(phrase => {
      this._messageBox.confirm(phrase)
        .subscribe(confirmed => {
          if (confirmed) {
            this._userService.logout(e);
            this._cdRef.detectChanges();
          }
        });
    });
  }

  public isLoggedIn() {
    return this._userService.isAuthenticated();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
  }

}
