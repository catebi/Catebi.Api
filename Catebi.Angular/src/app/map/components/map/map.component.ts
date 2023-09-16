import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, Subscription, combineLatest, take, takeLast } from 'rxjs';

import * as L from 'leaflet';
import { Control, Icon, icon } from 'leaflet';
import 'leaflet.markercluster';
import LayersOptions = Control.LayersOptions;
import { NotionService } from '@core/services/notion.service';
import { Cat } from '@core/models/cat';


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
    zoom: 15,
    center: L.latLng([this.tbilisiLat, this.tbilisiLon])
  };

  // Marker cluster stuff
  markerClusterGroup?: L.MarkerClusterGroup;
  markerClusterData: L.Marker[] = [];
  markerClusterOptions: L.MarkerClusterGroupOptions;

  private databaseSubscription?: Subscription;

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

    this.notionService.fetchCats().subscribe();

    this.databaseSubscription = this.notionService.database$
    .pipe(take(1))
    .subscribe((data: Cat[]) => {
      console.log('getCatsFromNotion_database$.subscribe');

      this.markerClusterData = this.getCatData(data);
      this.notionService.isLoading(false);
    });
  }

  getCatData(catData: Cat[]): L.Marker[] {

    const data: L.Marker[] = [];

    for (let i = 0; i < catData.length; i++) {
      if (!catData[i].geoLocation) {
        continue;
      }

      // split and convert to float
      const latLongArray =
        catData[i]
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

      data.push(L.marker([latLongArray[0], latLongArray[1]], { icon }));
    }

    return data;
  }

  constructor(private notionService: NotionService) {
    this.markerClusterOptions = this.getMarkerClusterOptions();
  }

  private getMarkerClusterOptions() {
    return <L.MarkerClusterGroupOptions>{
      showCoverageOnHover: false,
      zoomToBoundsOnClick: true,
      spiderfyOnMaxZoom: false,
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
