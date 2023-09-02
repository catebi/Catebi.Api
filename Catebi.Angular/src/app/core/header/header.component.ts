import { Component } from '@angular/core';
//import { BioService } from '../services/bio.service';
import { HeaderService } from '../services/header.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  //bio$ = this.bioService.getBio();
  isHome$ = this.headerService.isHome();

  menuItems = [
    { title: 'Home', homePath: '/', fragment: 'Home', pagePath: '/home', cssButton: "btn-outline-dark", fontColor: "white" },
    //{ title: 'Mission', homePath: '/', fragment: 'mission', pagePath: '/mission', cssButton: "btn-outline-dark", fontColor: "black" },
    { title: 'Map', homePath: '/map', fragment: '', pagePath: '/map', cssButton: "btn-outline-dark", fontColor: "black" },
    //{ title: 'Donate!', homePath: '/donate', fragment: '', pagePath: '/donate', cssButton: "btn-success", fontColor: "white" },
  ];

  constructor(private headerService: HeaderService) { }
}
