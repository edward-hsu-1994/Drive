import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { driveApi } from '../../../../../../environments/driveApi';
import { environment } from '../../../../../../environments/environment.prod';
import { HttpClientBase } from '../../../../../services/http-client-base.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  constructor(public http: HttpClientBase) {}

  public list(
    path: string | string[],
    type: 'Directory' | 'File' | null = null,
    skip: number = 0,
    take: number = 10
  ): Observable<any[]> {
    return this.http.get(driveApi.file.list, {
      path,
      type,
      skip,
      take
    });
  }

  public delete(paths: string[]) {
    return this.http.put(driveApi.file.delete, {}, paths, {});
  }

  public move(from: string, to: string) {
    return this.http.put(driveApi.file.move, { from, to }, {}, {});
  }

  public upload(path: string, files: FileList) {
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      formData.append('files', files.item(i));
    }

    return this.http.post(driveApi.file.upload, { path }, formData);
  }

  public createDirectory(path: string, name: string) {
    return this.http.post(
      driveApi.file.createChild,
      {
        path: path + '/' + name
      },
      {}
    );
  }
}
