import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { 
  ApiResponse, 
  SalesByPeriod, 
  SalesByProduct, 
  SalesByCustomer, 
  Kpi,
  KpiDto,
  SalesByPeriodDto,
  SalesByProductDto,
  SalesByCustomerDto,
  DashboardData 
} from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private readonly baseUrl = `${environment.apiUrl}/analytics`;

  constructor(private http: HttpClient) {}

  getSalesByPeriod(startDate?: string, endDate?: string): Observable<ApiResponse<SalesByPeriodDto[]>> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    return this.http.get<ApiResponse<SalesByPeriodDto[]>>(`${this.baseUrl}/sales/period`, { params });
  }

  getSalesByProduct(topN: number = 10): Observable<ApiResponse<SalesByProductDto[]>> {
    const params = new HttpParams().set('top', topN.toString());
    return this.http.get<ApiResponse<SalesByProductDto[]>>(`${this.baseUrl}/sales/product`, { params });
  }

  getSalesByCustomer(topN: number = 10): Observable<ApiResponse<SalesByCustomerDto[]>> {
    const params = new HttpParams().set('top', topN.toString());
    return this.http.get<ApiResponse<SalesByCustomerDto[]>>(`${this.baseUrl}/sales/customer`, { params });
  }

  getKpis(): Observable<ApiResponse<KpiDto>> {
    return this.http.get<ApiResponse<KpiDto>>(`${this.baseUrl}/kpis`);
  }

  getDashboardData(): Observable<ApiResponse<DashboardData>> {
    return this.http.get<ApiResponse<DashboardData>>(`${this.baseUrl}/dashboard`);
  }
}
