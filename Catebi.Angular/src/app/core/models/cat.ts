import { NotionFile } from './notion-file';

export interface Cat {
  catId: string;
  name?: string;
  address?: string;
  geoLocation?: string;
  createdTime: Date;
  lastEditedTime: Date;
  url: string;
  images: NotionFile[];
}
