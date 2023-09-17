import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NotionService } from '@core/services/notion.service';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    HeaderComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    NgbModule,
    HttpClientModule
  ],
  exports: [
    HeaderComponent
  ],
  providers: [NotionService],
})
export class CoreModule { }
