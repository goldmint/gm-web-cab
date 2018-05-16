import {Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";

enum Pages {termsOfSale, privacy, kycpolicy}
let linksArray:[string] = [
  'https://www.goldmint.io/media/documents/Gold_Coin_Terms_of_sale.pdf',
  'https://www.goldmint.io/media/documents/Consumer%20data%20privacy%20Policy.pdf',
  'https://www.goldmint.io/media/documents/KYC&AML%20Policy.pdf'
];

@Component({
  selector: 'app-static-pages',
  templateUrl: './static-pages.component.html',
  styleUrls: ['./static-pages.component.sass']
})
export class StaticPagesComponent implements OnInit {
  public pagePath:string;
  private _pages = Pages;
  private _links = linksArray;

  constructor(
      private _route: ActivatedRoute,
  ) {
    this._route.params
        .subscribe(params => {
          let page = params.page;
          if (page) {
            this.pagePath = this._links[this._pages[page]];
          }
        })
  }

  ngOnInit() {
  }

}
