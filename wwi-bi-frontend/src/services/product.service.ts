import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse, Product, ProductDetail, ProductDto, PaginatedResponse } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly baseUrl = `${environment.apiUrl}/products`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<Product[]>> {
    return this.http.get<ApiResponse<Product[]>>(this.baseUrl);
  }

  getById(id: number): Observable<ApiResponse<ProductDetail>> {
    return this.http.get<ApiResponse<ProductDetail>>(`${this.baseUrl}/${id}`);
  }

  getProducts(page: number = 1, pageSize: number = 10, search?: string): Observable<PaginatedResponse<ProductDto>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    if (search) params = params.set('search', search);
    return this.http.get<PaginatedResponse<ProductDto>>(this.baseUrl, { params });
  }

  search(term: string): Observable<ApiResponse<Product[]>> {
    const params = new HttpParams().set('term', term);
    return this.http.get<ApiResponse<Product[]>>(`${this.baseUrl}/search`, { params });
  }

  getByPriceRange(minPrice: number, maxPrice: number): Observable<ApiResponse<Product[]>> {
    const params = new HttpParams()
      .set('minPrice', minPrice.toString())
      .set('maxPrice', maxPrice.toString());
    return this.http.get<ApiResponse<Product[]>>(`${this.baseUrl}/price-range`, { params });
  }

  createProduct(product: any): Observable<ApiResponse<ProductDto>> {
    return this.http.post<ApiResponse<ProductDto>>(this.baseUrl, product);
  }

  updateProduct(id: number, product: any): Observable<ApiResponse<ProductDto>> {
    return this.http.put<ApiResponse<ProductDto>>(`${this.baseUrl}/${id}`, product);
  }

  deleteProduct(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}
