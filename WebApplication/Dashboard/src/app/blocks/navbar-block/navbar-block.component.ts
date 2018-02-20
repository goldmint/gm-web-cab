import {Component, OnDestroy, OnInit} from '@angular/core';
import {UserService} from "../../services";
import {Subscription} from "rxjs/Subscription";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar-block.component.html',
  styleUrls: ['./navbar-block.component.sass']
})
export class NavbarBlockComponent implements OnInit, OnDestroy {

  public canShowNav = false;
  public sub1: Subscription;

  constructor(private _userService: UserService) { }

  ngOnInit() {
    if (this._userService.isAuthenticated()) {
      this.canShowNav = true;
    }
    this.sub1 = this._userService.canShowNav$.subscribe((flag: boolean) => {
      this.canShowNav = flag;
    });
  }

  ngOnDestroy() {
    if (this.sub1) {
      this.sub1.unsubscribe();
    }
  }

}
