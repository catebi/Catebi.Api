import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { LeafletMarkerClusterModule } from '@asymmetrik/ngx-leaflet-markercluster';

import { MapRoutingModule } from './map-routing.module';
import { MapComponent } from './components/map/map.component';
import { PopupService } from './services/popup.service';


@NgModule({
  declarations: [
    MapComponent
  ],
  imports: [
    CommonModule,
    MapRoutingModule,
    LeafletModule,
    LeafletMarkerClusterModule
  ],
  exports: [MapComponent],
  providers: [PopupService],
  bootstrap: [MapComponent]
})
export class MapModule { }
