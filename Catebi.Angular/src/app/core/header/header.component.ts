import { Component } from '@angular/core';
//import { BioService } from '../services/bio.service';
import { HeaderService } from '../services/header.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  //bio$ = this.bioService.getBio();
  isHome$ = this.headerService.isHome();
  isSaveButtonActive = false;
  isButtonActive: string = this.isSaveButtonActive ? 'active'  : '';

  menuItems = [
    { title: 'Home', homePath: '/', fragment: 'Home', pagePath: '/home', cssButton: "btn-outline-dark", fontColor: "white" },
    //{ title: 'Mission', homePath: '/', fragment: 'mission', pagePath: '/mission', cssButton: "btn-outline-dark", fontColor: "black" },
    { title: 'Map', homePath: '/map', fragment: '', pagePath: '/map', cssButton: "btn-outline-dark", fontColor: "black" },
    //{ title: 'Donate!', homePath: '/donate', fragment: '', pagePath: '/donate', cssButton: "btn-success", fontColor: "white" },
  ];

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private headerService: HeaderService) {}

  isActive(item: any): boolean {
    let currentRoute = this.router.url.split('#')[0]; // remove fragment
    let homePath = this.isHome$ ? item.homePath : item.pagePath;
    return currentRoute === homePath;
  }
}
