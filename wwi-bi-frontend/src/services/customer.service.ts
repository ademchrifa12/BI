import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { 
  ApiResponse, 
  Customer, 
  CustomerDetail, 
  CustomerCreate, 
  CustomerUpdate,
  CustomerDto,
  CustomerDetailDto,
  PaginatedResponse
} from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly baseUrl = `${environment.apiUrl}/customers`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<Customer[]>> {
    return this.http.get<ApiResponse<Customer[]>>(this.baseUrl);
  }

  getById(id: number): Observable<ApiResponse<CustomerDetail>> {
    return this.http.get<ApiResponse<CustomerDetail>>(`${this.baseUrl}/${id}`);
  }

  getCustomerById(id: number): Observable<ApiResponse<CustomerDetailDto>> {
    return this.http.get<ApiResponse<CustomerDetailDto>>(`${this.baseUrl}/${id}`);
  }

  getCustomers(page: number = 1, pageSize: number = 10, search?: string): Observable<PaginatedResponse<CustomerDto>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<PaginatedResponse<CustomerDto>>(this.baseUrl, { params });
  }

  search(term: string): Observable<ApiResponse<Customer[]>> {
    const params = new HttpParams().set('term', term);
    return this.http.get<ApiResponse<Customer[]>>(`${this.baseUrl}/search`, { params });
  }

  create(customer: CustomerCreate): Observable<ApiResponse<Customer>> {
    return this.http.post<ApiResponse<Customer>>(this.baseUrl, customer);
  }

  createCustomer(customer: any): Observable<ApiResponse<CustomerDto>> {
    return this.http.post<ApiResponse<CustomerDto>>(this.baseUrl, customer);
  }

  update(id: number, customer: CustomerUpdate): Observable<ApiResponse<Customer>> {
    return this.http.put<ApiResponse<Customer>>(`${this.baseUrl}/${id}`, customer);
  }

  updateCustomer(id: number, customer: any): Observable<ApiResponse<CustomerDto>> {
    return this.http.put<ApiResponse<CustomerDto>>(`${this.baseUrl}/${id}`, customer);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }

  deleteCustomer(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}
