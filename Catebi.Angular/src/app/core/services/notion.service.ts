import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

// ACCESS CONSTANTS
export const NOTION = {
  bearerToken: 'secret_entb4diRbuA3yDFpd6s14ClEqn1yCdLENUuUOWLvTok',
  database: {
    //api: 'https://api.notion.com/v1/databases',
    api: 'http://localhost:3000/database',
    id: '64da38a2d05347f4a9ad1e8c79c8e4f7',
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
