import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import {
  ApiResponse,
  DwKpi,
  DwSalesByProduct,
  DwSalesByClient,
  DwSalesByEmployee
} from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class DwDashboardService {
  private readonly baseUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  getKpis(): Observable<ApiResponse<DwKpi>> {
    return this.http.get<ApiResponse<DwKpi>>(`${this.baseUrl}/kpis`);
  }

  getTopProducts(topN: number = 10): Observable<ApiResponse<DwSalesByProduct[]>> {
    const params = new HttpParams().set('topN', topN.toString());
    return this.http.get<ApiResponse<DwSalesByProduct[]>>(`${this.baseUrl}/sales/by-product`, { params });
  }

  getTopClients(topN: number = 10): Observable<ApiResponse<DwSalesByClient[]>> {
    const params = new HttpParams().set('topN', topN.toString());
    return this.http.get<ApiResponse<DwSalesByClient[]>>(`${this.baseUrl}/sales/by-client`, { params });
  }

  getSalesByEmployee(): Observable<ApiResponse<DwSalesByEmployee[]>> {
    return this.http.get<ApiResponse<DwSalesByEmployee[]>>(`${this.baseUrl}/sales/by-employee`);
  }
}
