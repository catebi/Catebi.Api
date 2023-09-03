import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

import * as L from 'leaflet';
import { Control, Icon, icon } from 'leaflet';
import 'leaflet.markercluster';
import LayersOptions = Control.LayersOptions;
import { NotionService } from '@core/services/notion.service';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit {

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

  ngOnInit() {
    this.getData();
  }

  async getData() {
    const database = await this.getCatsFromNotion();
    console.log(database);
  }

  private async getCatsFromNotion() {
    const database = await this.notionService.getDatabase();
    database.subscribe((data: any) => {
      //const jsonData = data; // Now jsonData is a JSON object
      // Your original JSON array
      // Filter out entries where geo_location is null or empty, then transform
      const transformedArray = data.results
        .filter((item: any) => {
          const geo_location = item.properties.geo_location.rich_text && item.properties.geo_location.rich_text[0]
            ? item.properties.geo_location.rich_text[0].plain_text
            : null;
          return geo_location !== null && geo_location !== '';
        })
        .map((item: any) => {
          const title = item.properties["cat\\name"].title && item.properties["cat\\name"].title[0]
            ? item.properties["cat\\name"].title[0].plain_text
            : 'Unknown';
          const geo_location = item.properties.geo_location.rich_text && item.properties.geo_location.rich_text[0]
            ? item.properties.geo_location.rich_text[0].plain_text
            : 'Unknown';

          return {
            title,
            geo_location
          };
        });

      console.log(transformedArray);

      this.markerClusterData = this.getCatData(transformedArray);

      this.notionService.isLoading(false);
    });
    return database;
  }

  getCatData(catData: any[]): L.Marker[] {

    const data: L.Marker[] = [];

    for (let i = 0; i < catData.length; i++) {

      // split and convert to float
      const latLongArray = catData[i].geo_location.split(", ").map((value: string) => parseFloat(value));
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

  // Generators for lat/lon values
  generateLat() {
    const lat = 0.2 * (Math.random() * 0.1 - 0.05); // Generate a random number between -0.05 and 0.05
    return this.tbilisiLat + lat;
  }

  generateLon() {
    const lon = 0.2 * (Math.random() * 0.1 - 0.05); // Generate a random number between -0.05 and 0.05
    return this.tbilisiLon + lon;
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
    this.markerClusterData = await this.getCatsFromNotion();
    //this.generateData(100);
  }

  generateData(count: number): L.Marker[] {

    const data: L.Marker[] = [];

    for (let i = 0; i < count; i++) {

      const icon = L.icon({
        ...Icon.Default.prototype.options,
        iconSize: [25, 41],
        iconAnchor: [13, 41],
        iconUrl: 'assets/marker-icon.png',
        iconRetinaUrl: 'assets/marker-icon-2x.png',
        shadowUrl: 'assets/marker-shadow.png'
      });

      data.push(L.marker([this.generateLat(), this.generateLon()], { icon }));
    }

    return data;
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
