import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  HostBinding, OnDestroy,
  OnInit,
  ViewEncapsulation
} from '@angular/core';
import {APIService, UserService} from "../../services";
import {MessageBoxService} from "../../services/message-box.service";
import 'anychart';
import {TransactionsList} from "../../interfaces/transactions-list";
import {BlocksList} from "../../interfaces/blocks-list";
import * as bs58 from 'bs58';
import * as CRC32 from 'crc-32';
import {combineLatest} from "rxjs/observable/combineLatest";
import {Router} from "@angular/router";
import {TranslateService} from "@ngx-translate/core";
import {Subject} from "rxjs/Subject";


@Component({
  selector: 'app-scaner-page',
  templateUrl: './scaner-page.component.html',
  styleUrls: ['./scaner-page.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class ScanerPageComponent implements OnInit, OnDestroy {

  @HostBinding('class') class = 'page';

  public loading: boolean = false;
  public isDataLoaded: boolean = false;
  public isValidSumusAddress: boolean = false;
  public isValidDigest: boolean = false;
  public searchAddress: string = '';
  public searchDigest: string = '';
  public numberNodes: number;
  public numberMNT: number;
  public numberReward: number = 0;
  public anyChartRewardData = [];
  public anyChartTxData = [];
  public transactionsList: TransactionsList[];
  public blocksList: BlocksList[];
  public switchModel: {
    type: 'gold'|'mnt'
  };

  private destroy$: Subject<boolean> = new Subject<boolean>();
  private interval: any;
  private lastItems: number = 5;
  private charts = {
    reward: {},
    tx: {}
  };

  constructor(
    private apiService: APIService,
    private userService: UserService,
    private cdRef: ChangeDetectorRef,
    private messageBox: MessageBoxService,
    private translate: TranslateService,
    private router: Router
  ) { }

  ngOnInit() {
    this.switchModel = {
      type: 'gold'
    };

    this.userService.currentLocale.takeUntil(this.destroy$).subscribe(() => {
      if (this.isDataLoaded) {
        this.translate.get('PAGES.Scanner.LatestStatistic.Charts.Reward').subscribe(phrase => {
          this.charts['reward']['chart'].title(phrase);
        });
        this.translate.get('PAGES.Scanner.LatestStatistic.Charts.Tx').subscribe(phrase => {
          this.charts['tx']['chart'].title(phrase);
        });
      }
    });

    this.apiService.transferCurrentSumusNetwork.subscribe(() => {
      this.updateData();
    });

    const combined = combineLatest(
      this.apiService.getNodesCount(),
      this.apiService.getMNTCount(),
      this.apiService.getMNTRewardDay(-1),

      this.apiService.getTransactions(this.lastItems),
      this.apiService.getBlocks(this.lastItems),

      this.apiService.getDailyStatistic()
    );

    combined.subscribe(data => {
      const rewardList = data[2]['data'].rewardList;
      this.numberNodes = data[0]['data'];
      this.numberMNT = data[1]['data'];
      rewardList !== null && (this.numberReward = rewardList[rewardList.length -1].commodityReward);

      this.getChartsData(data[5]['res']);

      this.initRewardChart();
      this.initTxChart();

      this.setBlockAndTransactionsInfo(data[3]['data'], data[4]['data'])

      this.isDataLoaded = true;
      this.cdRef.markForCheck();
    });

    this.interval = setInterval(() => {
      this.updateData();
    }, 60000);
  }

  updateData() {
    const combined = combineLatest(
      this.apiService.getMNTRewardDay(-1),
      this.apiService.getTransactions(this.lastItems),
      this.apiService.getBlocks(this.lastItems),
      this.apiService.getDailyStatistic()
    );

    combined.subscribe(data => {
      this.getChartsData(data[3]['res']);
      this.setBlockAndTransactionsInfo(data[1]['data'], data[2]['data']);
      this.updateChartsData();

      this.cdRef.markForCheck();
    });
  }

  updateChartsData() {
    this.charts['reward']['table'].remove();
    this.charts['reward']['table'].addData(this.anyChartRewardData);
    this.charts['tx']['table'].remove();
    this.charts['tx']['table'].addData(this.anyChartTxData);
  }

  getChartsData(res) {
    this.setCommissionChartData(res);
    this.setTxChartData(res);
  }

  setBlockAndTransactionsInfo(txs, blocks) {
    this.transactionsList = txs.map(item => {
      item.timeStamp = new Date(item.timeStamp.toString() + 'Z');
      return item;
    });

    this.blocksList = blocks.map(item => {
      item.timeStamp = new Date(item.timeStamp.toString() + 'Z');
      return item;
    });
  }

  checkAddress(address: string, type: string) {
    this.checkSumusAddressValidity(address, type);
  }

  checkSumusAddressValidity(address: string, type: string) {
    let bytes;
    try {
      bytes = bs58.decode(address);
    } catch (e) {
      type === 'address' ? this.isValidSumusAddress = false : this.isValidDigest = false;
      return
    }

    let field = type === 'address' ? this.searchAddress : this.searchDigest;
    if (bytes.length <= 4 || field.length <= 4) {
      type === 'address' ? this.isValidSumusAddress = false : this.isValidDigest = false;
      return
    }

    let payloadCrc = CRC32.buf(bytes.slice(0, -4));
    let crcBytes = bytes.slice(-4);
    let crc = crcBytes[0] | crcBytes[1] << 8 | crcBytes[2] << 16 | crcBytes[3] << 24;

    type === 'address' ? this.isValidSumusAddress = payloadCrc === crc : this.isValidDigest = payloadCrc === crc;
  }

  setCommissionChartData(data) {
    if (data) {
      data.forEach(item => {
        const date = new Date(item.timestamp * 1000);
        let month = (date.getMonth()+1).toString();
        month.length === 1 && (month = '0' + month);
        const dateString = date.getFullYear() + '-' + month + '-' + date.getDate();
        this.anyChartRewardData.push([dateString, +item.fee_gold, +item.fee_mnt]);
      });
    }
  }

  setTxChartData(data) {
    if (data) {
      data.forEach(item => {
        const date = new Date(item.timestamp * 1000);
        let month = (date.getMonth()+1).toString();
        month.length === 1 && (month = '0' + month);
        const dateString = date.getFullYear() + '-' + month + '-' + date.getDate();
        this.anyChartTxData.push([dateString, item.transactions]);
      });
    }
  }

  toggleCommissionChart(isGold: boolean) {
    this.charts['reward']['chart'].plot(0).enabled(isGold);
    this.charts['reward']['chart'].plot(1).enabled(!isGold);
  }

  initRewardChart() {
    anychart.onDocumentReady( () => {
      this.charts['reward']['table'] = anychart.data['table']();
      this.charts['reward']['table'].addData(this.anyChartRewardData);

      this.charts['reward']['mapping_gold'] = this.charts['reward']['table'].mapAs();
      this.charts['reward']['mapping_gold'].addField('value', 1);

      this.charts['reward']['mapping_mnt'] = this.charts['reward']['table'].mapAs();
      this.charts['reward']['mapping_mnt'].addField('value', 2);

      this.charts['reward']['chart'] = anychart.stock();

      this.charts['reward']['chart'].plot(0).line(this.charts['reward']['mapping_gold']).name('GOLD');
      this.charts['reward']['chart'].plot(1).line(this.charts['reward']['mapping_mnt']).name('MNT');

      this.charts['reward']['chart'].plot(0).legend().itemsFormatter(() => {
        return []
      });
      this.charts['reward']['chart'].plot(1).legend().itemsFormatter(() => {
        return []
      });

      this.charts['reward']['chart'].plot(1).enabled(false);

      this.translate.get('PAGES.Scanner.LatestStatistic.Charts.Reward').subscribe(phrase => {
        this.charts['reward']['chart'].title(phrase);
      });
      this.charts['reward']['chart'].container('reward-chart-container');
      this.charts['reward']['chart'].draw();
    });
  }

  initTxChart() {
    anychart.onDocumentReady( () => {
      this.charts['tx']['table'] = anychart.data['table']();
      this.charts['tx']['table'].addData(this.anyChartTxData);

      this.charts['tx']['mapping'] = this.charts['tx']['table'].mapAs();
      this.charts['tx']['mapping'].addField('value', 1);

      this.charts['tx']['chart'] = anychart.stock();
      this.charts['tx']['chart'].plot(0).line(this.charts['tx']['mapping']).name('Transactions');
      this.charts['tx']['chart'].plot(0).legend().itemsFormatter(() => {
        return [
          {text: "Transactions", iconFill:"#63B7F7"}
        ]
      });

      this.translate.get('PAGES.Scanner.LatestStatistic.Charts.Tx').subscribe(phrase => {
        this.charts['tx']['chart'].title(phrase);
      });
      this.charts['tx']['chart'].container('tx-chart-container');
      this.charts['tx']['chart'].draw();
    });
  }

  searchByAddress() {
    this.router.navigate(['/scanner/address', this.searchAddress]);
  }

  searchByDigest() {
    this.router.navigate(['/scanner/tx', this.searchDigest]);
  }

  ngOnDestroy() {
    clearInterval(this.interval);
    this.destroy$.next(true);
  }
}
