import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { interval } from "rxjs/observable/interval";
import * as Web3 from "web3";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { UserService } from "./user.service";
import { BigNumber } from 'bignumber.js'
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {Subject} from "rxjs/Subject";

@Injectable()
export class EthereumService {
  private _infuraUrl = environment.infuraUrl;
  private _etherscanGetABIUrl = environment.etherscanGetABIUrl;
  // main contract
  private EthContractAddress = environment.EthContractAddress;
  private EthContractABI: string;
  // gold token
  private EthGoldContractAddress: string
  private EthGoldContractABI: string;
  // mntp token
  private EthMntpContractAddress: string;
  private EthMntpContractABI: string;

  private _web3Infura: Web3 = null;
  private _web3Metamask: Web3 = null;
  private _lastAddress: string | null;
  private _userId: string | null;

  private _contractInfura: any;
  private _contractMetamask: any;

  public _contractGold: any;
  public _contractHotGold: any;
  private _contractMntp: any;
  private _contactsInitted: boolean = false;
  private _totalGoldBalances = {issued: null, burnt: null};

  private _obsEthAddressSubject = new BehaviorSubject<string>(null);
  private _obsEthAddress: Observable<string> = this._obsEthAddressSubject.asObservable();
  private _obsGoldBalanceSubject = new BehaviorSubject<BigNumber>(null);
  private _obsGoldBalance: Observable<BigNumber> = this._obsGoldBalanceSubject.asObservable();
  private _obsMntpBalanceSubject = new BehaviorSubject<BigNumber>(null);
  private _obsMntpBalance: Observable<BigNumber> = this._obsMntpBalanceSubject.asObservable();
  private _obsHotGoldBalanceSubject = new BehaviorSubject<BigNumber>(null);
  private _obsHotGoldBalance: Observable<BigNumber> = this._obsHotGoldBalanceSubject.asObservable();
  private _obsEthBalanceSubject = new BehaviorSubject<BigNumber>(null);
  private _obsEthBalance: Observable<BigNumber> = this._obsEthBalanceSubject.asObservable();
  private _obsEthLimitBalanceSubject = new BehaviorSubject<BigNumber>(null);
  private _obsEthLimitBalance: Observable<BigNumber> = this._obsEthLimitBalanceSubject.asObservable();
  private _obsTotalGoldBalancesSubject = new BehaviorSubject<Object>(null);
  private _obsTotalGoldBalances: Observable<Object> = this._obsTotalGoldBalancesSubject.asObservable();
  private _obsGasPriceSubject = new BehaviorSubject<Object>(null);
  private _obsGasPrice: Observable<Object> = this._obsGasPriceSubject.asObservable();

  public getSuccessBuyRequestLink$ = new Subject();
  public getSuccessSellRequestLink$ = new Subject();

  constructor(
    private _userService: UserService,
    private _http: HttpClient
  ) {
    console.log('EthereumService constructor');

    this._userService.currentUser.subscribe(currentUser => {
      this._userId = currentUser != null && currentUser.id ? currentUser.id : null;
    });

    interval(500).subscribe(this.checkWeb3.bind(this));

    interval(7500).subscribe(this.checkBalance.bind(this));
  }

  getContractABI(address) {
    return this._http.get(`${this._etherscanGetABIUrl}/api?module=contract&action=getabi&address=${address}&forma=raw`)
  }

  private checkWeb3() {

    if (!this._web3Infura) {
      this._web3Infura = new Web3(new Web3.providers.HttpProvider(this._infuraUrl));

      this.getContractABI(this.EthContractAddress).subscribe(abi => {
        this.EthContractABI = abi['result'];

        if (this._web3Infura.eth) {
          this._contractInfura = this._web3Infura.eth.contract(JSON.parse(this.EthContractABI)).at(this.EthContractAddress);

          this._contractInfura.mntpToken((error, address) => {
            this.EthMntpContractAddress = address;
          });

          this._contractInfura.goldToken((error, address) => {
            this.EthGoldContractAddress = address;

            this.getContractABI(this.EthGoldContractAddress).subscribe(abi => {
              this.EthGoldContractABI = this.EthMntpContractABI = abi['result'];

              this._contractHotGold = this._web3Infura.eth.contract(JSON.parse(this.EthGoldContractABI)).at(this.EthGoldContractAddress);
            });
         });

        } else {
          this._web3Infura = null;
        }
      });
    }

    if (!this._web3Metamask && window.hasOwnProperty('web3') && this.EthGoldContractABI) {
      this._web3Metamask = new Web3(window['web3'].currentProvider);

      if (this._web3Metamask.eth) {
        this._contractMetamask = this._web3Metamask.eth.contract(JSON.parse(this.EthContractABI)).at(this.EthContractAddress);
        this._contractGold = this._web3Metamask.eth.contract(JSON.parse(this.EthGoldContractABI)).at(this.EthGoldContractAddress);
        this._contractMntp = this._web3Metamask.eth.contract(JSON.parse(this.EthMntpContractABI)).at(this.EthMntpContractAddress);
      } else {
        this._web3Metamask = null;
      }
    }

    if (!this._contactsInitted && this._userId) {
      this._contactsInitted = true;
      this.checkBalance();
    }

    var addr = this._web3Metamask && this._web3Metamask.eth && this._web3Metamask.eth.accounts.length
      ? this._web3Metamask.eth.accounts[0] : null;
    if (this._lastAddress != addr) {
      this._lastAddress = addr;
      console.log("EthereumService: new eth address (MM): " + addr);
      this.emitAddress(addr);
    }
  }

  private checkBalance() {
    if (this._lastAddress != null) {
      // check via eth
      this.updateGoldBalance(this._lastAddress);
      this.updateMntpBalance(this._lastAddress);
      this.updateEthBalance(this._lastAddress);
    }

    this.checkHotBalance();
    this.updateTotalGoldBalances();
    this.updateEthLimitBalance(this.EthContractAddress);
  }

  private checkHotBalance() {
    this._userId != null && this._contractInfura && this._contractInfura.getUserHotGoldBalance(this._userId, (err, res) => {
      this._obsHotGoldBalanceSubject.next(res.div(new BigNumber(10).pow(18)));
    });
  }

  private emitAddress(ethAddress: string) {
    this._obsEthAddressSubject.next(ethAddress);
    this._obsGoldBalanceSubject.next(null);
    this._obsMntpBalanceSubject.next(null);
    this.checkBalance();
  }

  private updateGoldBalance(addr: string) {
    if (addr == null || this._contractGold == null) {
      this._obsGoldBalanceSubject.next(null);
    } else {
      this._contractGold.balanceOf(addr, (err, res) => {
        this._obsGoldBalanceSubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
      });
    }
  }

  private updateMntpBalance(addr: string) {
    if (addr == null || this._contractGold == null) {
      this._obsMntpBalanceSubject.next(null);
    } else {
      this._contractMntp.balanceOf(addr, (err, res) => {
        this._obsMntpBalanceSubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
      });
    }
  }

  private updateEthBalance(addr: string) {
    if (addr == null || this._contractGold == null) {
      this._obsEthBalanceSubject.next(null);
    } else {
      this._contractGold._eth.getBalance(addr, (err, res) => {
        this._obsEthBalanceSubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
      });
    }
  }

  private updateEthLimitBalance(addr: string) {
    this._contractMetamask && this._web3Metamask.eth.getBalance(addr, (err, res) => {
      this._obsEthLimitBalanceSubject.next(new BigNumber(res.toString()).div(new BigNumber(10).pow(18)));
    });
  }

  private updateTotalGoldBalances() {
    if (this._contractHotGold) {
        this._contractHotGold.getTotalBurnt((err, res) => {
        if (!this._totalGoldBalances.burnt || !this._totalGoldBalances.burnt.eq(res)) {
          this._totalGoldBalances.burnt = res;
          this._totalGoldBalances.issued && this._obsTotalGoldBalancesSubject.next(this._totalGoldBalances);
        }
      });
      this._contractHotGold.getTotalIssued((err, res) => {
        if (!this._totalGoldBalances.issued || !this._totalGoldBalances.issued.eq(res)) {
          this._totalGoldBalances.issued = res;
          this._totalGoldBalances.burnt && this._obsTotalGoldBalancesSubject.next(this._totalGoldBalances);
        }
      });
    }
  }

  private getGasPrice() {
    this._web3Metamask && this._web3Metamask.eth.getGasPrice((err, res) => {
      this._obsGasPriceSubject.next(res);
    });
  }

  // ---

  public isValidAddress(addr: string): boolean {
    return (new Web3()).isAddress(addr);
  }

  public getEthAddress(): string | null {
    return this._obsEthAddressSubject.value;
  }

  public getObservableEthAddress(): Observable<string> {
    return this._obsEthAddress;
  }

  public getObservableGoldBalance(): Observable<BigNumber> {
    return this._obsGoldBalance;
  }

  public getObservableHotGoldBalance(): Observable<BigNumber> {
    return this._obsHotGoldBalance;
  }

  public getObservableMntpBalance(): Observable<BigNumber> {
    return this._obsMntpBalance;
  }

  public getObservableEthBalance(): Observable<BigNumber> {
    return this._obsEthBalance;
  }

  public getObservableEthLimitBalance(): Observable<BigNumber> {
    return this._obsEthLimitBalance;
  }

  public getObservableTotalGoldBalances(): Observable<Object> {
    return this._obsTotalGoldBalances;
  }

  public getObservableGasPrice(): Observable<Object> {
    this.getGasPrice();
    return this._obsGasPrice;
  }

  // ---
  public sendBuyRequest(fromAddr: string, userID: string, requestId: number, amount: BigNumber, gasPrice: number) {
    if (this._contractMetamask == null) return;
    const wei = new BigNumber(amount).times(new BigNumber(10).pow(18).decimalPlaces(0, BigNumber.ROUND_DOWN));
    const reference = new BigNumber(requestId);

    this._contractMetamask.addBuyTokensRequest(userID, reference.toString(), { from: fromAddr, value: wei.toString(), gasPrice: gasPrice }, (err, res) => {
      this.getSuccessBuyRequestLink$.next(res);
    });
  }

  public sendSellRequest(fromAddr: string, userID: string, requestId: number, amount: BigNumber, gasPrice: number) {
    if (this._contractMetamask == null) return;
    const wei = new BigNumber(amount).times(new BigNumber(10).pow(18).decimalPlaces(0, BigNumber.ROUND_DOWN));
    const reference = new BigNumber(requestId);

    this._contractMetamask.addSellTokensRequest(userID, reference.toString(), wei.toString(), { from: fromAddr, value: 0, gasPrice: gasPrice }, (err, res) => {
      this.getSuccessSellRequestLink$.next(res);
    });
  }

  public transferGoldToWallet(fromAddr: string, toAddr: string, goldAmount: BigNumber) {
    if (this._contractGold == null) return;
    var goldAmountStr = goldAmount.times(new BigNumber(10).pow(18)).decimalPlaces(0, BigNumber.ROUND_DOWN).toString();
    this._contractGold.transfer.sendTransaction(toAddr, goldAmountStr, { from: fromAddr, value: 0 }, (err, res) => { });
  }
}
