import { NotionFile } from './notion-file';

export interface Cat {
  notionCatId: string;
  name?: string;
  address?: string;
  geoLocation?: string;
  createdTime: Date;
  lastEditedTime: Date;
  notionPageUrl: string;
  images: NotionFile[];
}
