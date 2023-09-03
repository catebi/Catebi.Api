import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

// ACCESS CONSTANTS
export const NOTION = {
  bearerToken: '',
  database: {
    api: 'http://localhost:3000/database',
    id: '',
  }
};

// HEADERS
export const headerOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${NOTION.bearerToken}`,
    'Access-Control-Allow-Origin': 'http://localhost:4200',
  }),
};

@Injectable()
export class NotionService {
  private _database$: BehaviorSubject<any> = new BehaviorSubject<any>({});
  public database$: Observable<any> = this._database$.asObservable();

  private _pageData$: BehaviorSubject<any> = new BehaviorSubject<any>({});
  public pageData$: Observable<any> = this._pageData$.asObservable();

  public loading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false)
  public isLoading(state: boolean): void { this.loading.next(state) }

  constructor(private http: HttpClient) {}

  async getDatabase(): Promise<any> {
    this.isLoading(true);
    const databaseData = await this.http.get(
      `${NOTION.database.api}`,
      headerOptions
    );
    //console.log('databaseData:', databaseData);
    databaseData.subscribe((res) => console.log(res));
    this._database$.next(databaseData);
    return databaseData;
  }
}
