import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse, Invoice, InvoiceDetail, InvoiceDto, InvoiceDetailDto, PaginatedResponse } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly baseUrl = `${environment.apiUrl}/invoices`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<Invoice[]>> {
    return this.http.get<ApiResponse<Invoice[]>>(this.baseUrl);
  }

  getById(id: number): Observable<ApiResponse<InvoiceDetail>> {
    return this.http.get<ApiResponse<InvoiceDetail>>(`${this.baseUrl}/${id}`);
  }

  getInvoiceById(id: number): Observable<ApiResponse<InvoiceDetailDto>> {
    return this.http.get<ApiResponse<InvoiceDetailDto>>(`${this.baseUrl}/${id}`);
  }

  getInvoices(page: number = 1, pageSize: number = 10, customerName?: string, dateFrom?: string, dateTo?: string): Observable<PaginatedResponse<InvoiceDto>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    if (customerName) params = params.set('customerName', customerName);
    if (dateFrom) params = params.set('dateFrom', dateFrom);
    if (dateTo) params = params.set('dateTo', dateTo);
    return this.http.get<PaginatedResponse<InvoiceDto>>(this.baseUrl, { params });
  }

  getByCustomer(customerId: number): Observable<ApiResponse<Invoice[]>> {
    return this.http.get<ApiResponse<Invoice[]>>(`${this.baseUrl}/customer/${customerId}`);
  }

  getByDateRange(startDate: string, endDate: string): Observable<ApiResponse<Invoice[]>> {
    const params = new HttpParams()
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<ApiResponse<Invoice[]>>(`${this.baseUrl}/date-range`, { params });
  }

  getRecent(count: number = 10): Observable<ApiResponse<Invoice[]>> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<ApiResponse<Invoice[]>>(`${this.baseUrl}/recent`, { params });
  }

  createInvoice(invoice: any): Observable<ApiResponse<InvoiceDto>> {
    return this.http.post<ApiResponse<InvoiceDto>>(this.baseUrl, invoice);
  }

  updateInvoice(id: number, invoice: any): Observable<ApiResponse<InvoiceDto>> {
    return this.http.put<ApiResponse<InvoiceDto>>(`${this.baseUrl}/${id}`, invoice);
  }

  deleteInvoice(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}
