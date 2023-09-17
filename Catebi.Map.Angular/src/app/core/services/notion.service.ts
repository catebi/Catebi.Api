import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, finalize, shareReplay, tap } from 'rxjs';

import { Cat } from '@core/models/cat';

// ACCESS CONSTANTS
export const NOTION = {
  catsApiUrl: 'https://localhost:7294/api/notion/getcats'
};

// HEADERS
export const headerOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': 'http://localhost:4200',
    'Cache-Control': 'no-cache, no-store, must-revalidate, post-check=0, pre-check=0',
    'Pragma': 'no-cache',
    'Expires': '0'
  }),
};

@Injectable()
export class NotionService {
  private _database$: BehaviorSubject<Cat[]> = new BehaviorSubject<Cat[]>([]);
  public readonly database$: Observable<Cat[]> = this._database$.asObservable();

  public loading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false)
  public isLoading(state: boolean): void { this.loading.next(state) }

  constructor(private http: HttpClient) {}

  fetchCats(): Observable<any> {
    this.isLoading(true);
    return this.http.get<Cat[]>(NOTION.catsApiUrl, headerOptions)
      .pipe(
        tap(data => this._database$.next(data)),
        catchError(error => {
          console.error('An error occurred:', error);
          return [];
        }),
        finalize(() => this.isLoading(false))
      );
  }
}
