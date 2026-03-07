import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse, Order, OrderDetail, OrderDto, PaginatedResponse } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly baseUrl = `${environment.apiUrl}/orders`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<Order[]>> {
    return this.http.get<ApiResponse<Order[]>>(this.baseUrl);
  }

  getById(id: number): Observable<ApiResponse<OrderDetail>> {
    return this.http.get<ApiResponse<OrderDetail>>(`${this.baseUrl}/${id}`);
  }

  getOrderById(id: number): Observable<ApiResponse<OrderDto>> {
    return this.http.get<ApiResponse<OrderDto>>(`${this.baseUrl}/${id}`);
  }

  getOrders(page: number = 1, pageSize: number = 10, customerName?: string, dateFrom?: string, dateTo?: string): Observable<PaginatedResponse<OrderDto>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    if (customerName) params = params.set('customerName', customerName);
    if (dateFrom) params = params.set('dateFrom', dateFrom);
    if (dateTo) params = params.set('dateTo', dateTo);
    return this.http.get<PaginatedResponse<OrderDto>>(this.baseUrl, { params });
  }

  getByCustomer(customerId: number): Observable<ApiResponse<Order[]>> {
    return this.http.get<ApiResponse<Order[]>>(`${this.baseUrl}/customer/${customerId}`);
  }

  getByDateRange(startDate: string, endDate: string): Observable<ApiResponse<Order[]>> {
    const params = new HttpParams()
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<ApiResponse<Order[]>>(`${this.baseUrl}/date-range`, { params });
  }

  getRecent(count: number = 10): Observable<ApiResponse<Order[]>> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<ApiResponse<Order[]>>(`${this.baseUrl}/recent`, { params });
  }

  createOrder(order: any): Observable<ApiResponse<OrderDto>> {
    return this.http.post<ApiResponse<OrderDto>>(this.baseUrl, order);
  }

  updateOrder(id: number, order: any): Observable<ApiResponse<OrderDto>> {
    return this.http.put<ApiResponse<OrderDto>>(`${this.baseUrl}/${id}`, order);
  }

  deleteOrder(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}
