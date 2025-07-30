import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiGetterService {
  constructor(private http: HttpClient) {}

  get<T>(url: string, params?: any): Observable<T> {
    return this.http.get<T>(url, { params });
  }

  post<T>(url: string, model: any): Observable<T> {
    return this.http.post<T>(url, model);
  }

  put<T>(url: string, model: any): Observable<T> {
    return this.http.put<T>(url, model);
  }

  delete<T>(url: string, params?: any): Observable<T> {
    return this.http.delete<T>(url, { params });
  }
}