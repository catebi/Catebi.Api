import { Injectable } from '@angular/core';
import { Cat } from '@core/models/cat';

@Injectable({
  providedIn: 'root'
})
export class PopupService {
  constructor() { }

  makeCapitalPopup(data: Cat): string {
    var image = data.images && data.images[0]
    ? `
<div style='display: flex; justify-content: center; margin-bottom: 10px;'>
  <img src='${ data.images[0].url }' style='max-width: 400px; max-height: 400px; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'/>
</div>`
    :
`
<div style='margin-bottom: 10px; font-size: 16px; color: #888;'>у кошки нет фотографий 😔</div>
`

return `${image}
<div style='display: flex; flex-direction: column; align-items: left; padding: 15px; border: 1px solid #ccc; border-radius: 10px; background-color: #f9f9f9;'>
  <div style='margin-bottom: 5px; font-size: 14px;'><strong>Id:</strong> ${ data.catId }</div>
  <div style='margin-bottom: 5px; font-size: 14px;'><strong>Имя/описание:</strong> ${ data.name }</div>
  <div style='font-size: 14px;'><strong><a href='${ data.url }' target='_blank' style='color: #007bff;'>Notion link</a></strong></div>
</div>
`
  }
}
