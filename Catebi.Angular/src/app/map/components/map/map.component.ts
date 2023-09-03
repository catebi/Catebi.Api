import { Component, OnInit } from '@angular/core';

import * as L from 'leaflet';
import { Control, Icon } from 'leaflet';
import 'leaflet.markercluster';
import LayersOptions = Control.LayersOptions;


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

  options = {
    zoom: 15,
    center: L.latLng([this.tbilisiLat, this.tbilisiLon])
  };

  // Marker cluster stuff
  markerClusterGroup?: L.MarkerClusterGroup;
  markerClusterData: L.Marker[] = [];
  markerClusterOptions: L.MarkerClusterGroupOptions = {
    showCoverageOnHover: false,
    zoomToBoundsOnClick: false,
    spiderfyOnMaxZoom: false,
    spiderfyOnEveryZoom: false,
    removeOutsideVisibleBounds: true,
    animate: true,
    animateAddingMarkers: true,
    chunkedLoading: false,
  };

  // Generators for lat/lon values
  generateLat() {
    const lat = 0.2 * (Math.random() * 0.1 - 0.05); // Generate a random number between -0.05 and 0.05
    return this.tbilisiLat + lat;
  }

  generateLon() {
    const lon = 0.2 * (Math.random() * 0.1 - 0.05); // Generate a random number between -0.05 and 0.05
    return this.tbilisiLon + lon;
  }

  constructor() {
    this.markerClusterOptions = {}; // Initialize the property here
  }

  ngOnInit() {
    this.refreshData();
  }

  markerClusterReady(group: L.MarkerClusterGroup) {

    this.markerClusterGroup = group;

  }

  refreshData(): void {
    this.markerClusterData = this.generateData(100);
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
}
