import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, Subscription, combineLatest, take, takeLast } from 'rxjs';

import * as L from 'leaflet';
import { Control, Icon, icon } from 'leaflet';
import 'leaflet.markercluster';
import LayersOptions = Control.LayersOptions;
import { NotionService } from '@core/services/notion.service';
import { Cat } from '@core/models/cat';
import { PopupService } from '@map/services/popup.service';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit, OnDestroy {

  // Open Street Map Definition
  LAYER_OSM = {
    id: 'openstreetmap',
    name: 'Open Street Map',
    enabled: false,
    layer: L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      noWrap: true,
      attribution: 'Open Street Map'
    })
  };

  // Values to bind to Leaflet Directive
  layersControlOptions: LayersOptions = { position: 'bottomright' };
  baseLayers = {
    'Open Street Map': this.LAYER_OSM.layer,
  };

  private readonly tbilisiLat = 41.6938;
  private readonly tbilisiLon = 44.8015;


  public loading$ = this.notionService.loading.asObservable()

  options = {
    zoom: 13,
    center: L.latLng([this.tbilisiLat, this.tbilisiLon])
  };

  // Marker cluster stuff
  markerClusterGroup?: L.MarkerClusterGroup;
  markerClusterData: L.Marker[] = [];
  markerClusterOptions: L.MarkerClusterGroupOptions;

  private databaseSubscription?: Subscription;

  constructor(
    private notionService: NotionService,
    private popupService: PopupService
    ) {
    this.markerClusterOptions = this.getMarkerClusterOptions();
  }

  ngOnInit() {
    this.getCatsFromNotion();
  }

  ngOnDestroy(): void {
    if (this.databaseSubscription) {
      this.databaseSubscription.unsubscribe();
    }
  }

  private getCatsFromNotion() {
    // Unsubscribe from any existing subscription before creating a new one
    if (this.databaseSubscription) {
      this.databaseSubscription.unsubscribe();
    }

    this.notionService
      .fetchCats()
      .subscribe((data: Cat[]) => {
        this.markerClusterData = this.getCatData(data);
        this.notionService.isLoading(false);
      });
  }

  getCatData(catData: Cat[]): L.Marker[] {

    const data: L.Marker[] = [];

    for (let i = 0; i < catData.length; i++) {
      const catModel = catData[i];
      if (!catModel.geoLocation) {
        continue;
      }

      // split and convert to float
      const latLongArray =
        catModel
          .geoLocation!
          .split(", ")
          .map((value: string) =>
              parseFloat(value));
              const icon = L.icon({
                ...Icon.Default.prototype.options,
                iconSize: [25, 41],
                iconAnchor: [13, 41],
                iconUrl: 'assets/marker-icon.png',
                iconRetinaUrl: 'assets/marker-icon-2x.png',
                shadowUrl: 'assets/marker-shadow.png'
              });
      const marker = L.marker([latLongArray[0], latLongArray[1]], { icon });
      marker.bindPopup(this.popupService.makeCapitalPopup(catModel), {
          autoClose: true,
          closeOnClick: false,
          maxWidth: 500,
          maxHeight: 500,
          closeOnEscapeKey: true,
          minWidth: 400
          // TODO: handle dynamic autoPanPadding correction based on cat's picture size
          //autoPanPadding: [5, 5],
       });
      data.push(marker);

    }

    return data;
  }

  private getMarkerClusterOptions() {
    return <L.MarkerClusterGroupOptions>{
      showCoverageOnHover: false,
      zoomToBoundsOnClick: true,
      spiderfyOnMaxZoom: true,
      spiderfyOnEveryZoom: false,
      removeOutsideVisibleBounds: true,
      animate: true,
      animateAddingMarkers: true,
      chunkedLoading: false,
      iconCreateFunction: this.iconCreateFunction
    };
  }


  markerClusterReady(group: L.MarkerClusterGroup) {

    this.markerClusterGroup = group;

  }

  async refreshData(): Promise<void> {
    this.getCatsFromNotion();
  }

  iconCreateFunction(cluster: L.MarkerCluster): L.DivIcon {
    let childCount = cluster.getChildCount();
    let c = ' marker-cluster-';
    if (childCount < 10) {
      c += 'small';
    }
    else if (childCount < 100) {
      c += 'medium';
    }
    else {
      c += 'large';
    }

    return new L.DivIcon({
      html: '<div><span>' + childCount + '</span></div>',
      className: 'marker-cluster' + c, iconSize: new L.Point(40, 40)
    });
  }
}
